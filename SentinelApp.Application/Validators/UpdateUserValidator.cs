using FluentValidation;
using SentinelApp.Application.DTO;

namespace SentinelApp.Application.Validators
{
    public class UpdateUserValidator : AbstractValidator<UpdateUserDto>
    {
        public UpdateUserValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Username is required.")
                .MinimumLength(3).WithMessage("Username must be at least 3 characters.")
                .MaximumLength(50).WithMessage("Username cannot exceed 50 characters.");

            RuleFor(x => x.Gender)
                .NotEmpty().WithMessage("Gender is required.")
                .Must(x => x == "M" || x == "F")
                .WithMessage("Gender must be Male or Female.");

            RuleFor(x => x.DateOfBirth)
                .LessThan(DateTime.Today).WithMessage("Date of birth must be in the past.");
                //.Must(BeAtLeast18YearsOld).WithMessage("You must be at least 18 years old.");

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^[0-9]{10,15}$")
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber))
                .WithMessage("Phone number must contain 10 to 15 digits.");

            RuleFor(x => x.MobileNumber)
                .NotEmpty().WithMessage("Mobile number is required.")
                .Matches(@"^[0-9]{10,15}$")
                .WithMessage("Mobile number must contain 10 to 15 digits.");

            RuleFor(x => x.EmailId)
                .EmailAddress().WithMessage("Please enter a valid email address.")
                .MaximumLength(100).WithMessage("Email cannot exceed 100 characters.");

            RuleFor(x => x.Address)
                //.NotEmpty().WithMessage("Address is required.")
                .MaximumLength(250).WithMessage("Address cannot exceed 250 characters.");

			RuleFor(x => x.PostCode)
	          .Matches(@"^[A-Za-z0-9\s\-]{3,10}$")
	          .When(x => !string.IsNullOrWhiteSpace(x.PostCode))
	          .WithMessage("Invalid post code.");

			RuleFor(x => x.Country)
                .MaximumLength(100);

        }

        private bool BeAtLeast18YearsOld(DateTime dob)
        {
            return dob <= DateTime.Today.AddYears(-18);
        }
    }
}