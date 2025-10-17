using JobApplicationsShared.Interfaces;
using JobApplicationsShared.Models;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Json;

namespace JobAppManagement.Services
{
    public class ImportResult
    {
        public int Inserted { get; set; }
    }

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

        public async Task<int> ImportJobsAsync(IBrowserFile file)
        {
            if (file == null) return 0;

            using var content = new MultipartFormDataContent();
            using var stream = file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024);
            content.Add(new StreamContent(stream), "file", file.Name);

            var response = await _http.PostAsync("api/jobapplications/import", content);

            if (!response.IsSuccessStatusCode)
                return 0;

            var result = await response.Content.ReadFromJsonAsync<ImportResult>();
            return result?.Inserted ?? 0;
        }


    }
}
