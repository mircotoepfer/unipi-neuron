using adcoso.iot.devicedrivers.neuron.driver.analogoutput;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace adcoso.iot.devicedrivers.neuron.application
{
    public sealed partial class AnalogOutputControl
    {


        public AnalogOutputControl(IAnalogOutput analogOutput)
        {
            ViewModel = new AnalogOutputControlViewModel(analogOutput, Dispatcher);
            InitializeComponent();
        }

        public AnalogOutputControlViewModel ViewModel { get; set; }
    }
}
