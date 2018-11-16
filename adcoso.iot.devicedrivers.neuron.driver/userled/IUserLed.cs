using adcoso.iot.devicedrivers.neuron.driver.board;
using adcoso.iot.devicedrivers.neuron.driver.commons;

namespace adcoso.iot.devicedrivers.neuron.driver.userled
{

    public delegate void UserLedStateChanged(IUserLed userLed, OnOffValue value);


    public interface IUserLed : INeuronDataResource
    {
        void SetUserLed(OnOffValue value);
        OnOffValue GetLedStatus();

        event UserLedStateChanged OnUserLedStateChanged;
        void Toggle();
    }
}
