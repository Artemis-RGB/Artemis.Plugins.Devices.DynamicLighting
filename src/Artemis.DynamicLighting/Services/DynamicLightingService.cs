using System;
using System.Threading.Tasks;
using Windows.UI;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Artemis.DynamicLighting.Services
{
    public class DynamicLightingService(DeviceManager deviceManager, ILogger<DynamicLightingService> logger) : DynamicLighting.DynamicLightingBase
    {
        public override Task<SetColorResponse> SetDeviceLEDsColor(SetDeviceLEDsColorRequest request, ServerCallContext context)
        {
            try
            {
                var targetDevice = deviceManager.GetDeviceById(request.DeviceId);

                if (targetDevice == null)
                {
                    return Task.FromResult(new SetColorResponse {Success = false});
                }

                var ledUpdates = request.LedUpdates;
                var colors = new Color[ledUpdates.Count];
                var indices = new int[ledUpdates.Count];
                for (var i = 0; i < ledUpdates.Count; i++)
                {
                    var update = ledUpdates[i];
                    colors[i] = Color.FromArgb((byte) update.Alpha, (byte) update.Red, (byte) update.Green, (byte) update.Blue);
                    indices[i] = update.LedIndex;
                }

                targetDevice.Device.SetColorsForIndices(colors, indices);
                return Task.FromResult(new SetColorResponse {Success = true});
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error setting LED colors on device {DeviceId}", request.DeviceId);
                return Task.FromResult(new SetColorResponse {Success = false});
            }
        }

        public override Task<GetDevicesStatusResponse> GetDevicesStatus(GetDevicesStatusRequest request, ServerCallContext context)
        {
            try
            {
                var response = new GetDevicesStatusResponse();

                foreach (var managedDevice in deviceManager.GetAllDevices())
                {
                    var deviceInfoMsg = new DeviceInfo
                    {
                        Id = managedDevice.Device.DeviceId,
                        Name = managedDevice.DeviceInfo.Name,
                        VendorId = managedDevice.Device.HardwareVendorId,
                        UpdateLimit = managedDevice.Device.MinUpdateInterval.TotalSeconds,
                        DeviceType = managedDevice.Device.LampArrayKind.ToString(),
                        Dimensions = new DeviceDimensions
                        {
                            Width = managedDevice.Device.BoundingBox.X,
                            Depth = managedDevice.Device.BoundingBox.Y,
                            Height = managedDevice.Device.BoundingBox.Z
                        },
                        
                    };

                    // Add LED information
                    foreach (var lampInfo in managedDevice.LampInfo)
                    {
                        var ledInfo = new LEDInfo
                        {
                            Index = lampInfo.Index,
                            Position = new LEDPosition
                            {
                                X = lampInfo.Position.X,
                                Y = lampInfo.Position.Y,
                                Z = lampInfo.Position.Z
                            }
                        };

                        if (managedDevice.VirtualKeys.TryGetValue(lampInfo, out var virtualKey))
                        {
                            ledInfo.VirtualKey = (int) virtualKey;
                            ledInfo.HasVirtualKey = true;
                        }

                        deviceInfoMsg.Leds.Add(ledInfo);
                    }

                    response.Devices.Add(deviceInfoMsg);
                }

                logger.LogInformation("Returning status for {DeviceCount} devices", response.Devices.Count);
                return Task.FromResult(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting device status");
                return Task.FromResult(new GetDevicesStatusResponse());
            }
        }
    }
}