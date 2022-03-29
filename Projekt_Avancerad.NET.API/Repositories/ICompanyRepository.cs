public interface ICompanyRepository<T, U, V>
{
    Task<IEnumerable<T>> GetAll();
    Task<T> GetAllInfo(int id);
    Task<U> GetProjectInfoById(int id);
    Task<T> GetRegisteredHoursInWeek(int id, int weekNum);

    Task<T> AddEmployee(EmployeeDto employee);
    Task<U> AddProject(ProjectDto projectDto);
    Task<V> AddTimeReport(TimeReportDto timeReportDto);

    Task<T> DeleteEmployee(int id);
    Task<U> DeleteProject(int id);
    Task<V> DeleteTimeReport(int id);

    Task<T> UpdateEmployee(int id, EmployeeDto employee);
    Task<U> UpdateProject(int id, ProjectDto projectDto);
    Task<V> UpdateTimeReport(int id, TimeReportDto timeReportDto);


}