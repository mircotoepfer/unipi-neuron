using adcoso.iot.devicedrivers.neuron.driver.analoginput;
using adcoso.iot.devicedrivers.neuron.driver.analogoutput;
using adcoso.iot.devicedrivers.neuron.driver.board;
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
using System.Threading.Tasks;

// ReSharper disable ConvertToExpressionBodyWhenPossible

namespace adcoso.iot.devicedrivers.neuron.driver
{
    public class NeuronDevice : INeuron
    {
        #region Private Members

        
        private readonly NeuronSpiConnector _spiConnector;
        
        private readonly DriverLogger _driverLogger;
        
        private readonly I2CConnector _i2CConnector;
        
        private readonly List<INeuronGroupBoard> _boards;

        #endregion  Private Members

        #region Constructor

        public NeuronDevice(bool autoInitialize = false)
        {
            _boards = new List<INeuronGroupBoard>();

            _driverLogger = new DriverLogger(LogLevel.Debug);

            _driverLogger.OnLogInformation += OnLogInformation;

            _spiConnector = new NeuronSpiConnector(_driverLogger);
            _i2CConnector = new I2CConnector(_driverLogger);

            if (autoInitialize)
            {
                Task.Factory?.StartNew(Initialize);
            }
        }


        #endregion Constructor

        #region Public Events
        public event LogInformation OnLogInformation;
        public event InitializationFinish OnInitializationFinish;
        public event DigitalInputStateChanged OnDigitalInputStateChanged;
        public event OutputStateChanged OnDigitalOutputStateChanged;
        public event OutputStateChanged OnRelayOutputStateChanged;
        public event AnalogInputChanged OnAnalogInputChanged;
        public event AnalogOutputChanged OnAnalogOutputChanged;
        public event UserLedStateChanged OnUserLedStateChanged;
        #endregion Public Events

        #region Public Properties
        public IReadOnlyList<IBoardInformation> BoardInformations
        {
            get
            {
                var items = _boards.Where(neuronBoard => neuronBoard != null).Select(neuronBoard => neuronBoard.BoardSystemInformation).ToList();

                return new ReadOnlyCollection<IBoardInformation>(items);
            }
        }
        #endregion Public Properties

        #region Public Methods
        public async Task Initialize()
        {
            try
            {
                var i2Init = _i2CConnector.Initialize();
                var spiInit = _spiConnector.Initialize();

                await spiInit;
                await i2Init;

                var factory = new GeneralFactory(_i2CConnector, _spiConnector, _driverLogger);

                foreach (var foundGroupId in _spiConnector.GetFoundGroupIds())
                {
                    var board = factory.CreateNeuronBoard(foundGroupId);

                    if (board == null)
                    {
                        continue;
                    }

                    _driverLogger.LogInformation(this, "Found Board " + board.BoardSystemInformation?.HardwareName + " on group " + foundGroupId);
                    _boards.Add(board);
                }

                OnInitializationFinish?.Invoke();
            }
            catch (Exception exception)
            {
                _driverLogger.LogException(this, exception);
            }
        }
        public void SetLogLevel(LogLevel logLevel) => _driverLogger.SetLogLevel(logLevel);
        #endregion Public Methods

        #region Digital Outputs
        public IReadOnlyList<IDigitalOutput> DigitalOutputs
        {
            get
            {
                var items = new List<IDigitalOutput>();
                foreach (var neuronBoard in _boards.Where(neuronBoard => neuronBoard != null))
                {
                    items.AddRange(neuronBoard.DigitalOutputs);
                }

                return new ReadOnlyCollection<IDigitalOutput>(items);
            }
        }
        public IDigitalOutput GetDigitalOutput(IUniqueIdentifyer identifyer)
        {
            return _boards.Where(neuronBoard => neuronBoard != null).Select(neuronBoard => neuronBoard.GetDigitalOutput(identifyer)).FirstOrDefault(item => item != null);
        }
        #endregion Digital Outputs

        #region Digital Inputs
        public IReadOnlyList<IDigitalInput> DigitalInputs
        {
            get
            {
                var items = new List<IDigitalInput>();
                foreach (var neuronBoard in _boards.Where(neuronBoard => neuronBoard != null))
                {
                    items.AddRange(neuronBoard.DigitalInputs);
                }

                return new ReadOnlyCollection<IDigitalInput>(items);
            }
        }
        public IDigitalInput GetDigitalInput(IUniqueIdentifyer identifyer)
        {
            return _boards.Where(neuronBoard => neuronBoard != null).Select(neuronBoard => neuronBoard.GetDigitalInput(identifyer)).FirstOrDefault(item => item != null);

        }
        public IDigitalInput GetDigitalInput(string identifyer)
        {
            return DigitalInputs.FirstOrDefault(item => item?.UniqueIdentifyer.IdentifyerString.Equals(identifyer) == true);
        }
        #endregion Digital Inputs

        #region Relay Outputs
        public IReadOnlyList<IDigitalOutput> RelayOutputs
        {
            get
            {
                var items = new List<IDigitalOutput>();
                foreach (var neuronBoard in _boards.Where(neuronBoard => neuronBoard != null))
                {
                    items.AddRange(neuronBoard.RelayOutputs);
                }

                return new ReadOnlyCollection<IDigitalOutput>(items);
            }
        }
        public IDigitalOutput GetRelayOutput(IUniqueIdentifyer identifyer)
        {
            return _boards.Where(neuronBoard => neuronBoard != null).Select(neuronBoard => neuronBoard.GetRelayOutput(identifyer)).FirstOrDefault(item => item != null);
        }
        public IDigitalOutput GetRelayOutput(string identifyer)
        {
            return RelayOutputs.FirstOrDefault(item => item?.UniqueIdentifyer.IdentifyerString.Equals(identifyer) == true);
        }
        #endregion Relay Outputs

        #region User LED's
        public IReadOnlyList<IUserLed> UserLeds
        {
            get
            {
                var items = new List<IUserLed>();
                foreach (var neuronBoard in _boards.Where(neuronBoard => neuronBoard != null))
                {
                    items.AddRange(neuronBoard.UserLeds);
                }

                return new ReadOnlyCollection<IUserLed>(items);
            }
        }
        public IUserLed GetUserLed(IUniqueIdentifyer identifyer)
        {
            return _boards.Where(neuronBoard => neuronBoard != null).Select(neuronBoard => neuronBoard.GetUserLed(identifyer)).FirstOrDefault(item => item != null);

        }
        public IUserLed GetUserLed(string identifyer)
        {
            return UserLeds.FirstOrDefault(item => item?.UniqueIdentifyer.IdentifyerString.Equals(identifyer) == true);
        }
        #endregion User LED's

        #region Analog Inputs
        public IReadOnlyList<IAnalogInput> AnalogInputs
        {
            get
            {
                var items = new List<IAnalogInput>();
                foreach (var neuronBoard in _boards.Where(neuronBoard => neuronBoard != null))
                {
                    items.AddRange(neuronBoard.AnalogInputs);
                }

                return new ReadOnlyCollection<IAnalogInput>(items);
            }
        }
        public IAnalogInput GetAnalogInput(IUniqueIdentifyer identifyer)
        {
            return _boards.Where(neuronBoard => neuronBoard != null).Select(neuronBoard => neuronBoard.GetAnalogInput(identifyer)).FirstOrDefault(item => item != null);
        }
        public IAnalogInput GetAnalogInput(string identifyer)
        {
            return AnalogInputs.FirstOrDefault(item => item?.UniqueIdentifyer.IdentifyerString.Equals(identifyer) == true);
        }
        #endregion Analog Inputs

        #region Analog Outputs
        public IReadOnlyList<IAnalogOutput> AnalogOutputs
        {
            get
            {
                var items = new List<IAnalogOutput>();
                foreach (var neuronBoard in _boards.Where(neuronBoard => neuronBoard != null))
                {
                    items.AddRange(neuronBoard.AnalogOutputs);
                }

                return new ReadOnlyCollection<IAnalogOutput>(items);
            }
        }
        public IAnalogOutput GetAnalogOutput(IUniqueIdentifyer identifyer)
        {
            return _boards.Where(neuronBoard => neuronBoard != null).Select(neuronBoard => neuronBoard.GetAnalogOutput(identifyer)).FirstOrDefault(item => item != null);
        }
        public IAnalogOutput GetAnalogOutput(string identifyer)
        {
            return AnalogOutputs.FirstOrDefault(item => item?.UniqueIdentifyer?.IdentifyerString.Equals(identifyer) == true);
        }
        #endregion Analog Outputs

        #region One Wire
        public IReadOnlyList<IOneWireConnector> OneWireConnectors
        {
            get
            {
                var items = new List<IOneWireConnector>();
                foreach (var neuronBoard in _boards.Where(neuronBoard => neuronBoard != null))
                {
                    items.AddRange(neuronBoard.OneWireConnectors);
                }

                return new ReadOnlyCollection<IOneWireConnector>(items);
            }
        }
        public IOneWireConnector GetOneWireConnector(IUniqueIdentifyer identifyer)
        {
            return _boards.Where(neuronBoard => neuronBoard != null).Select(neuronBoard => neuronBoard.GetOneWireConnector(identifyer)).FirstOrDefault(item => item != null);
        }
        public IOneWireConnector GetOneWireConnector(string identifyer)
        {
            return OneWireConnectors.FirstOrDefault(item => item?.UniqueIdentifyer.IdentifyerString.Equals(identifyer) == true);
        }
        #endregion One Wire

        #region Modbus
        public IReadOnlyList<IModbusConnector> ModbusConnectors
        {
            get
            {
                var items = new List<IModbusConnector>();
                foreach (var neuronBoard in _boards.Where(neuronBoard => neuronBoard != null))
                {
                    items.AddRange(neuronBoard.ModbusConnectors);
                }

                return new ReadOnlyCollection<IModbusConnector>(items);
            }
        }
        public IModbusConnector GetModbusConnector(IUniqueIdentifyer identifyer)
        {
            return _boards.Where(neuronBoard => neuronBoard != null).Select(neuronBoard => neuronBoard.GetModbusConnector(identifyer)).FirstOrDefault(item => item != null);
        }
        public IModbusConnector GetModbusConnector(string identifyer)
        {
            return ModbusConnectors.FirstOrDefault(item => item?.UniqueIdentifyer.IdentifyerString.Equals(identifyer) == true);
        }

        public void RaiseAllObjectEvents()
        {
        }

        #endregion Modbus

    }
}
