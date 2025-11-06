using Artemis.Core;
using Artemis.Plugins.Devices.DynamicLighting.Prerequisites;

namespace Artemis.Plugins.Devices.DynamicLighting;

public class Bootstrapper : PluginBootstrapper
{
    public override void OnPluginLoaded(Plugin plugin)
    {
        AddPluginPrerequisite(new RootCertificatePrerequisite(plugin));
        AddPluginPrerequisite(new CompanionAppPrerequisite(plugin));
        AddPluginPrerequisite(new BackgroundLightControlPrerequisite());
    }

    public override void OnPluginEnabled(Plugin plugin)
    {
    }

    public override void OnPluginDisabled(Plugin plugin)
    {
    }
}