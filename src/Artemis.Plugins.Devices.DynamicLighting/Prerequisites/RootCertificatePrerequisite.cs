using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Artemis.Core;

namespace Artemis.Plugins.Devices.DynamicLighting.Prerequisites;

public class RootCertificatePrerequisite : PluginPrerequisite
{
    public override string Name => "Companion App Root Certificate";
    public override string Description => "In order to install the companion app, a root certificate is required.";
    public override List<PluginPrerequisiteAction> InstallActions { get; }
    public override List<PluginPrerequisiteAction> UninstallActions { get; }

    public RootCertificatePrerequisite(Plugin plugin)
    {
        InstallActions =
        [
            new RunPowerShellAction("Install Companion App Root Certificate",
                plugin.ResolveRelativePath("Scripts/ManageCertificate.ps1"),
                true,
                $"-Action \"install\" -Thumbprint \"{Constants.CertificateThumbprint}\" -Password \"{Constants.CertificatePassword}\"")
        ];
        UninstallActions =
        [
            new RunPowerShellAction("Uninstall Companion App Root Certificate",
                plugin.ResolveRelativePath("Scripts/ManageCertificate.ps1"),
                true,
                $"-Action \"uninstall\" -Thumbprint \"{Constants.CertificateThumbprint}\" -Password \"{Constants.CertificatePassword}\"")
        ];
    }

    public override bool IsMet()
    {
        using var store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
        store.Open(OpenFlags.ReadOnly);
        var results = store.Certificates.Find(X509FindType.FindByThumbprint, Constants.CertificateThumbprint, false);
        return results.Count > 0;
    }
}