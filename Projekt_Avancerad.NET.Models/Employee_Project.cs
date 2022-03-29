public class Employee_Project
{
#pragma warning disable CS8618
    public int Id { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Employee Employee { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public int EmployeeId { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Project Project { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public int ProjectId { get; set; }

    public Employee_Project() { }

}


