namespace EQIMTANVIZITI.DTOs;

public record LabTestCreateRequest(string Name, string? Description);
public class LabTestResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
}

public record PrescriptionCreateRequest(int ReservationId, List<string>? Medications, List<int>? LabTestIds, string? Notes);
public class PrescriptionResponse
{
    public int Id { get; set; }
    public int ReservationId { get; set; }
    public string DoctorName { get; set; } = "";
    public string PatientName { get; set; } = "";
    public List<string> Medications { get; set; } = new();
    public List<LabTestResponse> LabTests { get; set; } = new();
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}
