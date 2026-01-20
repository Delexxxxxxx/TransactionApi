using System;

namespace TransactionApi.Models.Dto
{
    public record TransactionInsertResponse
    {
        public required DateTime InsertDateTime { get; init; }
    }
}
