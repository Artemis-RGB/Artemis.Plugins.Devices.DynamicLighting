using Artemis.DynamicLighting;
using RGB.NET.Core;

namespace RGB.NET.Devices.GRPC.Generic;

public class GrpcUpdateQueue : UpdateQueue
{
    private readonly DynamicLighting.DynamicLightingClient _client;
    private readonly SetDeviceLEDsColorRequest _request;
    private readonly List<LEDColorUpdate> _ledUpdateBuffer = [];

    /// <summary>
    /// Creates a new instance of the <see cref="GrpcUpdateQueue"/> class.
    /// </summary>
    /// <param name="updateTrigger">The update trigger.</param>
    /// <param name="deviceInfo">The device info.</param>
    /// <param name="client">The gRPC client.</param>
    public GrpcUpdateQueue(IDeviceUpdateTrigger updateTrigger, DeviceInfo deviceInfo, DynamicLighting.DynamicLightingClient client) : base(updateTrigger)
    {
        _client = client;
        _request = new SetDeviceLEDsColorRequest {DeviceId = deviceInfo.Id};
    }

    /// <inheritdoc />
    protected override bool Update(ReadOnlySpan<(object key, Color color)> dataSet)
    {
        try
        {
            // Ensure buffer capacity without reallocations
            if (_ledUpdateBuffer.Capacity < dataSet.Length)
                _ledUpdateBuffer.Capacity = dataSet.Length;

            // Reuse existing objects or create new ones as needed
            var dataIndex = 0;
            foreach (var (key, color) in dataSet)
            {
                LEDColorUpdate ledUpdate;
                if (dataIndex < _ledUpdateBuffer.Count)
                {
                    // Reuse existing object
                    ledUpdate = _ledUpdateBuffer[dataIndex];
                }
                else
                {
                    // Create new object and add to buffer
                    ledUpdate = new LEDColorUpdate();
                    _ledUpdateBuffer.Add(ledUpdate);
                }

                // Update the LED data efficiently
                ledUpdate.LedIndex = (int) key;
                ledUpdate.Red = color.GetR();
                ledUpdate.Green = color.GetG();
                ledUpdate.Blue = color.GetB();
                ledUpdate.Alpha = color.GetA();

                dataIndex++;
            }

            // Trim buffer to actual data size for this update
            if (_ledUpdateBuffer.Count > dataSet.Length)
            {
                _ledUpdateBuffer.RemoveRange(dataSet.Length, _ledUpdateBuffer.Count - dataSet.Length);
            }

            // Reuse the request object
            _request.LedUpdates.Clear();
            _request.LedUpdates.AddRange(_ledUpdateBuffer);

            _client.SetDeviceLEDsColor(_request);
            return true;
        }
        catch (Exception e)
        {
            GrpcDeviceProvider.Instance.Throw(e);
        }

        return false;
    }
}