using RGB.NET.Core;
using RGB.NET.Devices.GRPC.Abstract;

namespace RGB.NET.Devices.GRPC.Generic;

public class GrpcDevice : AbstractRGBDevice<GrpcDeviceInfo>, IGrpcDevice
{
    /// <summary>
    /// Creates a new instance of the <see cref="GrpcDevice"/> class.
    /// </summary>
    /// <param name="deviceInfo">The generic information for the device.</param>
    /// <param name="updateQueue">The queue used to update the device.</param>
    public GrpcDevice(GrpcDeviceInfo deviceInfo, IUpdateQueue updateQueue) : base(deviceInfo, updateQueue)
    {
        InitializeLayout();
    }

    private void InitializeLayout()
    {
        var currentLedId = Helper.GetInitialLedIdForDeviceType(DeviceInfo.DeviceType);
        foreach (var sourceLed in DeviceInfo.Source.Leds)
        {
            // Scale the position up to match RGB.NET's coordinate system
            var position = new Point(sourceLed.Position.X * 1000, sourceLed.Position.Y * 1000);
            // Sadly LampArray defines no size, only position, so we have to assume 5x5
            var size = new Size(5, 5);
            if (sourceLed.HasVirtualKey)
            {
                var ledId = Helper.GetLedIdForVirtualKey(sourceLed.VirtualKey);
                if (ledId != null)
                {
                    AddLed(ledId.Value, position, size, sourceLed.Index);
                }
                else
                {
                    AddLed(currentLedId, position, size, sourceLed.Index);
                    currentLedId++;
                }
            }
            else
            {
                AddLed(currentLedId, position, size, sourceLed.Index);
                currentLedId++;
            }
        }
    }
}