using ExpenseControl.Application.Transactions;
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
public class TransactionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TransactionsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<TransactionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] TransactionType? type = null,
        [FromQuery] Guid? accountId = null,
        [FromQuery] Guid? categoryId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new ListTransactionsQuery(page, pageSize, type, accountId, categoryId, fromDate, toDate), cancellationToken);
        if (!result.IsSuccess)
            return result.ErrorCode == "UNAUTHORIZED" ? Unauthorized() : BadRequest(new { code = result.ErrorCode, message = result.Message });
        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetTransactionQuery(id), cancellationToken);
        if (!result.IsSuccess)
            return result.ErrorCode == "UNAUTHORIZED" ? Unauthorized() : result.ErrorCode == "NOT_FOUND" ? NotFound(new { code = result.ErrorCode, message = result.Message }) : BadRequest(new { code = result.ErrorCode, message = result.Message });
        return Ok(result.Value);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateTransactionRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateTransactionCommand(
            request.AccountId,
            request.TransferAccountId,
            request.CategoryId,
            request.Type,
            request.Description,
            request.Amount,
            request.Currency ?? "USD",
            request.TransactionDateUtc,
            request.ValueDateUtc,
            request.Notes,
            request.PaymentMethod,
            request.Merchant), cancellationToken);
        if (!result.IsSuccess)
            return result.ErrorCode == "UNAUTHORIZED" ? Unauthorized() : BadRequest(new { code = result.ErrorCode, message = result.Message });
        return CreatedAtAction(nameof(Get), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTransactionRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateTransactionCommand(
            id,
            request.AccountId,
            request.TransferAccountId,
            request.CategoryId,
            request.Type,
            request.Description,
            request.Amount,
            request.Currency ?? "USD",
            request.TransactionDateUtc,
            request.ValueDateUtc,
            request.Notes,
            request.PaymentMethod,
            request.Merchant,
            request.IsReconciled), cancellationToken);
        if (!result.IsSuccess)
            return result.ErrorCode == "UNAUTHORIZED" ? Unauthorized() : result.ErrorCode == "NOT_FOUND" ? NotFound(new { code = result.ErrorCode, message = result.Message }) : BadRequest(new { code = result.ErrorCode, message = result.Message });
        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteTransactionCommand(id), cancellationToken);
        if (!result.IsSuccess)
            return result.ErrorCode == "UNAUTHORIZED" ? Unauthorized() : result.ErrorCode == "NOT_FOUND" ? NotFound(new { code = result.ErrorCode, message = result.Message }) : BadRequest(new { code = result.ErrorCode, message = result.Message });
        return NoContent();
    }
}

public record CreateTransactionRequest(
    Guid AccountId,
    Guid? TransferAccountId,
    Guid CategoryId,
    TransactionType Type,
    string Description,
    decimal Amount,
    string? Currency,
    DateTime TransactionDateUtc,
    DateTime? ValueDateUtc = null,
    string? Notes = null,
    string? PaymentMethod = null,
    string? Merchant = null);

public record UpdateTransactionRequest(
    Guid AccountId,
    Guid? TransferAccountId,
    Guid CategoryId,
    TransactionType Type,
    string Description,
    decimal Amount,
    string? Currency,
    DateTime TransactionDateUtc,
    DateTime? ValueDateUtc,
    string? Notes,
    string? PaymentMethod,
    string? Merchant,
    bool IsReconciled = false);
