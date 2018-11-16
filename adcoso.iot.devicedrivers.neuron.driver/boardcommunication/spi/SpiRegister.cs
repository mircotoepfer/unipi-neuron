namespace adcoso.iot.devicedrivers.neuron.driver.boardcommunication.spi
{
    internal class SpiRegister
    {
        internal int RegisterNumber { get; }
        internal ushort Value { get; }

        internal SpiRegister(int registerNumber, ushort value)
        {
            RegisterNumber = registerNumber;
            Value = value;
        }
    }
}
