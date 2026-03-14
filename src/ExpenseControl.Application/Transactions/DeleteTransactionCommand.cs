using ExpenseControl.Shared.Common;
using MediatR;

namespace ExpenseControl.Application.Transactions;

public record DeleteTransactionCommand(Guid Id) : IRequest<Result>;
