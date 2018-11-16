using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace adcoso.iot.devicedrivers.neuron.driver.boardcommunication.i2c
{

    internal delegate void GpioInterrupt();

    internal class InterruptConnector
    {

        public async Task Initialize()
        {
            var asyncOperation = GpioController.GetDefaultAsync();
            if (asyncOperation == null)
            {
                throw new Exception("GpioController.GetDefaultAsync Failed!");
            }

            var gpioController = await asyncOperation;

            if (gpioController == null)
            {
                throw new Exception("No GPIO Controller found!");
            }

            var gpios = new[] { 27, 23, 22 };

            foreach (var pin in gpios.Select(pinNumber => gpioController.OpenPin(pinNumber)).Where(pin => pin != null))
            {
                pin.SetDriveMode(GpioPinDriveMode.Input);
                pin.ValueChanged += InterruptCalled;
            }
        }

        internal event GpioInterrupt OnGpioInterrupt;

        private void InterruptCalled(GpioPin sender, GpioPinValueChangedEventArgs args) => OnGpioInterrupt?.Invoke();


    }


}
