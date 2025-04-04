using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Turnero;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


// ✅ Configurar Kestrel para que use el puerto 7124
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5173);
});

// 🔹 Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 🔹 Add Database Context (SQLite)
builder.Services.AddDbContext<TurneroDataContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🔹 Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 🔹 Retrieve JWT settings from appsettings.json
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]);

// 🔹 Configure JWT authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(secretKey)
        };
    });

builder.Services.AddAuthorization();

// 📌 Enable SignalR
builder.Services.AddSignalR();

var app = builder.Build();

// 🔹 Enable CORS globally
app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 🔹 Enable authentication & authorization
app.UseAuthentication();
app.UseAuthorization();

// ✅ Serve static files (React frontend)
app.UseDefaultFiles();
app.UseStaticFiles();

// ✅ Handle React routes (SPA fallback to index.html)
app.Use(async (context, next) =>
{
    await next();
    if (context.Response.StatusCode == 404 && !context.Request.Path.Value.StartsWith("/api"))
    {
        context.Response.StatusCode = 200;
        await context.Response.SendFileAsync("wwwroot/index.html");
    }
});

// 🔹 Map API Controllers
app.MapControllers();

// 📌 Register SignalR Hub
app.MapHub<TurnoHub>("/turnoHub");

// 🔥 Start application
app.Run();
