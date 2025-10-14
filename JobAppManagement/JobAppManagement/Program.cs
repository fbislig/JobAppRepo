using JobAppManagement;
using JobApplicationsShared.Interfaces;
using JobAppManagement.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Load configuration early
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);

// Register root components
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Dynamically resolve API base URL based on environment
string apiBaseUrl;

if (builder.HostEnvironment.IsDevelopment())
{
    // Local development: use value from appsettings.json
    apiBaseUrl = "https://localhost:7040";
}
else
{
    // Production: use SWA-relative path or full API endpoint
    apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://jobapplicationswebapi20251014121824-h8eybrhkgzgwapb7.canadacentral-01.azurewebsites.net/";   
}

if (!apiBaseUrl.EndsWith("/"))
    apiBaseUrl += "/";

// Register HttpClient and service using AddHttpClient
builder.Services.AddHttpClient<IJobApplicationService, JobApplicationService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

// Optional: Enable client-side logging
builder.Logging.SetMinimumLevel(LogLevel.Information);

await builder.Build().RunAsync();
