**JWT, авторизация, CORS и rate limiting** — полный текстовый разбор лекции с кодом, пояснениями и готовыми фрагментами для ASP.NET Core Web API.

Ниже — пошаговая инструкция, которую можно почти «скопировать-вставить» в проект (предполагается, что у вас уже есть `AppDbContext`, контроллеры книг и т.д.).

---

### Шаг 1. Сущность пользователя и таблица

**В SSMS** (в той же БД, где Books/Authors/Tags):

```sql
CREATE TABLE Users (
    Id INT IDENTITY PRIMARY KEY,
    Login NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(500) NOT NULL,
    Role NVARCHAR(50) NOT NULL DEFAULT 'User',
    IsActive BIT NOT NULL DEFAULT 1
);
```

**Модель** (`Models/AppUser.cs`):

```csharp
public class AppUser
{
    public int Id { get; set; }
    public string Login { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string Role { get; set; } = "User"; // "User" / "Librarian" / "Admin"
    public bool IsActive { get; set; } = true;
}
```

В `AppDbContext` добавьте:

```csharp
public DbSet<AppUser> Users { get; set; }
```

После изменения БД можно либо сделать миграцию, либо просто Scaffold-DbContext / обновить контекст вручную.

**Пояснение:**  
`PasswordHash` хранит **хэш**, а не пароль в открытом виде. `Role` и `IsActive` нужны для авторизации и возможности «отключить» пользователя без удаления.

---

### Шаг 2. UserService — регистрация и проверка пароля

Используем `PasswordHasher<AppUser>` из `Microsoft.AspNetCore.Identity` (пакет уже есть в Web API-проекте, отдельная установка не нужна).

**Интерфейс** (`Services/IUserService.cs`):

```csharp
public interface IUserService
{
    Task<AppUser> RegisterAsync(string login, string password);
    Task<AppUser?> ValidateCredentialsAsync(string login, string password);
}
```

**Реализация** (`Services/UserService.cs`):

```csharp
public class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly PasswordHasher<AppUser> _hasher = new();
   
    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AppUser> RegisterAsync(string login, string password)
    {
        var exists = await _context.Users.AnyAsync(u => u.Login == login);
        if (exists)
            throw new InvalidOperationException($"Логин '{login}' уже занят.");

        var user = new AppUser { Login = login };
        user.PasswordHash = _hasher.HashPassword(user, password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<AppUser?> ValidateCredentialsAsync(string login, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(
            u => u.Login == login && u.IsActive);

        if (user is null) return null;

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);
        return result == PasswordVerificationResult.Failed ? null : user;
    }
}
```

В `Program.cs`:

```csharp
builder.Services.AddScoped<IUserService, UserService>();
```

**Пояснение:**  
`PasswordHasher` использует PBKDF2 с солью. Не храните пароли в открытом виде и не пишите свой хэшер.

---

### Шаг 3. Пакет JWT

```bash
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
```

---

### Шаг 4. Секция Jwt в `appsettings.json`

```json
"Jwt": {
  "Key": "JCAffsAfQbokVpxppysshTDBNGzFtGB2G1MkqCQc7o3",  // минимум 32 символа, лучше длиннее и секретный
  "Issuer": "BooksApi",
  "Audience": "BooksApiClient",
  "ExpiresMinutes": 15
}
```

**Важно:** в продакшене ключ берите из переменных окружения / User Secrets / Azure Key Vault. Не коммитьте реальный секрет.

---

### Шаг 5. TokenService

**Интерфейс:**

```csharp
public interface ITokenService
{
    string CreateAccessToken(AppUser user);
}
```

**Реализация:**

```csharp
public class TokenService : ITokenService
{
    private readonly IConfiguration _config;

    public TokenService(IConfiguration config)
    {
        _config = config;
    }

    public string CreateAccessToken(AppUser user)
    {
        var jwt = _config.GetSection("Jwt");

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Login),
            new(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwt["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var minutes = double.Parse(jwt["ExpiresMinutes"]!);

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(minutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
```

Регистрация:

```csharp
builder.Services.AddScoped<ITokenService, TokenService>();
```

**Пояснение claims:**  
`NameIdentifier` — Id пользователя, `Name` — логин, `Role` — роль (используется политиками авторизации).

---

### Шаг 6. AuthController + DTO

**DTO:**

```csharp
public class RegisterDto
{
    public string Login { get; set; } = "";
    public string Password { get; set; } = "";
}

public class LoginDto
{
    public string Login { get; set; } = "";
    public string Password { get; set; } = "";
}

public class LoginResponseDto
{
    public string AccessToken { get; set; } = "";
    public int ExpiresInMinutes { get; set; }
}
```

**Контроллер:**

```csharp
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;

    public AuthController(IUserService userService, ITokenService tokenService)
    {
        _userService = userService;
        _tokenService = tokenService;
    }

    // POST /api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        try
        {
            var user = await _userService.RegisterAsync(dto.Login, dto.Password);
            var accessToken = _tokenService.CreateAccessToken(user);

            return Ok(new LoginResponseDto
            {
                AccessToken = accessToken,
                ExpiresInMinutes = 15
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // POST /api/auth/login
    // Ограничен rate limiting-политикой "LoginPolicy"
    [EnableRateLimiting("LoginPolicy")]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _userService.ValidateCredentialsAsync(dto.Login, dto.Password);
        if (user is null)
            return Unauthorized("Неверный логин или пароль.");

        var accessToken = _tokenService.CreateAccessToken(user);

        return Ok(new LoginResponseDto
        {
            AccessToken = accessToken,
            ExpiresInMinutes = 15
        });
    }
}
```

---

### Шаг 7. Включение JWT-аутентификации в Program.cs

```csharp
var jwtSection = builder.Configuration.GetSection("Jwt");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSection["Key"]!))
        };
    });
```

---

### Шаг 8. Политики авторизации

```csharp
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanManageBooks", policy =>
        policy.RequireRole("Admin", "Librarian"));

    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));
});
```

---

### Шаг 9. Атрибуты [Authorize] на контроллерах

Пример в `BooksController`:

```csharp
// DELETE /api/books/5
[Authorize(Policy = "AdminOnly")]
[HttpDelete("{id}")]
public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
{
    var deleted = await _bookService.DeleteAsync(id, cancellationToken);
    if (!deleted) return NotFound();
    return NoContent();
}

// PATCH /api/books/5
[Authorize(Policy = "CanManageBooks")]
[HttpPatch("{id}")]
public async Task<IActionResult> Patch(int id, PatchBookDto dto, CancellationToken cancellationToken)
{
    try
    {
        var book = await _bookService.PatchAsync(id, dto, cancellationToken);
        if (book is null) return NotFound();
        return Ok(book);
    }
    catch (InvalidOperationException ex)
    {
        return BadRequest(new { error = ex.Message });
    }
}
```

GET-эндпоинты обычно оставляют открытыми (`[AllowAnonymous]` или без атрибута).

---

### Шаг 11. CORS

В `appsettings.json`:

```json
"Cors": {
  "AllowedOrigins": [ "http://localhost:3000", "http://localhost:5173" ]
}
```

В `Program.cs` (до `var app = builder.Build()`):

```csharp
const string FrontendPolicy = "FrontendPolicy";
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendPolicy, policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
            // .AllowCredentials() — если нужны cookies
    });
});
```

---

### Шаг 12. Rate limiting на вход

```csharp
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("LoginPolicy", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1);
        opt.PermitLimit = 5;
        opt.QueueLimit = 0;
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});
```

Атрибут `[EnableRateLimiting("LoginPolicy")]` уже стоит на `Login` в AuthController.

---

### Шаг 13. Кнопка Authorize и «замки» только на защищённых эндпоинтах в Swagger

Глобальное `AddSecurityRequirement` вешает замок на **все** эндпоинты. Лучше так:

```csharp
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Введите: Bearer {ваш JWT-токен}"
    });

    // Фильтр: замок только там, где реально есть [Authorize]
    options.OperationFilter<AuthorizeCheckOperationFilter>();
});
```

**Фильтр** (`Swagger/AuthorizeCheckOperationFilter.cs`):

```csharp
public class AuthorizeCheckOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var hasAuthorize = context.MethodInfo.DeclaringType!
            .GetCustomAttributes(true)
            .OfType<AuthorizeAttribute>()
            .Any()
            || context.MethodInfo
                .GetCustomAttributes(true)
                .OfType<AuthorizeAttribute>()
                .Any();

        var hasAllowAnonymous = context.MethodInfo
            .GetCustomAttributes(true)
            .OfType<AllowAnonymousAttribute>()
            .Any();

        if (!hasAuthorize || hasAllowAnonymous)
            return;

        operation.Security = new List<OpenApiSecurityRequirement>
        {
            new()
            {
                [new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                }] = Array.Empty<string>()
            }
        };
    }
}
```

Нужны using:
```csharp
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Authorization;
```

---

### Шаг 14. Итоговый порядок middleware в Program.cs

```csharp
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 1. CORS — решает, пропускать ли запрос от этого origin
// 2. Authentication — кто ты
// 3. Authorization — что тебе можно
// 4. Rate Limiter — не слишком ли часто
app.UseCors(FrontendPolicy);
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

app.MapControllers();

app.Run();
```

**Порядок критичен.** CORS должен быть до Authentication/Authorization. RateLimiter — после них (или перед MapControllers).

---

### Дополнительные рекомендации

1. **Валидация DTO** — добавьте `[Required]`, `[MinLength(6)]` на пароль и т.д. + `builder.Services.AddControllers()` уже включает ModelState.
2. **Refresh-токены** — в лекции только access-токен (15 мин). Для реальных приложений обычно добавляют refresh-токен + отдельную таблицу.
3. **Хранение ключа** — в Development используйте User Secrets:
   ```bash
   dotnet user-secrets set "Jwt:Key" "ваш-очень-длинный-секрет"
   ```
4. **Роли** — при регистрации можно по умолчанию ставить `"User"`, а Admin/Librarian выдавать вручную в БД или отдельным эндпоинтом.
5. **Логирование неудачных логинов** — полезно для безопасности.
6. **HTTPS** — в продакшене обязателен (JWT в заголовке).

После этих шагов у вас будет:
- регистрация и логин с JWT;
- роли и политики;
- CORS для фронтенда;
- rate limiting на логин;
- корректная кнопка Authorize в Swagger только на защищённых методах.

Можете брать любой фрагмент и вставлять в свой проект. Если нужно — могу отдельно дать полный пример `Program.cs` или миграцию.