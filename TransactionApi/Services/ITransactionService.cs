using System.Threading;
using TransactionApi.Models.Dto;

namespace TransactionApi.Services
{
    public interface ITransactionService
    {
        Task<TransactionInsertResponse> CreateAsync(TransactionCreateRequest tx, CancellationToken cancellationToken);
        Task<TransactionResponse?> GetAsync(Guid id, CancellationToken cancellationToken);
    }
}
