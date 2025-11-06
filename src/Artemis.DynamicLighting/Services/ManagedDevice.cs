using System.Collections.Generic;
using Windows.Devices.Enumeration;
using Windows.Devices.Lights;
using Windows.System;
using Windows.UI;

namespace Artemis.DynamicLighting.Services;

public record ManagedDevice(DeviceInformation DeviceInfo, LampArray Device, LampInfo[] LampInfo, Dictionary<LampInfo, VirtualKey> VirtualKeys);