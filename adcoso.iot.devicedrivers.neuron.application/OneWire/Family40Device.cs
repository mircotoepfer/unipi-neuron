using adcoso.iot.devicedrivers.neuron.driver.onewire.interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace adcoso.iot.devicedrivers.neuron.application.OneWire
{
    public delegate void NewValue(double? value);

    /// <summary>
    /// 
    /// </summary>
    public class Family40Device
    {
        private readonly IOneWireDevice _device;
        private Timer _timer;

        private const byte ConvertTCommand = 0x44;
        private const byte ReadScratchpadCommand = 0xBE;
        private const int TemperatureLsb = 0;
        private const int TemperatureMsb = 1;

        public event NewValue OnNewValue;

        public Family40Device(IOneWireDevice device, TimeSpan getValueTimeSpan)
        {
            if (device.FamilyCode != 40)
            {
                throw new Exception("The Device doesn't fit to the family code");
            }

            _device = device;

            if (getValueTimeSpan != TimeSpan.Zero)
            {
                _timer = new Timer(GetValue, new object(), getValueTimeSpan, getValueTimeSpan);
            }
        }

        public string SerialNumber
        {
            get { return _device.SerialNumber; }
        }

        public byte FamilyCode
        {
            get { return _device.FamilyCode; }
        }

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

        public double? GetTemperature()
        {
            _device.SendCommand(true, 500, ConvertTCommand);

            var data = _device.ReadData(9, ReadScratchpadCommand);

            var value = false;

            foreach (var b in data.Where(b => b != 255))
            {
                value = true;
            }

            if (!value)
            {
                return null;
            }

            return GetTemp_Read(data[TemperatureMsb], data[TemperatureLsb]);

        }

        private static double GetTemp_Read(byte msb, byte lsb)
        {
            double tempRead = 0;
            var negative = false;

            if (msb > 0xF8)
            {
                negative = true;
                msb = (byte)~msb;
                lsb = (byte)~lsb;
                var addOne = (ushort)lsb;
                addOne |= (ushort)(msb << 8);
                addOne++;
                lsb = (byte)(addOne & 0xFFu);
                msb = (byte)((addOne >> 8) & 0xFFu);
            }

            if (GetBit(lsb, 0))
            {
                tempRead += Math.Pow(2, -4);
            }
            if (GetBit(lsb, 1))
            {
                tempRead += Math.Pow(2, -3);
            }
            if (GetBit(lsb, 2))
            {
                tempRead += Math.Pow(2, -2);
            }
            if (GetBit(lsb, 3))
            {
                tempRead += Math.Pow(2, -1);
            }
            if (GetBit(lsb, 4))
            {
                tempRead += Math.Pow(2, 0);
            }
            if (GetBit(lsb, 5))
            {
                tempRead += Math.Pow(2, 1);
            }
            if (GetBit(lsb, 6))
            {
                tempRead += Math.Pow(2, 2);
            }
            if (GetBit(lsb, 7))
            {
                tempRead += Math.Pow(2, 3);
            }
            if (GetBit(msb, 0))
            {
                tempRead += Math.Pow(2, 4);
            }
            if (GetBit(msb, 1))
            {
                tempRead += Math.Pow(2, 5);
            }
            if (GetBit(msb, 2))
            {
                tempRead += Math.Pow(2, 6);
            }

            if (negative)
            {
                tempRead = tempRead * -1;
            }

            return tempRead;
        }

        private static bool GetBit(byte b, int bitNumber)
        {
            return (b & (1 << bitNumber)) != 0;
        }


    }
}
