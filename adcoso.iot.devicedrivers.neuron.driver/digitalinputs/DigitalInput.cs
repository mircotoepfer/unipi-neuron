using adcoso.iot.devicedrivers.neuron.driver.board;
using adcoso.iot.devicedrivers.neuron.driver.commons;
using System.Threading.Tasks;

namespace adcoso.iot.devicedrivers.neuron.driver.digitalinputs
{
    public class DigitalInput : IDigitalInput, IObservedRegisterObject
    {

        #region Private Members
        private readonly ushort _registerNumber;
        private readonly ushort _bitInRegister;
        private OnOffValue _currentValue;
        #endregion Private Members

        #region Constructor
        internal DigitalInput(int numberOfInputOnGroup, NeuronGroup neuronGroup, ushort registerNumber, ushort bitInRegister)
        {
            _registerNumber = registerNumber;
            _bitInRegister = bitInRegister;
            UniqueIdentifyer = new UniqueIdentifyer(neuronGroup, NeuronResource.DigitalInput, numberOfInputOnGroup);
            _currentValue = OnOffValue.Unknown;
        }
        #endregion Constructor

        #region Public Methods
        public IUniqueIdentifyer UniqueIdentifyer { get; }
        public OnOffValue GetDigitalInputValue() => _currentValue;
        public void RaiseAllObjectEvents() => RaiseOnDigitalInputChanged(this, _currentValue);
        #endregion Public Methods

        #region Public Events
        public event DigitalInputStateChanged OnDigitalInputChanged;
        #endregion Public Events

        #region Private Methods

        private static bool IsBitSet(ushort value, ushort bitPosition) => (value & (1 << bitPosition)) != 0;
        private void RaiseOnDigitalInputChanged(IDigitalInput digitalinput, OnOffValue value) => Task.Run(() => OnDigitalInputChanged?.Invoke(digitalinput, value));
        public void SetRegisterValue(ushort registerNumber, ushort value)
        {
            if (registerNumber != _registerNumber)
            {
                return;
            }

            var bitValue = IsBitSet(value, _bitInRegister);

            if (bitValue && _currentValue != OnOffValue.On)
            {
                _currentValue = OnOffValue.On;
                RaiseOnDigitalInputChanged(this, _currentValue);
            }

            if (bitValue || _currentValue == OnOffValue.Off)
            {
                return;
            }

            _currentValue = OnOffValue.Off;

            RaiseOnDigitalInputChanged(this, _currentValue);
        }

        #endregion Private Methods
    }
}
