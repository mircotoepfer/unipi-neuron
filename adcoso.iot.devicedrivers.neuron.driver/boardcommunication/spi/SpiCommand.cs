using adcoso.iot.devicedrivers.neuron.driver.commons;

namespace adcoso.iot.devicedrivers.neuron.driver.boardcommunication.spi
{
    internal class NeuronSpiMessage
    {
        public NeuronSpiCommand Operation { get; }
        public byte NumberOfRegisters { get; }
        public ushort Register { get; }

        internal NeuronSpiMessage(NeuronSpiCommand operation, ushort register, byte numberOfRegisters)
        {
            Operation = operation;
            NumberOfRegisters = numberOfRegisters;
            Register = register;
        }

        
        public override string ToString() => "Operation " + Operation + " RegisterNumber " + Register + " Number of Registers " + NumberOfRegisters;
    }
}
