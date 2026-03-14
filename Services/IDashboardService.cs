using ExpenseControlApp.Models.DTOs;

namespace ExpenseControlApp.Services
{
    public interface IDashboardService
    {
        Task<DashboardDto> GetDashboardDataAsync(int userId, DateTime? startDate = null, DateTime? endDate = null);
        Task<IEnumerable<MonthlyTrendDto>> GetMonthlyTrendsAsync(int userId, int months = 12);
        Task<IEnumerable<ProjectionDto>> GetCashFlowProjectionsAsync(int userId, int months = 6);
    }
}

