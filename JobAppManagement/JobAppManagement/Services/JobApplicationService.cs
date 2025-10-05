using JobApplicationsShared.Interfaces;
using JobApplicationsShared.Models;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Json;

namespace JobAppManagement.Services
{
    public class JobApplicationService : IJobApplicationService
    {
        private readonly HttpClient _http;

        public JobApplicationService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<JobApplication>> GetAllJobsAsync() =>
            await _http.GetFromJsonAsync<List<JobApplication>>("api/jobapplications") ?? new();

        public async Task<JobApplication?> AddJobAsync(JobApplication job)
        {
            var response = await _http.PostAsJsonAsync("api/jobapplications", job);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<JobApplication>();
            }
            return null;
        }


        public async Task UpdateJobAsync(JobApplication job) =>
            await _http.PutAsJsonAsync($"api/jobapplications/{job.Id}", job);

        public async Task DeleteJobAsync(int id) =>
            await _http.DeleteAsync($"api/jobapplications/{id}");

        public async Task<bool> ImportJobsAsync(IBrowserFile file)
        {
            if (file == null) return false;

            // Prepare multipart form data
            using var content = new MultipartFormDataContent();
            using var stream = file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024); // 10 MB max
            content.Add(new StreamContent(stream), "file", file.Name);

            // Send POST request to API
            var response = await _http.PostAsync("api/jobapplications/import", content);

            return response.IsSuccessStatusCode;
        }
    }
}
