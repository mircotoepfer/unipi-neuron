using adcoso.iot.devicedrivers.neuron.driver.commons;
using System;
using System.Collections.Generic;

namespace adcoso.iot.devicedrivers.neuron.driver.board
{
    public class BoardInformation : IBoardInformation
    {
        internal BoardInformation(Version firmwareVersion, Version hardwareVersion, int digitalInputCount, int digitalOutputCount, int analogInputCount, int analogOutputCount, int userLedCount, int hwVersion, NeuronGroup neuronGroup)
        {
            NeuronGroup = neuronGroup;
            FirmwareVersion = firmwareVersion;
            HardwareVersion = hardwareVersion;
            DigitalInputCount = digitalInputCount;
            DigitalOutputCount = digitalOutputCount;
            AnalogInputCount = analogInputCount;
            AnalogOutputCount = analogOutputCount;
            UserLedCount = userLedCount;
            BoardType = GetBoardType(hwVersion);
        }

        public Version FirmwareVersion { get; }
        public Version HardwareVersion { get; }
        public int DigitalInputCount { get; }
        public int DigitalOutputCount { get; }
        public int AnalogInputCount { get; }
        public int AnalogOutputCount { get; }
        public int UserLedCount { get; }

        public string HardwareName => GetBoardName(BoardType);
        internal BoardType BoardType { get; }


        
        private static string GetBoardName(BoardType boardType)
        {

            switch (boardType)
            {
                case BoardType.UnknownBoard:
                    return "Unknown Board";
                case BoardType.B10001:
                    return "B-1000-1";
                case BoardType.E8Di8Ro1:
                    return "E-8Di8Ro-1";
                case BoardType.E14Ro1:
                    return "E-14Ro-1";
                case BoardType.E16Di1:
                    return "E-16Di-1";
                case BoardType.E8Di8Ro1P11DiMb485:
                    return "E-8Di8Ro-1_P-11DiMb485";
                case BoardType.E14Ro1P11DiR4851:
                    return "E-14Ro-1_P-11DiR485-1";
                case BoardType.E16Di1P11DiR4851:
                    return "E-16Di-1_P-11DiR485-1";
                case BoardType.E14Ro1U14Ro1:
                    return "E-14Ro-1_U-14Ro-1";
                case BoardType.E16Di1U14Ro1:
                    return "E-16Di-1_U-14Ro-1";
                case BoardType.E14Ro1U14Di1:
                    return "E-14Ro-1_U-14Di-1";
                case BoardType.E16Di1U14Di1:
                    return "E-16Di-1_U-14Di-1";
                default:
                    throw new ArgumentOutOfRangeException(nameof(boardType), boardType, null);
            }
        }

        private static BoardType GetBoardType(int hwVersionNumber)
        {
            var hwnames = new Dictionary<int, BoardType>
            {
                {0, BoardType.B10001},
                {1, BoardType.E8Di8Ro1},
                {2, BoardType.E14Ro1},
                {3, BoardType.E16Di1},
                {4, BoardType.E8Di8Ro1P11DiMb485},
                {5, BoardType.E14Ro1P11DiR4851},
                {6, BoardType.E16Di1P11DiR4851},
                {7, BoardType.E14Ro1U14Ro1},
                {8, BoardType.E16Di1U14Ro1},
                {9, BoardType.E14Ro1U14Di1},
                {10,BoardType.E16Di1U14Di1}
            };

            return hwnames.ContainsKey(hwVersionNumber) ? hwnames[hwVersionNumber] : BoardType.UnknownBoard;
        }

        public NeuronGroup NeuronGroup { get; }
        
    }
}
