using API_First_Lesson.Authorization;
using API_First_Lesson.Models;
using API_First_Lesson.Repositories;
using API_First_Lesson.Services;
using API_First_Lesson.Swagger;
using API_First_Lesson.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// ========== Database ==========
builder.Services.AddDbContext<WebAppContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("con")));

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

// ===== CORS =====
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
            .WithOrigins(allowedOrigins)      // только конкретные origins
            .AllowAnyHeader()
            .AllowAnyMethod();
        // НЕ используй AllowAnyOrigin()
        // .AllowCredentials() — добавляй только если нужны cookies
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
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApp API", Version = "v1" });

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
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(FrontendPolicy);        

app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();


app.MapControllers();

app.Run();