using ExpenseControl.Shared.Common;
using MediatR;

namespace ExpenseControl.Application.Dashboard;

public record GetDashboardQuery(DateTime? FromDate = null, DateTime? ToDate = null) : IRequest<Result<DashboardDto>>;
