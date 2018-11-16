using System;
using Windows.UI.Core;

namespace adcoso.iot.devicedrivers.neuron.application
{
    public sealed partial class OneWireDeviceControl
    {


        public OneWireDeviceControl(string id, byte familycode)
        {
            InitializeComponent();
            TextBoxFamilyCode.Text = familycode.ToString("x2");
            TextBoxId.Text = id;
        }

        public async void SetValue(string value)
        {
            await TextBoxValue.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                TextBoxValue.Text = value;
            });
        }
    }
}
