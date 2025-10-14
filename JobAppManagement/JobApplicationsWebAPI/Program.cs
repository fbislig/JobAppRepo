using JobApplicationsWebAPI.Data;
using JobApplicationsWebAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Serilog;



var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
builder.Host.UseSerilog();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add services to the container.
builder.Services.AddControllers();

// Configure CORS for Azure Static Web Apps
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSWAClient",
        policy => policy
            .WithOrigins(
                "https://calm-sand-03ccd8010.2.azurestaticapps.net", // Production SWA
                "https://localhost:7122" // Local Blazor client
            )
            .AllowAnyHeader()
            .AllowAnyMethod());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext for SQLite
//builder.Services.AddDbContext<JobApplicationDbContext>(options =>
//    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddDbContext<JobApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure()
    ));

// Register repository for DI
builder.Services.AddScoped<IJobApplicationRepository, JobApplicationRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();
app.UseCors("AllowSWAClient");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();