using adcoso.iot.devicedrivers.neuron.driver.board;
using adcoso.iot.devicedrivers.neuron.driver.commons;

namespace adcoso.iot.devicedrivers.neuron.driver.digitalinputs
{

    public delegate void DigitalInputStateChanged(IDigitalInput digitalInput, OnOffValue value);

    public interface IDigitalInput : INeuronDataResource
    {
        OnOffValue GetDigitalInputValue();
        event DigitalInputStateChanged OnDigitalInputChanged;
    }
}
