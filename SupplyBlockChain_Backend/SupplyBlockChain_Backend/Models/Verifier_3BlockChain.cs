using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SupplyBlockChain_Backend.Models
{
    //BlockChain of First Verifier
    public class Verifier_3BlockChain
    {
        public List<Block> Chain { get; set; }
        public int Difficulty { get; set; } = 5;

        public Verifier_3BlockChain()
        {
            Chain = new List<Block>
            {
                new Block("",new List<Transaction>(),Difficulty)
            };
        }

        public void AddBlock(Block MinedBlock)
        {
            Chain.Add(MinedBlock);
        }

        public bool IsChainValid()
        {
            for (int i = 1; i < Chain.Count; i++)
            {
                var currentBlock = Chain[i];
                var previousBlock = Chain[i - 1];
                if (currentBlock.CurrentHash != CalculteHashOfBlock(currentBlock))
                {
                    return false;
                }
                if (currentBlock.PreviousHash != previousBlock.CurrentHash)
                {
                    return false;
                }
            }
            return true;
        }

        private string CalculteHashOfBlock(Block block)
        {
            var transactionString = String.Empty;
            foreach (var item in block.Transactions)
            {
                transactionString += item.ToString();
            }
            SHA256 sha256 = SHA256Managed.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(block.PreviousHash + transactionString + block.Nounce.ToString() + block.BlockAddedTimeStamp.Trim());
            byte[] hash = sha256.ComputeHash(bytes);
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("X2"));
            }
            return result.ToString();
        }
    }
}
