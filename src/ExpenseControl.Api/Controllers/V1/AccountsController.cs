using ExpenseControl.Application.Accounts;
using ExpenseControl.Domain.Enums;
using ExpenseControl.Shared.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseControl.Api.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
[Produces("application/json")]
public class AccountsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AccountsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AccountDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new ListAccountsQuery(page, pageSize, includeInactive), cancellationToken);
        if (!result.IsSuccess)
            return result.ErrorCode == "UNAUTHORIZED" ? Unauthorized() : BadRequest(new { code = result.ErrorCode, message = result.Message });
        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAccountQuery(id), cancellationToken);
        if (!result.IsSuccess)
            return result.ErrorCode == "UNAUTHORIZED" ? Unauthorized() : result.ErrorCode == "NOT_FOUND" ? NotFound(new { code = result.ErrorCode, message = result.Message }) : BadRequest(new { code = result.ErrorCode, message = result.Message });
        return Ok(result.Value);
    }

    [HttpPost]
    [ProducesResponseType(typeof(AccountDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateAccountRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateAccountCommand(request.Name, request.Type, request.Currency, request.InitialBalance), cancellationToken);
        if (!result.IsSuccess)
            return result.ErrorCode == "UNAUTHORIZED" ? Unauthorized() : BadRequest(new { code = result.ErrorCode, message = result.Message });
        return CreatedAtAction(nameof(Get), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(AccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAccountRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateAccountCommand(id, request.Name, request.Type, request.Currency, request.InitialBalance, request.IsActive), cancellationToken);
        if (!result.IsSuccess)
            return result.ErrorCode == "UNAUTHORIZED" ? Unauthorized() : result.ErrorCode == "NOT_FOUND" ? NotFound(new { code = result.ErrorCode, message = result.Message }) : BadRequest(new { code = result.ErrorCode, message = result.Message });
        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteAccountCommand(id), cancellationToken);
        if (!result.IsSuccess)
            return result.ErrorCode == "UNAUTHORIZED" ? Unauthorized() : result.ErrorCode == "NOT_FOUND" ? NotFound(new { code = result.ErrorCode, message = result.Message }) : BadRequest(new { code = result.ErrorCode, message = result.Message });
        return NoContent();
    }
}

public record CreateAccountRequest(string Name, AccountType Type, string Currency = "USD", decimal InitialBalance = 0);
public record UpdateAccountRequest(string Name, AccountType Type, string Currency, decimal InitialBalance, bool IsActive = true);
