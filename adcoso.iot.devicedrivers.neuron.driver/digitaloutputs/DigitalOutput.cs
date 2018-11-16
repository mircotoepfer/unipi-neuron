using adcoso.iot.devicedrivers.neuron.driver.board;
using adcoso.iot.devicedrivers.neuron.driver.boardcommunication.spi;
using adcoso.iot.devicedrivers.neuron.driver.commons;
using System.Threading.Tasks;

namespace adcoso.iot.devicedrivers.neuron.driver.digitaloutputs
{
    public class DigitalOutput : IDigitalOutput
    {
        #region Private Members

        private readonly NeuronGroup _neuronGroup;
        private readonly ushort _coilNumber;

        
        private readonly NeuronSpiConnector _neuronSpiConnector;

        private readonly ushort _registerNumber;
        private readonly ushort _bitNumber;

        private OnOffValue _currentValue;

        #endregion Private Members

        #region Constructor
        internal DigitalOutput(int numberOfOutputOnGroup, NeuronGroup neuronGroup, ushort coilNumber, NeuronSpiConnector neuronSpiConnector, DriverLogger logger, DigitalRelayOutputType digitalRelayOutputType, ushort registerNumber, ushort bitNumber)
        {
            _neuronGroup = neuronGroup;
            _coilNumber = coilNumber;
            _neuronSpiConnector = neuronSpiConnector;
            _registerNumber = registerNumber;
            _bitNumber = bitNumber;
            logger.LogInstantiating(this);


            UniqueIdentifyer = digitalRelayOutputType == DigitalRelayOutputType.DigitalOutput ?
                new UniqueIdentifyer(_neuronGroup, NeuronResource.DigitalOutput, numberOfOutputOnGroup) :
                new UniqueIdentifyer(_neuronGroup, NeuronResource.RelayOutput, numberOfOutputOnGroup);

            UpdateLocalValue();
        }
        #endregion Constructor

        #region Public Methods
        public IUniqueIdentifyer UniqueIdentifyer { get; }
        public void SetOutputValue(OnOffValue value)
        {
            _neuronSpiConnector.SetCoilsValue(_neuronGroup, _coilNumber, value == OnOffValue.On);
            UpdateLocalValue();
        }

        private void UpdateLocalValue(bool raiseEvent = false)
        {
            var value = _neuronSpiConnector.GetSingleRegisterValue(_neuronGroup, _registerNumber);

            if (value == null)
            {
                return;
            }

            if (IsBitSet(value.Value, _bitNumber))
            {
                if (_currentValue != OnOffValue.On)
                {
                    _currentValue = OnOffValue.On;
                    raiseEvent = true;
                }
            }
            else
            {
                if (_currentValue != OnOffValue.Off)
                {
                    _currentValue = OnOffValue.Off;
                    raiseEvent = true;
                }

            }

            if (raiseEvent)
            {
                RaiseOnDigitalOutputStateChanged(this, _currentValue);
            }
        }

        public void ToggleOutput()
        {
            if (_currentValue == OnOffValue.Off)
            {
                SetOutputValue(OnOffValue.On);
                return;
            }

            if (_currentValue == OnOffValue.On)
            {
                SetOutputValue(OnOffValue.Off);
                return;
            }
        }

        public OnOffValue GetOutputValue() => _currentValue;

        public void RaiseAllObjectEvents()
        {
            RaiseOnDigitalOutputStateChanged(this, _currentValue);
        }

        #endregion

        #region Public Events
        public event OutputStateChanged OnOutputStateChanged;
        #endregion Public Events

        #region Private Methods

        private void RaiseOnDigitalOutputStateChanged(IDigitalOutput digitaloutput, OnOffValue value)
        {
            Task.Run(() =>
            {
                OnOutputStateChanged?.Invoke(digitaloutput, value);
            });
        }

        private static bool IsBitSet(ushort value, ushort bitPosition) => (value & (1 << bitPosition)) != 0;
        #endregion Private Methods



    }
}
