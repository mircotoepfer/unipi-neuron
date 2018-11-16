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
    internal interface INeuronGroupBoard
    {
        IBoardInformation BoardSystemInformation { get; }

        
        IReadOnlyList<IDigitalOutput> DigitalOutputs { get; }

        
        IReadOnlyList<IDigitalInput> DigitalInputs { get; }

        
        IReadOnlyList<IDigitalOutput> RelayOutputs { get; }

        
        IReadOnlyList<IUserLed> UserLeds { get; }

        
        IReadOnlyList<IAnalogInput> AnalogInputs { get; }

        
        IReadOnlyList<IAnalogOutput> AnalogOutputs { get; }

        
        IReadOnlyList<IOneWireConnector> OneWireConnectors { get; }

        
        IReadOnlyList<IModbusConnector> ModbusConnectors { get; }

        event DigitalInputStateChanged OnDigitalInputStateChanged;
        event OutputStateChanged OnDigitalOutputStateChanged;
        event OutputStateChanged OnRelayOutputStateChanged;
        event AnalogInputChanged OnAnalogInputChanged;
        event AnalogOutputChanged OnAnalogOutputChanged;
        event UserLedStateChanged OnUserLedStateChanged;

        
        IDigitalOutput GetDigitalOutput(IUniqueIdentifier identifier);

        
        IDigitalInput GetDigitalInput(IUniqueIdentifier identifier);

        
        IDigitalOutput GetRelayOutput(IUniqueIdentifier identifier);

        
        IUserLed GetUserLed(IUniqueIdentifier identifier);

        
        IAnalogInput GetAnalogInput(IUniqueIdentifier identifier);

        
        IAnalogOutput GetAnalogOutput(IUniqueIdentifier identifier);

        
        IOneWireConnector GetOneWireConnector(IUniqueIdentifier identifier);

        
        IModbusConnector GetModbusConnector(IUniqueIdentifier identifier);

        void RaiseAllObjectEvents();
    }
}