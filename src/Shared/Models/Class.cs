
namespace Shared.Models;

public class Class {
    public int Id { get; set; }
    public int TeacherId { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }
    public List<Student> Students { get; set; }
}