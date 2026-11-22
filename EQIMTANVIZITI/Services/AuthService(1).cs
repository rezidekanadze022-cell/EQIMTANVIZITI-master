using System.Security.Cryptography;

namespace EQIMTANVIZITI.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    private readonly IJwtService _jwt;
    private readonly IEmailService _email;
    private readonly ILogger<AuthService> _log;
    private readonly IHostEnvironment _env;

    public AuthService(AppDbContext db, IMapper mapper, IJwtService jwt, IEmailService email,
        ILogger<AuthService> log, IHostEnvironment env)
    { _db = db; _mapper = mapper; _jwt = jwt; _email = email; _log = log; _env = env; }

    public async Task<MessageResponse> RegisterAsync(RegisterRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        if (await _db.Users.AnyAsync(u => u.Email == email))
            throw new AppException("ამ ელფოსტით მომხმარებელი უკვე არსებობს", 409);

        var user = _mapper.Map<User>(request);
        user.Email = email;
        user.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
        user.Role = Role.Patient;

        if (string.IsNullOrWhiteSpace(user.FirstName) && !string.IsNullOrWhiteSpace(request.FullName))
        {
            var parts = request.FullName.Trim().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            user.FirstName = parts[0];
            user.LastName = parts.Length > 1 ? parts[1] : parts[0];
        }

        await DependencyInjection.SMTP.SMTPService.SendEmail(user);

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return new MessageResponse("რეგისტრაცია წარმატებულია. დადასტურების კოდი გამოგზავნილია ელფოსტაზე");
    }

    public async Task<MessageResponse> VerifyAsync(VerifyRequest request)
    {
        var user = await FindByEmailAsync(request.Email);
        if (user.IsVerified) throw new AppException("ელფოსტა უკვე დადასტურებულია");
        if (user.Code != request.Code || user.CodeExpiresAt < DateTime.UtcNow)
            throw new AppException("კოდი არასწორია ან ვადაგასულია");

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
        await DependencyInjection.SMTP.SMTPService.SendEmail(user);
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

    private static void GenerateCode(User user)
    {
        user.Code = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
        user.CodeExpiresAt = DateTime.UtcNow.AddMinutes(10);
    }

    private async Task SendCodeAsync(User user)
    {
        try
        {
            await _email.SendAsync(user.Email, "ელფოსტის დადასტურება",
                $"<h3>გამარჯობა, {user.FirstName}!</h3><p>თქვენი დადასტურების კოდია: <b>{user.Code}</b></p><p>კოდი მოქმედებს 10 წუთის განმავლობაში.</p>");
        }
        catch (Exception ex)
        {
            _log.LogWarning(ex, "SMTP გაგზავნა ვერ მოხერხდა {Email}", user.Email);
            if (_env.IsDevelopment())
                _log.LogWarning("DEV verification code for {Email}: {Code}", user.Email, user.Code);
        }
    }
}
