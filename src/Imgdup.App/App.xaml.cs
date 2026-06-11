using System.IO;
using System.Windows;
using Imgdup.App.Services;
using Imgdup.App.ViewModels;
using Imgdup.Core.Caching;
using Imgdup.Core.Hashing;
using Imgdup.Core.Scanning;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Imgdup.App;

public partial class App : Application
{
    private readonly IHost _host;

    public App()
    {
        // WinExe apps detach from the console, so unhandled startup exceptions would vanish.
        // Capture them to a log file and surface a dialog instead of exiting silently.
        AppDomain.CurrentDomain.UnhandledException += (_, args) => Fatal(args.ExceptionObject as Exception);
        DispatcherUnhandledException += (_, args) =>
        {
            Fatal(args.Exception);
            args.Handled = true;
        };

        _host = Host.CreateApplicationBuilder()
            .ConfigureServices()
            .Build();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        _host.Start();
        _host.Services.GetRequiredService<MainWindow>().Show();
    }

    private static void Fatal(Exception? ex)
    {
        if (ex is null)
            return;

        var logPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Imgdup", "startup-error.log");
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);
            File.WriteAllText(logPath, $"{DateTime.Now:O}\n{ex}");
        }
        catch (IOException) { /* logging is best-effort */ }

        MessageBox.Show($"{ex.GetType().Name}: {ex.Message}\n\nLog: {logPath}", "Imgdup — erro na inicialização",
            MessageBoxButton.OK, MessageBoxImage.Error);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        (_host.Services.GetService<IHashCache>() as IDisposable)?.Dispose();
        _host.Dispose();
        base.OnExit(e);
    }
}

file static class HostBuilderExtensions
{
    public static HostApplicationBuilder ConfigureServices(this HostApplicationBuilder builder)
    {
        var services = builder.Services;

        var cacheDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Imgdup");
        Directory.CreateDirectory(cacheDir);

        services.AddSingleton<IImageHasher, ImageHasher>();
        services.AddSingleton<IHashCache>(_ => new SqliteHashCache(Path.Combine(cacheDir, "hashes.db")));
        services.AddSingleton<ImageScanEngine>();

        services.AddSingleton<IThumbnailService, SkiaThumbnailService>();
        services.AddSingleton<IFolderPickerService, FolderPickerService>();
        services.AddSingleton<IRecycleBinService, RecycleBinService>();
        services.AddSingleton<IUserDialogService, UserDialogService>();

        services.AddSingleton<MainViewModel>();
        services.AddSingleton<MainWindow>();

        return builder;
    }
}
