using adcoso.iot.devicedrivers.neuron.driver.boardcommunication.i2c;
using adcoso.iot.devicedrivers.neuron.driver.commons;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;


namespace adcoso.iot.devicedrivers.neuron.driver.onewire
{
    internal class NeuronConnectedOneWireDevice
    {
        #region Private Properties

        
        private readonly I2CConnector _i2CGateway;
        private int _lastDiscrepancy;
        private bool _lastDeviceFlag;
        private byte _crc8;
        private byte[] _romNumber;
        #endregion Private Properties

        #region Constructor
        internal NeuronConnectedOneWireDevice(I2CConnector connector)
        {
            _i2CGateway = connector;
            _romNumber = new byte[8];
        }
        #endregion Constructor

        #region Internal Accessors

        
        internal byte[] CurrentRomNumber
        {
            get
            {
                var numberArray = new List<byte>(_romNumber).ToArray();
                return numberArray ?? new byte[8];
            }
            set { _romNumber = value; }
        }
        #endregion Internal Accessors

        #region Internal Methods

        internal bool FindFirstOneWireDeviceOnTheBus()
        {
            _lastDiscrepancy = 0;
            _lastDeviceFlag = false;
            return OneWireSearchDevice();
        }

        internal bool JumpToNextDevice() => OneWireSearchDevice();

        internal bool OneWireResetBus()
        {
            _i2CGateway.Write(UnipiI2CDevice.OneWire, new[] { FunctionCommand.OnewireReset });

            var status = ReadStatus();

            if (GetBit(status, StatusBit.ShortDetected))
            {
                throw new InvalidOperationException("One Wire short detected");
            }

            return GetBit(status, StatusBit.PresencePulseDetected);
        }

        internal void OneWireWriteByte(params byte[] byteValues)
        {

            foreach (byte byteValue in byteValues)
            {
                _i2CGateway.Write(UnipiI2CDevice.OneWire, new[] { FunctionCommand.OnewireWriteByte, byteValue });

                ReadStatus();
            }

            //ReadStatus();
        }

        internal byte OneWireReadByte()
        {
            var buffer = new byte[1];
            _i2CGateway.Write(UnipiI2CDevice.OneWire, new[] { FunctionCommand.OnewireReadByte });
            ReadStatus();
            _i2CGateway.WriteRead(UnipiI2CDevice.OneWire, new[] { FunctionCommand.SetReadPointer, RegisterSelection.ReadData }, buffer);
            return buffer[0];
        }

        internal void EnableStrongPullup()
        {
            var configuration = new byte();
            configuration |= 1 << 2;
            configuration |= 1 << 7;
            configuration |= 1 << 5;
            configuration |= 1 << 4;

            _i2CGateway.Write(UnipiI2CDevice.OneWire, new[] { FunctionCommand.WriteConfiguration, configuration });
        }

        #endregion Internal Methods

        #region Private Methods

        private bool OneWireSearchDevice()
        {
            var idBitNumber = 1;
            var lastZero = 0;
            var romByteNumber = 0;
            byte romByteMask = 1;
            var searchResult = false;
            _crc8 = 0;

            if (!_lastDeviceFlag)
            {
                if (!OneWireResetBus())
                {
                    _lastDiscrepancy = 0;
                    _lastDeviceFlag = false;
                    return false;
                }

                OneWireWriteByte(0xF0);

                do
                {
                    var idBit = OneWireReadBit();
                    var cmpIdBit = OneWireReadBit();


                    if (idBit && cmpIdBit)
                    {
                        break;
                    }

                    bool searchDirection;

                    if (idBit != cmpIdBit)
                    {
                        searchDirection = idBit;
                    }
                    else
                    {
                        if (idBitNumber < _lastDiscrepancy)
                        {
                            searchDirection = (_romNumber[romByteNumber] & romByteMask) > 0;
                        }
                        else
                        {
                            searchDirection = idBitNumber == _lastDiscrepancy;
                        }

                        if (!searchDirection)
                        {
                            lastZero = idBitNumber;

                            if (lastZero < 9)
                            {
                            }
                        }
                    }

                    if (searchDirection)
                    {
                        _romNumber[romByteNumber] |= romByteMask;
                    }
                    else
                    {
                        var result = (byte)~romByteMask;
                        _romNumber[romByteNumber] &= result;
                    }


                    OneWireWriteBit(searchDirection);

                    idBitNumber++;
                    romByteMask <<= 1;

                    if (romByteMask != 0)
                    {
                        continue;
                    }

                    Docrc8(_romNumber[romByteNumber]);
                    romByteNumber++;
                    romByteMask = 1;
                }
                while (romByteNumber < 8);


                if (!((idBitNumber < 65) || (_crc8 != 0)))
                {
                    _lastDiscrepancy = lastZero;

                    if (_lastDiscrepancy == 0)
                    {
                        _lastDeviceFlag = true;
                    }

                    searchResult = true;
                }
            }


            if (searchResult && (_romNumber[0] != 0))
            {
                return true;
            }

            _lastDiscrepancy = 0;
            _lastDeviceFlag = false;

            return false;
        }

        private byte ReadStatus(bool setReadPointerToStatus = false)
        {
            var statusBuffer = new byte[1];
            if (setReadPointerToStatus)
            {
                _i2CGateway.WriteRead(UnipiI2CDevice.OneWire, new[] { FunctionCommand.SetReadPointer, RegisterSelection.Status }, statusBuffer);
            }
            else
            {
                _i2CGateway.Read(UnipiI2CDevice.OneWire, statusBuffer);
            }

            if (statusBuffer.Length < 1)
            {
                throw new InvalidOperationException("Read status failed");
            }

            var stopWatch = new Stopwatch();
            do
            {
                if (stopWatch.ElapsedMilliseconds > 1)
                {
                    throw new InvalidOperationException("One Wire bus busy for too long");
                }

                _i2CGateway.Read(UnipiI2CDevice.OneWire, statusBuffer);
            } while (GetBit(statusBuffer[0], StatusBit.OneWireBusy));

            return statusBuffer[0];
        }

        private void OneWireWriteBit(bool bitValue)
        {

            var byteValue = new byte();
            if (bitValue)
            {
                byteValue |= 1 << 7;
            }

            _i2CGateway.Write(UnipiI2CDevice.OneWire, new[] { FunctionCommand.OnewireSingleBit, byteValue });


            ReadStatus();
        }

        private bool OneWireReadBit()
        {

            var byteValue = new byte();

            byteValue |= 1 << 7;

            _i2CGateway.Write(UnipiI2CDevice.OneWire, new[] { FunctionCommand.OnewireSingleBit, byteValue });

            var status = ReadStatus();

            return GetBit(status, StatusBit.SingleBitResult);
        }

        private static bool GetBit(byte b, int bitNumber) => (b & (1 << bitNumber)) != 0;

        private void Docrc8(byte value) => _crc8 = CrcTable[_crc8 ^ value];

        #endregion Private Methods

        #region Private Properies

        private static readonly byte[] CrcTable = new[] {
            0, 94,188,226, 97, 63,221,131,194,156,126, 32,163,253, 31, 65,
            157,195, 33,127,252,162, 64, 30, 95,  1,227,189, 62, 96,130,220,
            35,125,159,193, 66, 28,254,160,225,191, 93,  3,128,222, 60, 98,
            190,224,  2, 92,223,129, 99, 61,124, 34,192,158, 29, 67,161,255,
            70, 24,250,164, 39,121,155,197,132,218, 56,102,229,187, 89,  7,
            219,133,103, 57,186,228,  6, 88, 25, 71,165,251,120, 38,196,154,
            101, 59,217,135,  4, 90,184,230,167,249, 27, 69,198,152,122, 36,
            248,166, 68, 26,153,199, 37,123, 58,100,134,216, 91,  5,231,185,
            140,210, 48,110,237,179, 81, 15, 78, 16,242,172, 47,113,147,205,
            17, 79,173,243,112, 46,204,146,211,141,111, 49,178,236, 14, 80,
            175,241, 19, 77,206,144,114, 44,109, 51,209,143, 12, 82,176,238,
            50,108,142,208, 83, 13,239,177,240,174, 76, 18,145,207, 45,115,
            202,148,118, 40,171,245, 23, 73,  8, 86,180,234,105, 55,213,139,
            87,  9,235,181, 54,104,138,212,149,203, 41,119,244,170, 72, 22,
            233,183, 85, 11,136,214, 52,106, 43,117,151,201, 74, 20,246,168,
            116, 42,200,150, 21, 75,169,247,182,232, 10, 84,215,137,107, 53}.Select(x => (byte)x).ToArray();

        #endregion Private Properies

        #region Internal Classes
        internal class FunctionCommand
        {
            internal const byte SetReadPointer = 0xE1;
            internal const byte WriteConfiguration = 0xD2;
            internal const byte OnewireReset = 0xB4;
            internal const byte OnewireSingleBit = 0x87;
            internal const byte OnewireWriteByte = 0xA5;
            internal const byte OnewireReadByte = 0x96;
        }

        internal class RegisterSelection
        {
            internal const byte Status = 0xF0;
            internal const byte ReadData = 0xE1;
        }

        internal class StatusBit
        {
            internal const int SingleBitResult = 5;
            internal const int ShortDetected = 2;
            internal const int PresencePulseDetected = 1;
            internal const int OneWireBusy = 0;
        }
        #endregion Internal Classes
    }
}