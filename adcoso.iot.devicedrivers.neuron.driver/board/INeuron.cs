using adcoso.iot.devicedrivers.neuron.driver.analoginput;
using adcoso.iot.devicedrivers.neuron.driver.analogoutput;
using adcoso.iot.devicedrivers.neuron.driver.commons;
using adcoso.iot.devicedrivers.neuron.driver.digitalinputs;
using adcoso.iot.devicedrivers.neuron.driver.digitaloutputs;
using adcoso.iot.devicedrivers.neuron.driver.onewire.interfaces;
using adcoso.iot.devicedrivers.neuron.driver.rs485modbus;
using adcoso.iot.devicedrivers.neuron.driver.userled;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace adcoso.iot.devicedrivers.neuron.driver.board
{

    public delegate void InitializationFinish();

    public interface INeuron : INeuronDataResources
    {

        
        IUserLed GetUserLed(string identifyer);


        
        IDigitalOutput GetRelayOutput(string identifyer);

        
        IDigitalInput GetDigitalInput(string identifyer);


        
        IAnalogInput GetAnalogInput(string identifyer);

        
        IAnalogOutput GetAnalogOutput(string identifyer);

        
        IOneWireConnector GetOneWireConnector(string identifyer);

        
        IModbusConnector GetModbusConnector(string identifyer);

        void SetLogLevel(LogLevel logLevel);

        
        IReadOnlyList<IBoardInformation> BoardInformations { get; }

        event LogInformation OnLogInformation;
        event InitializationFinish OnInitializationFinish;

        Task Initialize();
    }
}
