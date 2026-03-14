using Microsoft.EntityFrameworkCore;
using ExpenseControlApp.Data;
using ExpenseControlApp.Models;
using ExpenseControlApp.Models.DTOs;

namespace ExpenseControlApp.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ExpenseDbContext _context;

        public TransactionService(ExpenseDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TransactionDto>> GetTransactionsAsync(int userId, DateTime? startDate = null, DateTime? endDate = null, string? categoryId = null, string? type = null)
        {
            var query = _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId);

            if (startDate.HasValue)
                query = query.Where(t => t.Date >= startDate.Value.ToUniversalTime());

            if (endDate.HasValue)
                query = query.Where(t => t.Date <= endDate.Value.ToUniversalTime());

            if (!string.IsNullOrEmpty(categoryId) && int.TryParse(categoryId, out int catId))
                query = query.Where(t => t.CategoryId == catId);

            if (!string.IsNullOrEmpty(type) && Enum.TryParse<TransactionType>(type, true, out TransactionType transactionType))
                query = query.Where(t => t.Type == transactionType);

            var transactions = await query
                .OrderByDescending(t => t.Date)
                .ThenByDescending(t => t.CreatedAt)
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

            return transactions;
        }

        public async Task<TransactionDto?> GetTransactionByIdAsync(int id, int userId)
        {
            var transaction = await _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.Id == id && t.UserId == userId)
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
                .FirstOrDefaultAsync();

            return transaction;
        }

        public async Task<TransactionDto> CreateTransactionAsync(CreateTransactionRequest request, int userId)
        {
            var transaction = new Transaction
            {
                Description = request.Description,
                Amount = request.Amount,
                Type = Enum.Parse<TransactionType>(request.Type, true),
                Date = request.Date,
                Notes = request.Notes,
                Location = request.Location,
                PaymentMethod = request.PaymentMethod,
                IsRecurring = request.IsRecurring,
                RecurringFrequency = request.RecurringFrequency,
                UserId = userId,
                CategoryId = request.CategoryId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            if (request.IsRecurring && !string.IsNullOrEmpty(request.RecurringFrequency))
            {
                transaction.NextRecurringDate = CalculateNextRecurringDate(request.Date, request.RecurringFrequency);
            }

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            var category = await _context.Categories.FindAsync(transaction.CategoryId);

            return new TransactionDto
            {
                Id = transaction.Id,
                Description = transaction.Description,
                Amount = transaction.Amount,
                Type = transaction.Type.ToString(),
                Date = transaction.Date,
                Notes = transaction.Notes,
                Location = transaction.Location,
                PaymentMethod = transaction.PaymentMethod,
                IsRecurring = transaction.IsRecurring,
                RecurringFrequency = transaction.RecurringFrequency,
                NextRecurringDate = transaction.NextRecurringDate,
                CategoryId = transaction.CategoryId,
                CategoryName = category?.Name ?? string.Empty,
                CategoryColor = category?.Color ?? string.Empty,
                CategoryIcon = category?.Icon ?? string.Empty
            };
        }

        public async Task<TransactionDto?> UpdateTransactionAsync(int id, CreateTransactionRequest request, int userId)
        {
            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (transaction == null)
                return null;

            transaction.Description = request.Description;
            transaction.Amount = request.Amount;
            transaction.Type = Enum.Parse<TransactionType>(request.Type, true);
            transaction.Date = request.Date;
            transaction.Notes = request.Notes;
            transaction.Location = request.Location;
            transaction.PaymentMethod = request.PaymentMethod;
            transaction.IsRecurring = request.IsRecurring;
            transaction.RecurringFrequency = request.RecurringFrequency;
            transaction.CategoryId = request.CategoryId;
            transaction.UpdatedAt = DateTime.UtcNow;

            if (request.IsRecurring && !string.IsNullOrEmpty(request.RecurringFrequency))
            {
                transaction.NextRecurringDate = CalculateNextRecurringDate(request.Date, request.RecurringFrequency);
            }
            else
            {
                transaction.NextRecurringDate = null;
            }

            await _context.SaveChangesAsync();

            var category = await _context.Categories.FindAsync(transaction.CategoryId);

            return new TransactionDto
            {
                Id = transaction.Id,
                Description = transaction.Description,
                Amount = transaction.Amount,
                Type = transaction.Type.ToString(),
                Date = transaction.Date,
                Notes = transaction.Notes,
                Location = transaction.Location,
                PaymentMethod = transaction.PaymentMethod,
                IsRecurring = transaction.IsRecurring,
                RecurringFrequency = transaction.RecurringFrequency,
                NextRecurringDate = transaction.NextRecurringDate,
                CategoryId = transaction.CategoryId,
                CategoryName = category?.Name ?? string.Empty,
                CategoryColor = category?.Color ?? string.Empty,
                CategoryIcon = category?.Icon ?? string.Empty
            };
        }

        public async Task<bool> DeleteTransactionAsync(int id, int userId)
        {
            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (transaction == null)
                return false;

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<CategorySummaryDto>> GetCategorySummaryAsync(int userId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId);

            if (startDate.HasValue)
                query = query.Where(t => t.Date >= startDate.Value.ToUniversalTime());

            if (endDate.HasValue)
                query = query.Where(t => t.Date <= endDate.Value.ToUniversalTime());

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
                .ToListAsync();

            var totalAmount = summary.Sum(s => s.TotalAmount);

            foreach (var item in summary)
            {
                item.Percentage = totalAmount > 0 ? (item.TotalAmount / totalAmount) * 100 : 0;
            }

            return summary;
        }

        private DateTime CalculateNextRecurringDate(DateTime currentDate, string frequency)
        {
            return frequency.ToLower() switch
            {
                "daily" => currentDate.AddDays(1),
                "weekly" => currentDate.AddDays(7),
                "monthly" => currentDate.AddMonths(1),
                "yearly" => currentDate.AddYears(1),
                _ => currentDate.AddDays(1)
            };
        }
    }
}
