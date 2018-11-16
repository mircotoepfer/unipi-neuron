using adcoso.iot.devicedrivers.neuron.driver.board;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace adcoso.iot.devicedrivers.neuron.application
{
    public class HardwareInformationControlViewModel : INotifyPropertyChanged
    {
        private int _userLedCount;
        private Version _hardwareVersion;
        private string _hardwareName;
        private Version _firmwareVersion;
        private int _digitalOutputCount;
        private int _digitalInputCount;
        private int _analogOutputCount;
        private int _analogInputCount;
        private NeuronGroup _neuronGroup;

        public HardwareInformationControlViewModel(IBoardInformation boardInformation)
        {
            AnalogInputCount = boardInformation.AnalogInputCount;
            AnalogOutputCount = boardInformation.AnalogOutputCount;
            DigitalInputCount = boardInformation.DigitalInputCount;
            DigitalOutputCount = boardInformation.DigitalOutputCount;
            FirmwareVersion = boardInformation.FirmwareVersion;
            HardwareName = boardInformation.HardwareName;
            HardwareVersion = boardInformation.HardwareVersion;
            UserLedCount = boardInformation.UserLedCount;
            NeuronGroup = boardInformation.NeuronGroup;
        }

        public NeuronGroup NeuronGroup
        {
            get { return _neuronGroup; }
            set
            {
                if (value == _neuronGroup) return;
                _neuronGroup = value;
                OnPropertyChanged();
            }
        }

        public int UserLedCount
        {
            get { return _userLedCount; }
            set
            {
                if (value == _userLedCount) return;
                _userLedCount = value;
                OnPropertyChanged();
            }
        }

        public Version HardwareVersion
        {
            get { return _hardwareVersion; }
            set
            {
                if (Equals(value, _hardwareVersion)) return;
                _hardwareVersion = value;
                OnPropertyChanged();
            }
        }

        public string HardwareName
        {
            get { return _hardwareName; }
            set
            {
                if (value == _hardwareName) return;
                _hardwareName = value;
                OnPropertyChanged();
            }
        }

        public Version FirmwareVersion
        {
            get { return _firmwareVersion; }
            set
            {
                if (Equals(value, _firmwareVersion)) return;
                _firmwareVersion = value;
                OnPropertyChanged();
            }
        }

        public int DigitalOutputCount
        {
            get { return _digitalOutputCount; }
            set
            {
                if (value == _digitalOutputCount) return;
                _digitalOutputCount = value;
                OnPropertyChanged();
            }
        }

        public int DigitalInputCount
        {
            get { return _digitalInputCount; }
            set
            {
                if (value == _digitalInputCount) return;
                _digitalInputCount = value;
                OnPropertyChanged();
            }
        }

        public int AnalogOutputCount
        {
            get { return _analogOutputCount; }
            set
            {
                if (value == _analogOutputCount) return;
                _analogOutputCount = value;
                OnPropertyChanged();
            }
        }

        public int AnalogInputCount
        {
            get { return _analogInputCount; }
            set
            {
                if (value == _analogInputCount) return;
                _analogInputCount = value;
                OnPropertyChanged();
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



    }
}
