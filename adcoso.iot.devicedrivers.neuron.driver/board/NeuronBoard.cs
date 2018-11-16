using adcoso.iot.devicedrivers.neuron.driver.analoginput;
using adcoso.iot.devicedrivers.neuron.driver.analogoutput;
using adcoso.iot.devicedrivers.neuron.driver.boardcommunication.i2c;
using adcoso.iot.devicedrivers.neuron.driver.boardcommunication.spi;
using adcoso.iot.devicedrivers.neuron.driver.commons;
using adcoso.iot.devicedrivers.neuron.driver.digitalinputs;
using adcoso.iot.devicedrivers.neuron.driver.digitaloutputs;
using adcoso.iot.devicedrivers.neuron.driver.onewire.interfaces;
using adcoso.iot.devicedrivers.neuron.driver.rs485modbus;
using adcoso.iot.devicedrivers.neuron.driver.userled;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace adcoso.iot.devicedrivers.neuron.driver.board
{
    internal abstract class NeuronGroupBoardBase : INeuronGroupBoard
    {
        #region Protected Members
        protected readonly NeuronGroup NeuronGroup;
        
        protected readonly NeuronSpiConnector SpiConnector;
        
        protected readonly I2CConnector I2CConnector;
        
        protected readonly DriverLogger Logger;
        
        protected readonly Dictionary<IUniqueIdentifier, IDigitalInput> DigitalInputDictionary;
        
        protected readonly Dictionary<IUniqueIdentifier, IDigitalOutput> DigitalOutputsDictionary;
        
        protected readonly Dictionary<IUniqueIdentifier, IDigitalOutput> RelayOutputsDictionary;
        
        protected readonly Dictionary<IUniqueIdentifier, IUserLed> UserLedsDictionary;
        
        protected readonly Dictionary<IUniqueIdentifier, IAnalogInput> AnalogInputsDictionary;
        
        protected readonly Dictionary<IUniqueIdentifier, IAnalogOutput> AnalogOutputsDictionary;
        
        protected readonly Dictionary<IUniqueIdentifier, IOneWireConnector> OneWireConnectorsDictionary;
        
        protected readonly Dictionary<IUniqueIdentifier, IModbusConnector> ModbusConnectorsDictionary;


        #endregion Protected Members

        #region Private Members
        
        private readonly Dictionary<ushort, List<IObservedRegisterObject>> _registersToObserve;
        // ReSharper disable once NotAccessedField.Local
        private Timer _pollingTimer;
        private bool _pollInProgess;
        
        private readonly object _observeLocker;
        #endregion Private Members

        #region Constructor
        internal NeuronGroupBoardBase(NeuronGroup neuronGroup, IBoardInformation boardSystemInformation, NeuronSpiConnector spiConnector, I2CConnector i2CConnector, DriverLogger logger)
        {
            NeuronGroup = neuronGroup;
            SpiConnector = spiConnector;
            I2CConnector = i2CConnector;
            Logger = logger;

            _observeLocker = new object();

            _registersToObserve = new Dictionary<ushort, List<IObservedRegisterObject>>();

            BoardSystemInformation = boardSystemInformation;

            DigitalInputDictionary = new Dictionary<IUniqueIdentifier, IDigitalInput>();
            DigitalOutputsDictionary = new Dictionary<IUniqueIdentifier, IDigitalOutput>();
            RelayOutputsDictionary = new Dictionary<IUniqueIdentifier, IDigitalOutput>();
            UserLedsDictionary = new Dictionary<IUniqueIdentifier, IUserLed>();
            AnalogInputsDictionary = new Dictionary<IUniqueIdentifier, IAnalogInput>();
            AnalogOutputsDictionary = new Dictionary<IUniqueIdentifier, IAnalogOutput>();
            OneWireConnectorsDictionary = new Dictionary<IUniqueIdentifier, IOneWireConnector>();
            ModbusConnectorsDictionary = new Dictionary<IUniqueIdentifier, IModbusConnector>();

            _pollInProgess = false;
            _pollingTimer = new Timer(PollInformation, new object(), 50, 50);

        }
        #endregion Constructor

        #region Public Properties
        public IBoardInformation BoardSystemInformation { get; }
        public IReadOnlyList<IDigitalOutput> DigitalOutputs => new ReadOnlyCollection<IDigitalOutput>(DigitalOutputsDictionary.Values.ToArray());
        public IReadOnlyList<IDigitalInput> DigitalInputs => new ReadOnlyCollection<IDigitalInput>(DigitalInputDictionary.Values.ToArray());
        public IReadOnlyList<IDigitalOutput> RelayOutputs => new ReadOnlyCollection<IDigitalOutput>(RelayOutputsDictionary.Values.ToArray());
        public IReadOnlyList<IUserLed> UserLeds => new ReadOnlyCollection<IUserLed>(UserLedsDictionary.Values.ToArray());
        public IReadOnlyList<IAnalogInput> AnalogInputs => new ReadOnlyCollection<IAnalogInput>(AnalogInputsDictionary.Values.ToArray());
        public IReadOnlyList<IAnalogOutput> AnalogOutputs => new ReadOnlyCollection<IAnalogOutput>(AnalogOutputsDictionary.Values.ToArray());
        public IReadOnlyList<IOneWireConnector> OneWireConnectors => new ReadOnlyCollection<IOneWireConnector>(OneWireConnectorsDictionary.Values.ToArray());
        public IReadOnlyList<IModbusConnector> ModbusConnectors => new ReadOnlyCollection<IModbusConnector>(ModbusConnectorsDictionary.Values.ToArray());
        #endregion Public Properties

        #region Public Events
        public event DigitalInputStateChanged OnDigitalInputStateChanged;
        public event OutputStateChanged OnDigitalOutputStateChanged;
        public event OutputStateChanged OnRelayOutputStateChanged;
        public event AnalogInputChanged OnAnalogInputChanged;
        public event AnalogOutputChanged OnAnalogOutputChanged;
        public event UserLedStateChanged OnUserLedStateChanged;
        #endregion Public Events

        #region Protected Methods

        protected void SetObservation(ushort registerNumber, IObservedRegisterObject observedRegisterObject)
        {
            lock (_observeLocker)
            {
                if (!_registersToObserve.ContainsKey(registerNumber))
                {
                    _registersToObserve.Add(registerNumber, new List<IObservedRegisterObject>());
                }

                var list = _registersToObserve[registerNumber];

                if (!list?.Contains(observedRegisterObject) == true)
                {
                    list?.Add(observedRegisterObject);
                }
            }
        }

        #endregion Protected Methods


        #region Public Methods

        public IDigitalOutput GetDigitalOutput(IUniqueIdentifier identifier)
        {
            IDigitalOutput item;
            return DigitalOutputsDictionary.TryGetValue(identifier, out item) ? item : null;
        }

        public IDigitalInput GetDigitalInput(IUniqueIdentifier identifier)
        {
            IDigitalInput item;
            return DigitalInputDictionary.TryGetValue(identifier, out item) ? item : null;
        }

        public IDigitalOutput GetRelayOutput(IUniqueIdentifier identifier)
        {
            IDigitalOutput item;
            return RelayOutputsDictionary.TryGetValue(identifier, out item) ? item : null;
        }

        public IUserLed GetUserLed(IUniqueIdentifier identifier)
        {
            IUserLed item;
            return UserLedsDictionary.TryGetValue(identifier, out item) ? item : null;
        }

        public IAnalogInput GetAnalogInput(IUniqueIdentifier identifier)
        {
            IAnalogInput item;
            return AnalogInputsDictionary.TryGetValue(identifier, out item) ? item : null;
        }

        public IAnalogOutput GetAnalogOutput(IUniqueIdentifier identifier)
        {
            IAnalogOutput item;
            return AnalogOutputsDictionary.TryGetValue(identifier, out item) ? item : null;
        }

        public IOneWireConnector GetOneWireConnector(IUniqueIdentifier identifier)
        {
            IOneWireConnector item;
            return OneWireConnectorsDictionary.TryGetValue(identifier, out item) ? item : null;
        }

        public IModbusConnector GetModbusConnector(IUniqueIdentifier identifier)
        {
            IModbusConnector item;
            return ModbusConnectorsDictionary.TryGetValue(identifier, out item) ? item : null;
        }

        public void RaiseAllObjectEvents()
        {
            foreach (var item in AnalogInputs)
            {
                item?.RaiseAllObjectEvents();
            }

            foreach (var item in AnalogOutputs)
            {
                item?.RaiseAllObjectEvents();
            }

            foreach (var item in DigitalOutputs)
            {
                item?.RaiseAllObjectEvents();
            }

            foreach (var item in DigitalInputs)
            {
                item?.RaiseAllObjectEvents();
            }

            foreach (var item in RelayOutputs)
            {
                item?.RaiseAllObjectEvents();
            }

            foreach (var item in UserLeds)
            {
                item?.RaiseAllObjectEvents();
            }
        }

        #endregion Public Methods

        #region Private Methods

        private void PollInformation(object state)
        {
            if (_pollInProgess)
            {
                return;
            }

            _pollInProgess = true;

            try
            {
                lock (_observeLocker)
                {
                    foreach (var keyValuePair in _registersToObserve)
                    {
                        var registerNumber = keyValuePair.Key;

                        var result = SpiConnector.GetSingleRegisterValue(NeuronGroup, registerNumber);

                        if (result == null)
                        {
                            continue;
                        }

                        if (keyValuePair.Value == null)
                        {
                            continue;
                        }

                        foreach (var observedObject in keyValuePair.Value)
                        {
                            observedObject.SetRegisterValue(registerNumber, result.Value);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogException(this, e);

            }

            _pollInProgess = false;
        }

        #endregion Private Methods


    }
}
