using ExpenseControlApp.Models.DTOs;

namespace ExpenseControlApp.Services
{
    public interface IReportService
    {
        Task<byte[]> GeneratePdfReportAsync(int userId, DateTime startDate, DateTime endDate);
        Task<byte[]> GenerateExcelReportAsync(int userId, DateTime startDate, DateTime endDate);
        Task<IEnumerable<TransactionDto>> GetTransactionsForReportAsync(int userId, DateTime startDate, DateTime endDate);
    }
}

