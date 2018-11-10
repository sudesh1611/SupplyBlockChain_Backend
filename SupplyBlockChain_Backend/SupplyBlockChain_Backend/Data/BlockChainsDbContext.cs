using Microsoft.EntityFrameworkCore;
using SupplyBlockChain_Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyBlockChain_Backend.Data
{
    public class BlockChainsDbContext:DbContext
    {
        public BlockChainsDbContext(DbContextOptions<BlockChainsDbContext> options):base(options)
        {

        }

        public DbSet<StoreBlockChains> AllBlockChains { get; set; }
    }
}
