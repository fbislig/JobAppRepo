using JobApplicationsShared.Models;

namespace JobApplicationsWebAPI.Repositories
{
    public interface IJobApplicationRepository
    {
        Task<List<JobApplication>> GetJobApplicationsAsync();
        Task<JobApplication?> GetJobApplicationByIdAsync(int id);
        Task<JobApplication> AddJobApplicationAsync(JobApplication job);
        Task<JobApplication?> UpdateJobApplicationAsync(int id, JobApplication job);
        Task<bool> DeleteJobApplicationAsync(int id);
        Task<int> AddJobApplicationsAsync(IEnumerable<JobApplication> jobs);
        Task<bool> JobExistsAsync(string company, string position);
    }
}
