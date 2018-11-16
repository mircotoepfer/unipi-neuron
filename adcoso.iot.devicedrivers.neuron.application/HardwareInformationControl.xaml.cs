using adcoso.iot.devicedrivers.neuron.driver.board;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace adcoso.iot.devicedrivers.neuron.application
{
    public sealed partial class HardwareInformationControl
    {


        public HardwareInformationControl(IBoardInformation boardInformation)
        {
            ViewModel = new HardwareInformationControlViewModel(boardInformation);
            InitializeComponent();
        }

        public HardwareInformationControlViewModel ViewModel { get; set; }
    }
}
