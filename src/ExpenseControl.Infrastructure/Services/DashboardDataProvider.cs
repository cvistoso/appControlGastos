using ExpenseControl.Application.Dashboard;
using ExpenseControl.Application.Interfaces;
using ExpenseControl.Domain.Enums;
using ExpenseControl.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ExpenseControl.Infrastructure.Services;

public class DashboardDataProvider : IDashboardDataProvider
{
    private readonly AppDbContext _db;

    public DashboardDataProvider(AppDbContext db) => _db = db;

    public async Task<DashboardDto> GetAsync(Guid userId, DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var monthEnd = monthStart.AddMonths(1).AddTicks(-1);

        var baseQuery = _db.Transactions
            .Where(t => t.UserId == userId && t.DeletedAtUtc == null);
        if (fromDate.HasValue)
            baseQuery = baseQuery.Where(t => t.TransactionDateUtc >= fromDate.Value);
        if (toDate.HasValue)
            baseQuery = baseQuery.Where(t => t.TransactionDateUtc <= toDate.Value);

        var totalIncome = await baseQuery.Where(t => t.Type == TransactionType.Income).SumAsync(t => t.Amount, cancellationToken);
        var totalExpenses = await baseQuery.Where(t => t.Type == TransactionType.Expense).SumAsync(t => t.Amount, cancellationToken);

        var monthlyQuery = _db.Transactions
            .Where(t => t.UserId == userId && t.DeletedAtUtc == null && t.TransactionDateUtc >= monthStart && t.TransactionDateUtc <= monthEnd);
        var monthlyIncome = await monthlyQuery.Where(t => t.Type == TransactionType.Income).SumAsync(t => t.Amount, cancellationToken);
        var monthlyExpenses = await monthlyQuery.Where(t => t.Type == TransactionType.Expense).SumAsync(t => t.Amount, cancellationToken);

        var recent = await _db.Transactions
            .AsNoTracking()
            .Where(t => t.UserId == userId && t.DeletedAtUtc == null)
            .OrderByDescending(t => t.TransactionDateUtc)
            .ThenByDescending(t => t.CreatedAtUtc)
            .Take(10)
            .Select(t => new TransactionSummaryDto(
                t.Id,
                t.Description,
                t.Amount,
                t.Type.ToString(),
                t.TransactionDateUtc,
                t.Category.Name))
            .ToListAsync(cancellationToken);

        return new DashboardDto(
            totalIncome,
            totalExpenses,
            totalIncome - totalExpenses,
            monthlyIncome,
            monthlyExpenses,
            monthlyIncome - monthlyExpenses,
            recent);
    }
}
