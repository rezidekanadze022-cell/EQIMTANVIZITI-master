using FluentValidation;

namespace EQIMTANVIZITI.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(150);
        RuleFor(x => x.PhoneNumber).NotEmpty().Matches(@"^\+?[0-9]{9,15}$").WithMessage("ტელეფონის ფორმატი არასწორია");
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);

        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.FullName) || (!string.IsNullOrWhiteSpace(x.FirstName) && !string.IsNullOrWhiteSpace(x.LastName)))
            .WithMessage("მიუთითეთ სრული სახელი (FullName) ან სახელი და გვარი (FirstName, LastName)");
    }
}

public class VerifyRequestValidator : AbstractValidator<VerifyRequest>
{
    public VerifyRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Code).NotEmpty().Matches(@"^[0-9]{6}$").WithMessage("კოდი უნდა იყოს 6 ციფრი");
    }
}

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}

public class ResendCodeRequestValidator : AbstractValidator<ResendCodeRequest>
{
    public ResendCodeRequestValidator() => RuleFor(x => x.Email).NotEmpty().EmailAddress();
}

public class DoctorCreateRequestValidator : AbstractValidator<DoctorCreateRequest>
{
    public DoctorCreateRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(150);
        RuleFor(x => x.PhoneNumber).NotEmpty().Matches(@"^\+?[0-9]{9,15}$");
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        RuleFor(x => x.Specialization).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ConsultationPrice).GreaterThanOrEqualTo(0);
    }
}

public class ReservationCreateRequestValidator : AbstractValidator<ReservationCreateRequest>
{
    public ReservationCreateRequestValidator()
    {
        RuleFor(x => x.DoctorId).GreaterThan(0);
        RuleFor(x => x.ReservationDate).Must(d => d.ToUniversalTime() > DateTime.UtcNow)
            .WithMessage("ვიზიტის თარიღი უნდა იყოს მომავალში");
        RuleFor(x => x.Description).MaximumLength(500);
    }
}

public class ReservationStatusUpdateRequestValidator : AbstractValidator<ReservationStatusUpdateRequest>
{
    public ReservationStatusUpdateRequestValidator() => RuleFor(x => x.Status).IsInEnum();
}

public class LabTestCreateRequestValidator : AbstractValidator<LabTestCreateRequest>
{
    public LabTestCreateRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Description).MaximumLength(500);
    }
}

public class PrescriptionCreateRequestValidator : AbstractValidator<PrescriptionCreateRequest>
{
    public PrescriptionCreateRequestValidator()
    {
        RuleFor(x => x.ReservationId).GreaterThan(0);
        RuleFor(x => x.Notes).MaximumLength(1000);
        RuleFor(x => x).Must(x => (x.Medications?.Count ?? 0) > 0 || (x.LabTestIds?.Count ?? 0) > 0)
            .WithName("Items").WithMessage("მიუთითე მინიმუმ ერთი წამალი ან ანალიზი");
        RuleForEach(x => x.Medications).NotEmpty().MaximumLength(200);
    }
}
