using adcoso.iot.devicedrivers.neuron.driver.boardcommunication.i2c;
using adcoso.iot.devicedrivers.neuron.driver.boardcommunication.spi;
using adcoso.iot.devicedrivers.neuron.driver.boards;
using adcoso.iot.devicedrivers.neuron.driver.commons;
using System;

namespace adcoso.iot.devicedrivers.neuron.driver.board
{
    internal class GeneralFactory
    {
        
        private readonly I2CConnector _i2CConnector;
        
        private readonly NeuronSpiConnector _spiConnector;
        
        private readonly DriverLogger _logger;

        public GeneralFactory(I2CConnector i2CConnector, NeuronSpiConnector spiConnector, DriverLogger logger)
        {
            _i2CConnector = i2CConnector;
            _spiConnector = spiConnector;
            _logger = logger;
        }


        
        internal static BoardInformation GetBoardInfoFromRegisters(SpiRegisterSet registerSet, DriverLogger logger, NeuronGroup neuronGroup)
        {
            try
            {
                if (registerSet == null || registerSet.Count != 5)
                {
                    return null;
                }


                if (!registerSet.ContainsRegister(1000) ||
                    !registerSet.ContainsRegister(1001) ||
                    !registerSet.ContainsRegister(1002) ||
                    !registerSet.ContainsRegister(1003) ||
                    !registerSet.ContainsRegister(1004))
                {
                    logger.LogError(null, "i got the wrong registerSet to create the system information!");
                    return null;
                }

                var configRegisters = registerSet.ToRegisterValueArray();

                int swSubver;
                int hwVersion;
                int hwSubver;

                var swVersion = configRegisters[0] >> 8;
                var diCount = configRegisters[1] >> 8;
                var doCount = configRegisters[1] & 0xff;
                var aiCount = configRegisters[2] >> 8;
                var aoCount = (configRegisters[2] & 0xff) >> 4;

                var uledCount = 0;

                if (swVersion < 4)
                {
                    swSubver = 0;
                    hwVersion = (configRegisters[0] & 0xff) >> 4;
                    hwSubver = configRegisters[0] & 0xf;
                }
                else
                {
                    swSubver = configRegisters[0] & 0xff;
                    hwVersion = configRegisters[3] >> 8;
                    hwSubver = configRegisters[3] & 0xff;

                    if (hwSubver < 3)
                    {
                    }
                    if (hwVersion == 0)
                    {
                        if (configRegisters[0] != 0x0400)
                        {
                            uledCount = 4;
                        }
                    }
                }


                var firmwareVersion = new Version(swVersion, swSubver);
                var digitalInputCount = diCount;
                var digitalOutputCount = doCount;
                var analogInputCount = aiCount;
                var analogOutputCount = aoCount;
                var userLedCount = uledCount;
                var hardwareVersion = new Version(hwVersion, hwSubver);


                return new BoardInformation(firmwareVersion, hardwareVersion, digitalInputCount, digitalOutputCount, analogInputCount, analogOutputCount, userLedCount, hwVersion, neuronGroup);
            }
            catch (Exception e)
            {
                logger.LogException(null, e);
                return null;
            }
        }



        
        internal INeuronGroupBoard CreateNeuronBoard(NeuronGroup neuronGroup)
        {
            try
            {

                var boardInformationRegisterSet = _spiConnector.GetRegisterValues(neuronGroup, 1000, 5);
                var boardInformation = GetBoardInfoFromRegisters(boardInformationRegisterSet, _logger, neuronGroup);

                if (boardInformation != null)
                    switch (boardInformation.BoardType)
                    {
                        case BoardType.UnknownBoard:
                            _logger.LogInformation(this, "Board " + boardInformation.BoardType + " not supported!");
                            break;
                        case BoardType.B10001:
                            return new B10001GroupBoard(neuronGroup, boardInformation, _spiConnector, _i2CConnector, _logger);
                        case BoardType.E8Di8Ro1:
                            return new UniversalNeuronGroupDiRoBoard(8, 8, neuronGroup, boardInformation, _spiConnector, _i2CConnector, _logger);
                        case BoardType.E14Ro1:
                            return new UniversalNeuronGroupDiRoBoard(0, 16, neuronGroup, boardInformation, _spiConnector, _i2CConnector, _logger);
                        case BoardType.E16Di1:
                            return new UniversalNeuronGroupDiRoBoard(16, 0, neuronGroup, boardInformation, _spiConnector, _i2CConnector, _logger);
                        case BoardType.E8Di8Ro1P11DiMb485:
                            _logger.LogInformation(this, "Board " + boardInformation.BoardType + " not supported!");
                            break;
                        case BoardType.E14Ro1P11DiR4851:
                            _logger.LogInformation(this, "Board " + boardInformation.BoardType + " not supported!");
                            break;
                        case BoardType.E16Di1P11DiR4851:
                            _logger.LogInformation(this, "Board " + boardInformation.BoardType + " not supported!");
                            break;
                        case BoardType.E14Ro1U14Ro1:
                            _logger.LogInformation(this, "Board " + boardInformation.BoardType + " not supported!");
                            break;
                        case BoardType.E16Di1U14Ro1:
                            return new UniversalNeuronGroupDiRoBoard(16, 14, neuronGroup, boardInformation, _spiConnector, _i2CConnector, _logger);
                        case BoardType.E14Ro1U14Di1:
                            return new UniversalNeuronGroupDiRoBoard(14, 14, neuronGroup, boardInformation, _spiConnector, _i2CConnector, _logger);
                        case BoardType.E16Di1U14Di1:
                            _logger.LogInformation(this, "Board " + boardInformation.BoardType + " not supported!");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
            }
            catch (Exception e)
            {
                _logger.LogException(this, e);
                return null;
            }

            return null;

        }
    }
}
