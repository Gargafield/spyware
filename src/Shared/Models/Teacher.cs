
using System.Text.Json.Serialization;

namespace Shared.Models;

public class Teacher : User {
    public override string Role => "Teacher";
}