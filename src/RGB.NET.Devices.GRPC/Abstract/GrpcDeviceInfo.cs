using Artemis.DynamicLighting;
using RGB.NET.Core;

namespace RGB.NET.Devices.GRPC.Abstract;

public class GrpcDeviceInfo : IRGBDeviceInfo
{
    public RGBDeviceType DeviceType { get; }
    public string DeviceName { get; }
    public string Manufacturer { get; }
    public string Model { get; }
    public object? LayoutMetadata { get; set; }
    public DeviceInfo Source { get; }

    public GrpcDeviceInfo(DeviceInfo deviceInfo)
    {
        Source = deviceInfo;
        DeviceName = deviceInfo.Id;
        Model = deviceInfo.Name;
        DeviceType = Helper.GetDeviceType(deviceInfo.DeviceType);
        Manufacturer = Helper.GetVendorDisplayName(deviceInfo.VendorId);
        LayoutMetadata = null;
    }
}