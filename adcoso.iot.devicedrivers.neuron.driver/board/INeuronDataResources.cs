using adcoso.iot.devicedrivers.neuron.driver.analoginput;
using adcoso.iot.devicedrivers.neuron.driver.analogoutput;
using adcoso.iot.devicedrivers.neuron.driver.digitalinputs;
using adcoso.iot.devicedrivers.neuron.driver.digitaloutputs;
using adcoso.iot.devicedrivers.neuron.driver.onewire.interfaces;
using adcoso.iot.devicedrivers.neuron.driver.rs485modbus;
using adcoso.iot.devicedrivers.neuron.driver.userled;
using System.Collections.Generic;

namespace adcoso.iot.devicedrivers.neuron.driver.board
{
    public interface INeuronDataResources : IDataEventSource
    {

        #region Digital Outputs

        
        IReadOnlyList<IDigitalOutput> DigitalOutputs { get; }

        
        IDigitalOutput GetDigitalOutput(IUniqueIdentifier identifier);

        #endregion Digital Outputs

        #region Digital Inputs

        
        IReadOnlyList<IDigitalInput> DigitalInputs { get; }

        
        IDigitalInput GetDigitalInput(IUniqueIdentifier identifier);

        #endregion Digital Inputs

        #region Relay Outputs
        
        IReadOnlyList<IDigitalOutput> RelayOutputs { get; }

        
        IDigitalOutput GetRelayOutput(IUniqueIdentifier identifier);

        #endregion Relay Outputs

        #region User LED's

        
        IReadOnlyList<IUserLed> UserLeds { get; }

        
        IUserLed GetUserLed(IUniqueIdentifier identifier);

        #endregion User LED's

        #region Analog Inputs
        
        IReadOnlyList<IAnalogInput> AnalogInputs { get; }

        
        IAnalogInput GetAnalogInput(IUniqueIdentifier identifier);
        #endregion Analog Inputs

        #region Analog Outputs

        
        IReadOnlyList<IAnalogOutput> AnalogOutputs { get; }

        
        IAnalogOutput GetAnalogOutput(IUniqueIdentifier identifier);

        #endregion Analog Outputs

        #region One Wire
        
        IReadOnlyList<IOneWireConnector> OneWireConnectors { get; }

        
        IOneWireConnector GetOneWireConnector(IUniqueIdentifier identifier);


        #endregion One Wire

        #region Modbus
        
        IReadOnlyList<IModbusConnector> ModbusConnectors { get; }

        
        IModbusConnector GetModbusConnector(IUniqueIdentifier identifier);

        #endregion Modbus


        event DigitalInputStateChanged OnDigitalInputStateChanged;
        event OutputStateChanged OnDigitalOutputStateChanged;
        event OutputStateChanged OnRelayOutputStateChanged;
        event AnalogInputChanged OnAnalogInputChanged;
        event AnalogOutputChanged OnAnalogOutputChanged;
        event UserLedStateChanged OnUserLedStateChanged;
    }
}