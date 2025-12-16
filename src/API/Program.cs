using API.Middlewares;
using Application.Interfaces.Http;
using Application.Interfaces.Logging;
using Application.Interfaces.Services;
using Application.Services;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Security;
using Infrastructure.Auth;
using Infrastructure.Background.HostedService;
using Infrastructure.Configurations;
using Infrastructure.HttpAcessor;
using Infrastructure.Logging;
using Infrastructure.Repositories;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
                .AddJsonOptions(o =>
                {
                    o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    o.JsonSerializerOptions.DefaultIgnoreCondition =
                        System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
                });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<NeuroFlowContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtSettings!.Issuer,

        ValidateAudience = true,
        ValidAudience = jwtSettings!.Audience,

        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),

        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddResponseCaching();
builder.Services.AddResponseCompression(o => o.EnableForHttps = true);

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("default", opt =>
    {
        opt.PermitLimit = 100;
        opt.Window = TimeSpan.FromSeconds(10);
        opt.QueueLimit = 0;
    });
    options.AddFixedWindowLimiter("document-verification", limiterOptions =>
    {
        limiterOptions.PermitLimit = 3;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueLimit = 0;
    });
});
builder.Services.AddHttpClient<CrpVerifier>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(10);
});
builder.Services.AddHttpClient<CrmVerifier>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(10);
});
builder.Services.AddSingleton<IProfessionalDocumentVerifier>(sp =>
{
    var env = sp.GetRequiredService<IHostEnvironment>();

    if (env.IsDevelopment())
        return new MockDocumentVerifier();

    return new ProfessionalDocumentVerifier(
        sp.GetRequiredService<CrpVerifier>(),
        sp.GetRequiredService<CrmVerifier>()
    );
});
builder.Services.Configure<MercadoPagoSettings>(
    builder.Configuration.GetSection("MercadoPago"));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IProfessionalProfileRepository, ProfessionalProfileRepository>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IClinicalAssessmentRepository, ClinicalAssessmentRepository>();
builder.Services.AddScoped<IClinicalNoteRepository, ClinicalNoteRepository>();
builder.Services.AddScoped<IMoodEntryRepository, MoodEntryRepository>();
builder.Services.AddScoped<IThoughtEntryRepository, ThoughtEntryRepository>();
builder.Services.AddScoped<IPatientInviteRepository, PatientInviteRepository>();
builder.Services.AddScoped<ITokenAuditRepository, TokenAuditRepository>();

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtProvider, JwtProvider>();

builder.Services.AddTransient<ErrorHandlingMiddleware>();
builder.Services.AddTransient<ValidationMiddleware>();
builder.Services.AddTransient<CorrelationIdMiddleware>();
builder.Services.AddTransient<RequestLoggingMiddleware>();

builder.Services.AddSingleton(typeof(IAppLogger<>), typeof(SerilogAppLogger<>));
builder.Services.AddHostedService<RetryFailedPaymentsService>();


Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        path: "logs/verification-.json",
        rollingInterval: RollingInterval.Day,
        formatter: new Serilog.Formatting.Json.JsonFormatter()
)
    .CreateLogger();

builder.Host.UseSerilog();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseResponseCompression();
app.UseResponseCaching();
app.UseRateLimiter();

app.UseMiddleware<ValidationMiddleware>();
app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
