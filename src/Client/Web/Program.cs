using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Web;
using Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton(sp => {
    return new HttpClient() { BaseAddress = new Uri(builder.Configuration["API_URL"]!) };
});
builder.Services.AddSingleton<ILocalStorageService, LocalStorageService>();
builder.Services.AddSingleton<IAuthService, AuthService>();
builder.Services.AddSingleton<IHttpService, HttpService>();
builder.Services.AddSingleton<AuthenticationStateProvider, AuthProvider>();

builder.Services.AddAuthorizationCore();

var app = builder.Build();

await app.RunAsync();
