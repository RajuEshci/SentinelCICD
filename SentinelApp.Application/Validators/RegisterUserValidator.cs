using FluentValidation;
using SentinelApp.Application.DTO;

namespace SentinelApp.Application.Validators
{
    public class RegisterUserValidator : AbstractValidator<RegisterDto>
    {
        public RegisterUserValidator()
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
                .NotEmpty().WithMessage("Date of birth is required.")
                .LessThan(DateTime.Today).WithMessage("Date of birth must be in the past.")
                .Must(BeAtLeast10YearsOld).WithMessage("You must be at least 10 years old.");

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^[0-9]{10,15}$")
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber))
                .WithMessage("Phone number must contain 10 to 15 digits.");

            RuleFor(x => x.MobileNumber)
                //.NotEmpty().WithMessage("Mobile number is required.")
                .Matches(@"^[0-9]{10,15}$")
				.When(x => !string.IsNullOrWhiteSpace(x.MobileNumber))
				.WithMessage("Mobile number must contain 10 to 15 digits.");

            RuleFor(x => x.EmailId)
                .NotEmpty().WithMessage("Email is required.")
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
                //.NotEmpty().WithMessage("Country is required.")
                .MaximumLength(100);

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.");
                //.MinimumLength(8).WithMessage("Password must be at least 8 characters.")
                //.MaximumLength(100).WithMessage("Password cannot exceed 100 characters.")
                //.Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                //.Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                //.Matches("[0-9]").WithMessage("Password must contain at least one number.")
                //.Matches(@"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>/?]")
                //.WithMessage("Password must contain at least one special character.")
                //.Must(p => !p.Contains(" "))
                //.WithMessage("Password cannot contain spaces.");

            RuleFor(x => x.IsTnCChecked)
                .Equal(true)
                .WithMessage("You must accept the Terms and Conditions.");

            RuleFor(x => x.AccessCode)
                .NotEmpty().WithMessage("Access code is required.")
                .MaximumLength(100);
        }

        private bool BeAtLeast10YearsOld(DateTime dob)
        {
            return dob <= DateTime.Today.AddYears(-10);
        }
    }
}