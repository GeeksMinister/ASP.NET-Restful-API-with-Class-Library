public class Employee
{
#pragma warning disable CS8618
    public int Id { get; set; }
    [StringLength(50)]
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
    [StringLength(20)]
    public string City { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public IEnumerable<Employee_Project>? Employee_Projects { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<TimeReport> Time_Reports { get; set; }

    public string Role { get; set; } = "Employee";
    public byte[]? PasswordHash { get; set; }
    public byte[]? PasswordSalt { get; set; }
    public string? RefreshToken { get; set; } = string.Empty;
    public DateTime? TokenCreated { get; set; }
    public DateTime? TokenExpires { get; set; }
    
    public Employee() { }
}
