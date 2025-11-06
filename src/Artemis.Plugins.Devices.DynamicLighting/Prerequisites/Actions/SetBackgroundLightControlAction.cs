using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Artemis.Core;
using Artemis.Plugins.Devices.DynamicLighting.Services;
using Microsoft.Win32;

namespace Artemis.Plugins.Devices.DynamicLighting.Prerequisites.Actions;

public class SetBackgroundLightControlAction(string name, bool reset) : PluginPrerequisiteAction(name)
{
    public const string BasePath = @"Software\Microsoft\Lighting";

    public override Task Execute(CancellationToken cancellationToken)
    {
        var provider = reset ? "WindowsLighting" : CompanionAppService.GetAppFamilyName();
        if (string.IsNullOrWhiteSpace(provider))
            throw new ArtemisPluginException("Failed to determine companion app family name, not installed?");

        // 1) Per-device providers: HKCU\\Software\\Microsoft\\Lighting\\Devices\\<device>\\Providers
        using (var devicesKey = Registry.CurrentUser.OpenSubKey($"{BasePath}\\Devices", writable: true))
        {
            if (devicesKey != null)
            {
                foreach (var deviceName in devicesKey.GetSubKeyNames())
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    using var deviceKey = devicesKey.OpenSubKey(deviceName, writable: true);
                    using var providersKey = deviceKey?.OpenSubKey("Providers", writable: true);
                    if (providersKey == null)
                        continue;

                    ReorderProvidersSubKeys(providersKey, provider, cancellationToken);
                }
            }
        }

        // 2) Global providers: HKCU\\Software\\Microsoft\\Lighting\\Providers
        using (var globalProvidersKey = Registry.CurrentUser.OpenSubKey($"{BasePath}\\Providers", writable: true))
        {
            if (globalProvidersKey != null)
            {
                ReorderProvidersSubKeys(globalProvidersKey, provider, cancellationToken);
            }
        }

        return Task.CompletedTask;
    }

    private static void ReorderProvidersSubKeys(RegistryKey providersKey, string targetProvider, CancellationToken ct)
    {
        // The Windows Lighting registry uses numeric VALUE names ("1", "2", ...) under Providers.
        // We must reorder those VALUES, not subkeys. Keep non-numeric values intact.
        var numericValueNames = providersKey
            .GetValueNames()
            .Select(n => int.TryParse(n, out var i) ? (ok: true, i, n) : (ok: false, i: 0, n))
            .Where(t => t.ok)
            .OrderBy(t => t.i)
            .Select(t => t.n)
            .ToList();

        // Read existing provider IDs in order
        var orderedValues = new List<string>();
        foreach (var name in numericValueNames)
        {
            ct.ThrowIfCancellationRequested();
            var value = providersKey.GetValue(name) as string;
            if (!string.IsNullOrWhiteSpace(value))
                orderedValues.Add(value!);
        }

        // Build new order: target on top, then the rest in their original order without duplicates.
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var finalList = new List<string>();

        AddOnce(targetProvider);
        foreach (var v in orderedValues)
        {
            if (!v.Equals(targetProvider, StringComparison.OrdinalIgnoreCase))
                AddOnce(v);
        }

        // Delete existing numeric values to avoid leftovers
        foreach (var name in numericValueNames)
        {
            ct.ThrowIfCancellationRequested();
            providersKey.DeleteValue(name, throwOnMissingValue: false);
        }

        // Write back as 1..N
        for (var i = 0; i < finalList.Count; i++)
        {
            ct.ThrowIfCancellationRequested();
            providersKey.SetValue((i + 1).ToString(), finalList[i], RegistryValueKind.String);
        }

        void AddOnce(string v)
        {
            if (seen.Add(v))
                finalList.Add(v);
        }
    }
}