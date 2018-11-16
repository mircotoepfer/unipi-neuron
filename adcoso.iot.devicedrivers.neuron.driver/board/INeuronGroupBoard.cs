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

        
        IDigitalOutput GetDigitalOutput(IUniqueIdentifyer identifyer);

        
        IDigitalInput GetDigitalInput(IUniqueIdentifyer identifyer);

        
        IDigitalOutput GetRelayOutput(IUniqueIdentifyer identifyer);

        
        IUserLed GetUserLed(IUniqueIdentifyer identifyer);

        
        IAnalogInput GetAnalogInput(IUniqueIdentifyer identifyer);

        
        IAnalogOutput GetAnalogOutput(IUniqueIdentifyer identifyer);

        
        IOneWireConnector GetOneWireConnector(IUniqueIdentifyer identifyer);

        
        IModbusConnector GetModbusConnector(IUniqueIdentifyer identifyer);

        void RaiseAllObjectEvents();
    }
}