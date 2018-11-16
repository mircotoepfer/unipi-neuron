using adcoso.iot.devicedrivers.neuron.driver.analoginput;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Core;
using JetBrains.Annotations;

namespace adcoso.iot.devicedrivers.neuron.application
{
    public class AnalogInputControlViewModel : INotifyPropertyChanged
    {
        private readonly CoreDispatcher _dispatcher;
        private string _name;
        private double _voltage;
        private double _percent;
        private double _opacity;
        private double _milliAmpereValue;

        public AnalogInputControlViewModel(IAnalogInput analogInput, CoreDispatcher dispatcher)
        {
            _dispatcher = dispatcher;

            analogInput.OnAnalogInputChanged += AnalogInputOnOnAnalogInputChanged;

            Name = analogInput.UniqueIdentifyer.IdentifierString;
            analogInput.RaiseAllObjectEvents();
        }

        private void AnalogInputOnOnAnalogInputChanged(IAnalogInput input, double percentageValue, double voltageValue, double miliAmpereValue)
        {
            Percent = percentageValue;
            Voltage = voltageValue;
            MilliAmpereValue = miliAmpereValue;
            Opacity = 1- (( percentageValue -100.0) / 100 *-1);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged == null)
            {
                return;
            }

            _dispatcher?.RunAsync(CoreDispatcherPriority.Normal, () => PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName)));
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

        public double Voltage
        {
            get { return _voltage; }
            set
            {
                if (value.Equals(_voltage)) return;
                _voltage = value;
                OnPropertyChanged();
            }
        }

        public double MilliAmpereValue
        {
            get { return _milliAmpereValue; }
            set
            {
                if (value.Equals(_milliAmpereValue)) return;
                _milliAmpereValue = value;
                OnPropertyChanged();
            }
        }

        public double Percent
        {
            get { return _percent; }
            set
            {
                if (value.Equals(_percent)) return;
                _percent = value;
                OnPropertyChanged();
            }
        }

        public double Opacity
        {
            get { return _opacity; }
            set
            {
                if (value.Equals(_opacity)) return;
                _opacity = value;
                OnPropertyChanged();
            }
        }
    }
}
