using ExpenseControl.Domain.Enums;
using FluentValidation;

namespace ExpenseControl.Application.Accounts;

public class CreateAccountCommandValidator : AbstractValidator<CreateAccountCommand>
{
    public CreateAccountCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.Currency).NotEmpty().Length(3);
    }
}
