using Microsoft.EntityFrameworkCore;
using ExpenseControlApp.Data;
using ExpenseControlApp.Models;
using ExpenseControlApp.Models.DTOs;

namespace ExpenseControlApp.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ExpenseDbContext _context;

        public DashboardService(ExpenseDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardDto> GetDashboardDataAsync(int userId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var now = DateTime.UtcNow;
            var currentMonthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var currentMonthEnd = currentMonthStart.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);

            var query = _context.Transactions.Where(t => t.UserId == userId);

            if (startDate.HasValue)
                query = query.Where(t => t.Date >= startDate.Value.ToUniversalTime());

            if (endDate.HasValue)
                query = query.Where(t => t.Date <= endDate.Value.ToUniversalTime());

            var transactions = await query
                .Include(t => t.Category)
                .ToListAsync();

            var monthlyQuery = _context.Transactions
                .Where(t => t.UserId == userId && t.Date >= currentMonthStart && t.Date <= currentMonthEnd);

            var monthlyTransactions = await monthlyQuery
                .Include(t => t.Category)
                .ToListAsync();

            var totalIncome = transactions
                .Where(t => t.Type == TransactionType.Income)
                .Sum(t => t.Amount);

            var totalExpenses = transactions
                .Where(t => t.Type == TransactionType.Expense)
                .Sum(t => t.Amount);

            var monthlyIncome = monthlyTransactions
                .Where(t => t.Type == TransactionType.Income)
                .Sum(t => t.Amount);

            var monthlyExpenses = monthlyTransactions
                .Where(t => t.Type == TransactionType.Expense)
                .Sum(t => t.Amount);

            var topExpenseCategories = await GetTopExpenseCategoriesAsync(userId, startDate, endDate);
            var recentTransactions = await GetRecentTransactionsAsync(userId, 10);
            var monthlyTrends = await GetMonthlyTrendsAsync(userId, 12);
            var projections = await GetCashFlowProjectionsAsync(userId, 6);

            return new DashboardDto
            {
                TotalIncome = totalIncome,
                TotalExpenses = totalExpenses,
                CurrentBalance = totalIncome - totalExpenses,
                MonthlyIncome = monthlyIncome,
                MonthlyExpenses = monthlyExpenses,
                MonthlyBalance = monthlyIncome - monthlyExpenses,
                TopExpenseCategories = topExpenseCategories.ToList(),
                RecentTransactions = recentTransactions.ToList(),
                MonthlyTrends = monthlyTrends.ToList(),
                CashFlowProjections = projections.ToList()
            };
        }

        public async Task<IEnumerable<MonthlyTrendDto>> GetMonthlyTrendsAsync(int userId, int months = 12)
        {
            var endDate = DateTime.UtcNow;
            var startDate = endDate.AddMonths(-months);

            var trends = await _context.Transactions
                .Where(t => t.UserId == userId && t.Date >= startDate && t.Date <= endDate)
                .GroupBy(t => new { t.Date.Year, t.Date.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Income = g.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount),
                    Expenses = g.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount)
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToListAsync();

            var result = new List<MonthlyTrendDto>();
            var currentDate = startDate;

            while (currentDate <= endDate)
            {
                var trend = trends.FirstOrDefault(t => t.Year == currentDate.Year && t.Month == currentDate.Month);
                result.Add(new MonthlyTrendDto
                {
                    Year = currentDate.Year,
                    Month = currentDate.Month,
                    MonthName = currentDate.ToString("MMM yyyy"),
                    Income = trend?.Income ?? 0,
                    Expenses = trend?.Expenses ?? 0,
                    Balance = (trend?.Income ?? 0) - (trend?.Expenses ?? 0)
                });

                currentDate = currentDate.AddMonths(1);
            }

            return result;
        }

        public async Task<IEnumerable<ProjectionDto>> GetCashFlowProjectionsAsync(int userId, int months = 6)
        {
            var endDate = DateTime.UtcNow.AddMonths(-3); // Use last 3 months for projection
            var startDate = endDate.AddMonths(-3);

            var historicalData = await _context.Transactions
                .Where(t => t.UserId == userId && t.Date >= startDate && t.Date <= endDate)
                .GroupBy(t => new { t.Date.Year, t.Date.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Income = g.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount),
                    Expenses = g.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount)
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToListAsync();

            if (!historicalData.Any())
            {
                return new List<ProjectionDto>();
            }

            var avgIncome = historicalData.Average(x => x.Income);
            var avgExpenses = historicalData.Average(x => x.Expenses);

            var projections = new List<ProjectionDto>();
            var currentDate = DateTime.UtcNow.AddMonths(1);

            for (int i = 0; i < months; i++)
            {
                projections.Add(new ProjectionDto
                {
                    Date = currentDate,
                    ProjectedIncome = avgIncome,
                    ProjectedExpenses = avgExpenses,
                    ProjectedBalance = avgIncome - avgExpenses
                });

                currentDate = currentDate.AddMonths(1);
            }

            return projections;
        }

        private async Task<IEnumerable<CategorySummaryDto>> GetTopExpenseCategoriesAsync(int userId, DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId && t.Type == TransactionType.Expense);

            if (startDate.HasValue)
                query = query.Where(t => t.Date >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(t => t.Date <= endDate.Value);

            var summary = await query
                .GroupBy(t => new { t.CategoryId, t.Category.Name, t.Category.Color, t.Category.Icon })
                .Select(g => new CategorySummaryDto
                {
                    CategoryId = g.Key.CategoryId,
                    CategoryName = g.Key.Name,
                    CategoryColor = g.Key.Color,
                    CategoryIcon = g.Key.Icon,
                    TotalAmount = g.Sum(t => t.Amount),
                    TransactionCount = g.Count()
                })
                .OrderByDescending(s => s.TotalAmount)
                .Take(5)
                .ToListAsync();

            var totalAmount = summary.Sum(s => s.TotalAmount);

            foreach (var item in summary)
            {
                item.Percentage = totalAmount > 0 ? (item.TotalAmount / totalAmount) * 100 : 0;
            }

            return summary;
        }

        private async Task<IEnumerable<TransactionDto>> GetRecentTransactionsAsync(int userId, int count)
        {
            return await _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.Date)
                .ThenByDescending(t => t.CreatedAt)
                .Take(count)
                .Select(t => new TransactionDto
                {
                    Id = t.Id,
                    Description = t.Description,
                    Amount = t.Amount,
                    Type = t.Type.ToString(),
                    Date = t.Date,
                    Notes = t.Notes,
                    Location = t.Location,
                    PaymentMethod = t.PaymentMethod,
                    IsRecurring = t.IsRecurring,
                    RecurringFrequency = t.RecurringFrequency,
                    NextRecurringDate = t.NextRecurringDate,
                    CategoryId = t.CategoryId,
                    CategoryName = t.Category.Name,
                    CategoryColor = t.Category.Color,
                    CategoryIcon = t.Category.Icon
                })
                .ToListAsync();
        }
    }
}
