using ExpenseControl.Domain.Enums;
using FluentValidation;

namespace ExpenseControl.Application.Categories;

public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.Color).MaximumLength(20).When(x => !string.IsNullOrEmpty(x.Color));
        RuleFor(x => x.Icon).MaximumLength(50).When(x => !string.IsNullOrEmpty(x.Icon));
        RuleFor(x => x.SortOrder).GreaterThanOrEqualTo(0);
    }
}
