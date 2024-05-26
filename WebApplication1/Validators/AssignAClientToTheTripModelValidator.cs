using FluentValidation;
using WebApplication1.RequestModels;

namespace WebApplication1.Validators;

public class AssignAClientToTheTripModelValidator : AbstractValidator<AssignAClientToTheTripRequestModel>
{
    public AssignAClientToTheTripModelValidator()
    {
        RuleFor(e => e.FirstName).NotEmpty().NotNull().MaximumLength(120);
        RuleFor(e => e.LastName).NotEmpty().NotNull().MaximumLength(120);
        RuleFor(e => e.Email).NotEmpty().NotNull().MaximumLength(120);
        RuleFor(e => e.Telephone).NotEmpty().NotNull().MaximumLength(120);
        RuleFor(e => e.Pesel).NotEmpty().NotNull().MaximumLength(120);
    }
}