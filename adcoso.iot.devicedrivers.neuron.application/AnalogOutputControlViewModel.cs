using adcoso.iot.devicedrivers.neuron.driver.analogoutput;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Core;
using JetBrains.Annotations;

namespace adcoso.iot.devicedrivers.neuron.application
{
    public class AnalogOutputControlViewModel : INotifyPropertyChanged
    {
        private string _name;
        private double _voltage;
        private IAnalogOutput _analogOutput;
        private readonly CoreDispatcher _dispatcher;
        private double _percent;
        private double _tickFrequency;


        public AnalogOutputControlViewModel(IAnalogOutput analogOutput, CoreDispatcher dispatcher)
        {
            TickFrequency = 0.01;

            Name = analogOutput.UniqueIdentifyer.IdentifierString;
            _analogOutput = analogOutput;
            _dispatcher = dispatcher;
            _analogOutput.OnAnalogoutputChanged += _analogOutput_OnAnalogoutputChanged;

            analogOutput.RaiseAllObjectEvents();
        }

        private void _analogOutput_OnAnalogoutputChanged(IAnalogOutput output, double percentageValue, double voltageValue, double milliampereValue)
        {
            Voltage = voltageValue;
            Percent = percentageValue;
        }

        public event PropertyChangedEventHandler PropertyChanged;


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

        public double Percent
        {
            get { return _percent; }
            set
            {
                if (value.Equals(_percent)) return;
                _percent = value;
                OnPropertyChanged();

                _analogOutput.SetPercentValue(value);
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

        public double TickFrequency
        {
            get { return _tickFrequency; }
            set
            {
                if (value.Equals(_tickFrequency)) return;
                _tickFrequency = value;
                OnPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            _dispatcher?.RunAsync(CoreDispatcherPriority.Normal,
                () => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)));
        }



    }
}
