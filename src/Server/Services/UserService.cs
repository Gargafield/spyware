
using Shared.Models;

namespace Server.Services;

public interface IUserService {
    Task<User> GetUserAsync(int id);
    Task<Teacher> GetTeacherAsync(int id);
    Task<Student> GetStudentAsync(int id);
    public bool DoesUserExist(string username);
    public bool TryGetUser(string username, out User user);
}

public class UserService : IUserService {
    private readonly List<User> _users = new();

    public Student student(string username) {
        var student = new Student {
            Id = _users.Count,
            Username = username
        };
        _users.Add(student);
        return student;
    }

    public Teacher teacher(string username) {
        var teacher = new Teacher {
            Id = _users.Count,
            Username = username
        };
        _users.Add(teacher);
        return teacher;
    }

    public UserService() {
        student("alice");
        student("bob");
        student("charlie");
        student("david");
        teacher("carol");
        teacher("eve");
    }

    public bool DoesUserExist(string username) {
        return _users.Any(user => user.Username == username);
    }

    public bool TryGetUser(string username, out User user) {
        user = _users.FirstOrDefault(user => user.Username == username)!;
        return user != null;
    }

    public async Task<Teacher> GetTeacherAsync(int id) {
        return (Teacher) await GetUserAsync(id);
    }

    public async Task<Student> GetStudentAsync(int id) {
        return (Student) await GetUserAsync(id);
    }

    public async Task<User> GetUserAsync(int id) {
        return _users[id];
    }
}