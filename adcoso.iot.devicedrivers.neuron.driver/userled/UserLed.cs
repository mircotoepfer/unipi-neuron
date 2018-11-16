using adcoso.iot.devicedrivers.neuron.driver.board;
using adcoso.iot.devicedrivers.neuron.driver.boardcommunication.spi;
using adcoso.iot.devicedrivers.neuron.driver.commons;
using System;
using System.Threading.Tasks;

namespace adcoso.iot.devicedrivers.neuron.driver.userled
{
    public class UserLed : IUserLed
    {
        private readonly ushort _coilNumber;

        
        private readonly NeuronSpiConnector _neuronSpiConnector;

        private readonly ushort _registerNumber;
        private readonly ushort _bitNumber;

        private OnOffValue _currentValue;

        internal UserLed(int ledNumber, NeuronGroup neuronGroup, ushort coilNumber, NeuronSpiConnector neuronSpiConnector, ushort registerNumber, ushort bitNumber)
        {
            UniqueIdentifyer = new UniqueIdentifier(neuronGroup, NeuronResource.UserLed, ledNumber);

            _coilNumber = coilNumber;
            _neuronSpiConnector = neuronSpiConnector;
            _registerNumber = registerNumber;
            _bitNumber = bitNumber;
            _currentValue = OnOffValue.Unknown;
        }

        public IUniqueIdentifier UniqueIdentifyer { get; }

        public void SetUserLed(OnOffValue value)
        {
            _neuronSpiConnector.SetCoilsValue(UniqueIdentifyer.Group, _coilNumber, value == OnOffValue.On);
            UpdateLocalValue();
        }

        private void UpdateLocalValue(bool raiseEvent = false)
        {
            var value = _neuronSpiConnector.GetSingleRegisterValue(UniqueIdentifyer.Group, _registerNumber);

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
                RaiseOnUserLedStateChanged(this, _currentValue);
            }

        }

        public OnOffValue GetLedStatus()
        {
            UpdateLocalValue();
            return _currentValue;
        }
        public void Toggle()
        {
            switch (_currentValue)
            {
                case OnOffValue.Off:
                    SetUserLed(OnOffValue.On);
                    return;
                case OnOffValue.On:
                    SetUserLed(OnOffValue.Off);
                    return;
                case OnOffValue.Unknown:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public event UserLedStateChanged OnUserLedStateChanged;
        public void RaiseAllObjectEvents() => UpdateLocalValue(true);

        private static bool IsBitSet(ushort value, ushort bitPosition) => (value & (1 << bitPosition)) != 0;
        private void RaiseOnUserLedStateChanged(IUserLed userled, OnOffValue value) => Task.Run(() => OnUserLedStateChanged?.Invoke(userled, value));

    }
}
