using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

[Route("api/[controller]")]
[ApiController]
public class CompanyController : ControllerBase
{
    private readonly ICompanyRepository<Employee, Project, TimeReport> _companyRepository;
    public CompanyController(ICompanyRepository<Employee, Project, TimeReport> companyRepository)
    {
        _companyRepository = companyRepository;
    }

    [HttpGet("AllEmployees")]
    public async Task<IActionResult> GetAllEmployees()
    {
        try
        {
            var employees = await _companyRepository.GetAll();
            if (employees != null)
            {
                return Ok(employees);
            }
            return NotFound();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                        "Failed to retrive data from server");
        }
    }

    [HttpGet("GetTimeTables")]
    public async Task<IActionResult> GetEmployeeTimeReport(int id)
    {
        try
        {
            var employee = await _companyRepository.GetAllInfo(id);
            if (employee != null)
            {
                return Ok(employee);
            }
            return NotFound($"Employee with Id [{id}] was not found");
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                        "Failed to retrieve data");
        }
    }

    [HttpGet("GetProjectEmployees")]
    public async Task<IActionResult> GetProjectEmployees(int id)
    {
        try
        {
            var project = await _companyRepository.GetProjectInfoById(id);
            if (project != null)
            {
                return Ok(project);
            }
            return NotFound($"Project with Id [{id}] was not found");
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                        "Failed to retrieve data");
        }
    }

    [HttpGet("GetWeekTimeTable")]
    public async Task<IActionResult> GetWeekTimeTable([FromQuery] int employeeId, int weekNum)
    {
        try
        {
            var employee = await _companyRepository.GetRegisteredHoursInWeek(employeeId, weekNum);
            if (employee != null && employee.Time_Reports.Any())
            {
                var hours = employee.Time_Reports.Select(report => report.WorkingHours);
                Response.Headers.Add("totalHours", JsonConvert.SerializeObject(hours));
                return Ok(employee);
            }
            return NotFound($"The entered (EmployeeId [{employeeId}] , week number" +
                            $" [{weekNum}]) weren't found");
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
            "Failed to retrieve data");
        }
    }

    [HttpPost("AddEmployee")]
    public async Task<IActionResult> AddEmployee(EmployeeDto employeeDto)
    {
        try
        {
            var employee = await _companyRepository.AddEmployee(employeeDto);
            if (employee != null)
            {
                return Ok(employee);
            }
            return BadRequest();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
            "Failed to add a new Employee");
        }
    }

    [HttpPost("AddProject")]
    public async Task<IActionResult> AddProject(ProjectDto projectDto)
    {
        try
        {
            var project = await _companyRepository.AddProject(projectDto);
            if (project != null)
            {
                return Ok(project);
            }
            return BadRequest();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
            "Failed to add a new Project");
        }
    }

    [HttpPost("AddTimeReport")]
    public async Task<IActionResult> AddTimeResult(TimeReportDto timeReportDto)
    {
        try
        {
            var timeReport = await _companyRepository.AddTimeReport(timeReportDto);
            if (timeReport != null)
            {
                return Ok(timeReport);
            }
            return BadRequest();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
            "Failed to add a new TimeReport or the entered (EmployeeId" +
                $" [{timeReportDto.EmployeeId}] , ProjectId [{timeReportDto.ProjectId}]) weren't found");
        }
    }

    [HttpDelete("DeleteEmployee")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        try
        {
            var employee = await _companyRepository.DeleteEmployee(id);
            if (employee != null)
            {
                return Ok(employee);
            }
            return NotFound($"Employee with Id [{id}] was not found");
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
            "Failed to remove Employee");
        }
    }

    [HttpDelete("DeleteProject")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        try
        {
            var project = await _companyRepository.DeleteProject(id);
            if (project != null)
            {
                return Ok(project);
            }
            return NotFound($"Project with Id [{id}] wasn't found");
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
            "Failed to remove Project");
        }
    }

    [HttpDelete("DeleteTimeReport")]
    public async Task<IActionResult> DeleteTimeReport(int id)
    {
        try
        {
            var timeReport = await _companyRepository.DeleteTimeReport(id);
            if (timeReport != null)
            {
                return Ok(timeReport);
            }
            return NotFound($"TimeReport with Id [{id}] wasn't found");
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                "Failed to remove TimeReport");
        }
    }

    [HttpPut("UpdateEmployee")]
    public async Task<IActionResult> UpdateEmployee(int id, EmployeeDto employeeDto)
    {
        try
        {
            var employee = await _companyRepository.UpdateEmployee(id, employeeDto);
            if (employee != null)
            {
                return Ok(employee);
            }
            return NotFound($"Employee with Id [{id}] was NOT found");
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                "Failed to update Employee");
        }
    }

    [HttpPut("UpdateProject")]
    public async Task<IActionResult> UpdateProject(int id, ProjectDto projectDto)
    {
        try
        {
            var project = await _companyRepository.UpdateProject(id, projectDto);
            if (project != null)
            {
                return Ok(project);
            }
            return NotFound($"Project with id [{id}] was NOT fount");
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to update Project");
        }
    }

    [HttpPut("UpdateTimeReport")]
    public async Task<IActionResult> UpdateTimeReport([FromQuery] int id, TimeReportDto timeReportDto)
    {
        try
        {
            var timeReport = await _companyRepository.UpdateTimeReport(id, timeReportDto);
            if (timeReport != null)
            {
                return Ok(timeReport);
            }
            return BadRequest($"The entered(Id [{id}], EmployeeId[{ timeReportDto.EmployeeId}]" +
                             $", ProjectId [{timeReportDto.ProjectId}]) weren't found");
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to update TimeReport");
        }
    }

}