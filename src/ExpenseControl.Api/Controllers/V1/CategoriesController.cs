using ExpenseControl.Application.Categories;
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
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<CategoryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] CategoryType? type = null, [FromQuery] bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new ListCategoriesQuery(page, pageSize, type, includeInactive), cancellationToken);
        if (!result.IsSuccess)
            return result.ErrorCode == "UNAUTHORIZED" ? Unauthorized() : BadRequest(new { code = result.ErrorCode, message = result.Message });
        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCategoryQuery(id), cancellationToken);
        if (!result.IsSuccess)
            return result.ErrorCode == "UNAUTHORIZED" ? Unauthorized() : result.ErrorCode == "NOT_FOUND" ? NotFound(new { code = result.ErrorCode, message = result.Message }) : BadRequest(new { code = result.ErrorCode, message = result.Message });
        return Ok(result.Value);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateCategoryCommand(
            request.Name,
            request.Description,
            request.Type,
            request.Color,
            request.Icon,
            request.SortOrder,
            request.ParentCategoryId), cancellationToken);
        if (!result.IsSuccess)
            return result.ErrorCode == "UNAUTHORIZED" ? Unauthorized() : BadRequest(new { code = result.ErrorCode, message = result.Message });
        return CreatedAtAction(nameof(Get), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateCategoryCommand(
            id,
            request.Name,
            request.Description,
            request.Type,
            request.Color,
            request.Icon,
            request.SortOrder,
            request.IsActive), cancellationToken);
        if (!result.IsSuccess)
            return result.ErrorCode == "UNAUTHORIZED" ? Unauthorized() : result.ErrorCode == "NOT_FOUND" ? NotFound(new { code = result.ErrorCode, message = result.Message }) : BadRequest(new { code = result.ErrorCode, message = result.Message });
        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteCategoryCommand(id), cancellationToken);
        if (!result.IsSuccess)
            return result.ErrorCode == "UNAUTHORIZED" ? Unauthorized() : result.ErrorCode == "NOT_FOUND" ? NotFound(new { code = result.ErrorCode, message = result.Message }) : BadRequest(new { code = result.ErrorCode, message = result.Message });
        return NoContent();
    }
}

public record CreateCategoryRequest(string Name, string? Description, CategoryType Type, string? Color, string? Icon, int SortOrder = 0, Guid? ParentCategoryId = null);
public record UpdateCategoryRequest(string Name, string? Description, CategoryType Type, string? Color, string? Icon, int SortOrder, bool IsActive = true);
