using System.Collections.Generic;
using Artemis.Core;
using Artemis.Plugins.Devices.DynamicLighting.Services;

namespace Artemis.Plugins.Devices.DynamicLighting.Prerequisites;

public class CompanionAppPrerequisite : PluginPrerequisite
{
    public override string Name => "Companion App";
    public override string Description => "Due to Microsoft API limitations, a companion app is required to use this plugin.";
    public override List<PluginPrerequisiteAction> InstallActions { get; }
    public override List<PluginPrerequisiteAction> UninstallActions { get; }

    public CompanionAppPrerequisite(Plugin plugin)
    {
        InstallActions =
        [
            new RunPowerShellAction("Install Companion App", plugin.ResolveRelativePath("Scripts/ManageApp.ps1"), false, $"-Action \"install\" -PackageName \"{Constants.CompanionAppPackageName}\"")
        ];
        UninstallActions =
        [
            new RunPowerShellAction("Uninstall Companion App", plugin.ResolveRelativePath("Scripts/ManageApp.ps1"), false, $"-Action \"uninstall\" -PackageName \"{Constants.CompanionAppPackageName}\"")
        ];
    }

    public override bool IsMet()
    {
        return CompanionAppService.IsAppInstalled();
    }
}