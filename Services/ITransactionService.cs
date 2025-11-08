using ExpenseControlApp.Models.DTOs;

namespace ExpenseControlApp.Services
{
    public interface ITransactionService
    {
        Task<IEnumerable<TransactionDto>> GetTransactionsAsync(int userId, DateTime? startDate = null, DateTime? endDate = null, string? categoryId = null, string? type = null);
        Task<TransactionDto?> GetTransactionByIdAsync(int id, int userId);
        Task<TransactionDto> CreateTransactionAsync(CreateTransactionRequest request, int userId);
        Task<TransactionDto?> UpdateTransactionAsync(int id, CreateTransactionRequest request, int userId);
        Task<bool> DeleteTransactionAsync(int id, int userId);
        Task<IEnumerable<CategorySummaryDto>> GetCategorySummaryAsync(int userId, DateTime? startDate = null, DateTime? endDate = null);
    }
}

