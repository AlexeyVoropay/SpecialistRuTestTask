namespace Specialist.Models;

public class Course
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime StartDate { get; set; }
}
