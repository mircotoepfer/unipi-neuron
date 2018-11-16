using adcoso.iot.devicedrivers.neuron.driver.digitaloutputs;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace adcoso.iot.devicedrivers.neuron.application
{
    public sealed partial class DigitalOutputControl
    {


        public DigitalOutputControl(IDigitalOutput digitalOutput)
        {
            ViewModel = new DigitalOutputControlViewModel(digitalOutput);
            InitializeComponent();
        }

        public DigitalOutputControlViewModel ViewModel { get; set; }
    }
}
