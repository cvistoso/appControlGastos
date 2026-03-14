namespace ExpenseControlApp.Models.DTOs
{
    public class DashboardDto
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal CurrentBalance { get; set; }
        public decimal MonthlyIncome { get; set; }
        public decimal MonthlyExpenses { get; set; }
        public decimal MonthlyBalance { get; set; }
        public List<CategorySummaryDto> TopExpenseCategories { get; set; } = new List<CategorySummaryDto>();
        public List<TransactionDto> RecentTransactions { get; set; } = new List<TransactionDto>();
        public List<MonthlyTrendDto> MonthlyTrends { get; set; } = new List<MonthlyTrendDto>();
        public List<ProjectionDto> CashFlowProjections { get; set; } = new List<ProjectionDto>();
    }

    public class CategorySummaryDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string CategoryColor { get; set; } = string.Empty;
        public string CategoryIcon { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public int TransactionCount { get; set; }
        public decimal Percentage { get; set; }
    }

    public class MonthlyTrendDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public decimal Income { get; set; }
        public decimal Expenses { get; set; }
        public decimal Balance { get; set; }
    }

    public class ProjectionDto
    {
        public DateTime Date { get; set; }
        public decimal ProjectedIncome { get; set; }
        public decimal ProjectedExpenses { get; set; }
        public decimal ProjectedBalance { get; set; }
    }
}

