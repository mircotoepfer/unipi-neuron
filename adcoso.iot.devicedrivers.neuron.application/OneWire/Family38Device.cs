using adcoso.iot.devicedrivers.neuron.driver.onewire.interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace adcoso.iot.devicedrivers.neuron.application.OneWire
{


    /// <summary>
    /// 
    /// </summary>
    public class Family38Device
    {
        private readonly IOneWireDevice _device;
        private Timer _timer;

        private const byte ConvertTCommand = 0x44;
        private const byte ReadScratchpadCommand = 0xBE;
        private const int TemperatureLsb = 0;
        private const int TemperatureMsb = 1;


        public event NewValue OnNewValue;

        public Family38Device(IOneWireDevice device, TimeSpan getValueTimeSpan)
        {
            if (device.FamilyCode != 38)
            {
                throw new Exception("The Device doesn't fit to the family code");
            }

            _device = device;

            if (getValueTimeSpan != TimeSpan.Zero)
            {
                _timer = new Timer(GetValue, new object(), getValueTimeSpan, getValueTimeSpan);
            }
        }

        public string SerialNumber => _device.SerialNumber;

        public byte FamilyCode => _device.FamilyCode;

        private void GetValue(object state)
        {
            if (_device.DeviceIsDead)
            {
                state = null;
                _timer.Dispose();
                return;
            }

            try
            {
                if (OnNewValue == null)
                {
                    return;
                }

                var temp = GetTemperature();

                Task.Run(() => OnNewValue.Invoke(temp));

            }
            catch (Exception)
            {
                //ignore
            }
        }

        private double? GetTemperature()
        {
            _device.SendCommand(true, 100, 0xB4);
            _device.SendCommand(true, 100, 0xB8, 0x00);
            

            var data = _device.ReadData(8, ReadScratchpadCommand, 0x00);
            var Temp = 0.0;

            if (GetBit(data[2],8) )
            {
                // Negative Temperature
                var TempWhole = ~data[2];
                var TempFrac = ((~data[1]) & 0xF8) * (2 ^ -8);
                Temp = -1 * (TempWhole + TempFrac);
            }
            else
            {
                // Positive Temperature
                var TempWhole = data[2];
                var TempFrac = (data[1] & 0xF8) * (2 ^ -8);
                Temp = data[2] + TempFrac;
            }



            var vdd = ((data[3] + data[4] * 0xFF) & 0x3FF) * 0.01;

            var vdd2 = ((data[5] + data[6] * 0xFF) & 0x3FF) * 0.01;


            return 0.0;

        }


        private static bool GetBit(byte b, int bitNumber)
        {
            return (b & (1 << bitNumber)) != 0;
        }


    }
}
