using System.Security.Claims;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

[Route("api/[controller]")]
[ApiController]
public class CompanyController : ControllerBase
{
    private readonly ICompanyRepository<Employee, Project, TimeReport, Employee_Project> _companyRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _config;

    public CompanyController(ICompanyRepository<Employee, Project, TimeReport, Employee_Project> companyRepository,
        IHttpContextAccessor httpContextAccessor, IConfiguration config)
    {
        _companyRepository = companyRepository;
        _httpContextAccessor = httpContextAccessor;
        _config = config;
    }

    [HttpPost("Login")]
    public async Task<ActionResult<string>> Login(int id, string password)
    {
        var employee = await _companyRepository.GetEmployeeById(id);
        if (employee == null) return BadRequest("User Was NOT Found.");
        if (!_companyRepository.VerifyPassword(employee, password))
        {
            return BadRequest("Wrong password.");
        }
        string token = CreateToken(employee);
        SetRefreshToken(employee, new RefreshToken());
        return Ok(token);
    }

    [HttpPost("refresh-token"), Authorize(Roles = "Admin")]
    public async Task<ActionResult<string>> RefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        var employee = await GetEmployeeFromClaimIdentity();
        if (employee == null) return BadRequest();
        if (employee.RefreshToken!.Equals(refreshToken))
        {
            return Unauthorized("Invalid Refresh Token.");
        }
        else if (employee.TokenExpires < DateTime.Now)
        {
            return Unauthorized("Token expired.");
        }
        string token = CreateToken(employee);
        SetRefreshToken(employee, new RefreshToken());
        return Ok(token);
    }

    private void SetRefreshToken(Employee employee, RefreshToken refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = refreshToken.Expires
        };
        Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);
        employee.RefreshToken = refreshToken.Token;
        employee.TokenCreated = refreshToken.Created;
        employee.TokenExpires = refreshToken.Expires;
    }
    
    private async Task<Employee?> GetEmployeeFromClaimIdentity()
    {
        var id = _httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.SerialNumber);
        if (id == null) return null;
        return await _companyRepository.GetEmployeeById(int.Parse(id));
    }

    private string CreateToken(Employee employee)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Role, employee.Role),
            new Claim(ClaimTypes.Name, employee.FirstName),
            new Claim(ClaimTypes.MobilePhone, employee.Phone),
            new Claim(ClaimTypes.Email, employee.Email),
            new Claim(ClaimTypes.StreetAddress, employee.City),
            new Claim(ClaimTypes.SerialNumber, employee.Id.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _config.GetValue<string>("AppSettings:Token")));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }


    [HttpGet("AllEmployees"), Authorize(Roles = "Admin")]
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

    [HttpGet("GetTimeReport-Pagination")]
    public async Task<IActionResult> GetSomeTimeReports([FromQuery] ObjectParameters objectParameters)
    {
        try
        {
            var timeReport = await _companyRepository.GetSomeObjects(objectParameters);
            if (timeReport != null)
            {
                return Ok(timeReport);
            }
            return NotFound();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                        "Failed to retrive data from server");
        }
    }

    [HttpGet("GetTimeReports")]
    public async Task<IActionResult> GetEmployeeTimeReport(int id)
    {
        try
        {
            var employee = await _companyRepository.GetTimeReports(id);
            if (employee != null)
            {
                var totalReports = employee.Time_Reports.Count();
                if (totalReports == 0)
                {
                    return NotFound($"Employee with Id [{id}] has no registered TimeReports");
                }
                Response.Headers.Add("Total_TimeReports", JsonConvert.SerializeObject(totalReports));
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
                var totalEmployees = project.Employee_Project?.Count();
                if (totalEmployees == 0)
                {
                    return NotFound($"Project with Id [{id}] has no assigned employee");
                }
                Response.Headers.Add("Total_Employees", JsonConvert.SerializeObject(totalEmployees));
                return Ok(project);
            }
            return NotFound($"Project with Id [{id}] was NOT found");
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                        "Failed to retrieve data");
        }
    }

    [HttpGet("GetHoursInWeekTimeTable")]
    public async Task<IActionResult> GetHoursInWeekTimeTable([FromQuery] int employeeId, int weekNum)
    {
        try
        {
            var employee = await _companyRepository.GetRegisteredHoursInWeek(employeeId, weekNum);
            if (employee?.Time_Reports?.Any() ?? false)
            {
                var hours = employee.Time_Reports.Select(report => report.WorkingHours);
                Response.Headers.Add("Total_Hours", JsonConvert.SerializeObject(hours));
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
                //GetEmployeeTimeReport will only retrun the newly add employee.
                return CreatedAtAction(nameof(GetEmployeeTimeReport), new { Id = employee.Id }, employee);
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
                $"[{timeReportDto.EmployeeId}], ProjectId[{timeReportDto.ProjectId}]) weren't found");
        }
    }

    [HttpPost("AssignEmployeeToProject")]
    public async Task<IActionResult> AssignEmployee(Employee_ProjectDto employee_ProjectDto)
    {
        try
        {
            if (await _companyRepository.CheckDuplication(employee_ProjectDto))
            {
                return BadRequest("The employee is already assigned to the project");
            }
            if (employee_ProjectDto != null)
            {
                await _companyRepository.AssignEmployeeToProject(employee_ProjectDto);
                return Ok("The employee was successfully assigned");
            }
            return NotFound($"The entered(EmployeeId  [{employee_ProjectDto?.EmployeeId}]" +
                $" , ProjectId [{employee_ProjectDto?.ProjectId}]) weren't found");
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
            $"Failed to assign employee to project or the entered (EmployeeId[{employee_ProjectDto?.EmployeeId}]" +
                $", ProjectId[{employee_ProjectDto?.ProjectId}]) weren't found");
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
                return Ok("Employee was successfully removed");
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
                return Ok("Project was successfully removed");
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
                return Ok("TimeReport was successfully removed");
            }
            return NotFound($"TimeReport with Id [{id}] wasn't found");
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                "Failed to remove TimeReport");
        }
    }

    [HttpDelete("DismissEmployee")]
    public async Task<IActionResult> DismissEmployee(int employeeId, int projectId)
    {
        try
        {
            var emp_proj = await _companyRepository.DismissEmployeeFromProject(employeeId, projectId);
            if (emp_proj != null)
            {
                return Ok("Employee was successfully dismissed from project");
            }
            return NotFound($"The entered (EmployeeId[{employeeId}]," +
                $" ProjectId[{projectId}]) weren't found or not actually assigned");
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
      "Failed to dismiss employee from project");
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
            return BadRequest($"The entered(Id[{id}], EmployeeId[{ timeReportDto.EmployeeId}]" +
                             $", ProjectId[{timeReportDto.ProjectId}]) weren't found");
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to update TimeReport");
        }
    }

}