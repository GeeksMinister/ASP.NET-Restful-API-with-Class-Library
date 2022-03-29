public class Project
{
#pragma warning disable CS8618
    public int Id { get; set; }
    [StringLength(50)]
    public string ProjectName { get; set; }
    [StringLength(50)]
    public string Customer { get; set; }
    [Column(TypeName = "Date")]
    public DateTime StartDate { get; set; } = DateTime.Now;
    public bool Delivered { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<Employee_Project>? Employee_Project { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<TimeReport> TimeReport { get; set; }

    public Project() { }


}
