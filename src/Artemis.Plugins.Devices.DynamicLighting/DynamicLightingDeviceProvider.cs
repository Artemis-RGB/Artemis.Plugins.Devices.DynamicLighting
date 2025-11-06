using System;
using System.IO.Pipes;
using System.Net.Http;
using System.Threading.Tasks;
using Artemis.Core;
using Artemis.Core.DeviceProviders;
using Artemis.Core.Services;
using Artemis.Plugins.Devices.DynamicLighting.Services;
using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
using RGB.NET.Core;
using Serilog;
using RGBDeviceProvider = RGB.NET.Devices.GRPC.GrpcDeviceProvider;

namespace Artemis.Plugins.Devices.DynamicLighting;

public class DynamicLightingDeviceProvider : DeviceProvider
{
    private readonly IDeviceService _deviceService;
    private readonly ILogger _logger;

    public DynamicLightingDeviceProvider(IDeviceService deviceService, ILogger logger)
    {
        _deviceService = deviceService;
        _logger = logger;

        CreateMissingLedsSupported = false;
        RemoveExcessiveLedsSupported = false;
    }

    public override void Enable()
    {
        CompanionAppService.StartCompanionApp();
        
        var channel = GrpcChannel.ForAddress("http://localhost", new GrpcChannelOptions {HttpHandler = CreateNamedPipeHandler("Artemis.DynamicLighting")});
        RgbDeviceProvider.GrpcChannel = channel;
        RgbDeviceProvider.Exception += RgbDeviceProviderOnException;

        try
        {
            _deviceService.AddDeviceProvider(this);
        }
        catch (Exception e)
        {
            throw new ArtemisPluginException("Failed to activate DynamicLighting plugin", e);
        }
    }

    public override void Disable()
    {
        _deviceService.RemoveDeviceProvider(this);
        RgbDeviceProvider.Exception -= RgbDeviceProviderOnException;
        RgbDeviceProvider.Dispose();
        RgbDeviceProvider.GrpcChannel?.Dispose();
        RgbDeviceProvider.GrpcChannel = null;

        CompanionAppService.StopCompanionApp();
    }

    public override RGBDeviceProvider RgbDeviceProvider => RGBDeviceProvider.Instance;

    private void RgbDeviceProviderOnException(object? sender, ExceptionEventArgs e)
    {
        _logger.Debug(e.Exception, "DynamicLighting Exception: {message}", e.Exception.Message);
    }

    private static SocketsHttpHandler CreateNamedPipeHandler(string pipeName)
    {
        return new SocketsHttpHandler
        {
            ConnectCallback = async (_, cancellationToken) =>
            {
                var pipeClientStream = new NamedPipeClientStream(
                    serverName: ".",
                    pipeName: pipeName,
                    direction: PipeDirection.InOut,
                    options: PipeOptions.Asynchronous);

                try
                {
                    await pipeClientStream.ConnectAsync(5000, cancellationToken);
                    await Task.Delay(200, cancellationToken);
                    return pipeClientStream;
                }
                catch
                {
                    await pipeClientStream.DisposeAsync();
                    throw;
                }
            }
        };
    }
}