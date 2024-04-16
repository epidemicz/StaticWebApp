using System.Diagnostics;
using static AppBuilderExtensions;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.ConfigureFileServing(args, builder.Configuration);

app.MapGet("/hello", () => $"Hello from ${Environment.GetEnvironmentVariable("HOSTNAME")}");

Console.WriteLine($"Serving files from {Options.ServeDirectory} at https://localhost:{Options.ServerPort}");

if (!RunningInContainer)
{
    if (OperatingSystem.IsWindows())
    {
        Process.Start(new ProcessStartInfo($"https://localhost:{Options.ServerPort}") { UseShellExecute = true });
    }
    else if (OperatingSystem.IsLinux())
    {
        Process.Start("xdg-open", $"http://localhost:{Options.ServerPort}");
    }
}

app.Run("https://*:" + Options.ServerPort);
