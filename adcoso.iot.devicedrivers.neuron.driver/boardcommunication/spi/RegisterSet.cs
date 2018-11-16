using System.Collections.Generic;
using System.Linq;

namespace adcoso.iot.devicedrivers.neuron.driver.boardcommunication.spi
{
    internal class SpiRegisterSet
    {

        
        private readonly Dictionary<ushort, ushort> _registerValues;


        public SpiRegisterSet()
        {
            _registerValues = new Dictionary<ushort, ushort>();
        }

        public int Count => _registerValues.Count;

        internal void SetRegisterWithValue(ushort register, ushort value)
        {
            if (!_registerValues.ContainsKey(register))
            {
                _registerValues.Add(register, value);
            }
        }


        
        internal SpiRegister GetSpiRegister(ushort registerNumber) =>
            _registerValues.ContainsKey(registerNumber) ? new SpiRegister(registerNumber, _registerValues[registerNumber]) : null;

        
        internal List<SpiRegister> GetSpiRegisters()
        {
            var list = new List<SpiRegister>();

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var registerValue in _registerValues)
            {
                list.Add(new SpiRegister(registerValue.Key, registerValue.Value));
            }

            return list;
        }

        internal ushort? GetSpiRegisterValue(ushort registerNumber)
        {

            if (_registerValues.ContainsKey(registerNumber))
            {
                return _registerValues[registerNumber];
            }

            return null;
        }

        internal bool ContainsRegister(ushort registerNumber) => _registerValues.ContainsKey(registerNumber);

        
        public ushort[] ToRegisterValueArray()
        {
            var array = _registerValues.Values.ToArray();
            return array ?? new ushort[0];
        }
    }
}
