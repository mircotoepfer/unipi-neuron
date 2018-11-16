namespace adcoso.iot.devicedrivers.neuron.driver.board
{
    internal interface IObservedRegisterObject
    {
        void SetRegisterValue(ushort registerNumber, ushort value);
    }
    internal interface IObservedCoilObject
    {
        void SetCoilValue(ushort coilNumber, bool value);
    }
}
