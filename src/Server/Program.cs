using Microsoft.AspNetCore.Authorization;
using Server.Auth;
using Server.Services;
using Server.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<ISocketService, SocketService>();
builder.Services.AddSingleton<IAuthService, AuthService>();

var configuration = new ConfigurationBuilder()
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

builder.Configuration.AddConfiguration(configuration);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication()
    .AddScheme<AuthSchemeOptions, AuthHandler>("Session", null);

builder.Services.AddAuthorization(options => {
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .AddAuthenticationSchemes("Session")
        .RequireAuthenticatedUser()
        .Build();
});

var app = builder.Build();

app.UseWebSockets(new WebSocketOptions() {
    KeepAliveInterval = TimeSpan.FromMinutes(2),
});

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
