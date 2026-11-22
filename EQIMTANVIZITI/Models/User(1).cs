using System.ComponentModel.DataAnnotations.Schema;
using EQIMTANVIZITI.Models.Entity;

namespace EQIMTANVIZITI.Models;

public class User : BaseEntity
{
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    // verify-ს 6-ნიშნა კოდი
    public string? Code { get; set; } = string.Empty;
    public DateTime? CodeExpiresAt { get; set; }

    // აქ ინახება bcrypt hash (არა ჩვეულებრივი პაროლი)
    public string Password { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public bool IsVerified { get; set; } = false;

    public Role Role { get; set; }

    // one-to-one: User - Doctor
    public Doctor? DoctorProfile { get; set; }

    // one-to-many: პაციენტის ვიზიტები და რეცეპტები
    public List<Reservation> Reservations { get; set; } = new();
    public List<Prescriptions> Prescriptions { get; set; } = new();

    // Helper properties for WebApplication1 compatibility
    private string? _fullName;

    [NotMapped]
    public string FullName
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(_fullName))
                return _fullName;
            var combined = $"{FirstName} {LastName}".Trim();
            return string.IsNullOrWhiteSpace(combined) ? Email : combined;
        }
        set
        {
            _fullName = value;
            if (!string.IsNullOrWhiteSpace(value))
            {
                var parts = value.Trim().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 0 && string.IsNullOrWhiteSpace(FirstName))
                    FirstName = parts[0];
                if (parts.Length > 1 && string.IsNullOrWhiteSpace(LastName))
                    LastName = parts[1];
                else if (string.IsNullOrWhiteSpace(LastName))
                    LastName = parts[0];
            }
        }
    }

    [NotMapped]
    public int Age { get; set; }
}
