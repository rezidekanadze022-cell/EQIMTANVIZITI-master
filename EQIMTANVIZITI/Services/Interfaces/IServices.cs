namespace EQIMTANVIZITI.Services.Interfaces;

public interface IJwtService { AuthResponse CreateToken(User user); }
public interface IEmailService { Task SendAsync(string to, string subject, string htmlBody); }

public interface IAuthService
{
    Task<MessageResponse> RegisterAsync(RegisterRequest request);
    Task<MessageResponse> VerifyAsync(VerifyRequest request);
    Task<MessageResponse> ResendCodeAsync(ResendCodeRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
}

public interface IDoctorService
{
    Task<List<DoctorResponse>> GetAllAsync(string? specialization);
    Task<DoctorResponse> GetAsync(int id);
    Task<DoctorResponse> CreateAsync(DoctorCreateRequest request);
    Task DeleteAsync(int id);
}

public interface IReservationService
{
    Task<ReservationResponse> CreateAsync(int patientId, ReservationCreateRequest request);
    Task<List<ReservationResponse>> GetMyAsync(int patientId);
    Task<List<ReservationResponse>> GetForDoctorAsync(int doctorId);
    Task<List<ReservationResponse>> GetAllAsync();
    Task<ReservationResponse> CancelAsync(int patientId, int reservationId);
    Task<ReservationResponse> UpdateStatusAsync(int userId, bool isAdmin, int reservationId, ReservationStatus status);
}

public interface ILabTestService
{
    Task<List<LabTestResponse>> GetAllAsync(string? search);
    Task<LabTestResponse> CreateAsync(LabTestCreateRequest request);
}

public interface IPrescriptionService
{
    Task<PrescriptionResponse> CreateAsync(int doctorId, PrescriptionCreateRequest request);
    Task<List<PrescriptionResponse>> GetMyAsync(int patientId);
    Task<List<PrescriptionResponse>> GetIssuedAsync(int doctorId);
    Task<PrescriptionResponse> GetByReservationAsync(int userId, bool isAdmin, int reservationId);
}
