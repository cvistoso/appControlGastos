using ExpenseControl.Shared.Common;
using MediatR;

namespace ExpenseControl.Application.Transactions;

public record GetTransactionQuery(Guid Id) : IRequest<Result<TransactionDto>>;
