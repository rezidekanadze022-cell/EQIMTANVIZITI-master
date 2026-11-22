namespace EQIMTANVIZITI.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    private readonly IJwtService _jwt;

    public AuthService(AppDbContext db, IMapper mapper, IJwtService jwt)
    { _db = db; _mapper = mapper; _jwt = jwt; }

    public async Task<bool> RegisterAsync(RegisterRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var userexists = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (userexists != null) return false;

        string hashedpassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var mapped = _mapper.Map<User>(request);
        mapped.Email = email;
        mapped.Password = hashedpassword;
        mapped.Role = Role.Patient;

        await SMTPService.SendEmail(mapped);

        _db.Users.Add(mapped);
        await _db.SaveChangesAsync();

        return true;
    }

    public async Task<MessageResponse> VerifyAsync(VerifyRequest request)
    {
        var user = await FindByEmailAsync(request.Email);
        if (user.IsVerified) throw new AppException("ელფოსტა უკვე დადასტურებულია");
        if (user.Code != request.Code)
            throw new AppException("კოდი არასწორია");

        user.IsVerified = true;
        user.Code = null;
        user.CodeExpiresAt = null;
        await _db.SaveChangesAsync();
        return new MessageResponse("ელფოსტა დადასტურებულია. ახლა შეგიძლიათ შეხვიდეთ სისტემაში");
    }

    public async Task<MessageResponse> ResendCodeAsync(ResendCodeRequest request)
    {
        var user = await FindByEmailAsync(request.Email);
        if (user.IsVerified) throw new AppException("ელფოსტა უკვე დადასტურებულია");
        await SMTPService.SendEmail(user);
        await _db.SaveChangesAsync();
        return new MessageResponse("ახალი კოდი გამოგზავნილია");
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            throw new AppException("ელფოსტა ან პაროლი არასწორია", 401);
        if (!user.IsVerified)
            throw new AppException("ჯერ დაადასტურეთ ელფოსტა (verify)", 403);
        return _jwt.CreateToken(user);
    }

    private async Task<User> FindByEmailAsync(string email)
    {
        var e = email.Trim().ToLowerInvariant();
        return await _db.Users.FirstOrDefaultAsync(u => u.Email == e)
               ?? throw new AppException("მომხმარებელი ვერ მოიძებნა", 404);
    }
}