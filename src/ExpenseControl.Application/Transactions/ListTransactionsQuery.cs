using ExpenseControl.Domain.Enums;
using ExpenseControl.Shared.Common;
using MediatR;

namespace ExpenseControl.Application.Transactions;

public record ListTransactionsQuery(
    int Page = 1,
    int PageSize = 20,
    TransactionType? Type = null,
    Guid? AccountId = null,
    Guid? CategoryId = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null) : IRequest<Result<PagedResult<TransactionDto>>>;
