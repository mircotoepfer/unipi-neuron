using adcoso.iot.devicedrivers.neuron.driver.board;
using adcoso.iot.devicedrivers.neuron.driver.commons;

namespace adcoso.iot.devicedrivers.neuron.driver.digitaloutputs
{
    public delegate void OutputStateChanged(IDigitalOutput digitalOutput, OnOffValue value);

    internal enum DigitalRelayOutputType
    {
        DigitalOutput, RelayOutput
    }


    public interface IDigitalOutput : INeuronDataResource
    {
        void SetOutputValue(OnOffValue value);
        void ToggleOutput();
        OnOffValue GetOutputValue();

        event OutputStateChanged OnOutputStateChanged;

    }
}
