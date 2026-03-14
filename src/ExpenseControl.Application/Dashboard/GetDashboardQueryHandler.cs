using ExpenseControl.Application.Interfaces;
using ExpenseControl.Shared.Common;
using MediatR;

namespace ExpenseControl.Application.Dashboard;

public class GetDashboardQueryHandler(IDashboardDataProvider dashboardDataProvider, IUserContext userContext)
    : IRequestHandler<GetDashboardQuery, Result<DashboardDto>>
{
    public async Task<Result<DashboardDto>> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
    {
        if (userContext.UserId is null)
            return Result<DashboardDto>.Failure(ErrorCodes.Unauthorized, "Not authenticated.");
        var dto = await dashboardDataProvider.GetAsync(userContext.UserId.Value, request.FromDate, request.ToDate, cancellationToken);
        return Result<DashboardDto>.Success(dto);
    }
}
