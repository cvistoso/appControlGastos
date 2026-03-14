using ExpenseControl.Application.Interfaces;
using ExpenseControl.Domain.Entities;
using ExpenseControl.Shared.Common;
using Microsoft.EntityFrameworkCore;

namespace ExpenseControl.Infrastructure.Data;

public class AccountRepository : IAccountRepository
{
    private readonly AppDbContext _db;

    public AccountRepository(AppDbContext db) => _db = db;

    public async Task<Account?> GetByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken = default)
        => await Query(userId, includeInactive: false).FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task<PagedResult<Account>> GetPagedAsync(Guid userId, int page, int pageSize, bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var query = Query(userId, includeInactive);
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(a => a.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        return new PagedResult<Account>(items, page, pageSize, totalCount, totalPages);
    }

    public async Task<Account> AddAsync(Account account, CancellationToken cancellationToken = default)
    {
        _db.Accounts.Add(account);
        await _db.SaveChangesAsync(cancellationToken);
        return account;
    }

    public async Task UpdateAsync(Account account, CancellationToken cancellationToken = default)
    {
        _db.Accounts.Update(account);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> SoftDeleteAsync(Guid userId, Guid id, CancellationToken cancellationToken = default)
    {
        var account = await _db.Accounts.FirstOrDefaultAsync(a => a.UserId == userId && a.Id == id && a.DeletedAtUtc == null, cancellationToken);
        if (account is null) return false;
        account.DeletedAtUtc = DateTime.UtcNow;
        account.IsActive = false;
        account.UpdatedAtUtc = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    private IQueryable<Account> Query(Guid userId, bool includeInactive)
    {
        var q = _db.Accounts.AsNoTracking().Where(a => a.UserId == userId && a.DeletedAtUtc == null);
        if (!includeInactive)
            q = q.Where(a => a.IsActive);
        return q;
    }
}
