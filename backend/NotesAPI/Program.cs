using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using NotesAPI.Data;
using NotesAPI.Services;


var builder = WebApplication.CreateBuilder(args);

// Use PORT environment variable if present (for Railway)
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
builder.WebHost.UseUrls($"http://*:{port}");

var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new Exception("Missing JWT key. Please set Jwt:Key (or env Jwt__Key).");
}

var useCookies = builder.Configuration.GetValue<bool>("Auth:UseCookies");

var allowedOrigins = builder.Environment.IsDevelopment() 
    ? new[] { "http://localhost:5173", "http://localhost:5174", "https://notes-app-tech.vercel.app", "https://notes-app-git-main-bunlongs-projects-cd694e88.vercel.app" }
    : new[] { "https://notes-app-tech.vercel.app", "https://notes-app-git-main-bunlongs-projects-cd694e88.vercel.app" };

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();

        if (useCookies)
        {
            policy.AllowCredentials();
        }
    });
});

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
        options.SaveToken = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new Exception("Missing connection string");

builder.Services.AddSingleton<IDbConnectionFactory>(sp => new DbConnectionFactory(connectionString));
builder.Services.AddScoped<INoteService, NoteService>();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

app.UseRouting();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/", () => Results.Ok("Notes API is running"));

app.Run();
