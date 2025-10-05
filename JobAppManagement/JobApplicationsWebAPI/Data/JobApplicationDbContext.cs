﻿using JobApplicationsShared.Models;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationsWebAPI.Data
{    
    public class JobApplicationDbContext : DbContext
    {
        public JobApplicationDbContext(DbContextOptions<JobApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<JobApplication> JobApplications { get; set; }
    }
}
