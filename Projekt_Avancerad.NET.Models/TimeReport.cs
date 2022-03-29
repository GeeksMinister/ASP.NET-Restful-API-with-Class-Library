public class TimeReport
{
#pragma warning disable CS8618
    public int Id { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Employee Employee { get; set; }
    public int EmployeeId { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Project Project { get; set; }
    public int ProjectId { get; set; }
    [Range(1,53,ErrorMessage = "Invalid number of the week")]
    public int WeekNumber { get; set; }
    [Range(1, 60, ErrorMessage = "Invalid number of hours")]
    public double WorkingHours { get; set; }
    public int Year { get; set; } = DateTime.Now.Year;

    public TimeReport() { }
}
