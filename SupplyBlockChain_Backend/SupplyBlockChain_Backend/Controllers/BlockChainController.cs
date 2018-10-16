using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public BlockChainController(IOptions<BlockChain> blockChain, UserDbContext userDbContext)
        {
            _userContext = userDbContext;
            SupplyBlockChain = blockChain.Value;
        }

        [HttpPost]
        public string GetBlockChain()
        {
            return JsonConvert.SerializeObject(SupplyBlockChain);
        }

        [HttpPost]
        public async Task<string> CreateTransaction(string transaction,string userName,string password)
        {
            var User = await _userContext.UserAccounts.FirstOrDefaultAsync(m => m.UserName == userName);
            if (User != null)
            {
                var AccessRights = JsonConvert.DeserializeObject<List<string>>(User.AccessRights);
                if (AccessRights.Contains("CreateTransaction"))
                {
                    Transaction newTransaction = JsonConvert.DeserializeObject<Transaction>(transaction);
                    SupplyBlockChain.CreateTransaction(newTransaction);
                    return "True";
                }
            }
            return "False";
        }

        [HttpPost]
        public async Task<string> MineTransactions(string userName,string password)
        {
            var User = await _userContext.UserAccounts.FirstOrDefaultAsync(m => m.UserName == userName);
            if(User!=null)
            {
                var AccessRights = JsonConvert.DeserializeObject<List<string>>(User.AccessRights);
                if(AccessRights.Contains("MineBlock"))
                {
                    await SupplyBlockChain.MineTransactions();
                    return "True";
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
            return "False";
        }
    }
}