using adcoso.iot.devicedrivers.neuron.driver.board;
using adcoso.iot.devicedrivers.neuron.driver.boardcommunication.i2c;
using adcoso.iot.devicedrivers.neuron.driver.boardcommunication.spi;
using adcoso.iot.devicedrivers.neuron.driver.commons;
using adcoso.iot.devicedrivers.neuron.driver.digitalinputs;
using adcoso.iot.devicedrivers.neuron.driver.digitaloutputs;

namespace adcoso.iot.devicedrivers.neuron.driver.boards
{
    internal class UniversalNeuronGroupDiRoBoard : NeuronGroupBoardBase
    {
        public UniversalNeuronGroupDiRoBoard(int digitalInCount, int relayOutCount, NeuronGroup neuronGroup, IBoardInformation boardSystemInformation, NeuronSpiConnector spiConnector, I2CConnector i2CConnector, DriverLogger logger) : base(neuronGroup, boardSystemInformation, spiConnector, i2CConnector, logger)
        {
            #region Digital Inputs

            for (ushort i = 0; i < digitalInCount; i++)
            {
                var input = new DigitalInput(i + 1, neuronGroup, 0, i);
                SetObservation(0, input);
                DigitalInputDictionary.Add(input.UniqueIdentifyer, input);
            }

            #endregion Digital Inputs

            #region Relay Outputs

            for (ushort i = 0; i < relayOutCount; i++)
            {
                var digitalOutput = new DigitalOutput(1 + i, neuronGroup, i, spiConnector, logger, DigitalRelayOutputType.RelayOutput, 1, i);
                DigitalOutputsDictionary.Add(digitalOutput.UniqueIdentifyer, digitalOutput);
            }

            #endregion Relay Outputs



        }


    }
}