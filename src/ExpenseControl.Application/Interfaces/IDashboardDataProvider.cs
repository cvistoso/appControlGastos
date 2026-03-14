namespace ExpenseControl.Application.Interfaces;

public interface IDashboardDataProvider
{
    Task<Dashboard.DashboardDto> GetAsync(Guid userId, DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default);
}
