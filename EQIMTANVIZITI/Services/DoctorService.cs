namespace EQIMTANVIZITI.Services;

public class DoctorService : IDoctorService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    public DoctorService(AppDbContext db, IMapper mapper) { _db = db; _mapper = mapper; }

    public async Task<List<DoctorResponse>> GetAllAsync(string? specialization)
    {
        var q = _db.Doctors.Include(d => d.User).AsQueryable();
        if (!string.IsNullOrWhiteSpace(specialization))
            q = q.Where(d => d.Specialization.Contains(specialization));
        return _mapper.Map<List<DoctorResponse>>(await q.OrderBy(d => d.User.FullName).ToListAsync());
    }

    public async Task<DoctorResponse> GetAsync(int id)
    {
        var doctor = await _db.Doctors.Include(d => d.User).FirstOrDefaultAsync(d => d.UserId == id)
                     ?? throw new AppException("ექიმი ვერ მოიძებნა", 404);
        return _mapper.Map<DoctorResponse>(doctor);
    }

    public async Task<DoctorResponse> CreateAsync(DoctorCreateRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        if (await _db.Users.AnyAsync(u => u.Email == email))
            throw new AppException("ამ ელფოსტით მომხმარებელი უკვე არსებობს", 409);

        // ექიმი = User (Role.Doctor) + Doctor პროფილი (one-to-one)
        var user = new User
        {
            FullName = request.FullName,
            Email = email,
            PhoneNumber = request.PhoneNumber,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = Role.Doctor,
            IsVerified = true,
            DoctorProfile = new Doctor
            {
                Specialization = request.Specialization,
                ConsultationPrice = request.ConsultationPrice
            }
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return await GetAsync(user.Id);
    }

    public async Task DeleteAsync(int id)
    {
        var user = await _db.Users.Include(u => u.DoctorProfile)
                       .FirstOrDefaultAsync(u => u.Id == id && u.Role == Role.Doctor)
                   ?? throw new AppException("ექიმი ვერ მოიძებნა", 404);
        if (await _db.Reservations.AnyAsync(r => r.DoctorId == id))
            throw new AppException("ექიმს აქვს ვიზიტები, წაშლა შეუძლებელია", 409);
        _db.Users.Remove(user); // Doctor პროფილიც cascade-ით იშლება
        await _db.SaveChangesAsync();
    }
}