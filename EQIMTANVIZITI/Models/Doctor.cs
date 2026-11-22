namespace EQIMTANVIZITI.Models;

public class Doctor
{

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public string Specialization { get; set; } = string.Empty;
    public decimal ConsultationPrice { get; set; }


    public List<Reservation> Reservations { get; set; } = new();
    public List<Prescriptions> Prescriptions { get; set; } = new();
}
