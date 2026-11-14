using API_First_Lesson.Authorization;
using API_First_Lesson.Controllers;
using API_First_Lesson.Data;
using API_First_Lesson.Models;
using API_First_Lesson.Repositories;
using API_First_Lesson.Services;
using API_First_Lesson.Swagger;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// ========== Database ==========
// UseSqlite=true  -> локальная файловая БД (не нужен SQL Server, удобно для защиты)
// UseSqlite=false -> SQL Server / LocalDB, как в исходном проекте
var useSqlite = builder.Configuration.GetValue<bool>("UseSqlite");

builder.Services.AddDbContext<WebAppContext>(options =>
{
    if (useSqlite)
        options.UseSqlite(builder.Configuration.GetConnectionString("Sqlite"));
    else
        options.UseSqlServer(builder.Configuration.GetConnectionString("con"));
});

// ========== Repositories ==========
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICartItemRepository, CartItemRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();
builder.Services.AddScoped<IOrderEventRepository, OrderEventRepository>();

// ========== Services ==========
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICartItemService, CartItemService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderItemService, OrderItemService>();
builder.Services.AddScoped<IOrderEventService, OrderEventService>();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddControllers();

// ========== API Versioning ==========
// Версия передаётся в query string: ?api-version=1.0 или ?api-version=2.0.
// AssumeDefaultVersionWhenUnspecified = false -> запрос без версии к версионируемому
// контроллеру завершается ошибкой 400, а не выбирает версию по умолчанию.
builder.Services
    .AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = false;
        options.ReportApiVersions = true;
        options.ApiVersionReader = new QueryStringApiVersionReader("api-version");
    })
    .AddMvc()
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = false;
    });

// ========== JWT ==========
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
                Encoding.UTF8.GetBytes(jwtSection["Key"]!)),
            ClockSkew = TimeSpan.Zero
        };
    });

// ========== Authorization policies ==========
builder.Services.AddAuthorization(options =>
{
    // Role-based
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));

    options.AddPolicy("AdminOrManager", policy =>
        policy.RequireRole("Admin", "Manager"));

    // Resource-based (свой заказ или Admin)
    options.AddPolicy("OrderOwnerOrAdmin", policy =>
        policy.Requirements.Add(new OrderOwnerRequirement()));
});

builder.Services.AddSingleton<IAuthorizationHandler, OrderOwnerAuthorizationHandler>();


const string FrontendPolicy = "FrontendPolicy";

var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>()
    ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendPolicy, policy =>
    {
        policy
            .WithOrigins(allowedOrigins)      
            .AllowAnyHeader()
            .AllowAnyMethod();

    });
});

// ========== Rate Limiting ==========
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

// ========== Swagger ==========
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Один документ Swagger со всеми версиями и реальными URL методов.
    options.SwaggerDoc("all", new OpenApiInfo
    {
        Title = "WebApp API - выбор версии в методе",
        Version = "all",
        Description = "В ProductController выберите api-version = 1.0 или 2.0 в параметрах HTTP-метода."
    });

    // Не фильтруем действия по GroupName: все версии попадают в один документ.
    options.DocInclusionPredicate((_, _) => true);

    // OpenAPI не допускает две операции GET с одинаковым URL, даже если сервер
    // различает их по api-version. Разрешаем ТОЛЬКО известный конфликт
    // GET /api/Product (GetProducts v1 и GetProductsV2), остальные конфликты
    // считаем реальной ошибкой маршрутов и не скрываем.
    options.ResolveConflictingActions(descriptions =>
    {
        var candidates = descriptions.ToList();

        // Одинаковые описания одного и того же действия (возникают у
        // [ApiVersionNeutral] контроллеров) — не конфликт маршрутов.
        var actionMethods = candidates
            .Select(d => (d.ActionDescriptor as ControllerActionDescriptor)?.MethodInfo)
            .Distinct()
            .ToList();

        bool isSameAction = actionMethods.Count == 1 && actionMethods[0] is not null;

        if (isSameAction)
            return candidates[0];

        // Известный конфликт версионирования: GET /api/Product имеет отдельные
        // реализации GetProducts (1.0) и GetProductsV2 (2.0). OpenAPI допускает
        // только одну операцию на пару "метод + путь", поэтому оставляем v1.
        var isProductGetAllConflict =
            candidates.Count == 2 &&
            candidates.All(d => d.ActionDescriptor is ControllerActionDescriptor cad &&
                                cad.ControllerTypeInfo.AsType() == typeof(ProductController)) &&
            candidates
                .Select(d => ((ControllerActionDescriptor)d.ActionDescriptor).ActionName)
                .OrderBy(n => n)
                .SequenceEqual(new[] { "GetProducts", "GetProductsV2" });

        if (isProductGetAllConflict)
        {
            // В Swagger показываем реализацию 1.0 как базовую схему ответа.
            return candidates.First(d =>
                ((ControllerActionDescriptor)d.ActionDescriptor).ActionName == "GetProducts");
        }

        throw new InvalidOperationException(
            "Swagger: неожиданный конфликт двух разных операций: " +
            string.Join(", ", candidates.Select(d => d.ActionDescriptor.DisplayName)));
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Введите JWT-токен (без слова Bearer)"
    });

    // Замок только на методах с [Authorize]
    options.OperationFilter<AuthorizeCheckOperationFilter>();

    // Выпадающий список api-version в Swagger UI
    options.OperationFilter<ApiVersionDropdownOperationFilter>();
});

var app = builder.Build();

// Подготавливаем БД и заполняем базовые роли/админа при первом запуске.
// SQLite создаётся по модели, для SQL Server применяются миграции.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<WebAppContext>();
    if (useSqlite)
        db.Database.EnsureCreated();
    else
        db.Database.Migrate();
    DbSeeder.Seed(db);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/all/swagger.json", "WebApp API - все версии");
    });
}

app.UseHttpsRedirection();

app.UseCors(FrontendPolicy);        

app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();


app.MapControllers();

app.Run();