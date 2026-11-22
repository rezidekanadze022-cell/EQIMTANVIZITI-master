namespace EQIMTANVIZITI.Services;

public class LabTestService : ILabTestService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    public LabTestService(AppDbContext db, IMapper mapper) { _db = db; _mapper = mapper; }

    public async Task<List<LabTestResponse>> GetAllAsync(string? search)
    {
        var q = _db.LabTests.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search)) q = q.Where(l => l.Name.Contains(search));
        return _mapper.Map<List<LabTestResponse>>(await q.OrderBy(l => l.Name).ToListAsync());
    }

    public async Task<LabTestResponse> CreateAsync(LabTestCreateRequest request)
    {
        if (await _db.LabTests.AnyAsync(l => l.Name == request.Name))
            throw new AppException("ასეთი ანალიზი უკვე არსებობს", 409);
        var entity = _mapper.Map<LabTest>(request);
        _db.LabTests.Add(entity);
        await _db.SaveChangesAsync();
        return _mapper.Map<LabTestResponse>(entity);
    }
}
