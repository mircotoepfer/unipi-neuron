using adcoso.iot.devicedrivers.neuron.driver.board;
using adcoso.iot.devicedrivers.neuron.driver.boardcommunication.spi;
using System;
using System.Threading.Tasks;

namespace adcoso.iot.devicedrivers.neuron.driver.analoginput
{

    public class AnalogInput : IAnalogInput, IObservedRegisterObject
    {
        private readonly ushort _registerNumber;
        private readonly double _vRealInt;
        private int _currentRegisterValue;
        private readonly int _aI1Vdev;
        private readonly int _ai1Offset;

        internal AnalogInput(NeuronGroup neuronGroup, int number, ushort registerNumber, NeuronSpiConnector spiConnector)
        {
            _registerNumber = registerNumber;
            UniqueIdentifyer = new UniqueIdentifyer(neuronGroup, NeuronResource.AnalogInput, number);

            var value = spiConnector.GetSingleRegisterValue(neuronGroup, 1021);

            if (value == null)
                throw new Exception("failure while getting Voffset from registers");


            var vRef = spiConnector.GetSingleRegisterValue(neuronGroup, 1009);

            var vRefInt = spiConnector.GetSingleRegisterValue(neuronGroup, 5);

            var aI1Vdev = spiConnector.GetSingleRegisterValue(neuronGroup, 1025);
            var ai1Offset = spiConnector.GetSingleRegisterValue(neuronGroup, 1026);

            if (vRefInt == null || vRef == null || aI1Vdev == null || ai1Offset == null)
            {
                throw new Exception("failure while getting the calibration values from the Registers!");
            }

            _aI1Vdev = BitConverter.ToInt16(BitConverter.GetBytes(aI1Vdev.Value), 0);
            _ai1Offset = BitConverter.ToInt16(BitConverter.GetBytes(ai1Offset.Value), 0);
            _vRealInt = 3.3 * ((double)vRef / (double)vRefInt);
        }

        public IUniqueIdentifyer UniqueIdentifyer { get; }
        public double GetPercentValue() => GetPercentValueFromCurrentRegisterValue();

        public double GetVoltageValue() => GetVoltageValueFromCurrentRegisterValue();
        public double GetMilliAmpere() => GetMilliampereFromCurrentRegisterValue();

        public event AnalogInputChanged OnAnalogInputChanged;
        public void RaiseAllObjectEvents() => RaiseOnAnalogInputChanged(this, GetPercentValueFromCurrentRegisterValue(),
            GetVoltageValueFromCurrentRegisterValue(), GetMilliampereFromCurrentRegisterValue());

        public void SetRegisterValue(ushort registerNumber, ushort value)
        {
            if (registerNumber != _registerNumber)
                return;


            if (value == _currentRegisterValue)
            {
                return;
            }

            if (value > 4096)
                value = 4096;

            if (Math.Abs(_currentRegisterValue - value) <= 2)
            {
                return;
            }

            _currentRegisterValue = value;
            RaiseOnAnalogInputChanged(this, GetPercentValueFromCurrentRegisterValue(),
                GetVoltageValueFromCurrentRegisterValue(), GetMilliampereFromCurrentRegisterValue());
        }

        private double GetPercentValueFromCurrentRegisterValue() => 100.0 / 10.0 * GetVoltageValueFromCurrentRegisterValue();
        private double GetVoltageValueFromCurrentRegisterValue()
        {

            var value = _vRealInt * 3 * (Convert.ToDouble(_currentRegisterValue) / 4096) * (1 + _aI1Vdev / 10000) + Convert.ToDouble(_ai1Offset) / 10000;

            var roundValue = Math.Round(value, 2, MidpointRounding.ToEven);
            return roundValue;


            //var value = 3 * _currentRegisterValue * _vRealInt / (_vOffset + 4095);
            //var roundValue = Convert.ToDouble(Math.Round(value, 2, MidpointRounding.ToEven));
            //return roundValue;
        }

        private double GetMilliampereFromCurrentRegisterValue()
        {

            var value = _vRealInt * (Convert.ToDouble(_currentRegisterValue) / 4096) * 10 * (1 + _aI1Vdev / 10000) + Convert.ToDouble(_ai1Offset) / 10000;

            var roundValue = Math.Round(value, 2, MidpointRounding.ToEven);
            return roundValue;

        }


        private void RaiseOnAnalogInputChanged( IAnalogInput input, double percentagevalue, double voltagevalue, double milliAmpereValue) => Task.Run(() => OnAnalogInputChanged?.Invoke(input, percentagevalue, voltagevalue, milliAmpereValue));

    }
}
