using adcoso.iot.devicedrivers.neuron.driver.board;

namespace adcoso.iot.devicedrivers.neuron.driver.analoginput
{

    public delegate void AnalogInputChanged(IAnalogInput input, double percentageValue, double voltageValue, double milliAmpereValue);


    public interface IAnalogInput : INeuronDataResource
    {

        double GetPercentValue();

        double GetVoltageValue();

        double GetMilliAmpere();

        event AnalogInputChanged OnAnalogInputChanged;
    }
}