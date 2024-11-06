
using Shared.Models;

namespace Server.Services;

public interface IClassService {
    Task<Class> GetClassAsync(int id);
    Task<Class> GetClassForTeacherAsync(int teacherId);
    Task<Class> GetClassForStudentAsync(int studentId);
}

public class ClassService : IClassService {
    public List<Class> Classes { get; } = new();

    public ClassService() {
        Classes.Add(new Class {
            Id = 0,
            TeacherId = 4,
            Name = "Math",
            Description = "Learn math",
            Students = new List<Student> {
                new Student { Id = 0, Username = "Alice" },
                new Student { Id = 1, Username = "Bob" }
            }
        });
        Classes.Add(new Class {
            Id = 1,
            TeacherId = 5,
            Name = "Science",
            Description = "Learn science",
            Students = new List<Student> {
                new Student { Id = 2, Username = "Charlie" },
                new Student { Id = 3, Username = "David" }
            }
        });
    }

    public async Task<Class> GetClassAsync(int id) {
        return Classes[id];
    }

    public async Task<Class> GetClassForTeacherAsync(int teacherId) {
        return Classes.FirstOrDefault(c => c.TeacherId == teacherId)!;
    }

    public async Task<Class> GetClassForStudentAsync(int studentId) {
        return Classes.FirstOrDefault(c => c.Students.Any(s => s.Id == studentId))!;
    }
}