using adcoso.iot.devicedrivers.neuron.driver.board;

namespace adcoso.iot.devicedrivers.neuron.driver.analogoutput
{

    public delegate void AnalogOutputChanged(IAnalogOutput output, double percentageValue, double voltageValue, double milliAmpereValue);

    public interface IAnalogOutput : INeuronDataResource, IDataEventSource
    {
        IUniqueIdentifyer UniqueIdentifyer { get; }
        void SetPercentValue(double percent);
        double GetCurrentPercentValue();
        double GetCurrentMilliAmpereValue();
        void SetCurrentMilliAmpereValue(double value);

        void SetVoltageValue(double voltage);
        double GetCurrentVoltageValue();
        event AnalogOutputChanged OnAnalogoutputChanged;
    }
}