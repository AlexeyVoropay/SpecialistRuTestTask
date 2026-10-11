using Specialist.Models;

namespace Specialist.DTOs;

public class CartRequest
{
    public bool IsGraduate { get; set; }
    public IEnumerable<Course> Courses { get; set; } = [];
}