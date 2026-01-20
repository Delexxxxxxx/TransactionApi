using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TransactionApi.Data.Configurations
{
    public class TransactionEntityConfiguration : IEntityTypeConfiguration<TransactionEntity>
    {
        public void Configure(EntityTypeBuilder<TransactionEntity> builder)
        {
			builder.ToTable("transactions");

			builder.HasKey(t => t.Id);

			builder.Property(t => t.TransactionDate)
				.IsRequired();

			builder.Property(t => t.Amount)
				.IsRequired();

			builder.Property(t => t.InsertedAt)
				.IsRequired();
		}
    }
}
