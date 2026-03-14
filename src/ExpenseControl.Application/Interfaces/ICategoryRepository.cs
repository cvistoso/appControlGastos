using ExpenseControl.Domain.Entities;
using ExpenseControl.Domain.Enums;
using ExpenseControl.Shared.Common;

namespace ExpenseControl.Application.Interfaces;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<Category>> GetPagedAsync(Guid userId, int page, int pageSize, CategoryType? typeFilter = null, bool includeInactive = false, CancellationToken cancellationToken = default);
    Task<Category> AddAsync(Category category, CancellationToken cancellationToken = default);
    Task UpdateAsync(Category category, CancellationToken cancellationToken = default);
    Task<bool> SoftDeleteAsync(Guid userId, Guid id, CancellationToken cancellationToken = default);
}
