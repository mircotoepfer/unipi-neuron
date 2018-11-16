namespace adcoso.iot.devicedrivers.neuron.driver.board
{
    public enum NeuronResource
    {
        DigitalInput,
        DigitalOutput,
        RelayOutput,
        UserLed,
        AnalogInput,
        AnalogOutput,
        OneWireConnector,
        ModbusConnector
    }
    public enum NeuronGroup
    {
        One = 1,
        Two = 2,
        Three = 3
    }

    public interface IUniqueIdentifier
    {
        NeuronResource Resource { get; }

        NeuronGroup Group { get; }

        int Number { get; }

        
        string IdentifierString { get; }
    }
}
