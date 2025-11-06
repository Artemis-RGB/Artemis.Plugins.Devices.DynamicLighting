using System;
using System.Collections.Generic;
using Artemis.Core;
using Artemis.Plugins.Devices.DynamicLighting.Prerequisites.Actions;
using Artemis.Plugins.Devices.DynamicLighting.Services;
using Microsoft.Win32;

namespace Artemis.Plugins.Devices.DynamicLighting.Prerequisites;

public class BackgroundLightControlPrerequisite : PluginPrerequisite
{
    public override string Name => "Background Light Control";
    public override string Description => "The Artemis companion app should have background light control.";

    public override List<PluginPrerequisiteAction> InstallActions { get; } =
    [
        new SetBackgroundLightControlAction("Move Artemis to top", false)
    ];

    // Removing the app cleans this up for us
    
    public override List<PluginPrerequisiteAction> UninstallActions { get; } =[];

    public override bool IsMet()
    {
        var targetProvider = CompanionAppService.GetAppFamilyName();
        if (string.IsNullOrWhiteSpace(targetProvider))
            return false; // Companion app not installed
        
        // Check per-device providers
        using var devicesKey = Registry.CurrentUser.OpenSubKey($"{SetBackgroundLightControlAction.BasePath}\\Devices", writable: false);
        if (devicesKey != null)
        {
            foreach (var deviceName in devicesKey.GetSubKeyNames())
            {
                using var deviceKey = devicesKey.OpenSubKey(deviceName, writable: false);
                using var providersKey = deviceKey?.OpenSubKey("Providers", writable: false);
                if (providersKey == null)
                    continue; // no providers configured for this device

                if (!IsTopProvider(providersKey, targetProvider))
                    return false;
            }
        }

        // Check global providers
        using var globalProvidersKey = Registry.CurrentUser.OpenSubKey($"{SetBackgroundLightControlAction.BasePath}\\Providers", writable: false);
        if (globalProvidersKey != null)
        {
            if (!IsTopProvider(globalProvidersKey, targetProvider))
                return false;
        }

        // If nothing to check, there is nothing to fix
        return true;
    }

    private static bool IsTopProvider(RegistryKey providersKey, string targetProvider)
    {
        try
        {
            var value = providersKey.GetValue("1")?.ToString();
            return value != null && value.Equals(targetProvider, StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }
}