using adcoso.iot.devicedrivers.neuron.driver.commons;

namespace adcoso.iot.devicedrivers.neuron.driver.boardcommunication.spi
{
    internal static class NeuronSpiMessageFactory
    {
        
        internal static NeuronSpiMessage BoradInformation() => new NeuronSpiMessage(NeuronSpiCommand.ReadRegister, 1000, 5);
    }
}
