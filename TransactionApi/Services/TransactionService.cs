using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TransactionApi.Configuration;
using TransactionApi.Data;
using TransactionApi.Models.Dto;

namespace TransactionApi.Services;

public class TransactionService : ITransactionService
{
	private readonly IDbContextFactory<TransactionDbContext> _dbFactory;
	private readonly TransactionApiSettings _settings;
	private readonly ILogger<TransactionService> _logger;

	public TransactionService(
		IDbContextFactory<TransactionDbContext> dbFactory,
		IOptions<TransactionApiSettings> options,
		ILogger<TransactionService> logger)
	{
		_dbFactory = dbFactory;
		_settings = options.Value;
		_logger = logger;
	}

	public async Task<TransactionResponse?> GetAsync(Guid id, CancellationToken cancellationToken)
	{
		try
		{
			await using var db = await _dbFactory.CreateDbContextAsync(cancellationToken);
			var transactionDto = await db.Transactions
				.AsNoTracking()
				.Where(t => t.Id == id)
				.Select(t => new TransactionResponse
				{
					Id = t.Id,
					TransactionDate = t.TransactionDate,
					Amount = t.Amount
				})
				.FirstOrDefaultAsync(cancellationToken);

			return transactionDto;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to retrieve transaction {TransactionId}", id);
			throw;
		}
	}

	public async Task<TransactionInsertResponse> CreateAsync(TransactionCreateRequest transactionRequest, CancellationToken cancellationToken)
	{
		Validate(transactionRequest);

		try
		{
			await using var db = await _dbFactory.CreateDbContextAsync(cancellationToken);
			await using var dbTransaction = await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable, cancellationToken);

			var existingTransaction = await db.Transactions.AsNoTracking()
				.FirstOrDefaultAsync(t => t.Id == transactionRequest.Id, cancellationToken);

			if (existingTransaction != null)
				return new TransactionInsertResponse { InsertDateTime = existingTransaction.InsertedAt };

			var count = await db.Transactions.CountAsync(cancellationToken);
			if (count >= _settings.TransactionLimit)
				throw new InvalidOperationException("Transaction storage limit reached.");

			var entity = new TransactionEntity
			{
				Id = transactionRequest.Id,
				TransactionDate = transactionRequest.TransactionDate,
				Amount = transactionRequest.Amount,
				InsertedAt = DateTime.UtcNow
			};

			db.Transactions.Add(entity);
			await db.SaveChangesAsync(cancellationToken);
			await dbTransaction.CommitAsync(cancellationToken);

			return new TransactionInsertResponse { InsertDateTime = entity.InsertedAt };
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to create transaction {TransactionId}", transactionRequest.Id);
			throw;
		}
	}

	private static void Validate(TransactionCreateRequest transactionRequest)
	{
		if (transactionRequest == null)
			throw new ArgumentException("Transaction request cannot be null.", nameof(transactionRequest));

		if (transactionRequest.Amount <= 0)
			throw new ArgumentException("Amount must be positive.");

		if (transactionRequest.TransactionDate.ToUniversalTime() > DateTime.UtcNow)
			throw new ArgumentException("Transaction date cannot be in the future.");
	}
}
