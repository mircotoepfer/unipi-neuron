namespace adcoso.iot.devicedrivers.neuron.driver.commons
{
    public enum OnOffValue
    {
        Unknown,
        On,
        Off
    }


    internal enum NeuronSpiCommand : ushort
    {
        ReadBit = 1,
        ReadRegister = 4,
        WriteBit = 5,
        WriteRegister = 6,
        WriteBits = 15,
        WriteChar = 65,
        WriteString = 100,
        ReadString = 101,
        Idle = 0xfa
    }

    /// <summary>
    /// The LogLevel
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// The debug
        /// </summary>
        Debug = 4,

        /// <summary>
        /// The monitor
        /// </summary>
        Monitor = 3,

        /// <summary>
        /// The information
        /// </summary>
        Information = 2,

        /// <summary>
        /// The error
        /// </summary>
        Error = 1,

        Exception = 0
    }

    internal enum UnipiI2CDevice : byte
    {
        OneWire = 0x18
    }


    internal enum BoardType
    {
        UnknownBoard,
        B10001,
        E8Di8Ro1,
        E14Ro1,
        E16Di1,
        E8Di8Ro1P11DiMb485,
        E14Ro1P11DiR4851,
        E16Di1P11DiR4851,
        E14Ro1U14Ro1,
        E16Di1U14Ro1,
        E14Ro1U14Di1,
        E16Di1U14Di1
    }

    internal enum DataAddressType
    {
        Bit, Word, DWord
    }
}



