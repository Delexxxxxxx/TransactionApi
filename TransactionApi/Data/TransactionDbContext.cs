using Microsoft.EntityFrameworkCore;
using TransactionApi.Data.Configurations;

namespace TransactionApi.Data
{
    public class TransactionDbContext : DbContext
    {
        public TransactionDbContext(DbContextOptions<TransactionDbContext> options) : base(options)
        {
        }

        public DbSet<TransactionEntity> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
			modelBuilder.ApplyConfiguration(new TransactionEntityConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}
