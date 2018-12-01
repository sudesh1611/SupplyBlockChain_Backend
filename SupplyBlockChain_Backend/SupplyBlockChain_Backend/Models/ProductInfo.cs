using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyBlockChain_Backend.Models
{
    public class ProductInfo
    {
        public int ID { get; set; }

        public string ProductName { get; set; }

        public string ProductType { get; set; }

        public string ProductID { get; set; }

        public int ProductCreator { get; set; }

        public string CreationDate { get; set; }
    }
}
