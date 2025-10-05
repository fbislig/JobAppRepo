using JobApplicationsShared.Enums;

namespace JobApplicationsShared.Models
{
    public static class JobAppRepository
    {
        private static List<JobApplication> jobApplications = new List<JobApplication>()
        {
          new JobApplication { Id = 1, Company = "Google", Position = "Software Engineer", Location = "NY", DateApplied = DateTime.Today.AddDays(-10), Status = Status.Interviewing },
          new JobApplication { Id = 2, Company = "Microsoft", Position = "Data Analyst", Location = "Remote", DateApplied = DateTime.Today.AddDays(-5), Status = Status.Rejected }

        };


        public static void AddJob(JobApplication job)
        {
            var maxId = jobApplications.Any() ? jobApplications.Max(s => s.Id) : 0;
            job.Id = maxId + 1;
            jobApplications.Add(job);
        }

        public static List<JobApplication> GetJobApplications() => jobApplications.Select(j => new JobApplication
        {
            Id = j.Id,
            Company = j.Company,
            Location = j.Location,
            Position = j.Position,
            DateApplied = j.DateApplied,
            Status = j.Status
        }).ToList();
       
        public static JobApplication? GetJobApplicationById(int id)
        {
            var jobApp = jobApplications.FirstOrDefault(s => s.Id == id);
            if (jobApp != null)
            {
                return new JobApplication
                {
                    Id = jobApp.Id,
                    Company = jobApp.Company,
                    Location = jobApp.Location,
                    Position = jobApp.Position,
                    DateApplied = jobApp.DateApplied,
                    Status = jobApp.Status
                };
            }

            return null;
        }

        public static void UpdateJobApplication(int jobId, JobApplication jobApp)
        {
            if (jobId != jobApp.Id) return;

            var jobAppToUpdate = jobApplications.FirstOrDefault(s => s.Id == jobId);
            if (jobAppToUpdate != null)
            {
                jobAppToUpdate.Company = jobApp.Company;
                jobAppToUpdate.Location = jobApp.Location;
                jobAppToUpdate.Position = jobApp.Position;
                jobAppToUpdate.DateApplied = jobApp.DateApplied;
                jobAppToUpdate.Status = jobApp.Status;
            }
        }

        public static void DeleteJobApplication(int jobId)
        {
            var jobApp = jobApplications.FirstOrDefault(s => s.Id == jobId);
            if (jobApp != null)
            {
                jobApplications.Remove(jobApp);
            }
        }
    }
}
