public class CompanyRepository : ICompanyRepository<Employee, Project, TimeReport, Employee_Project>
{
#pragma warning disable CS8600
#pragma warning disable CS8603
#pragma warning disable CS8620
    private readonly CompanyDbContext _companyDbContext;
    private readonly IMapper _mapper;

    public CompanyRepository(CompanyDbContext companyDbContext, IMapper mapper)
    {
        _companyDbContext = companyDbContext;
        _mapper = mapper;
    }

    public async Task<IEnumerable<Employee>> GetAll()
    {
        return await _companyDbContext.Employees.ToListAsync();
    }

    public async Task<IEnumerable<TimeReport>> GetSomeObjects(ObjectParameters objectParameters)
    {
        return await _companyDbContext.TimeReports
            .Skip((objectParameters.PageNumber - 1) * objectParameters.PageSize)
            .Take(objectParameters.PageSize)
            .ToListAsync();
    }

    public async Task<Employee> GetTimeReports(int id)
    {
        return await _companyDbContext.Employees
            .Include(e => e.Time_Reports).FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Project> GetProjectInfoById(int id)
    {
        return await (from project in _companyDbContext.Projects
                      where project.Id == id
                      select project).Include(e => e.Employee_Project).ThenInclude(e => e.Employee)
                     .FirstOrDefaultAsync();
    }

    public async Task<Employee> GetRegisteredHoursInWeek(int id, int weekNum)
    {
        var result = await _companyDbContext.Employees.Include(
            emp => emp.Time_Reports.Where(report => report.WeekNumber == weekNum)).
        FirstOrDefaultAsync(emp => emp.Id == id);
        return result;
    }

    public async Task<Employee> AddEmployee(EmployeeDto employeeDto)
    {
        Employee employee = _mapper.Map<Employee>(employeeDto);
        var newEmployee = await _companyDbContext.Employees.AddAsync(employee);
        await _companyDbContext.SaveChangesAsync();
        return newEmployee.Entity;
    }

    public async Task<Project> AddProject(ProjectDto projectDto)
    {
        Project project = _mapper.Map<Project>(projectDto);
        var newProject = await _companyDbContext.Projects.AddAsync(project);
        await _companyDbContext.SaveChangesAsync();
        return newProject.Entity;
    }

    public async Task<TimeReport> AddTimeReport(TimeReportDto timeReportDto)
    {
        TimeReport timeReport = _mapper.Map<TimeReport>(timeReportDto);
        var newTimeReport = await _companyDbContext.TimeReports.AddAsync(timeReport);
        await _companyDbContext.SaveChangesAsync();
        return newTimeReport.Entity;
    }
    public async Task<bool> CheckDuplication(Employee_ProjectDto employee_ProjectDto)
    {
        var result = await _companyDbContext.Employees_Projects.FirstOrDefaultAsync(
            emp_proj => emp_proj.EmployeeId == employee_ProjectDto.EmployeeId &&
            emp_proj.ProjectId == employee_ProjectDto.ProjectId);
        if (result != null)
        {
            return true;
        }
        else return false;
    }

    public async Task<Employee_Project> AssignEmployeeToProject(Employee_ProjectDto employee_ProjectDto)
    {
        Employee_Project employee_Project = _mapper.Map<Employee_Project>(employee_ProjectDto);
        var newEmployee_Project = await _companyDbContext.Employees_Projects.AddAsync(employee_Project);
        await _companyDbContext.SaveChangesAsync();
        return newEmployee_Project.Entity;
    }

    public async Task<Employee> DeleteEmployee(int id)
    {
        var employee = await _companyDbContext.Employees.FirstOrDefaultAsync(emp => emp.Id == id);
        if (employee != null)
        {
            _companyDbContext.Employees.Remove(employee);
            await _companyDbContext.SaveChangesAsync();
        }
        return employee;
    }

    public async Task<Project> DeleteProject(int id)
    {
        var project = await _companyDbContext.Projects.FirstOrDefaultAsync(proj => proj.Id == id);
        if (project != null)
        {
            _companyDbContext.Projects.Remove(project);
            await _companyDbContext.SaveChangesAsync();
        }
        return project;
    }

    public async Task<TimeReport> DeleteTimeReport(int id)
    {
        var timeReport = await _companyDbContext.TimeReports.FirstOrDefaultAsync(
            timeReportDto => timeReportDto.Id == id);
        if (timeReport != null)
        {
            _companyDbContext.TimeReports.Remove(timeReport);
            await _companyDbContext.SaveChangesAsync();
        }
        return timeReport;
    }

    public async Task<Employee_Project> DismissEmployeeFromProject(int employeeId, int projectID)
    {
        Employee_Project employee_Project = await _companyDbContext.Employees_Projects
            .FirstOrDefaultAsync(emp_proj => emp_proj.EmployeeId == employeeId && emp_proj.ProjectId == projectID);
        if (employee_Project != null)
        {
            _companyDbContext.Employees_Projects.Remove(employee_Project);
            await _companyDbContext.SaveChangesAsync();
        }
        return employee_Project;
    }

    public async Task<Employee> UpdateEmployee(int id, EmployeeDto employeeDto)
    {
        var employee = await _companyDbContext.Employees.FirstOrDefaultAsync(emp => emp.Id == id);
        if (employee != null && employeeDto != null)
        {
            employee.FirstName = employeeDto.FirstName;
            employee.LastName = employeeDto.LastName;
            employee.Age = employeeDto.Age;
            employee.Phone = employeeDto.Phone;
            employee.Email = employeeDto.Email;
            employee.City = employeeDto.City;
            await _companyDbContext.SaveChangesAsync();
        }
        return employee;
    }

    public async Task<Project> UpdateProject(int id, ProjectDto projectDto)
    {
        var project = await _companyDbContext.Projects.FirstOrDefaultAsync(project => project.Id == id);
        if (project != null && projectDto != null)
        {
            project.ProjectName = projectDto.ProjectName;
            project.Customer = projectDto.Customer;
            project.Delivered = projectDto.Delivered;
            await _companyDbContext.SaveChangesAsync();
        }
        return project;
    }

    public async Task<TimeReport> UpdateTimeReport(int id, TimeReportDto timeReportDto)
    {
        var timeReport = await _companyDbContext.TimeReports.FirstOrDefaultAsync(report => report.Id == id);
        if (timeReport != null && timeReportDto != null)
        {
            timeReport.EmployeeId = timeReportDto.EmployeeId;
            timeReport.ProjectId = timeReportDto.ProjectId;
            timeReport.WeekNumber = timeReportDto.WeekNumber;
            timeReport.WorkingHours = timeReportDto.WorkingHours;
            await _companyDbContext.SaveChangesAsync();
        }
        return timeReport;
    }

}