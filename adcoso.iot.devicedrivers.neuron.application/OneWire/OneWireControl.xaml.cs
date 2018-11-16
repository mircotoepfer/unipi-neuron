using adcoso.iot.devicedrivers.neuron.application.OneWire;
using adcoso.iot.devicedrivers.neuron.driver.onewire.interfaces;
using System;
using System.Globalization;
using System.Linq;
using Windows.UI.Core;
using Windows.UI.Xaml;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace adcoso.iot.devicedrivers.neuron.application
{
    public sealed partial class OneWireControl
    {
        private IOneWireConnector _oneWireConnector;

        public OneWireControl(IOneWireConnector oneWireConnector)
        {
            _oneWireConnector = oneWireConnector;
            InitializeComponent();

            _oneWireConnector.OnNewOnWireDevice += _oneWireConnector_OnNewOnWireDevice;
            _oneWireConnector.OnOneWireDeviceIsDead += _oneWireConnector_OnOneWireDeviceIsDead;

            _oneWireConnector.RaiseAllObjectEvents();
        }

        private async void _oneWireConnector_OnOneWireDeviceIsDead(IOneWireDevice device)
        {
            await OneWirePanel.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var uiElements = (from uiElement in OneWirePanel.Children let oneWireCtrl = uiElement as OneWireDeviceControl let controlDevice = oneWireCtrl?.Tag as IOneWireDevice where controlDevice != null where device.SerialNumber == controlDevice.SerialNumber select uiElement).ToList();

                foreach (var uiElement in uiElements)
                {
                    OneWirePanel.Children.Remove(uiElement);
                }
            });
        }

        private async void _oneWireConnector_OnNewOnWireDevice(IOneWireDevice device)
        {
            await OneWirePanel.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (device.FamilyCode == 40)
                {
                    var device40 = new Family40Device(device, TimeSpan.FromSeconds(10));


                    var control = new OneWireDeviceControl(device40.SerialNumber, device40.FamilyCode)
                    {
                        Margin = new Thickness(10)
                    };


                    device40.OnNewValue += value =>
                    {
                        if (value == null)
                        {
                            control.SetValue("----");
                        }
                        else
                        {
                            control.SetValue(value.Value.ToString(CultureInfo.InvariantCulture) + " °C");
                        }
                    };


                    control.Tag = device;

                    OneWirePanel.Children.Add(control);
                }
                //else if (device.FamilyCode == 38)
                //{
                //    var device38 = new Family38Device(device, TimeSpan.FromSeconds(10));


                //    var control = new OneWireDeviceControl(device38.SerialNumber, device38.FamilyCode)
                //    {
                //        Margin = new Thickness(10)
                //    };


                //    device38.OnNewValue += value =>
                //    {
                //        if (value == null)
                //        {
                //            control.SetValue("----");
                //        }
                //        else
                //        {
                //            control.SetValue(value.Value.ToString(CultureInfo.InvariantCulture) + "");
                //        }
                //    };


                //    control.Tag = device;

                //    OneWirePanel.Children.Add(control);
                //}
                else
                {
                    var control = new OneWireDeviceControl(device.SerialNumber, device.FamilyCode)
                    {
                        Tag = device,
                        Margin = new Thickness(10)
                    };
                    OneWirePanel.Children.Add(control);
                }
            });
        }
    }
}
