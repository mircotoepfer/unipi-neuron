using adcoso.iot.devicedrivers.neuron.driver.onewire.interfaces;
using System.Collections.Generic;
using System.Linq;

namespace adcoso.iot.devicedrivers.neuron.driver.onewire
{
    public class OneWireDevice : IOneWireDevice
    {
        #region Private Members

        
        private IOneWireConnector _connector;
        
        private readonly object _controllerLock;
        #endregion Private Members

        #region Constructor
        internal OneWireDevice(IReadOnlyList<byte> romAddress, IOneWireConnector connector)
        {
            _connector = connector;
            _controllerLock = new object();

            RomNumber = romAddress.ToArray();

            SerialNumber = string.Empty;

            foreach (var byteValue in romAddress)
            {
                SerialNumber += byteValue.ToString("x2")?.ToUpper();
            }

            FamilyCode = romAddress[0];
        }
        #endregion Constructor

        #region Public Members

        public byte FamilyCode { get; }
        public string SerialNumber { get; }
        public byte[] RomNumber { get; }
        public bool DeviceIsDead
        {
            get
            {
                lock (_controllerLock)
                {
                    return _connector == null;
                }
            }
        }

        #endregion Public Members

        public void SendCommand(bool enableStrongPullUp, params byte[] commands)
        {
            SendCommand(enableStrongPullUp,0,commands);
        }

        public void SendCommand(bool enableStrongPullUp, int waitInMilliSecAfterCommand, params byte[] commands)
        {
            lock (_controllerLock)
            {
                _connector?.SendCommand(RomNumber, waitInMilliSecAfterCommand, enableStrongPullUp, commands);
            }
        }

        public byte[] ReadData(int byteCount, params byte[] readDataCommands)
        {
            lock (_controllerLock)
            {
                return _connector?.ReadData(RomNumber, byteCount, readDataCommands);
            }
        }

        public void MarkAsDead()
        {
            lock (_controllerLock)
            {
                _connector = null;
            }
        }


    }


}
