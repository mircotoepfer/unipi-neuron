using adcoso.iot.devicedrivers.neuron.driver.digitalinputs;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace adcoso.iot.devicedrivers.neuron.application
{
    public sealed partial class DigitalInputControl
    {


        public DigitalInputControl(IDigitalInput digitalInput)
        {
            ViewModel = new DigitalInputControlViewModel(digitalInput);
            InitializeComponent();
        }

        public DigitalInputControlViewModel ViewModel { get; set; }
    }
}
