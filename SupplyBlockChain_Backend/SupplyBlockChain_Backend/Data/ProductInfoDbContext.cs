using Microsoft.EntityFrameworkCore;
using SupplyBlockChain_Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyBlockChain_Backend.Data
{
    public class ProductInfoDbContext:DbContext
    {
        public ProductInfoDbContext(DbContextOptions<ProductInfoDbContext> options):base(options)
        {

        }

        public DbSet<ProductInfo> ProductsInfos { set; get; }
    }
}
