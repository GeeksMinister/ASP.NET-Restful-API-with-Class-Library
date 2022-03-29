public class EmployeeDto
{
#pragma warning disable CS8618
    public string FirstName { get; set; }
    [StringLength(50)]
    public string LastName { get; set; }
    [Range(18, 90, ErrorMessage = "Age must be from 18 - 90")]
    public int Age { get; set; } = 20;
    [StringLength(50)]
    [Range(0, Int64.MaxValue, ErrorMessage = "Contact number should not contain characters")]
    public string Phone { get; set; }
    [StringLength(300)]
    public string Email { get; set; }
    [StringLength(30)]
    public string? City { get; set; }
    public EmployeeDto() { }
}
