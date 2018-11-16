using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;
using adcoso.iot.devicedrivers.neuron.driver.commons;

namespace adcoso.iot.devicedrivers.neuron.driver.boardcommunication.i2c
{
    internal class I2CConnector
    {
        #region Private Members

        
        private readonly Dictionary<UnipiI2CDevice, I2cDevice> _devices;
        
        private readonly object _busLock;
        
        private readonly DriverLogger _logger;

        #endregion

        #region Constructor

        internal I2CConnector(DriverLogger logger)
        {
            _logger = logger;
            _busLock = new object();
            _devices = new Dictionary<UnipiI2CDevice, I2cDevice>();
        }

        #endregion Constructor

        #region Internal Methods

        internal async Task Initialize()
        {
            try
            {
                _logger.LogDebug(this, "Initializing...");

                var deviceSelector = I2cDevice.GetDeviceSelector("I2C1");

                if (deviceSelector == null)
                {
                    throw new Exception("No deviceSelector for I2C found!");
                }

                var devices = DeviceInformation.FindAllAsync(deviceSelector);

                if (devices == null)
                {
                    throw new Exception("Find All Async failed");
                }

                var unipiI2CDevices = Enum.GetValues(typeof(UnipiI2CDevice));

                if (unipiI2CDevices == null)
                {
                    throw new Exception("No I2C Devices in Configuration Enumn");
                }

                foreach (UnipiI2CDevice unipiI2CDevice in unipiI2CDevices)
                {
                    var found = false;

                    await devices;

                    var deviceInformationCollection = devices.GetResults();

                    if (deviceInformationCollection == null)
                    {
                        throw new Exception("No devices found!");
                    }

                    foreach (var deviceInformation in deviceInformationCollection.Where(deviceInformation => deviceInformation != null))
                    {
                        _logger.LogDebug(this, "searching for device " + unipiI2CDevice + " on device " + deviceInformation.Id);

                        var i2CSettings = new I2cConnectionSettings((int)unipiI2CDevice) { BusSpeed = I2cBusSpeed.FastMode };

                        var device = I2cDevice.FromIdAsync(deviceInformation.Id, i2CSettings);

                        if (device == null)
                        {
                            throw new Exception("Find All Async failed");
                        }

                        await device;

                        _logger.LogDebug(this, "device " + unipiI2CDevice + " found");

                        lock (_busLock)
                        {
                            _devices.Add(unipiI2CDevice, device.GetResults());
                        }


                        found = true;

                        break;
                    }

                    if (!found)
                    {
                        _logger.LogError(this, "device " + unipiI2CDevice + " not found!");
                    }

                }
            }
            catch (Exception exception)
            {
                _logger.LogException(this, exception);
            }
        }

        internal void WriteRegister(UnipiI2CDevice device, byte registerAddress, byte registerValue)
        {
            lock (_busLock)
            {
                _logger.LogDebug(this, "Write RegisterNumber " + registerAddress + " value " + registerValue);

                var dependingDevice = GetDevice(device);

                if (dependingDevice == null)
                {
                    return;
                }

                var i2CWriteBuffer = new[] { registerAddress, registerValue };

                dependingDevice.Write(i2CWriteBuffer);
            }
        }

        internal byte? ReadRegister(UnipiI2CDevice device, byte registerAddress)
        {
            lock (_busLock)
            {
                _logger.LogDebug(this, "Read RegisterNumber " + registerAddress);

                var dependingDevice = GetDevice(device);

                if (dependingDevice == null)
                {
                    return null;
                }

                var i2CWriteBuffer = new[] { registerAddress };

                var i2CReadBuffer = new[] { registerAddress };

                dependingDevice.WriteRead(i2CWriteBuffer, i2CReadBuffer);

                return i2CReadBuffer[0];
            }
        }

        internal I2cDevice GetDevice(UnipiI2CDevice device)
        {
            lock (_busLock)
            {
                I2cDevice i2CDevice;
                if (_devices.TryGetValue(device, out i2CDevice))
                {
                    return i2CDevice;
                }

                _logger.LogError(this, "Device " + device + " not found! ");
                return null;
            }
        }

        internal void Write(UnipiI2CDevice i2CDevice, byte[] buffer)
        {
            lock (_busLock)
            {
                _logger.LogDebug(this, "Write " + string.Join("-", buffer) + " to device " + i2CDevice);
                var device = GetDevice(i2CDevice);
                device?.Write(buffer);
            }
        }

        internal void WriteRead(UnipiI2CDevice i2CDevice, byte[] bytes, byte[] statusBuffer)
        {
            lock (_busLock)
            {
                _logger.LogDebug(this, "WriteRead " + string.Join("-", bytes) + " and " + string.Join("-", statusBuffer) + " to device " + i2CDevice);
                var device = GetDevice(i2CDevice);
                device?.WriteRead(bytes, statusBuffer);
            }
        }

        internal void Read(UnipiI2CDevice i2CDevice, byte[] statusBuffer)
        {
            lock (_busLock)
            {
                _logger.LogDebug(this, "Read " + string.Join("-", statusBuffer) + " to device " + i2CDevice);
                var device = GetDevice(i2CDevice);
                device?.Read(statusBuffer);
            }
        }

        #endregion
    }
}
