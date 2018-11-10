using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SupplyBlockChain_Backend.Data;
using SupplyBlockChain_Backend.Models;

namespace SupplyBlockChain_Backend.Controllers
{
    public class BlockChainController : ControllerBase
    {
        private readonly UserDbContext _userContext;
        private BlockChain SupplyBlockChain;
        private readonly BlockChainsDbContext blockChainsDbContext;

        public BlockChainController(IOptions<BlockChain> blockChain, UserDbContext userDbContext, BlockChainsDbContext sdb)
        {
            _userContext = userDbContext;
            blockChainsDbContext = sdb;
            SupplyBlockChain = blockChain.Value;
        }

        [HttpPost]
        public string GetBlockChain()
        {
            return JsonConvert.SerializeObject(SupplyBlockChain.Chain);
        }

        [HttpPost]
        public async Task<string> GetFullBlockChain(string userName, string password)
        {
            var User = await _userContext.UserAccounts.FirstOrDefaultAsync(m => m.UserName == userName);
            if (User != null)
            {
                if(User.Password==password)
                {
                    var AccessRights = JsonConvert.DeserializeObject<List<string>>(User.AccessRights);
                    if (AccessRights.Contains("Admin"))
                    {
                        return JsonConvert.SerializeObject(SupplyBlockChain);
                    }
                }
            }
            return "False";
        }

        [HttpPost]
        public async Task<string> SetBlockChainDifficulty(string difficulty, string userName, string password)
        {
            var User = await _userContext.UserAccounts.FirstOrDefaultAsync(m => m.UserName == userName);
            if (User != null)
            {
                if(User.Password==password)
                {
                    var AccessRights = JsonConvert.DeserializeObject<List<string>>(User.AccessRights);
                    if (AccessRights.Contains("Admin"))
                    {
                        var temp = int.TryParse(difficulty, out int diff);
                        if (temp)
                        {
                            SupplyBlockChain.Difficulty = diff;
                            var tempChains = await blockChainsDbContext.AllBlockChains.SingleOrDefaultAsync(m => m.ID == 1);
                            if (tempChains != null)
                            {
                                tempChains.Difficulty = diff;
                                blockChainsDbContext.AllBlockChains.Update(tempChains);
                                await blockChainsDbContext.SaveChangesAsync();
                                return "True";
                            }
                        }
                    }
                }
            }
            return "False";
        }

        [HttpPost]
        public async Task<string> CreateTransaction(string transaction,string userName,string password)
        {
            var User = await _userContext.UserAccounts.FirstOrDefaultAsync(m => m.UserName == userName);
            if (User != null)
            {
                if(User.Password==password)
                {
                    var AccessRights = JsonConvert.DeserializeObject<List<string>>(User.AccessRights);
                    if (AccessRights.Contains("CreateTransaction") || AccessRights.Contains("Admin"))
                    {
                        Transaction newTransaction = JsonConvert.DeserializeObject<Transaction>(transaction);
                        SupplyBlockChain.CreateTransaction(newTransaction);
                        return "True";
                    }
                }
            }
            return "False";
        }

        [HttpPost]
        public string CheckTransactionID(string ID)
        {
            foreach(var block in SupplyBlockChain.Chain)
            {
                foreach (var Transaction in block.Transactions)
                {
                    if(Transaction.ProductID==ID)
                    {
                        return "True";
                    }
                }
            }
            foreach (var Transaction in SupplyBlockChain.PendingTransactions)
            {
                if(Transaction.ProductID==ID)
                {
                    return "True";
                }
            }
            foreach (var Transaction in SupplyBlockChain.MiningTransactions)
            {
                if (Transaction.ProductID == ID)
                {
                    return "True";
                }
            }
            return "False";
        }
    }
}