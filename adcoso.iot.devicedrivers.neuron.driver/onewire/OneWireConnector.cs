using adcoso.iot.devicedrivers.neuron.driver.board;
using adcoso.iot.devicedrivers.neuron.driver.boardcommunication.i2c;
using adcoso.iot.devicedrivers.neuron.driver.commons;
using adcoso.iot.devicedrivers.neuron.driver.onewire.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable InconsistentlySynchronizedField

namespace adcoso.iot.devicedrivers.neuron.driver.onewire
{

    /// <summary>
    /// The One Wire Controller so communicate with the one wire bus
    /// </summary>
    /// <seealso cref="IOneWireConnector" />
    public class OneWireConnector : IOneWireConnector
    {
        #region Private Members
        // ReSharper disable once NotAccessedField.Local
        private Timer _timer;
        
        private readonly object _controllerLock;
        
        private readonly DriverLogger _logger;
        private readonly NeuronConnectedOneWireDevice _neuronConnectedOneWireDevice;
        private bool _busScan;

        
        private readonly object _timerState;

        private bool _oneWirePeriodicBusScan;
        private int _periodicBusScanIntervalSeconds;

        #endregion Private Members

        #region Constructor

        internal OneWireConnector(NeuronGroup neuronGroup, int number, DriverLogger logger, I2CConnector i2CGateway)
        {
            _logger = logger;
            _logger.LogInformation(this, "create Instance...");

            UniqueIdentifyer = new UniqueIdentifier(neuronGroup, NeuronResource.OneWireConnector, number);

            OneWirePeriodicBusScan = true;
            _timerState = new object();
            _controllerLock = new object();

            Devices = new List<IOneWireDevice>();

            _neuronConnectedOneWireDevice = new NeuronConnectedOneWireDevice(i2CGateway);

            _logger.LogInformation(this, "scanning bus on startup");

            ScanForNewOrDeadDevices();

            PeriodicBusScanIntervalSeconds = 60;
        }

        public int PeriodicBusScanIntervalSeconds
        {
            get { return _periodicBusScanIntervalSeconds; }
            set
            {
                _periodicBusScanIntervalSeconds = value;
                OneWirePeriodicBusScan = _oneWirePeriodicBusScan;
            }
        }

        #endregion Constructor

        #region Public Events

        /// <summary>
        /// Occurs when [on new on wire device].
        /// </summary>
        public event NewOneWireDevice OnNewOnWireDevice;

        /// <summary>
        /// Occurs when [on one wire device is dead].
        /// </summary>
        public event OneWireDeviceIsDead OnOneWireDeviceIsDead;



        #endregion Public Events

        #region Public Properties

        public IUniqueIdentifier UniqueIdentifyer { get; }


        /// <summary>
        /// Gets the devices.
        /// </summary>
        /// <value>
        /// The devices.
        /// </value>
        
        public List<IOneWireDevice> Devices { get; }

        public bool OneWirePeriodicBusScan
        {
            get { return _oneWirePeriodicBusScan; }
            set
            {
                _oneWirePeriodicBusScan = value;

                if (_timer != null)
                {
                    _timer.Dispose();
                    _timer = null;
                }

                if (PeriodicBusScanIntervalSeconds <= 0 || _oneWirePeriodicBusScan == false)
                {
                    return;
                }

                _logger.LogDebug(this, "setup timer for periodic bus scan");
                _timer = new Timer(PeriodicBusScanTimerCallback, _timerState, TimeSpan.FromSeconds(_periodicBusScanIntervalSeconds), TimeSpan.FromSeconds(_periodicBusScanIntervalSeconds));
            }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Sends the command.
        /// </summary>
        /// <param name="romNumber">The rom number.<see cref="Byte"/></param>
        /// <param name="commands">The command.<see cref="Byte"/></param>
        /// <param name="enableStrongPullUp">if set to <c>true</c> [enable strong pull up].</param>
        /// <param name="waitAfterCommand">if set to <c>true</c> [wait after command].</param>

        public void SendCommand(byte[] romNumber, bool enableStrongPullUp = false, params byte[] commands) => SendCommand(romNumber,0, enableStrongPullUp, commands);

        public void SendCommand(byte[] romNumber, int waitInMilliSecAfterCommand, bool enableStrongPullUp = false, params byte[] commands)
        {
            lock (_controllerLock)
            {
                _logger.LogMonitor(this, "sending command " + string.Join("-", commands) + " to device with rom number " + string.Join("-", romNumber));

                _logger.LogDebug(this, "reset one wire bus");
                _neuronConnectedOneWireDevice.OneWireResetBus();

                _logger.LogDebug(this, "writing rom command match");
                _neuronConnectedOneWireDevice.OneWireWriteByte(0x55);

                _logger.LogDebug(this, "write romNumber " + string.Join("-", romNumber) + " for rom command match");
                foreach (var item in romNumber)
                {
                    _neuronConnectedOneWireDevice.OneWireWriteByte(item);
                }

                if (enableStrongPullUp)
                {
                    _logger.LogDebug(this, "enable strong pullup");
                    _neuronConnectedOneWireDevice.EnableStrongPullup();
                }

                _logger.LogDebug(this, "writing command " + string.Join("-", commands));
                _neuronConnectedOneWireDevice.OneWireWriteByte(commands);



                if (waitInMilliSecAfterCommand > 0)
                {
                    _logger.LogDebug(this, "waiting for command finish");

                    Task.Delay(waitInMilliSecAfterCommand)?.Wait();
                }



                _logger.LogDebug(this, "command finish");
            }
        }

        /// <summary>
        /// Reads the data.
        /// </summary>
        /// <param name="romNumber">The rom number.<see cref="Byte"/></param>
        /// <param name="command">The command.<see cref="Byte"/></param>
        /// <param name="byteLength">Length of the byte.</param>
        /// <returns></returns>
        
        public byte[] ReadData(byte[] romNumber, int byteLength, params byte[] commands)
        {
            lock (_controllerLock)
            {
                _logger.LogInformation(this, "reading data from device with rom number " + string.Join("-", romNumber) + " with commands " + string.Join("-", commands));

                _logger.LogDebug(this, "reset one wire bus");
                _neuronConnectedOneWireDevice.OneWireResetBus();

                _logger.LogDebug(this, "writing rom command match");
                _neuronConnectedOneWireDevice.OneWireWriteByte(0x55);

                _logger.LogDebug(this, "write romNumber " + string.Join("-", romNumber) + " for rom command match");
                foreach (var item in romNumber)
                {
                    _neuronConnectedOneWireDevice.OneWireWriteByte(item);
                }

                foreach (var command in commands)
                {
                    _logger.LogDebug(this, "writing command " + command.ToString("x2"));
                    _neuronConnectedOneWireDevice.OneWireWriteByte(command);
                }




                _logger.LogDebug(this, "read " + byteLength + " byte of data");
                var data = new byte[byteLength];


                for (var i = 0; i < byteLength; i++)
                {
                    var dataByte = _neuronConnectedOneWireDevice.OneWireReadByte();

                    data[i] = dataByte;
                }


                _logger.LogDebug(this, "data = " + string.Join("-", data));


                return data;
            }
        }

        /// <summary>
        /// Updates the one wire bus devices.
        /// </summary>
        public void ScanForNewOrDeadDevices()
        {
            lock (_controllerLock)
            {
                _logger.LogInformation(this, "scanning for new or dead devices");
                var currentDevices = new List<IOneWireDevice>();

                var deviceDetected = _neuronConnectedOneWireDevice.OneWireResetBus();

                if (!deviceDetected)
                {
                    _logger.LogDebug(this, "no one wire device found");
                    RemoveAllOneWireDevices();
                    return;
                }

                var keepGoing = _neuronConnectedOneWireDevice.FindFirstOneWireDeviceOnTheBus();

                while (keepGoing)
                {
                    _logger.LogDebug(this, "one wire device " + string.Join("-", _neuronConnectedOneWireDevice.CurrentRomNumber) + " found");

                    currentDevices.Add(new OneWireDevice(_neuronConnectedOneWireDevice.CurrentRomNumber, this));

                    keepGoing = _neuronConnectedOneWireDevice.JumpToNextDevice();
                }

                if (currentDevices.Count == 0)
                {
                    RemoveAllOneWireDevices();
                }
                else
                {
                    RemoveMissingDevices(currentDevices);
                }

                UpdateNewDevices(currentDevices);
            }
        }


        #endregion Public Methods

        #region Private Methods

        private void UpdateNewDevices(IEnumerable<IOneWireDevice> currentDevices)
        {
            foreach (var currentDevice in currentDevices.Where
                (
                    currentDevice => Devices.All
                    (
                        device => (currentDevice != null)
                        && (device != null)
                        && (device.SerialNumber != currentDevice.SerialNumber)
                    )
                ))
            {
                _logger.LogInformation(this, "adding new device " + currentDevice.SerialNumber + " to the device list");
                Devices.Add(currentDevice);
                RaiseOnNewOnWireDevice(currentDevice);
            }
        }

        private void RemoveMissingDevices(IReadOnlyCollection<IOneWireDevice> currentDevices)
        {
            for (var i = Devices.Count - 1; i >= 0; i--)
            {
                var oneWireDevice = Devices[i];

                if (oneWireDevice == null)
                {
                    continue;
                }

                var found = currentDevices.Any
                    (
                        currentDevice => (currentDevice != null)
                                      && (oneWireDevice.SerialNumber == currentDevice.SerialNumber)
                    );

                if (found)
                {
                    continue;
                }

                RemoveDevice(oneWireDevice);
            }
        }

        private void RemoveDevice(IOneWireDevice oneWireDevice)
        {
            oneWireDevice.MarkAsDead();
            _logger.LogInformation(this, "removing device " + oneWireDevice.SerialNumber + " from the device list");
            Devices.Remove(oneWireDevice);
            RaiseOneWireDeviceIsDead(oneWireDevice);
        }

        private void RemoveAllOneWireDevices()
        {
            _logger.LogDebug(this, "remove all one wire devices");
            for (var i = Devices.Count - 1; i >= 0; i--)
            {
                if (Devices[i] == null)
                {
                    Devices.RemoveAt(i);
                }

                RemoveDevice(Devices[i]);
            }
        }

        private async void RaiseOnNewOnWireDevice(IOneWireDevice device)
        {
            if (OnNewOnWireDevice == null)
            {
                return;
            }

            _logger.LogDebug(this, "raising new one wire device event");

            var task = Task.Run(() => OnNewOnWireDevice.Invoke(device));

            if (task != null)
            {
                await task.ConfigureAwait(false);
            }
        }

        private async void RaiseOneWireDeviceIsDead(IOneWireDevice device)
        {
            if (OnOneWireDeviceIsDead == null)
            {
                return;
            }

            _logger.LogDebug(this, "raising dead one wire device event");
            var task = Task.Run(() => OnOneWireDeviceIsDead.Invoke(device));

            if (task != null)
            {
                await task.ConfigureAwait(false);
            }
        }

        private void PeriodicBusScanTimerCallback(object state)
        {
            if (_busScan)
            {
                return;
            }

            _busScan = true;

            try
            {
                _logger.LogMonitor(this, "executing periodic bus scan");
                ScanForNewOrDeadDevices();
            }
            catch (Exception exception)
            {
                _logger.Log(this, exception);
            }

            _busScan = false;
        }

        public void RaiseAllObjectEvents()
        {
            // ReSharper disable once InconsistentlySynchronizedField
            _logger.LogInformation(this, "starting required value update");

            var oneWireDevices = Devices.ToArray();

            if (oneWireDevices == null)
            {
                return;
            }

            foreach (var oneWireDevice in oneWireDevices.Where(oneWireDevice => oneWireDevice != null))
            {
                RaiseOnNewOnWireDevice(oneWireDevice);
            }
        }

        #endregion Private Methods

    }
}
