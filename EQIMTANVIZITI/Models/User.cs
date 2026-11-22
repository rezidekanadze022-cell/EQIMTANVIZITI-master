using EQIMTANVIZITI.Models.Entity;

namespace EQIMTANVIZITI.Models;

public class User : BaseEntity
{
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Code { get; set; } = string.Empty;
    public DateTime? CodeExpiresAt { get; set; }

    public string Password { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public int Age { get; set; }

    public bool IsVerified { get; set; } = false;

    public Role Role { get; set; }

    public Doctor? DoctorProfile { get; set; }

    public List<Reservation> Reservations { get; set; } = new();
    public List<Prescriptions> Prescriptions { get; set; } = new();
}