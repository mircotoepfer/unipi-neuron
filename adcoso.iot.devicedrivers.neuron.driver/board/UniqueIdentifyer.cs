using System;

namespace adcoso.iot.devicedrivers.neuron.driver.board
{
    public class UniqueIdentifyer : IUniqueIdentifyer
    {
        #region Constructor
        internal UniqueIdentifyer(NeuronGroup group, NeuronResource resource, int number)
        {
            Resource = resource;
            Group = group;
            Number = number;
        }
        #endregion Constructor

        #region Public Members
        public NeuronResource Resource { get; }
        public NeuronGroup Group { get; }
        public int Number { get; }
        public string IdentifyerString
        {
            get
            {
                if (Group != NeuronGroup.One)
                {
                    return GetResourceString() + GetGroupString() + "." + Number;
                }

                return GetResourceString() + Number;
            }
        }

        #endregion Public Members

        #region Public Methods
        
        public override string ToString() => IdentifyerString;
        #endregion Public Methods

        #region Private Methods
        
        private string GetResourceString()
        {
            switch (Resource)
            {
                case NeuronResource.DigitalInput:
                    return "DI";
                case NeuronResource.DigitalOutput:
                    return "DO";
                case NeuronResource.RelayOutput:
                    return "RO";
                case NeuronResource.UserLed:
                    return "X";
                case NeuronResource.AnalogInput:
                    return "AI";
                case NeuronResource.AnalogOutput:
                    return "AO";
                case NeuronResource.OneWireConnector:
                    return "1-Wire";
                case NeuronResource.ModbusConnector:
                    return "RS485-END";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        
        private string GetGroupString()
        {
            switch (Group)
            {
                case NeuronGroup.One:
                    return "1";
                case NeuronGroup.Two:
                    return "2";
                case NeuronGroup.Three:
                    return "3";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        #endregion Private Methods
    }
}
