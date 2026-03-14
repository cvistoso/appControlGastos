using ExpenseControl.Domain.Entities;
using ExpenseControl.Domain.Enums;
using ExpenseControl.Shared.Common;

namespace ExpenseControl.Application.Interfaces;

public interface ITransactionRepository
{
    Task<Transaction?> GetByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<Transaction>> GetPagedAsync(Guid userId, int page, int pageSize, TransactionType? typeFilter = null, Guid? accountId = null, Guid? categoryId = null, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default);
    Task<Transaction> AddAsync(Transaction transaction, CancellationToken cancellationToken = default);
    Task UpdateAsync(Transaction transaction, CancellationToken cancellationToken = default);
    Task<bool> SoftDeleteAsync(Guid userId, Guid id, CancellationToken cancellationToken = default);
}
