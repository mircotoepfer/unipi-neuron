using adcoso.iot.devicedrivers.neuron.driver.commons;
using adcoso.iot.devicedrivers.neuron.driver.userled;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml.Media;
using JetBrains.Annotations;

namespace adcoso.iot.devicedrivers.neuron.application
{
    public class UserLedControlViewModel : INotifyPropertyChanged
    {
        private string _name;
        private RelayCommand _onCommand;
        private RelayCommand _offCommand;


        private SolidColorBrush _indicatocColor;
        private SolidColorBrush _onColor;
        private SolidColorBrush _offColor;
        private RelayCommand _blinkCommand;
        private IUserLed _userLed;
        private Timer _timer;

        public UserLedControlViewModel(IUserLed userLed)
        {
            userLed.OnUserLedStateChanged += UserLedOnOnUserLedStateChanged;
            _userLed = userLed;
            Name = userLed.UniqueIdentifyer.IdentifyerString;

            OnCommand = new RelayCommand(OnCommandMethod);
            OffCommand = new RelayCommand(OffCommandMethod);
            BlinkCommand = new RelayCommand(BlinkCommandMethod);

            _offColor = new SolidColorBrush(Colors.Gray);
            _onColor = new SolidColorBrush(Colors.Red);

            userLed.RaiseAllObjectEvents();
        }

        private void BlinkCommandMethod()
        {
            if (_timer != null)
            {
                return;
            }


            _timer = new Timer(state => _userLed.Toggle(), new object(), 0, 500);
        }

        private void OffCommandMethod()
        {
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }

            _userLed.SetUserLed(OnOffValue.Off);
        }

        private void OnCommandMethod()
        {
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }

            _userLed.SetUserLed(OnOffValue.On);
        }

        private void UserLedOnOnUserLedStateChanged(IUserLed userLed, OnOffValue value)
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

        public RelayCommand BlinkCommand
        {
            get { return _blinkCommand; }
            set
            {
                if (Equals(value, _blinkCommand)) return;
                _blinkCommand = value;
                OnPropertyChanged();
            }
        }
    }
}
