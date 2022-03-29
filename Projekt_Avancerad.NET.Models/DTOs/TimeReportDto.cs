public class TimeReportDto
{
#pragma warning disable CS8618
    public int EmployeeId { get; set; }
    public int ProjectId { get; set; }
    [Range(1, 53, ErrorMessage = "Invalid number of the week")]
    public int WeekNumber { get; set; }
    [Range(1, 60, ErrorMessage = "Invalid number of hours")]
    public double WorkingHours { get; set; }

    public TimeReportDto() { }
}
