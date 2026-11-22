namespace EQIMTANVIZITI.Services;

public class PrescriptionService : IPrescriptionService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    public PrescriptionService(AppDbContext db, IMapper mapper) { _db = db; _mapper = mapper; }

    private IQueryable<Prescriptions> Query() => _db.Prescriptions
        .Include(p => p.User)
        .Include(p => p.Doctor).ThenInclude(d => d.User)
        .Include(p => p.LabTests);

    public async Task<PrescriptionResponse> CreateAsync(int doctorId, PrescriptionCreateRequest request)
    {
        var reservation = await _db.Reservations.Include(r => r.Prescription)
                              .FirstOrDefaultAsync(r => r.Id == request.ReservationId)
                          ?? throw new AppException("ვიზიტი ვერ მოიძებნა", 404);
        if (reservation.DoctorId != doctorId)
            throw new AppException("ეს ვიზიტი თქვენი არ არის", 403);
        if (reservation.Status == ReservationStatus.Cancelled)
            throw new AppException("გაუქმებულ ვიზიტზე რეცეპტს ვერ დაწერთ");
        if (reservation.Prescription is not null)
            throw new AppException("ამ ვიზიტზე რეცეპტი უკვე არსებობს", 409);

        var medications = (request.Medications ?? new()).Select(m => m.Trim()).Where(m => m.Length > 0).ToList();
        var labIds = (request.LabTestIds ?? new()).Distinct().ToList();
        var labTests = await _db.LabTests.Where(l => labIds.Contains(l.Id)).ToListAsync();
        if (labTests.Count != labIds.Count)
            throw new AppException("ერთ-ერთი ანალიზი ვერ მოიძებნა", 404);

        var prescription = new Prescriptions
        {
            UserId = reservation.UserId,
            DoctorId = doctorId,
            ReservationId = reservation.Id,
            Medications = medications,
            LabTests = labTests,
            Notes = request.Notes
        };
        reservation.Status = ReservationStatus.Completed;
        _db.Prescriptions.Add(prescription);
        await _db.SaveChangesAsync();

        return _mapper.Map<PrescriptionResponse>(await Query().FirstAsync(p => p.Id == prescription.Id));
    }

    public async Task<List<PrescriptionResponse>> GetMyAsync(int patientId) =>
        _mapper.Map<List<PrescriptionResponse>>(
            await Query().Where(p => p.UserId == patientId).OrderByDescending(p => p.CreatedAt).ToListAsync());

    public async Task<List<PrescriptionResponse>> GetIssuedAsync(int doctorId) =>
        _mapper.Map<List<PrescriptionResponse>>(
            await Query().Where(p => p.DoctorId == doctorId).OrderByDescending(p => p.CreatedAt).ToListAsync());

    public async Task<PrescriptionResponse> GetByReservationAsync(int userId, bool isAdmin, int reservationId)
    {
        var p = await Query().FirstOrDefaultAsync(x => x.ReservationId == reservationId)
                ?? throw new AppException("რეცეპტი ვერ მოიძებნა", 404);
        if (!isAdmin && p.UserId != userId && p.DoctorId != userId)
            throw new AppException("წვდომა აკრძალულია", 403);
        return _mapper.Map<PrescriptionResponse>(p);
    }
}
