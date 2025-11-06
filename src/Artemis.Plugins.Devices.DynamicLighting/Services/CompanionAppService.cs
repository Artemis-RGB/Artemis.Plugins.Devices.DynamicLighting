using System.Diagnostics;
using System.Text;
using System.Threading;
using Artemis.Core;
using Path = System.IO.Path;

namespace Artemis.Plugins.Devices.DynamicLighting.Services;

public static class CompanionAppService
{
    public static bool IsAppInstalled()
    {
        return GetAppPath() != null;
    }

    public static string? GetAppPath()
    {
        return GetAppxPackageProperty("InstallLocation");
    }

    public static string? GetAppFamilyName()
    {
        return GetAppxPackageProperty("PackageFamilyName");
    }
    
    public static void StartCompanionApp()
    {
        // Start Artemis.DynamicLighting.exe as a child process if it is not already running
        if (Process.GetProcessesByName("Artemis.DynamicLighting").Length > 0)
            return;
        
        var appPath = GetAppPath();
        if (appPath == null)
            throw new ArtemisPluginException("Companion app is not installed.");

        Process.Start(Path.Combine(appPath, "Artemis.DynamicLighting.exe"));
        
        // Wait for GetDeviceStatus to succeed
        
    }

    public static void StopCompanionApp()
    {
        // Kill the companion app process if it is running
        var process = Process.GetProcessesByName("Artemis.DynamicLighting");
        if (process.Length > 0)
        {
            process[0].Kill();
        }
    }
    
    private static string? GetAppxPackageProperty(string propertyName)
    {
        try
        {
            var psCommand = $"Get-AppxPackage -Name {Constants.CompanionAppPackageName} | Select-Object -ExpandProperty {propertyName}";

            var startInfo = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{psCommand}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            };

            using var process = new Process { StartInfo = startInfo };
            process.Start();

            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit(1000); // 1 second timeout

            if (!process.HasExited)
            {
                try { process.Kill(true); }
                catch { /* ignore */ }
                return null;
            }

            if (process.ExitCode != 0)
                return null;

            var value = output.Trim();
            if (string.IsNullOrWhiteSpace(value))
                return null;

            value = value.Trim('\r', '\n', '"');
            return string.IsNullOrWhiteSpace(value) ? null : value;
        }
        catch
        {
            return null;
        }
    }
}