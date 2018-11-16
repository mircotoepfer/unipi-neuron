using adcoso.iot.devicedrivers.neuron.driver.analoginput;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace adcoso.iot.devicedrivers.neuron.application
{
    public sealed partial class AnalogInputControl
    {
        public AnalogInputControl(IAnalogInput analogInput)
        {
            ViewModel = new AnalogInputControlViewModel(analogInput, Dispatcher);
            InitializeComponent();
        }

        public AnalogInputControlViewModel ViewModel { get; set; }
    }
}
