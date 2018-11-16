using System;

namespace adcoso.iot.devicedrivers.neuron.driver.board
{
    public interface IBoardInformation
    {
        
        Version FirmwareVersion { get; }

        
        Version HardwareVersion { get; }

        int DigitalInputCount { get; }

        int DigitalOutputCount { get; }

        int AnalogInputCount { get; }

        int AnalogOutputCount { get; }

        int UserLedCount { get; }

        
        string HardwareName { get; }

        NeuronGroup NeuronGroup { get;  }
    }
}
