using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using CRM.Domain.Entities;
using CRM.Application.Interfaces;
using CRM.Infrastructure.Auth;
using CRM.Infrastructure.Data;
using CRM.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(Path.Combine("Logs", "crm-log-.txt"), rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Configure EF Core with SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString, b => b.MigrationsAssembly("CRM.Web")));

// Configure ASP.NET Core Identity
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// Configure Cookie Settings for MVC Application
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(120);
    options.SlidingExpiration = true;
});

// Configure JWT Settings and Authentication
var jwtSettingsSection = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(jwtSettingsSection);
var jwtSettings = jwtSettingsSection.Get<JwtSettings>();
var key = Encoding.ASCII.GetBytes(jwtSettings?.Secret ?? "SuperSecretKeyEnsureThisIsLongAndCryptographicForSecurity123!");

builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings?.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings?.Audience,
            ClockSkew = TimeSpan.Zero
        };
    });

// Configure Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("ManagerOrAbove", policy => policy.RequireRole("Admin", "Manager"));
    options.AddPolicy("TeamLeaderOrAbove", policy => policy.RequireRole("Admin", "Manager", "TeamLeader"));
    options.AddPolicy("StaffOnly", policy => policy.RequireRole("Admin", "Manager", "TeamLeader", "CustomerService"));
    options.AddPolicy("TeacherOnly", policy => policy.RequireRole("Teacher"));
});

// Add services to the container.
builder.Services.AddControllersWithViews();
// Register services
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ILeadService, LeadService>();
builder.Services.AddScoped<IStudentService, StudentService>();
var app = builder.Build();

// Seed Database on Startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.Migrate();

        var userManager = services.GetRequiredService<UserManager<AppUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        await DbInitializer.SeedAsync(userManager, roleManager);
    }
    catch (Exception ex)
    {
        Log.Fatal(ex, "An error occurred while migrating or seeding the database.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

try
{
    Log.Information("Starting web host");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
