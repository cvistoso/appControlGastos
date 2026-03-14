using ExpenseControl.Application.Interfaces;
using ExpenseControl.Domain.Entities;
using ExpenseControl.Domain.Enums;
using ExpenseControl.Shared.Common;
using Microsoft.EntityFrameworkCore;

namespace ExpenseControl.Infrastructure.Data;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _db;

    public CategoryRepository(AppDbContext db) => _db = db;

    public async Task<Category?> GetByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken = default)
    {
        return await Query(userId, includeInactive: false)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<PagedResult<Category>> GetPagedAsync(Guid userId, int page, int pageSize, CategoryType? typeFilter = null, bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var query = Query(userId, includeInactive);
        if (typeFilter.HasValue)
            query = query.Where(c => c.Type == typeFilter.Value);
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(c => c.SortOrder).ThenBy(c => c.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        return new PagedResult<Category>(items, page, pageSize, totalCount, totalPages);
    }

    public async Task<Category> AddAsync(Category category, CancellationToken cancellationToken = default)
    {
        _db.Categories.Add(category);
        await _db.SaveChangesAsync(cancellationToken);
        return category;
    }

    public async Task UpdateAsync(Category category, CancellationToken cancellationToken = default)
    {
        _db.Categories.Update(category);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> SoftDeleteAsync(Guid userId, Guid id, CancellationToken cancellationToken = default)
    {
        var category = await _db.Categories.FirstOrDefaultAsync(c => c.UserId == userId && c.Id == id && c.DeletedAtUtc == null, cancellationToken);
        if (category is null) return false;
        category.DeletedAtUtc = DateTime.UtcNow;
        category.IsActive = false;
        category.UpdatedAtUtc = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    private IQueryable<Category> Query(Guid userId, bool includeInactive)
    {
        var q = _db.Categories.AsNoTracking().Where(c => c.UserId == userId && c.DeletedAtUtc == null);
        if (!includeInactive)
            q = q.Where(c => c.IsActive);
        return q;
    }
}
