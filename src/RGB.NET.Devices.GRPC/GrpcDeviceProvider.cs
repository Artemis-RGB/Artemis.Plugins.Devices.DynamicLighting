using Artemis.DynamicLighting;
using Grpc.Net.Client;
using RGB.NET.Core;
using RGB.NET.Devices.GRPC.Abstract;
using RGB.NET.Devices.GRPC.Generic;

namespace RGB.NET.Devices.GRPC;

public class GrpcDeviceProvider : AbstractRGBDeviceProvider
{
    private DynamicLighting.DynamicLightingClient? _client;

    // ReSharper disable once InconsistentNaming
    private static readonly Lock _lock = new();
    private static GrpcDeviceProvider? _instance;

    /// <summary>
    /// Gets or sets the gRPC channel used to communicate with the Dynamic Lighting service.
    /// </summary>
    public GrpcChannel? GrpcChannel { get; set; }
    
    /// <summary>
    /// Gets the singleton <see cref="GrpcDeviceProvider"/> instance.
    /// </summary>
    public static GrpcDeviceProvider Instance
    {
        get
        {
            lock (_lock)
                return _instance ?? new GrpcDeviceProvider();
        }
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="GrpcDeviceProvider"/> class.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if this constructor is called even if there is already an instance of this class.</exception>
    public GrpcDeviceProvider()
    {
        lock (_lock)
        {
            if (_instance != null)
                throw new InvalidOperationException($"There can be only one instance of type {nameof(GrpcDeviceProvider)}");
            _instance = this;
        }   
    }

    /// <inheritdoc />
    protected override void InitializeSDK()
    {
        _client = new DynamicLighting.DynamicLightingClient(GrpcChannel);
    }

    protected override void Dispose(bool disposing)
    {
        lock (_lock)
        {
            _client = null;
            _instance = null;
        }
    }

    /// <inheritdoc />
    protected override IEnumerable<IRGBDevice> LoadDevices()
    {
        if (_client == null)
            throw new InvalidOperationException("gRPC client is not initialized.");

        var devicesStatus = _client.GetDevicesStatus(new GetDevicesStatusRequest());
        for (var index = 0; index < devicesStatus.Devices.Count; index++)
        {
            var deviceInfo = devicesStatus.Devices[index];
            var updateTrigger = GetUpdateTrigger(index, deviceInfo.UpdateLimit);
            yield return new GrpcDevice(new GrpcDeviceInfo(deviceInfo), new GrpcUpdateQueue(updateTrigger, deviceInfo, _client));
        }
    }
}