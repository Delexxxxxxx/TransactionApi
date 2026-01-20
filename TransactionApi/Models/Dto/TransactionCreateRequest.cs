using System;

namespace TransactionApi.Models.Dto
{
    public record TransactionCreateRequest
    {
        public required Guid Id { get; init; }
        public required DateTime TransactionDate { get; init; }
        public required decimal Amount { get; init; }
    }
}
