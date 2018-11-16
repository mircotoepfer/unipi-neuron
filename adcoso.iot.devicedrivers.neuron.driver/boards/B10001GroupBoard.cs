using adcoso.iot.devicedrivers.neuron.driver.analoginput;
using adcoso.iot.devicedrivers.neuron.driver.analogoutput;
using adcoso.iot.devicedrivers.neuron.driver.board;
using adcoso.iot.devicedrivers.neuron.driver.boardcommunication.i2c;
using adcoso.iot.devicedrivers.neuron.driver.boardcommunication.spi;
using adcoso.iot.devicedrivers.neuron.driver.commons;
using adcoso.iot.devicedrivers.neuron.driver.digitalinputs;
using adcoso.iot.devicedrivers.neuron.driver.digitaloutputs;
using adcoso.iot.devicedrivers.neuron.driver.onewire;
using adcoso.iot.devicedrivers.neuron.driver.userled;

namespace adcoso.iot.devicedrivers.neuron.driver.boards
{
    internal class B10001GroupBoard : NeuronGroupBoardBase
    {

        #region Private Members
        private const ushort AnzahlDigitalInputs = 4;
        private const ushort AnzahlDigitalOutputs = 4;
        private const ushort UserLedsCount = 4;
        #endregion

        public B10001GroupBoard(NeuronGroup neuronGroup, IBoardInformation boardSystemInformation, NeuronSpiConnector spiConnector, I2CConnector i2CConnector, DriverLogger logger) : base(neuronGroup, boardSystemInformation, spiConnector, i2CConnector, logger)
        {

            #region Digital Inputs

            for (ushort i = 0; i < AnzahlDigitalInputs; i++)
            {
                var input = new DigitalInput(i + 1, neuronGroup, 0, i);
                SetObservation(0, input);
                DigitalInputDictionary.Add(input.UniqueIdentifyer, input);
            }

            #endregion Digital Inputs

            #region Digital Outputs

            for (ushort i = 0; i < AnzahlDigitalOutputs; i++)
            {
                var digitalOutput = new DigitalOutput(1 + i, neuronGroup, i, spiConnector, logger, DigitalRelayOutputType.DigitalOutput,1,i);
                DigitalOutputsDictionary.Add(digitalOutput.UniqueIdentifyer, digitalOutput);
            }

            #endregion Digital Outputs

            #region One Wire

            var oneWireConnector = new OneWireConnector(neuronGroup, 1, Logger, I2CConnector);
            OneWireConnectorsDictionary.Add(oneWireConnector.UniqueIdentifyer, oneWireConnector);

            #endregion One Wire

            #region User LED's

            for (ushort i = 0; i < UserLedsCount; i++)
            {
                var userLed = new UserLed(i + 1, neuronGroup, (ushort)(8 + i), spiConnector,20,i);
                UserLedsDictionary.Add(userLed.UniqueIdentifyer, userLed);
            }

            #endregion User LED's

            #region Analog Output

            var analogOutput = new AnalogOutput(neuronGroup, 1, 2, spiConnector, logger);
            AnalogOutputsDictionary.Add(analogOutput.UniqueIdentifyer, analogOutput);

            #endregion Analog Output

            #region Analog Input

            var analogInput = new AnalogInput(neuronGroup, 1, 3,spiConnector);
            SetObservation(3, analogInput);
            AnalogInputsDictionary.Add(analogInput.UniqueIdentifyer, analogInput);

            #endregion Analog Input

        }

    }
}
