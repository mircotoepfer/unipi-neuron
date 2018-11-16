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

        
        IDigitalOutput GetDigitalOutput(IUniqueIdentifyer identifyer);

        #endregion Digital Outputs

        #region Digital Inputs

        
        IReadOnlyList<IDigitalInput> DigitalInputs { get; }

        
        IDigitalInput GetDigitalInput(IUniqueIdentifyer identifyer);

        #endregion Digital Inputs

        #region Relay Outputs
        
        IReadOnlyList<IDigitalOutput> RelayOutputs { get; }

        
        IDigitalOutput GetRelayOutput(IUniqueIdentifyer identifyer);

        #endregion Relay Outputs

        #region User LED's

        
        IReadOnlyList<IUserLed> UserLeds { get; }

        
        IUserLed GetUserLed(IUniqueIdentifyer identifyer);

        #endregion User LED's

        #region Analog Inputs
        
        IReadOnlyList<IAnalogInput> AnalogInputs { get; }

        
        IAnalogInput GetAnalogInput(IUniqueIdentifyer identifyer);
        #endregion Analog Inputs

        #region Analog Outputs

        
        IReadOnlyList<IAnalogOutput> AnalogOutputs { get; }

        
        IAnalogOutput GetAnalogOutput(IUniqueIdentifyer identifyer);

        #endregion Analog Outputs

        #region One Wire
        
        IReadOnlyList<IOneWireConnector> OneWireConnectors { get; }

        
        IOneWireConnector GetOneWireConnector(IUniqueIdentifyer identifyer);


        #endregion One Wire

        #region Modbus
        
        IReadOnlyList<IModbusConnector> ModbusConnectors { get; }

        
        IModbusConnector GetModbusConnector(IUniqueIdentifyer identifyer);

        #endregion Modbus


        event DigitalInputStateChanged OnDigitalInputStateChanged;
        event OutputStateChanged OnDigitalOutputStateChanged;
        event OutputStateChanged OnRelayOutputStateChanged;
        event AnalogInputChanged OnAnalogInputChanged;
        event AnalogOutputChanged OnAnalogOutputChanged;
        event UserLedStateChanged OnUserLedStateChanged;
    }
}