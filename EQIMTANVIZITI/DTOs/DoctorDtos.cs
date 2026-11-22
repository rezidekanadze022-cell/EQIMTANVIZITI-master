namespace EQIMTANVIZITI.DTOs;

public record DoctorCreateRequest(string FullName, string Email, string PhoneNumber,
    string Password, string Specialization, decimal ConsultationPrice);

public class DoctorResponse
{
    public int Id { get; set; }              // = Doctor.UserId
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string PhoneNumber { get; set; } = "";
    public string Specialization { get; set; } = "";
    public decimal ConsultationPrice { get; set; }
}