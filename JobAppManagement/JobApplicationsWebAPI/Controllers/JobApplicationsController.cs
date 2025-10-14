using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.Excel;
using JobApplicationsShared.Enums;
using JobApplicationsShared.Models;
using JobApplicationsWebAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;


namespace JobApplicationsWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobApplicationsController : ControllerBase
    {
        private readonly IJobApplicationRepository _repository;

        private readonly ILogger<JobApplicationsController> _logger;

        public JobApplicationsController(
            IJobApplicationRepository repository,
            ILogger<JobApplicationsController> logger)
        {
            _repository = repository;
            _logger = logger;
        }


        // GET: api/jobapplications
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobApplication>>> GetAll()
        {
            _logger.LogInformation("Fetching all job applications.");

            var jobs = await _repository.GetJobApplicationsAsync();

            _logger.LogInformation("Retrieved {Count} job applications.", jobs.Count());

            return Ok(jobs);
        }


        // GET: api/jobapplications/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<JobApplication>> GetById(int id)
        {
            _logger.LogInformation("Fetching job application with ID {Id}", id);

            var job = await _repository.GetJobApplicationByIdAsync(id);

            if (job == null)
            {
                _logger.LogWarning("Job application with ID {Id} not found.", id);
                return NotFound();
            }

            _logger.LogInformation("Successfully retrieved job application with ID {Id}", id);
            return Ok(job);
        }


        // POST: api/jobapplications
        [HttpPost]
        public async Task<ActionResult<JobApplication>> Create(JobApplication job)
        {
            _logger.LogInformation("Received request to create a new job application for company: {Company}, position: {Position}", job.Company, job.Position);

            // Normalize input for case-insensitive comparison
            var company = job.Company.Trim().ToLower();
            var position = job.Position.Trim().ToLower();

            // Check if job already exists
            bool exists = await _repository.JobExistsAsync(company, position);
            if (exists)
            {
                _logger.LogWarning("Job application already exists for company: {Company}, position: {Position}", job.Company, job.Position);
                return Conflict("Job application already exists.");
            }

            var created = await _repository.AddJobApplicationAsync(job);

            _logger.LogInformation("Successfully created job application with ID {Id}", created.Id);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }



        // PUT: api/jobapplications/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<JobApplication>> Update(int id, JobApplication job)
        {
            _logger.LogInformation("Attempting to update job application with ID {Id}", id);

            var updated = await _repository.UpdateJobApplicationAsync(id, job);

            if (updated == null)
            {
                _logger.LogWarning("Job application with ID {Id} not found for update.", id);
                return NotFound();
            }

            _logger.LogInformation("Successfully updated job application with ID {Id}", id);
            return Ok(updated);
        }


        // DELETE: api/jobapplications/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Attempting to delete job application with ID {Id}", id);

            var deleted = await _repository.DeleteJobApplicationAsync(id);

            if (!deleted)
            {
                _logger.LogWarning("Job application with ID {Id} not found for deletion.", id);
                return NotFound();
            }

            _logger.LogInformation("Successfully deleted job application with ID {Id}", id);
            return NoContent();
        }

        [HttpPost("import")]
        public async Task<IActionResult> Import(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("Import failed: No file uploaded.");
                return BadRequest("No file uploaded.");
            }

            _logger.LogInformation("Starting import of job applications.");
            var importedJobs = new List<JobApplication>();

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            stream.Position = 0;

            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheets.First();
            var headerRow = worksheet.Row(1);

            var headers = headerRow.CellsUsed()
                .ToDictionary(c => c.Address.ColumnNumber, c => c.GetString().Trim().ToLowerInvariant());

            bool hasPositionColumn = headers.Values.Contains("position");

            foreach (var row in worksheet.RowsUsed().Skip(1))
            {
                var jobApp = new JobApplication();

                foreach (var cell in row.CellsUsed())
                {
                    if (!headers.TryGetValue(cell.Address.ColumnNumber, out string header))
                        continue;

                    switch (header)
                    {
                        case "company":
                            jobApp.Company = cell.GetString();
                            break;

                        case "position":
                            jobApp.Position = cell.GetString();
                            break;

                        case "location":
                            jobApp.Location = cell.GetString();
                            break;

                        case "date":
                            string cellValue = cell.GetString().Trim();

                            if (cell.DataType == XLDataType.DateTime)
                            {
                                jobApp.DateApplied = cell.GetDateTime().Date;
                            }
                            else if (DateTime.TryParseExact(cellValue, "MM/dd/yyyy",
                                         CultureInfo.InvariantCulture,
                                         DateTimeStyles.None,
                                         out var parsedDate))
                            {
                                jobApp.DateApplied = parsedDate.Date;
                            }
                            else if (DateTime.TryParse(cellValue, out var fallbackDate))
                            {
                                jobApp.DateApplied = fallbackDate.Date;
                            }
                            break;

                        case "status":
                            if (Enum.TryParse<Status>(cell.GetString(), true, out var status))
                                jobApp.Status = status;
                            break;

                        case "userid":
                            jobApp.UserId = cell.GetString();
                            break;
                    }
                }

                // Assign default if position column is missing or value is blank
                if (!hasPositionColumn || string.IsNullOrWhiteSpace(jobApp.Position))
                {
                    jobApp.Position = "Not Specified";
                }

                // Skip if company is missing
                if (string.IsNullOrWhiteSpace(jobApp.Company))
                    continue;

                // Skip if job already exists
                if (!await _repository.JobExistsAsync(jobApp.Company, jobApp.Position))
                {
                    importedJobs.Add(jobApp);
                }
            }

            if (importedJobs.Any())
            {
                var inserted = await _repository.AddJobApplicationsAsync(importedJobs);
                _logger.LogInformation($"File successfully imported. {inserted} new job(s) added.");
                return Ok(new { Inserted = inserted });
            }

            _logger.LogInformation("No records imported.");
            return Ok(new { Inserted = 0, Message = "No new jobs to import." });
        }



    }
}
