using ExpenseControl.Domain.Entities;
using ExpenseControl.Shared.Common;

namespace ExpenseControl.Application.Interfaces;

public interface IAccountRepository
{
    Task<Account?> GetByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<Account>> GetPagedAsync(Guid userId, int page, int pageSize, bool includeInactive = false, CancellationToken cancellationToken = default);
    Task<Account> AddAsync(Account account, CancellationToken cancellationToken = default);
    Task UpdateAsync(Account account, CancellationToken cancellationToken = default);
    Task<bool> SoftDeleteAsync(Guid userId, Guid id, CancellationToken cancellationToken = default);
}
