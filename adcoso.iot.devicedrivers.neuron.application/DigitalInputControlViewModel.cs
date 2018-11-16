using adcoso.iot.devicedrivers.neuron.driver.commons;
using adcoso.iot.devicedrivers.neuron.driver.digitalinputs;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml.Media;
using JetBrains.Annotations;

namespace adcoso.iot.devicedrivers.neuron.application
{
    public class DigitalInputControlViewModel : INotifyPropertyChanged
    {
        private SolidColorBrush _indicatorBackground;
        private string _name;
        private RelayCommand _onCommand;

        public DigitalInputControlViewModel(IDigitalInput digitalInput)
        {
            digitalInput.OnDigitalInputChanged += DigitalInput_OnDigitalInputChanged;
            Name = digitalInput.UniqueIdentifyer.IdentifyerString;
            IndicatorBackground = new SolidColorBrush(Colors.Transparent);

            digitalInput.RaiseAllObjectEvents();
        }


        private void DigitalInput_OnDigitalInputChanged(IDigitalInput digitalInput, driver.commons.OnOffValue value)
        {
            switch (value)
            {
                case OnOffValue.On:
                    IndicatorBackground.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => IndicatorBackground = new SolidColorBrush(Colors.Red));
                    break;
                case OnOffValue.Off:
                    IndicatorBackground.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => IndicatorBackground = new SolidColorBrush(Colors.Gray));
                    break;
                case OnOffValue.Unknown:
                    IndicatorBackground.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => IndicatorBackground = new SolidColorBrush(Colors.Transparent));
                    break;
                default:
                    IndicatorBackground.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => IndicatorBackground = new SolidColorBrush(Colors.Transparent));
                    break;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public SolidColorBrush IndicatorBackground
        {
            get { return _indicatorBackground; }
            set
            {
                if (Equals(value, _indicatorBackground)) return;
                _indicatorBackground = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                if (value == _name) return;
                _name = value;
                OnPropertyChanged();
            }
        }

    }
}
