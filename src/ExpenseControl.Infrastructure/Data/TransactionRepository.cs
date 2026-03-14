using ExpenseControl.Application.Interfaces;
using ExpenseControl.Domain.Entities;
using ExpenseControl.Domain.Enums;
using ExpenseControl.Shared.Common;
using Microsoft.EntityFrameworkCore;

namespace ExpenseControl.Infrastructure.Data;

public class TransactionRepository : ITransactionRepository
{
    private readonly AppDbContext _db;

    public TransactionRepository(AppDbContext db) => _db = db;

    public async Task<Transaction?> GetByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken = default)
        => await _db.Transactions.AsNoTracking()
            .FirstOrDefaultAsync(t => t.UserId == userId && t.Id == id && t.DeletedAtUtc == null, cancellationToken);

    public async Task<PagedResult<Transaction>> GetPagedAsync(Guid userId, int page, int pageSize, TransactionType? typeFilter = null, Guid? accountId = null, Guid? categoryId = null, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default)
    {
        var query = _db.Transactions.AsNoTracking().Where(t => t.UserId == userId && t.DeletedAtUtc == null);
        if (typeFilter.HasValue)
            query = query.Where(t => t.Type == typeFilter.Value);
        if (accountId.HasValue)
            query = query.Where(t => t.AccountId == accountId.Value || t.TransferAccountId == accountId.Value);
        if (categoryId.HasValue)
            query = query.Where(t => t.CategoryId == categoryId.Value);
        if (fromDate.HasValue)
            query = query.Where(t => t.TransactionDateUtc >= fromDate.Value);
        if (toDate.HasValue)
            query = query.Where(t => t.TransactionDateUtc <= toDate.Value);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(t => t.TransactionDateUtc).ThenByDescending(t => t.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        return new PagedResult<Transaction>(items, page, pageSize, totalCount, totalPages);
    }

    public async Task<Transaction> AddAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        _db.Transactions.Add(transaction);
        await _db.SaveChangesAsync(cancellationToken);
        return transaction;
    }

    public async Task UpdateAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        _db.Transactions.Update(transaction);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> SoftDeleteAsync(Guid userId, Guid id, CancellationToken cancellationToken = default)
    {
        var transaction = await _db.Transactions.FirstOrDefaultAsync(t => t.UserId == userId && t.Id == id && t.DeletedAtUtc == null, cancellationToken);
        if (transaction is null) return false;
        transaction.DeletedAtUtc = DateTime.UtcNow;
        transaction.UpdatedAtUtc = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
