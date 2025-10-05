using JobApplicationsShared.Enums;

namespace JobApplicationsShared.Models
{
    public static class JobApplicationRepository
    {
        private static readonly List<JobApplication> jobApplications = new()
    {
        new JobApplication { Id = 1, Company = "Google", Position = "Software Engineer", Location = "NY", DateApplied = DateTime.Today.AddDays(-10), Status = Status.Interviewing },
        new JobApplication { Id = 2, Company = "Microsoft", Position = "Data Analyst", Location = "Remote", DateApplied = DateTime.Today.AddDays(-5), Status = Status.Rejected }
    };

        //public static Task<List<JobApplication>> GetJobApplicationsAsync()
        //    => Task.FromResult(jobApplications.Select(Clone).ToList());

        public static Task<List<JobApplication>> GetJobApplicationsAsync(string? userId = null)
        {
            var result = string.IsNullOrEmpty(userId)
                ? jobApplications.ToList()
                : jobApplications.Where(j => j.UserId == userId).ToList();

            return Task.FromResult(result);
        }

        public static Task<JobApplication?> GetJobApplicationByIdAsync(int id)
        {
            var jobApp = jobApplications.FirstOrDefault(j => j.Id == id);
            return Task.FromResult(jobApp is not null ? Clone(jobApp) : null);
        }


        public static Task AddJobAsync(JobApplication job, string userId = null)
        {
            var maxId = jobApplications.Any() ? jobApplications.Max(s => s.Id) : 0;
            job.Id = maxId + 1;
            job.UserId = userId;
            jobApplications.Add(job);

            return Task.CompletedTask;
        }


        public static Task UpdateJobApplicationAsync(int jobId, JobApplication jobApp)
        {
            var jobAppToUpdate = jobApplications.FirstOrDefault(s => s.Id == jobId);
            if (jobAppToUpdate is not null)
            {
                jobAppToUpdate.Company = jobApp.Company;
                jobAppToUpdate.Location = jobApp.Location;
                jobAppToUpdate.Position = jobApp.Position;
                jobAppToUpdate.DateApplied = jobApp.DateApplied;
                jobAppToUpdate.Status = jobApp.Status;
            }
            return Task.CompletedTask;
        }

        public static Task DeleteJobApplicationAsync(int jobId)
        {
            var jobApp = jobApplications.FirstOrDefault(s => s.Id == jobId);
            if (jobApp is not null)
                jobApplications.Remove(jobApp);

            return Task.CompletedTask;
        }

        private static JobApplication Clone(JobApplication job) => new()
        {
            Id = job.Id,
            Company = job.Company,
            Position = job.Position,
            Location = job.Location,
            DateApplied = job.DateApplied,
            Status = job.Status,
            UserId = job.UserId
        };
    }

}
