using adcoso.iot.devicedrivers.neuron.driver.board;
using adcoso.iot.devicedrivers.neuron.driver.boardcommunication.spi;
using adcoso.iot.devicedrivers.neuron.driver.commons;
using System;
using System.Threading.Tasks;

namespace adcoso.iot.devicedrivers.neuron.driver.analogoutput
{
    public class AnalogOutput : IAnalogOutput
    {
        #region Private Members
        
        private readonly NeuronSpiConnector _spiConnector;
        private readonly ushort _registerNumber;

        
        private readonly DriverLogger _logger;
        private ushort _currentRegisterValue;
        private readonly double _vOffset;
        private readonly double _vRealInt;
        private readonly NeuronGroup _neuronGroup;
        private readonly double _vDev;

        #endregion Private Members

        #region Constructor
        internal AnalogOutput(NeuronGroup neuronGroup, int number, ushort registerNumber, NeuronSpiConnector spiConnector, DriverLogger logger)
        {
            _registerNumber = registerNumber;
            _logger = logger;
            _logger.LogInstantiating(this);
            _neuronGroup = neuronGroup;
            _spiConnector = spiConnector;

            UniqueIdentifyer = new UniqueIdentifier(neuronGroup, NeuronResource.AnalogOutput, number);

            var vOffset = _spiConnector.GetSingleRegisterValue(neuronGroup, 1021);
            var vDef = _spiConnector.GetSingleRegisterValue(neuronGroup, 1020);
            var vRef = _spiConnector.GetSingleRegisterValue(neuronGroup, 1009);
            var vRefInt = _spiConnector.GetSingleRegisterValue(neuronGroup, 5);

            if (vRefInt == null || vRef == null || vOffset == null || vDef == null)
            {
                throw new Exception("failure while getting the calibration Values from the Registers!");
            }

            _vOffset = BitConverter.ToInt16(BitConverter.GetBytes(vOffset.Value), 0);
            _vDev = BitConverter.ToInt16(BitConverter.GetBytes(vDef.Value), 0);

            _vRealInt = 3.3 * ((double)vRef / (double)vRefInt);
        }
        #endregion Constructor

        #region Public Methods
        public IUniqueIdentifier UniqueIdentifyer { get; }
        public void SetPercentValue(double percent)
        {
            _logger.LogDebug(this, "set Percent value " + percent);
            var voltage = 10.0 / 100.0 * percent;
            SetVoltageValue(voltage);
        }

        public double GetCurrentPercentValue()
        {
            _logger.LogDebug(this, "get cuttent percent value ");
            return 100.0 / 10.0 * GetCurrentVoltageValue();
        }

        public double GetCurrentMilliAmpereValue()
        {
            return 0;
        }

        public void SetCurrentMilliAmpereValue(double value)
        {
        }

        public void SetVoltageValue(double voltage)
        {
            if (voltage > 10)
                voltage = 10;

            _logger.LogDebug(this, "set voltage value " + voltage);

            //var value = Convert.ToUInt16(voltage / (3 * _vRealInt) * (4095 + _vOffset));

            var value = (voltage - _vOffset / 10000) / (_vRealInt / 4095 * (1 + _vDev / 10000) * 3);
            if (value < 0)
                value = 0;


            _spiConnector.SetRegisterValue(_neuronGroup, _registerNumber, Convert.ToUInt16(value));
            UpdateLocalValue();
        }
        public double GetCurrentVoltageValue()
        {
            _logger.LogDebug(this, "get current voltage value ");
            UpdateLocalValue();
            return GetVoltageValueFromCurrentRegisterValue();
        }

        public void RaiseAllObjectEvents()
        {
            _logger.LogDebug(this, "raise all events");
            UpdateLocalValue(true);
        }
        #endregion Public Methods

        #region Public Events
        public event AnalogOutputChanged OnAnalogoutputChanged;
        #endregion Public Events

        #region Private Methods
        private double GetPercentValueFromCurrentRegisterValue() => 100.0 / 10.0 * GetVoltageValueFromCurrentRegisterValue();
        private double GetVoltageValueFromCurrentRegisterValue()
        {
            //var value = 3 * _currentRegisterValue * _vRealInt / (_vOffset + 4095);

            var value = _vRealInt * 3 * (Convert.ToDouble(_currentRegisterValue) / 4096) * (1 + _vDev / 10000) + _vOffset / 10000;

            var roundValue = Convert.ToDouble(Math.Round(value, 2, MidpointRounding.ToEven));
            return roundValue;
        }
        private double GetMilliAmpereValueFromCurrentRegisterValue()
        {

            var value = _vRealInt * (Convert.ToDouble(_currentRegisterValue) / 4096) * 10 * (1 + _vDev / 10000) + Convert.ToDouble(_vOffset) / 10000;

            var roundValue = Math.Round(value, 2, MidpointRounding.ToEven);
            return roundValue;
        }

        private void UpdateLocalValue(bool raiseEvent = false)
        {
            _logger.LogDebug(this, "update local value with register value");

            var registerSet = _spiConnector.GetRegisterValues(_neuronGroup, _registerNumber, 1);

            if (!registerSet.ContainsRegister(_registerNumber))
            {
                throw new Exception("Getting the current analog output value failed!");
            }

            if (!registerSet.ContainsRegister(_registerNumber))
            {
                return;
            }

            var registerValue = registerSet.GetSpiRegisterValue(_registerNumber);

            if (!registerValue.HasValue)
            {
                return;
            }

            if (registerValue.Value != _currentRegisterValue)
            {
                _currentRegisterValue = (ushort) (registerValue.Value > 4096 ? 4096 : registerValue.Value);

                raiseEvent = true;
            }

            if (raiseEvent)
            {
                RaiseOnAnalogoutputChanged(this, GetPercentValueFromCurrentRegisterValue(), GetVoltageValueFromCurrentRegisterValue(), GetMilliAmpereValueFromCurrentRegisterValue());
            }
        }
        private void RaiseOnAnalogoutputChanged(IAnalogOutput output, double percentagevalue, double voltagevalue, double milliAmpereValue) => Task.Run(() => OnAnalogoutputChanged?.Invoke(output, percentagevalue, voltagevalue, milliAmpereValue));
      

        #endregion Private Methods
    }
}
