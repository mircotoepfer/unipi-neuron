using adcoso.iot.devicedrivers.neuron.driver.userled;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace adcoso.iot.devicedrivers.neuron.application
{
    public sealed partial class UserLedControl
    {


        public UserLedControl(IUserLed userLed)
        {
            ViewModel = new UserLedControlViewModel(userLed);
            InitializeComponent();
        }

        public UserLedControlViewModel ViewModel { get; set; }
    }
}
