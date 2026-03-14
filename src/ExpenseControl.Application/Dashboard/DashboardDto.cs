namespace ExpenseControl.Application.Dashboard;

public record DashboardDto(
    decimal TotalIncome,
    decimal TotalExpenses,
    decimal CurrentBalance,
    decimal MonthlyIncome,
    decimal MonthlyExpenses,
    decimal MonthlyBalance,
    IReadOnlyList<TransactionSummaryDto> RecentTransactions);

public record TransactionSummaryDto(
    Guid Id,
    string Description,
    decimal Amount,
    string Type,
    DateTime TransactionDateUtc,
    string? CategoryName);
