namespace EQIMTANVIZITI.Services;

public class ReservationService : IReservationService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    public ReservationService(AppDbContext db, IMapper mapper) { _db = db; _mapper = mapper; }

    private IQueryable<Reservation> Query() => _db.Reservations
        .Include(r => r.User)
        .Include(r => r.Doctor).ThenInclude(d => d.User);

    public async Task<ReservationResponse> CreateAsync(int patientId, ReservationCreateRequest request)
    {
        var doctor = await _db.Doctors.FirstOrDefaultAsync(d => d.UserId == request.DoctorId)
                     ?? throw new AppException("ექიმი ვერ მოიძებნა", 404);

        var taken = await _db.Reservations.AnyAsync(r =>
            r.DoctorId == request.DoctorId &&
            r.ReservationDate == request.ReservationDate &&
            r.Status != ReservationStatus.Cancelled);
        if (taken) throw new AppException("ეს დრო ექიმთან უკვე დაკავებულია", 409);

        var reservation = new Reservation
        {
            UserId = patientId,
            DoctorId = doctor.UserId,
            ReservationDate = request.ReservationDate,
            Description = request.Description ?? string.Empty,
            TotalPrice = doctor.ConsultationPrice
        };
        _db.Reservations.Add(reservation);
        await _db.SaveChangesAsync();
        return await GetAsync(reservation.Id);
    }

    public async Task<List<ReservationResponse>> GetMyAsync(int patientId) =>
        _mapper.Map<List<ReservationResponse>>(
            await Query().Where(r => r.UserId == patientId).OrderByDescending(r => r.ReservationDate).ToListAsync());

    public async Task<List<ReservationResponse>> GetForDoctorAsync(int doctorId) =>
        _mapper.Map<List<ReservationResponse>>(
            await Query().Where(r => r.DoctorId == doctorId).OrderByDescending(r => r.ReservationDate).ToListAsync());

    public async Task<List<ReservationResponse>> GetAllAsync() =>
        _mapper.Map<List<ReservationResponse>>(await Query().OrderByDescending(r => r.ReservationDate).ToListAsync());

    public async Task<ReservationResponse> CancelAsync(int patientId, int reservationId)
    {
        var r = await _db.Reservations.FirstOrDefaultAsync(x => x.Id == reservationId && x.UserId == patientId)
                ?? throw new AppException("ვიზიტი ვერ მოიძებნა", 404);
        if (r.Status is ReservationStatus.Completed or ReservationStatus.Cancelled)
            throw new AppException("ამ ვიზიტის გაუქმება შეუძლებელია");
        r.Status = ReservationStatus.Cancelled;
        await _db.SaveChangesAsync();
        return await GetAsync(r.Id);
    }

    public async Task<ReservationResponse> UpdateStatusAsync(int userId, bool isAdmin, int reservationId, ReservationStatus status)
    {
        var r = await _db.Reservations.FindAsync(reservationId)
                ?? throw new AppException("ვიზიტი ვერ მოიძებნა", 404);
        if (!isAdmin && r.DoctorId != userId)
            throw new AppException("ეს ვიზიტი თქვენი არ არის", 403);
        if (r.Status is ReservationStatus.Completed or ReservationStatus.Cancelled)
            throw new AppException("დასრულებული ან გაუქმებული ვიზიტის სტატუსი ვერ შეიცვლება");
        r.Status = status;
        await _db.SaveChangesAsync();
        return await GetAsync(r.Id);
    }

    private async Task<ReservationResponse> GetAsync(int id) =>
        _mapper.Map<ReservationResponse>(await Query().FirstAsync(r => r.Id == id));
}
