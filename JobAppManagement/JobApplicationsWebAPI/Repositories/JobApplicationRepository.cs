using JobApplicationsShared.Models;
using JobApplicationsWebAPI.Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace JobApplicationsWebAPI.Repositories
{
    public class JobApplicationRepository : IJobApplicationRepository
    {
        private readonly JobApplicationDbContext _context;
        private readonly ILogger<JobApplicationRepository> _logger;

        public JobApplicationRepository(JobApplicationDbContext context, ILogger<JobApplicationRepository> logger)
        {
            _context = context;
            _logger = logger;
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
            try
            {
                if (jobs == null || !jobs.Any())
                {
                    _logger.LogWarning("AddJobApplicationsAsync called with empty or null job list.");
                    return 0;
                }

                _context.JobApplications.AddRange(jobs);
                var inserted = await _context.SaveChangesAsync();

                _logger.LogInformation($"{inserted} job application(s) successfully saved to database.");
                return inserted;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update failed while adding job applications.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while adding job applications.");
                throw;
            }
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


