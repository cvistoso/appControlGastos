using ExpenseControl.Application.Dashboard;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseControl.Api.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
[Produces("application/json")]
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [ProducesResponseType(typeof(DashboardDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetDashboardQuery(fromDate, toDate), cancellationToken);
        if (!result.IsSuccess)
            return result.ErrorCode == "UNAUTHORIZED" ? Unauthorized() : BadRequest(new { code = result.ErrorCode, message = result.Message });
        return Ok(result.Value);
    }
}
