public interface ICompanyRepository<T, U, V, W>
{
    // T : Employee
    // U : Project
    // V : TimeReport
    // W : Employee_Project
    Task<IEnumerable<T>> GetAll();
    Task<IEnumerable<V>> GetSomeObjects(ObjectParameters objectParameters);
    Task<Employee> GetEmployeeById(int id);
    bool VerifyPassword(Employee employee, string password);
    Task<T> GetTimeReports(int id);
    Task<U> GetProjectInfoById(int id);
    Task<T> GetRegisteredHoursInWeek(int id, int weekNum);
    Task<bool> CheckDuplication(Employee_ProjectDto employee_Project);

    Task<T> AddEmployee(EmployeeDto employee);
    Task<U> AddProject(ProjectDto projectDto);
    Task<V> AddTimeReport(TimeReportDto timeReportDto);
    Task<W> AssignEmployeeToProject(Employee_ProjectDto employee_ProjectDto);

    Task<T> DeleteEmployee(int id);
    Task<U> DeleteProject(int id);
    Task<V> DeleteTimeReport(int id);
    Task<W> DismissEmployeeFromProject(int employeeId, int projectID);

    Task<T> UpdateEmployee(int id, EmployeeDto employee);
    Task<U> UpdateProject(int id, ProjectDto projectDto);
    Task<V> UpdateTimeReport(int id, TimeReportDto timeReportDto);

}