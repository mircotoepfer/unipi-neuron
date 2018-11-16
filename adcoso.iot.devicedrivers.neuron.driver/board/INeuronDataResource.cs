using adcoso.iot.devicedrivers.neuron.driver.analogoutput;

namespace adcoso.iot.devicedrivers.neuron.driver.board
{
    public interface INeuronDataResource : IDataEventSource
    {
        
        IUniqueIdentifyer UniqueIdentifyer { get; }
    }
}