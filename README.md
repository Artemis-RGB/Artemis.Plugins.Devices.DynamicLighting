# Introduction
This is the companion app for the plugin. It acts as a proxy between the WinUI3 LampArray API and the rest of Artemis.  
The application sets up a gRPC server and exposes the LampArray API via a named pipe.

The proto file is located in the `Proto` folder and can be used to create a client, which is the RGB.NET device provider.

# Building the companion app
For the app to be installed it must be packaged and signed.  
A self-signed certificate is used and trusted by the plugin before installing the app.

1. First package the application as an `.appx` file
```shell
dotnet build Artemis.DynamicLighting -c Release -p:Platform=x64 -p:GenerateAppxPackageOnBuild=true -p:AppxPackageSigningEnabled=false
 ```

2. Generate a certificate (in an elevated PowerShell prompt)
```powershell
cd Artemis.DynamicLighting\AppPackages
$cert = New-SelfSignedCertificate -Type Custom -Subject "CN=Artemis RGB" -KeyUsage DigitalSignature -FriendlyName "Artemis RGB Certificate" -TextExtension @("2.5.29.37={text}1.3.6.1.5.5.7.3.3", "2.5.29.19={text}"); $thumbprint = $cert.Thumbprint; $cert | Export-PfxCertificate -FilePath "ArtemisRGB.pfx" -Password (ConvertTo-SecureString -String "artemis-rgb" -Force -AsPlainText); Write-Host "Thumbprint: $thumbprint"
```
> Note down the thumbprint.

3. Use SignTool to sign the package with the certificate
```powershell
SignTool sign /a /v /fd SHA256 /f "ArtemisRGB.pfx" /p "artemis-rgb" "Artemis.DynamicLighting.appx"
```

4. Add the `.pfx` and signed package to the plugin

5. Update the certificate thumbprint in the plugin
