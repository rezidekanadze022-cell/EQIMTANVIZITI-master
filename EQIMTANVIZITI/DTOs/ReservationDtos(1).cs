namespace EQIMTANVIZITI.DTOs;

public record ReservationCreateRequest(int DoctorId, DateTime ReservationDate, string? Description);
public record ReservationStatusUpdateRequest(ReservationStatus Status);

public class ReservationResponse
{
    public int Id { get; set; }
    public int DoctorId { get; set; }
    public string DoctorName { get; set; } = "";
    public string Specialization { get; set; } = "";
    public int PatientId { get; set; }
    public string PatientName { get; set; } = "";
    public DateTime ReservationDate { get; set; }
    public string Description { get; set; } = "";
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}
