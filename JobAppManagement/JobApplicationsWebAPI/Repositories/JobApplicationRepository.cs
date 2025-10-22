using JobApplicationsShared.Models;
using JobApplicationsWebAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationsWebAPI.Repositories
{
    public class JobApplicationRepository : IJobApplicationRepository
    {
        private readonly JobApplicationDbContext _context;

        public JobApplicationRepository(JobApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<JobApplication>> GetJobApplicationsAsync()
        {
            return await _context.JobApplications.ToListAsync();
        }

        public async Task<JobApplication?> GetJobApplicationByIdAsync(int id)
        {
            return await _context.JobApplications.FindAsync(id);
        }

        public async Task<JobApplication> AddJobApplicationAsync(JobApplication job)
        {
            _context.JobApplications.Add(job);
            await _context.SaveChangesAsync();
            return job;
        }

        public async Task<JobApplication?> UpdateJobApplicationAsync(int id, JobApplication updatedJob)
        {
            var job = await _context.JobApplications.FindAsync(id);
            if (job == null) return null;

            job.Company = updatedJob.Company;
            job.Position = updatedJob.Position;
            job.Location = updatedJob.Location;
            job.DateApplied = updatedJob.DateApplied;
            job.Status = updatedJob.Status;

            await _context.SaveChangesAsync();
            return job;
        }

        public async Task<bool> DeleteJobApplicationAsync(int id)
        {
            var job = await _context.JobApplications.FindAsync(id);
            if (job == null) return false;

            _context.JobApplications.Remove(job);
            await _context.SaveChangesAsync();
            return true;
        }

        // New method to add multiple job applications
        public async Task<int> AddJobApplicationsAsync(IEnumerable<JobApplication> jobs)
        {
            _context.JobApplications.AddRange(jobs);
            var inserted = await _context.SaveChangesAsync();
            return inserted; // number of rows inserted
        }

        public async Task<bool> JobExistsAsync(string company, string position/*, DateTime dateApplied*/)
        {
            return await _context.JobApplications
    .AnyAsync(j => j.Company.ToLower() == company.ToLower()
                && j.Position.ToLower() == position.ToLower());

        }

        public async Task<bool> CompanyExistsAsync(string company)
        {
            return await _context.JobApplications
                .AnyAsync(j => j.Company.ToLower() == company.ToLower());
        }



    }
}


