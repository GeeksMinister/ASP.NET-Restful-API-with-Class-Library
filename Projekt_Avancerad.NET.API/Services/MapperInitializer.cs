public class MapperInitializer : Profile
{
    public MapperInitializer()
    {
        CreateMap<EmployeeDto, Employee>().ReverseMap();
        CreateMap<Employee_ProjectDto, Employee_Project>().ReverseMap();
        CreateMap<ProjectDto, Project>().ReverseMap();
        CreateMap<TimeReportDto, TimeReport>().ReverseMap();
    }
}