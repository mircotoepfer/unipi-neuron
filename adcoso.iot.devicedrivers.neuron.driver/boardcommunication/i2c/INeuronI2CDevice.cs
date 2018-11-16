using System.Threading.Tasks;
using Windows.Devices.I2c;

namespace adcoso.iot.devicedrivers.neuron.driver.boardcommunication.i2c
{
    internal interface INeuronI2CDevice
    {
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        void Dispose();

        /// <summary>
        /// Writes the specified i2 c write buffer.
        /// </summary>
        /// <param name="i2CWriteBuffer">The i2 c write buffer.<see cref="byte"/></param>
        void Write(byte[] i2CWriteBuffer);

        /// <summary>
        /// Writes the read.
        /// </summary>
        /// <param name="i2CWriteBuffer">The i2 c write buffer.<see cref="byte"/></param>
        /// <param name="i2CReadBuffer">The i2 c read buffer.<see cref="byte"/></param>
        /// <returns></returns>
        byte WriteRead(byte[] i2CWriteBuffer, byte[] i2CReadBuffer);

        /// <summary>
        /// Initializes the specified i2 c device address.
        /// </summary>
        /// <param name="i2CDeviceAddress">The i2 c device address.<see cref="byte"/></param>
        /// <param name="i2CControllerName">Name of the i2 c controller.</param>
        /// <param name="busSpeed">The bus speed.<see cref="I2cBusSpeed"/></param>
        /// <returns></returns>
        Task Initialize(byte i2CDeviceAddress, string i2CControllerName, I2cBusSpeed busSpeed);
    }
}