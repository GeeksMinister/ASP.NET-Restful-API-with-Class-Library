public class ProjectDto
{
#pragma warning disable CS8618
    [StringLength(50)]
    public string ProjectName { get; set; }
    [StringLength(50)]
    public string Customer { get; set; }
    public bool Delivered { get; set; }

    public ProjectDto() { }


}