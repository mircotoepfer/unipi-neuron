namespace adcoso.iot.devicedrivers.neuron.driver.board
{
    internal interface IHasBoardInformation
    {
        IBoardInformation BoardSystemInformation { get; }
    }
}