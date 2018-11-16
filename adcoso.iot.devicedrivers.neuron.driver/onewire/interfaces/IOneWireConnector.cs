using adcoso.iot.devicedrivers.neuron.driver.board;
using System.Collections.Generic;

namespace adcoso.iot.devicedrivers.neuron.driver.onewire.interfaces
{

    /// <summary>
    /// Occures if a new device is found on the one wire bus
    /// </summary>
    /// <param name="device">The device.<see cref="IOneWireDevice"/></param>
    public delegate void NewOneWireDevice(IOneWireDevice device);

    /// <summary>
    /// Occures if an device is not more available
    /// </summary>
    /// <param name="device">The device.<see cref="IOneWireDevice"/></param>
    public delegate void OneWireDeviceIsDead(IOneWireDevice device);

    /// <summary>
    /// A controller to handle the one wire connection
    /// </summary>
    public interface IOneWireConnector : INeuronDataResource
    {


        int PeriodicBusScanIntervalSeconds { get; set; }

        /// <summary>
        /// Gets the devices.
        /// </summary>
        /// <value>
        /// The devices.
        /// </value>
        List<IOneWireDevice> Devices { get; }

        bool OneWirePeriodicBusScan { get; set; }

        /// <summary>
        /// Occurs when [on new on wire device].
        /// </summary>
        event NewOneWireDevice OnNewOnWireDevice;

        /// <summary>
        /// Occurs when [on one wire device is dead].
        /// </summary>
        event OneWireDeviceIsDead OnOneWireDeviceIsDead;

        /// <summary>
        /// Sends the command.
        /// </summary>
        /// <param name="romNumber">The rom number.<see cref="byte"/></param>
        /// <param name="waitwaitInMilliSecAfterCommand"></param>
        /// <param name="commands">The commands.<see cref="byte"/></param>
        /// <param name="enableStrongPullUp">if set to <c>true</c> [enable strong pull up].</param>
        /// <param name="waitAfterCommand">if set to <c>true</c> [wait after command].</param>
        void SendCommand(byte[] romNumber, int waitwaitInMilliSecAfterCommand , bool enableStrongPullUp = false, params byte[] commands);

        void SendCommand(byte[] romNumber, bool enableStrongPullUp = false, params byte[] commands);

        /// <summary>
        /// Reads the data.
        /// </summary>
        /// <param name="romNumber">The rom number.<see cref="byte"/></param>
        /// <param name="commands">The commands.<see cref="byte"/></param>
        /// <param name="byteLength">Length of the byte.</param>
        /// <returns></returns>
        byte[] ReadData(byte[] romNumber, int byteLength, params byte[] commands);

        /// <summary>
        /// Updates the one wire bus devices.
        /// </summary>
        void ScanForNewOrDeadDevices();

    }
}
