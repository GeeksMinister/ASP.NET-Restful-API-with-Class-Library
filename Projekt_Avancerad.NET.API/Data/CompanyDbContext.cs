public class CompanyDbContext : DbContext
{
#pragma warning disable CS8618
    public CompanyDbContext(DbContextOptions<CompanyDbContext> options) : base(options) { }

    public DbSet<Employee> Employees { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Employee_Project> Employees_Projects { get; set; }
    public DbSet<TimeReport> TimeReports { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee_Project>()
            .HasOne(emp_proj => emp_proj.Employee)
            .WithMany(emp => emp.Employee_Projects)
            .HasForeignKey(emp_proj => emp_proj.EmployeeId);

        modelBuilder.Entity<Employee_Project>()
            .HasOne(emp_proj => emp_proj.Project)
            .WithMany(proj => proj.Employee_Project)
            .HasForeignKey(emp_proj => emp_proj.ProjectId);
            
    }


}