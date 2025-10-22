
using JobApplicationsShared.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace JobApplicationsShared.Interfaces
{
    public interface IJobApplicationService
    {
        Task<List<JobApplication>> GetAllJobsAsync();
        Task<JobApplication?> AddJobAsync(JobApplication job);
        Task UpdateJobAsync(JobApplication job);
        Task DeleteJobAsync(int id);
        Task<int> ImportJobsAsync(IBrowserFile file);



    }
}
