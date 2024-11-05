
using Shared.Models;

namespace Web.Services;

public interface IClassService {
    Task<Class> GetCurrentClassAsync();
}

public class ClassService : IClassService {
    private readonly IHttpService _httpService;

    public ClassService(IHttpService httpService) {
        _httpService = httpService;
    }

    public async Task<Class> GetCurrentClassAsync() {
        return await _httpService.GetAsync<Class>("api/class/current");
    }
}