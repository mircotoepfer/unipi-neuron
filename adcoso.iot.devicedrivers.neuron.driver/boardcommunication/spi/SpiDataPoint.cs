using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace adcoso.iot.devicedrivers.neuron.driver.boardcommunication.spi
{
    internal class SpiDataPoint
    {
        
        internal static SpiDataPoint CreateDWordSpiDataPoint(ushort firstRegisterId, ushort secondRegisterId) => new SpiDataPoint(SpiDataPointType.DWord, new List<ushort> { firstRegisterId, secondRegisterId }, null);

        
        internal static SpiDataPoint CreateWordSpiDataPoint(ushort registerId) => new SpiDataPoint(SpiDataPointType.Word, new List<ushort> { registerId }, null);

        
        internal static SpiDataPoint CreateMixedBitsSpiDataPoint(ushort registerId, params ushort[] bits) => new SpiDataPoint(SpiDataPointType.Word, new List<ushort> { registerId }, new List<ushort>(bits));


        private SpiDataPoint(SpiDataPointType type,  IList<ushort> registers, IList<ushort> registerBits)
        {
            Type = type;
            Registers = new ReadOnlyCollection<ushort>(registers);
            RegisterBits = new ReadOnlyCollection<ushort>(registerBits);
        }

        
        internal IReadOnlyList<ushort> Registers { get; }

        internal SpiDataPointType Type { get; }


        
        internal IReadOnlyList<ushort> RegisterBits { get; }



    }

    internal enum SpiDataPointType
    {
        MixedBits,
        Word,
        DWord
    }
    internal enum SpiDatapointAccess
    {
        Read,
        Write,
        ReadWrite
    }
}
