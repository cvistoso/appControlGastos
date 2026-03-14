using ExpenseControl.Shared.Common;
using MediatR;

namespace ExpenseControl.Application.Accounts;

public record DeleteAccountCommand(Guid Id) : IRequest<Result>;
