using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Windows.Devices.Enumeration;
using Windows.Devices.Lights;
using Windows.Devices.Lights.Effects;
using Windows.System;
using Windows.UI;
using Enum = System.Enum;

namespace Artemis.DynamicLighting.Services
{
    public class DeviceManager
    {
        private readonly ConcurrentDictionary<string, ManagedDevice> _connectedDevices = [];

        public void AddDevice(string deviceId, DeviceInformation deviceInformation, LampArray lampArray)
        {
            // Gather lamp info
            var lampIndices = Enumerable.Range(0, lampArray.LampCount).ToArray();
            var lampInfo = lampIndices.Select(lampArray.GetLampInfo).ToArray();

            // Map virtual keys to lamp info if supported
            var virtualKeys = new Dictionary<LampInfo, VirtualKey>();
            if (lampArray.SupportsVirtualKeys)
            {
                foreach (var virtualKey in Enum.GetValues<VirtualKey>())
                {
                    var indexForKey = lampArray.GetIndicesForKey(virtualKey).FirstOrDefault(-1);
                    if (indexForKey == -1)
                        continue;

                    var lamp = lampInfo[indexForKey];
                    virtualKeys[lamp] = virtualKey;
                }
            }

            var managedDevice = new ManagedDevice(deviceInformation, lampArray, lampInfo, virtualKeys);
            _connectedDevices.TryAdd(deviceId, managedDevice);
        }

        public void RemoveDevice(string deviceId)
        {
            _connectedDevices.TryRemove(deviceId, out _);
        }

        public ManagedDevice? GetDeviceById(string deviceId)
        {
            _connectedDevices.TryGetValue(deviceId, out var device);
            return device;
        }

        public List<ManagedDevice> GetAllDevices()
        {
            return _connectedDevices.Values.ToList();
        }
    }
}