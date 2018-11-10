using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Quartz;
using SupplyBlockChain_Backend.Data;
using SupplyBlockChain_Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyBlockChain_Backend
{

    //This class contains miner and three verifiers
    public class ScheduledMiner : IJob
    {

        private readonly IConfiguration configuration;
        private readonly ILogger<ScheduledMiner> logger;
        private readonly BlockChainsDbContext blockChainsDbContext;

        //Global Instance of SupplyBlockchain
        private BlockChain SupplyBlockChain;

        //Blockchains from databse
        private StoreBlockChains tempBlockChains;

        //Blockchains of individual verifiers
        private Verifier_1BlockChain Verifier1Chain;
        private Verifier_2BlockChain Verifier2Chain;
        private Verifier_3BlockChain Verifier3Chain;

        public ScheduledMiner(IConfiguration cfg, IOptions<BlockChain> blockChain, BlockChainsDbContext sdb, ILogger<ScheduledMiner> log)
        {
            configuration = cfg;
            SupplyBlockChain = blockChain.Value;
            blockChainsDbContext = sdb;
            Verifier1Chain = new Verifier_1BlockChain();
            Verifier2Chain = new Verifier_2BlockChain();
            Verifier3Chain = new Verifier_3BlockChain();
            tempBlockChains = new StoreBlockChains();
            logger = log;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            tempBlockChains = await blockChainsDbContext.AllBlockChains.SingleOrDefaultAsync(m => m.ID == 1);
            
            //First time scheduler is running
            if(tempBlockChains==null)
            {
                //Initialize all block chains with genesis block
                var newBlockChains = new StoreBlockChains();
                newBlockChains.SupplyBlockChain = JsonConvert.SerializeObject(SupplyBlockChain.Chain);
                newBlockChains.Verifier_1BlockChain = JsonConvert.SerializeObject(SupplyBlockChain.Chain);
                newBlockChains.Verifier_2BlockChain = JsonConvert.SerializeObject(SupplyBlockChain.Chain);
                newBlockChains.Verifier_3BlockChain = JsonConvert.SerializeObject(SupplyBlockChain.Chain);
                newBlockChains.Difficulty = SupplyBlockChain.Difficulty;

                //Store Initialized blockChains in database
                await blockChainsDbContext.AllBlockChains.AddAsync(newBlockChains);
                await blockChainsDbContext.SaveChangesAsync();

                //Initialize local blockchains to genesis block
                tempBlockChains = new StoreBlockChains();
                tempBlockChains.SupplyBlockChain = JsonConvert.SerializeObject(SupplyBlockChain.Chain);
                tempBlockChains.Verifier_1BlockChain = JsonConvert.SerializeObject(SupplyBlockChain.Chain);
                tempBlockChains.Verifier_2BlockChain = JsonConvert.SerializeObject(SupplyBlockChain.Chain);
                tempBlockChains.Verifier_3BlockChain = JsonConvert.SerializeObject(SupplyBlockChain.Chain);
                tempBlockChains.Difficulty = SupplyBlockChain.Difficulty;
            }

            //Get mainBlockChain from database
            SupplyBlockChain.Chain = JsonConvert.DeserializeObject<List<Block>>(tempBlockChains.SupplyBlockChain);
            SupplyBlockChain.Difficulty = tempBlockChains.Difficulty;

            //Get all pending transactions till now and store them in mining transactions for mining
            SupplyBlockChain.MiningTransactions = SupplyBlockChain.PendingTransactions;
            SupplyBlockChain.PendingTransactions = new List<Transaction>();

            //Create and mine new block
            Block tempBlock = new Block(SupplyBlockChain.GetLatestBlock().CurrentHash, SupplyBlockChain.MiningTransactions,SupplyBlockChain.Difficulty);
            await tempBlock.MineBlock(SupplyBlockChain.Difficulty);

            //Fire up first verifier to verify if new block is okay
            if(await Verifier1(tempBlock)==true)
            {
                logger.LogInformation($"Block mined with hash : {tempBlock.CurrentHash}. Log Timestamp : {DateTime.Now.ToLongTimeString()}.");
            }
            else
            {
                //In Case block is not okay, restore all transactions to pending transactions
                SupplyBlockChain.PendingTransactions.AddRange(SupplyBlockChain.MiningTransactions);
                SupplyBlockChain.MiningTransactions = new List<Transaction>();
                logger.LogError($"Block can't be mined or verified. Log Timestamp : {DateTime.Now.ToLongDateString()}.");
            }
        }

        private async Task<bool> Verifier1(Block MinedBlock)
        {
            //Get first verifiers's block chain
            Verifier1Chain.Chain = JsonConvert.DeserializeObject<List<Block>>(tempBlockChains.Verifier_1BlockChain);

            //Add new block to this temporary blockchain
            Verifier1Chain.AddBlock(MinedBlock);

            //Check if block is genuine
            if(Verifier1Chain.IsChainValid()==false)
            {
                return false;
            }

            //Fire up second verifier to verify if new block is okay
            return await Verifier2(MinedBlock);
        }

        private async Task<bool> Verifier2(Block MinedBlock)
        {
            //Get second verifiers's block chain
            Verifier2Chain.Chain = JsonConvert.DeserializeObject<List<Block>>(tempBlockChains.Verifier_2BlockChain);

            //Add new block to this temporary blockchain
            Verifier2Chain.AddBlock(MinedBlock);

            //Check if block is genuine
            if (Verifier2Chain.IsChainValid() == false)
            {
                return false;
            }

            //Fire up third verifier to verify if new block is okay
            return await Verifier3(MinedBlock);
        }

        private async Task<bool> Verifier3(Block MinedBlock)
        {
            //Get third verifiers's block chain
            Verifier3Chain.Chain = JsonConvert.DeserializeObject<List<Block>>(tempBlockChains.Verifier_3BlockChain);

            //Add new block to this temporary blockchain
            Verifier3Chain.AddBlock(MinedBlock);

            //Check if block is genuine
            if (Verifier3Chain.IsChainValid() == false)
            {
                return false;
            }

            //Add this block to all blockchains permanently and store in database
            return await AddBlockToChain(MinedBlock);
        }

        private async Task<bool> AddBlockToChain(Block MinedBlock)
        {
            //Adding block to main SupplyChain
            SupplyBlockChain.AddBlock(MinedBlock);

            //Adding and storing in database
            var newBlockChains = await blockChainsDbContext.AllBlockChains.SingleOrDefaultAsync(m => m.ID == 1);
            newBlockChains.SupplyBlockChain = JsonConvert.SerializeObject(SupplyBlockChain.Chain);
            newBlockChains.Verifier_1BlockChain = JsonConvert.SerializeObject(Verifier1Chain.Chain);
            newBlockChains.Verifier_2BlockChain = JsonConvert.SerializeObject(Verifier2Chain.Chain);
            newBlockChains.Verifier_3BlockChain = JsonConvert.SerializeObject(Verifier3Chain.Chain);
            blockChainsDbContext.AllBlockChains.Update(newBlockChains);
            await blockChainsDbContext.SaveChangesAsync();
            return true;
        }
    }
}
