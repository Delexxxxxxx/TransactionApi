using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TransactionApi.Data
{
    [Table("transactions")]
    public class TransactionEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Column("transaction_date")]
        public DateTime TransactionDate { get; set; }

        [Column("amount")]
        public decimal Amount { get; set; }

        [Column("inserted_at")]
        public DateTime InsertedAt { get; set; }
    }
}
