namespace adcoso.iot.devicedrivers.neuron.driver.onewire.interfaces
{
    public interface IOneWireDevice
    {
        byte FamilyCode { get; }

        string SerialNumber { get; }

        byte[] RomNumber { get; }

        bool DeviceIsDead { get; }

        void SendCommand(bool enableStrongPullUp, int waitInMilliSecAfterCommand, params byte[] commands);

        void SendCommand(bool enableStrongPullUp, params byte[] commands);

        byte[] ReadData(int byteCount, params byte[] readDataCommands);

        void MarkAsDead();
    }

}
