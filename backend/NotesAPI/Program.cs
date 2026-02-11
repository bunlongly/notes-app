using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// ------------------------------------------------------------
// Config values (from appsettings.json + Railway env vars)
// ------------------------------------------------------------
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new Exception("Missing JWT key. Please set Jwt:Key (or env Jwt__Key).");
}

var useCookies = builder.Configuration.GetValue<bool>("Auth:UseCookies");

// If your Vercel domain changes, add it here.
var allowedOrigins = new[]
{
    "https://notes-app-five-peach.vercel.app"
    // "https://your-custom-domain.com"
};

// ------------------------------------------------------------
// Services
// ------------------------------------------------------------
builder.Services.AddControllers();

// CORS (fixes: Access-Control-Allow-Origin missing)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();

        // Only enable credentials if you truly use cookies
        // (UseCookies=true). Otherwise keep it off.
        if (useCookies)
        {
            policy.AllowCredentials();
        }
    });
});

// Authentication (JWT)
builder.Services
    .AddAuthentication(options =>
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
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization();

// If you already register your services (like NoteService, UserService, DbConnectionFactory)
// keep them here. Example:
// builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
// builder.Services.AddScoped<INoteService, NoteService>();
// builder.Services.AddScoped<IUserService, UserService>();

// ------------------------------------------------------------
// App
// ------------------------------------------------------------
var app = builder.Build();

// Helpful for Railway debugging
// app.UseDeveloperExceptionPage(); // enable only if you want detailed errors in non-prod

app.UseRouting();

// CORS must be BEFORE auth + controllers
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Simple health check (optional)
app.MapGet("/", () => Results.Ok("Notes API is running"));

app.Run();
