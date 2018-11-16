using adcoso.iot.devicedrivers.neuron.driver.board;
using adcoso.iot.devicedrivers.neuron.driver.commons;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Spi;

namespace adcoso.iot.devicedrivers.neuron.driver.boardcommunication.spi
{
    internal class NeuronSpiConnector
    {
        #region Private Members
        
        private readonly object _spiDeviceLocker;
        
        private readonly Dictionary<NeuronGroup, SpiDevice> _devices;
        
        private readonly Queue<byte> _destination;
        
        private readonly DriverLogger _driverLogger;
        #endregion Private Members

        #region Constructor
        public NeuronSpiConnector(DriverLogger driverLogger)
        {
            _spiDeviceLocker = new object();
            _devices = new Dictionary<NeuronGroup, SpiDevice>();
            _destination = new Queue<byte>(new byte[8]);
            _driverLogger = driverLogger;
        }
        #endregion Constructor

        #region Internal Methods

        internal ushort? GetSingleRegisterValue(NeuronGroup neuronGroup, ushort registerNumber)
        {
            var result = GetRegisterValues(neuronGroup, registerNumber, 1);
            return result.GetSpiRegisterValue(registerNumber);
        }


        
        internal SpiRegisterSet GetRegisterValues(NeuronGroup neuronGroup, ushort register, byte numberOfRegisters)
        {

            var result = new SpiRegisterSet();

            var returnMessage = new byte[0];

            try
            {
                var bytesToSendPhaseOne = ByteArrayForReadRegisterMessagePhase1(NeuronSpiCommand.ReadRegister, register, numberOfRegisters);
                var bytesToSendPhaseTwo = ByteArrayForReadRegisterMessagePhase2(NeuronSpiCommand.ReadRegister, register, numberOfRegisters);

                returnMessage = TwoPhaseCommunication(neuronGroup, bytesToSendPhaseOne, bytesToSendPhaseTwo);

                if (returnMessage == null)
                {
                    return result;
                }

                var firstRegister = 4;

                for (ushort i = 0; i < numberOfRegisters; i++)
                {
                    var completeRegister = new[] { returnMessage[firstRegister++], returnMessage[firstRegister++] };
                    var registerValue = BitConverter.ToUInt16(completeRegister, 0);
                    var registerNumber = Convert.ToUInt16(i + register);
                    result.SetRegisterWithValue(registerNumber, registerValue);
                }

                return result;
            }
            catch (Exception e)
            {
                _driverLogger.LogException(this, e);
                _driverLogger.LogDebug(this, "SPI Return Value:" + string.Join("-", returnMessage));
                return new SpiRegisterSet();
            }

        }

        internal void SetCoilsValue(NeuronGroup neuronGroup, ushort coil, bool value)
        {
            byte intValue = 0;

            if (value)
                intValue = 1;


            var message = ByteArrayForSetCoilsMessage(NeuronSpiCommand.WriteBit, coil, intValue);
            OnePhaseCommunication(neuronGroup, message);

        }
        //public bool GetCoilsValue(NeuronGroup neuronGroup, ushort coil)
        //{

        //    var messagePhase1 = ByteArrayForGetCoilsMessagePhase1(NeuronSpiCommand.ReadBit, coil, 1);
        //    var messagePhase2 = ByteArrayForGetCoilsMessagePhase2(NeuronSpiCommand.ReadBit, coil, 1);
        //    var value = TwoPhaseCommunication(neuronGroup, messagePhase1, messagePhase2);
        //    Debug.WriteLine(String.Join("-", value));
        //    if (value[4] == 0) return false;
        //    return true;
        //}



        internal void SetRegisterValue(NeuronGroup neuronGroup, ushort register, ushort value)
        {
            var messagePhaseOne = ByteArrayForReadRegisterMessagePhase1(NeuronSpiCommand.WriteRegister, register, 1);

            var crc = BitConverter.ToUInt16(messagePhaseOne, messagePhaseOne.Length - 2);

            var messagePhaseTwo = ByteArrayForSetRegisterMessagePhase2(NeuronSpiCommand.WriteRegister, register, value, crc);

            TwoPhaseCommunication(neuronGroup, messagePhaseOne, messagePhaseTwo);
        }



        internal async Task Initialize()
        {
            //var chipSelectGroup = new[] { 1, 3, 0 };

            //1 ist Group 1


            var chipSelectGroup = new Dictionary<NeuronGroup, int>
            {
                {NeuronGroup.One, 1},
                { NeuronGroup.Two, 0}
            };


            foreach (var chipSelectId in chipSelectGroup)
            {
                try
                {

                    SpiDevice device = null;

                    var spiAqs = SpiDevice.GetDeviceSelector("SPI0");

                    if (spiAqs == null)
                    {
                        throw new Exception("Async Operation Error");
                    }

                    var findAllAsync = DeviceInformation.FindAllAsync(spiAqs);
                    if (findAllAsync == null)
                    {
                        throw new Exception("Async Operation Error");
                    }

                    var devicesInfo = await findAllAsync;

                    var settings = new SpiConnectionSettings(chipSelectId.Value)
                    {
                        ClockFrequency = 12000000,
                        SharingMode = SpiSharingMode.Shared
                    };



                    var deviceInformation = devicesInfo?[0];

                    if (deviceInformation != null)
                    {


                        var fromIdAsync = SpiDevice.FromIdAsync(deviceInformation.Id, settings);

                        if (fromIdAsync != null)
                        {
                            device = await fromIdAsync;
                        }
                    }
                    else
                    {
                        throw new Exception("SPI findAllAsync failed, result is null");
                    }

                    if (device == null)
                    {
                        throw new Exception("SPI Device not found!");
                    }

                    _devices.Add(chipSelectId.Key, device);
                }
                catch (Exception e)
                {
                    _driverLogger.LogException(this, e);
                }
            }



        }

        
        internal IEnumerable<NeuronGroup> GetFoundGroupIds() => new List<NeuronGroup>(_devices.Keys);

        #endregion Internal Methods





        #region Private Static Methods

        
        private static byte[] ByteArrayForSetCoilsMessage(NeuronSpiCommand command, ushort register, byte value)
        {
            var data = new List<byte> { (byte)command, value };

            data.AddRange(BitConverter.GetBytes(register));

            var crc = CreateCrc(data.ToArray());

            data.AddRange(crc);

            var array = data.ToArray();

            return array ?? new byte[0];
        }

        
        private static byte[] ByteArrayForGetCoilsMessagePhase1(NeuronSpiCommand command, ushort coil, ushort numberOfCoils)
        {


            var lenght = Convert.ToByte(4 + (((numberOfCoils + 15) >> 4) << 1));
            var data = new List<byte> { (byte)command, lenght };
            data.AddRange(BitConverter.GetBytes(coil));
            var crc = CreateCrc(data.ToArray());
            data.AddRange(crc);
            var array = data.ToArray();
            return array ?? new byte[0];



            //var lenght = Convert.ToByte(4 + numberOfCoils);
            //var data = new List<byte> { (byte)command, lenght };
            //data.AddRange(BitConverter.GetBytes(coil));
            //var crc = CreateCrc(data.ToArray());
            //data.AddRange(crc);
            //var array = data.ToArray();
            //return array ?? new byte[0];
        }

        
        private static byte[] ByteArrayForGetCoilsMessagePhase2(NeuronSpiCommand command, ushort coil, ushort numberOfCoils)
        {

            var data = new List<byte> { (byte)command, 0 };
            data.AddRange(BitConverter.GetBytes(coil));
            data.AddRange(new byte[4 + (((numberOfCoils + 15) >> 4) << 1) - data.Count + 2]);
            var array = data.ToArray();
            return array ?? new byte[0];

            //var data = new List<byte> { (byte)command, 0 };
            //data.AddRange(BitConverter.GetBytes(coil));
            //data.AddRange(new byte[4 + numberOfCoils - data.Count + 2]);
            //var array = data.ToArray();
            //return array ?? new byte[0];
        }

        
        private static byte[] ByteArrayForReadRegisterMessagePhase1(NeuronSpiCommand command, ushort register, byte numberOfRegisters)
        {
            var lenght = Convert.ToByte(4 + numberOfRegisters * 2);
            var data = new List<byte> { (byte)command, lenght };
            data.AddRange(BitConverter.GetBytes(register));
            var crc = CreateCrc(data.ToArray());
            data.AddRange(crc);
            var array = data.ToArray();
            return array ?? new byte[0];
        }
        
        private static byte[] ByteArrayForReadRegisterMessagePhase2(NeuronSpiCommand command, ushort register, byte numberOfRegisters)
        {
            var data = new List<byte> { (byte)command, 0 };
            data.AddRange(BitConverter.GetBytes(register));
            data.AddRange(new byte[4 + numberOfRegisters * 2 - data.Count + 2]);
            var array = data.ToArray();
            return array ?? new byte[0];
        }

        
        private static byte[] ByteArrayForSetRegisterMessagePhase2(NeuronSpiCommand command, ushort register, ushort value, ushort currentCrc)
        {
            var lenght = Convert.ToByte(1);
            var data = new List<byte> { (byte)command, lenght };
            data.AddRange(BitConverter.GetBytes(register));
            data.AddRange(BitConverter.GetBytes(value));

            var crc = CreateCrc(data.ToArray(), 0, currentCrc);
            data.AddRange(crc);

            var array = data.ToArray();
            return array ?? new byte[0];
        }

        
        private static byte[] CreateCrc(IReadOnlyList<byte> byteArray, int lenght = 0, ushort initValue = 0)
        {
            if (byteArray == null)
            {
                return null;
            }

            var result = initValue;

            if (lenght == 0)
            {
                lenght = byteArray.Count;
            }

            ushort[] spiCrc16Table =
            {
                    0,  1408,  3968,  2560,  7040,  7680,  5120,  4480, 13184, 13824, 15360,
                14720, 10240, 11648, 10112,  8704, 25472, 26112, 27648, 27008, 30720, 32128,
                30592, 29184, 20480, 21888, 24448, 23040, 19328, 19968, 17408, 16768, 50048,
                50688, 52224, 51584, 55296, 56704, 55168, 53760, 61440, 62848, 65408, 64000,
                60288, 60928, 58368, 57728, 40960, 42368, 44928, 43520, 48000, 48640, 46080,
                45440, 37760, 38400, 39936, 39296, 34816, 36224, 34688, 33280, 33665, 34305,
                35841, 35201, 38913, 40321, 38785, 37377, 45057, 46465, 49025, 47617, 43905,
                44545, 41985, 41345, 57345, 58753, 61313, 59905, 64385, 65025, 62465, 61825,
                54145, 54785, 56321, 55681, 51201, 52609, 51073, 49665, 16385, 17793, 20353,
                18945, 23425, 24065, 21505, 20865, 29569, 30209, 31745, 31105, 26625, 28033,
                26497, 25089,  9089,  9729, 11265, 10625, 14337, 15745, 14209, 12801,  4097,
                5505,  8065,  6657,  2945,  3585,  1025,   385,   899,  1539,  3075,  2435,
                6147,  7555,  6019,  4611, 12291, 13699, 16259, 14851, 11139, 11779,  9219,
                8579, 24579, 25987, 28547, 27139, 31619, 32259, 29699, 29059, 21379, 22019,
                23555, 22915, 18435, 19843, 18307, 16899, 49155, 50563, 53123, 51715, 56195,
                56835, 54275, 53635, 62339, 62979, 64515, 63875, 59395, 60803, 59267, 57859,
                41859, 42499, 44035, 43395, 47107, 48515, 46979, 45571, 36867, 38275, 40835,
                39427, 35715, 36355, 33795, 33155, 32770, 34178, 36738, 35330, 39810, 40450,
                37890, 37250, 45954, 46594, 48130, 47490, 43010, 44418, 42882, 41474, 58242,
                58882, 60418, 59778, 63490, 64898, 63362, 61954, 53250, 54658, 57218, 55810,
                52098, 52738, 50178, 49538, 17282, 17922, 19458, 18818, 22530, 23938, 22402,
                20994, 28674, 30082, 32642, 31234, 27522, 28162, 25602, 24962,  8194,  9602,
                12162, 10754, 15234, 15874, 13314, 12674,  4994,  5634,  7170,  6530,  2050,
                    3458,  1922,   514
            };

            for (var i = 0; i < lenght; i++)
            {
                result = (ushort)((result >> 8) ^ spiCrc16Table[(result ^ byteArray[i]) & 0xff]);
            }

            return BitConverter.GetBytes(result);
        }

        #endregion Private Static Methods


        #region Private Communication Methods

        // ReSharper disable once SuggestBaseTypeForParameter
        private void PrepareSpiCommunication(SpiDevice device)
        {
            var timeout = DateTime.Now.AddSeconds(1);
            while (timeout > DateTime.Now)
            {
                var buff = new byte[1];

                device.Read(buff);

                while (_destination.Count > 7)
                {
                    _destination.Dequeue();
                }

                _destination.Enqueue(buff[0]);

                var arrayVal = _destination.ToArray();

                if (arrayVal != null &&
                    arrayVal[0] == 0 &&
                    arrayVal[1] == 250 &&
                    arrayVal[2] == 0 &&
                    arrayVal[3] == 85 &&
                    arrayVal[4] == 14 &&
                    arrayVal[5] == 236 &&
                    arrayVal[6] == 189 &&
                    arrayVal[7] == 0)
                {
                    return;
                }
            }

            throw new Exception("SPI Syncronization failed for SPI Device" +
                                " ChipSelectLine:" + device.ConnectionSettings?.ChipSelectLine +
                                " ClockFrequency:" + device.ConnectionSettings?.ClockFrequency +
                                " DataBitLength:" + device.ConnectionSettings?.DataBitLength +
                                " Mode:" + device.ConnectionSettings?.Mode +
                                " SharingMode:" + device.ConnectionSettings?.SharingMode);
        }



        
        private byte[] TwoPhaseCommunication(NeuronGroup neuronGroup, byte[] messagePhaseOne, byte[] messagePhaseTwo)
        {
            var returnMessagePhaseOne = new byte[messagePhaseOne.Length];
            messagePhaseOne.CopyTo(returnMessagePhaseOne, 0);

            var returnMessagePhaseTwo = new byte[messagePhaseTwo.Length];
            messagePhaseTwo.CopyTo(returnMessagePhaseTwo, 0);

            byte[] correctByte = null;

            SpiDevice device;

            if (!_devices.TryGetValue(neuronGroup, out device) || device == null)
            {
                _driverLogger.LogError(this, "Device not found!");
                return null;
            }

            var crcCheck = false;
            var tryCounter = 10;

            while (!crcCheck)
            {
                try
                {
                    if (tryCounter-- < 0)
                    {
                        throw new Exception("the spi communication failed!");
                    }

                    lock (_spiDeviceLocker)
                    {


                        PrepareSpiCommunication(device);

                        device.TransferFullDuplex(messagePhaseOne, returnMessagePhaseOne);


                        if (returnMessagePhaseOne[0] == 0)
                        {
                            correctByte = new byte[1];
                            device.Read(correctByte);
                        }

                        device.TransferFullDuplex(messagePhaseTwo, returnMessagePhaseTwo);


                    }

                    if (correctByte != null)
                    {

                        for (var i = 1; i < returnMessagePhaseOne.Length; i++)
                        {
                            returnMessagePhaseOne[i - 1] = returnMessagePhaseOne[i];
                        }

                        returnMessagePhaseOne[returnMessagePhaseOne.Length - 1] = correctByte[0];
                    }

                    var firstCrc = CreateCrc(returnMessagePhaseOne, returnMessagePhaseOne.Length - 2);


                    var crc = CreateCrc(returnMessagePhaseTwo, returnMessagePhaseTwo.Length - 2, BitConverter.ToUInt16(firstCrc, 0));

                    if (crc != null
                        && crc[0] == returnMessagePhaseTwo[returnMessagePhaseTwo.Length - 2]
                        && crc[1] == returnMessagePhaseTwo[returnMessagePhaseTwo.Length - 1])
                    {
                        crcCheck = true;
                    }
                    else
                    {
                        _driverLogger.LogError(this, "CRC Check failed for answer " + string.Join("-", returnMessagePhaseTwo));
                        Task.Delay(100)?.Wait();
                    }
                }

                catch (Exception e)
                {
                    _driverLogger.LogException(this, e);

                    _driverLogger.LogError(this, "#1 PI->Board " + string.Join("-", messagePhaseOne));
                    _driverLogger.LogError(this, "#1 PI<-Board " + string.Join("-", returnMessagePhaseOne));

                    _driverLogger.LogError(this, "#2 PI->Board " + string.Join("-", messagePhaseTwo));
                    _driverLogger.LogError(this, "#2 PI<-Board " + string.Join("-", returnMessagePhaseTwo));
                    if (tryCounter < 0)
                    {
                        throw;
                    }

                    Task.Delay(10)?.Wait();
                }

            }

            return returnMessagePhaseTwo;

        }


        private void OnePhaseCommunication(NeuronGroup neuronGroup, byte[] message)
        {
            var returnMessage = new byte[message.Length];
            message.CopyTo(returnMessage, 0);

            SpiDevice device;

            if (!_devices.TryGetValue(neuronGroup, out device) || device == null)
            {
                _driverLogger.LogError(this, "Device not found!");
                return;
            }

            var crcCheck = false;

            var tryCounter = 10;

            while (!crcCheck)
            {
                try
                {
                    if (tryCounter-- < 0)
                    {
                        throw new Exception("the spi communication failed!");
                    }

                    lock (_spiDeviceLocker)
                    {
                        PrepareSpiCommunication(device);
                        device.TransferFullDuplex(message, returnMessage);
                    }

                    var crc = CreateCrc(returnMessage, returnMessage.Length - 2);

                    if (crc != null && crc[0] == returnMessage[returnMessage.Length - 2] && crc[1] == returnMessage[returnMessage.Length - 1])
                    {
                        crcCheck = true;
                    }
                    else
                    {
                        throw new Exception("CRC Check failed for answer " + string.Join("-", returnMessage));
                    }

                }
                catch (Exception e)
                {
                    _driverLogger.LogException(this, e);

                    if (tryCounter < 0)
                    {
                        throw;
                    }

                    Task.Delay(100)?.Wait();
                }

            }
        }

        #endregion Private Communication Methods


    }
}
