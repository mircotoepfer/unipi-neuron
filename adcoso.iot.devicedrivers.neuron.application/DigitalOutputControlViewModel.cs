using adcoso.iot.devicedrivers.neuron.driver.commons;
using adcoso.iot.devicedrivers.neuron.driver.digitaloutputs;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml.Media;
using JetBrains.Annotations;

namespace adcoso.iot.devicedrivers.neuron.application
{
    public class DigitalOutputControlViewModel : INotifyPropertyChanged
    {
        private string _name;
        private RelayCommand _onCommand;
        private RelayCommand _offCommand;
        private SolidColorBrush _indicatocColor;
        private SolidColorBrush _onColor;
        private SolidColorBrush _offColor;
        private RelayCommand _toggleCommand;

        public DigitalOutputControlViewModel(IDigitalOutput digitalOutput)
        {
            digitalOutput.OnOutputStateChanged += DigitalInput_OnDigitalOutputChanged;
            Name = digitalOutput.UniqueIdentifyer.IdentifierString;

            OnCommand = new RelayCommand(() => digitalOutput.SetOutputValue(OnOffValue.On));
            OffCommand = new RelayCommand(() => digitalOutput.SetOutputValue(OnOffValue.Off));
            ToggleCommand = new RelayCommand(digitalOutput.ToggleOutput);

            _offColor = new SolidColorBrush(Colors.Gray);
            _onColor = new SolidColorBrush(Colors.Red);

            digitalOutput.RaiseAllObjectEvents();
        }

        private void DigitalInput_OnDigitalOutputChanged(IDigitalOutput digitaloutput, OnOffValue value)
        {
            if (value == OnOffValue.On)
            {
                IndicatocColor = _onColor;
            }
            else
            {
                IndicatocColor = _offColor;
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public SolidColorBrush IndicatocColor
        {
            get { return _indicatocColor; }
            set
            {
                if (Equals(value, _indicatocColor)) return;
                _indicatocColor = value;
                value.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => OnPropertyChanged());
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

        public RelayCommand OnCommand
        {
            get { return _onCommand; }
            set
            {
                if (Equals(value, _onCommand)) return;
                _onCommand = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand OffCommand
        {
            get { return _offCommand; }
            set
            {
                if (Equals(value, _offCommand)) return;
                _offCommand = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand ToggleCommand
        {
            get { return _toggleCommand; }
            set
            {
                if (Equals(value, _toggleCommand)) return;
                _toggleCommand = value;
                OnPropertyChanged();
            }
        }
    }
}
