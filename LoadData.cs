using System;

namespace Array371A
{
    public class LoadData
    {
        //measurments
        public decimal current;
        public decimal voltage;
        public decimal power;
        public decimal resistance;
        //settings
        public decimal MaxPower;
        public decimal MaxCurrent;
        public byte RegMode;
        public decimal SetPoint;
        public bool LoadStat;
        public bool PCControl;
        //status
        public bool WrongPolarity;
        public bool OverTemp;
        public bool OverVoltage;
        public bool OverPower;

        //raw data
        byte generalStatus;             //made available incase raw byte value is value is more usful than individual flags
        public byte[] DataArray;    //made available incase raw array of values is more useful also contains measuresurments

        public LoadData(LoadControlFrame inputFrame)
        {
            DataArray = inputFrame.GetPayload();
            //measurments
            current = (decimal)(BitConverter.ToUInt16(DataArray, 0)) / 1000;
            voltage = (decimal)(BitConverter.ToUInt32(DataArray, 2)) / 1000;
            power = (decimal)(BitConverter.ToUInt16(DataArray, 6)) / 10;
            resistance = (decimal)(BitConverter.ToUInt16(DataArray, 12)) / 100;
            //settings
            MaxPower = (decimal)(BitConverter.ToUInt16(DataArray, 10)) / 10; ;
            MaxCurrent = (decimal)(BitConverter.ToUInt16(DataArray, 8)) / 1000;
            RegMode = DataArray[15];
            generalStatus = DataArray[14];
            //split the general status byte to individual flags
            PCControl = (generalStatus & 0x01) == 0x01;
            LoadStat = (generalStatus & 0x02) == 0x02;
            WrongPolarity = (generalStatus & 0x04) == 0x04;
            OverTemp = (generalStatus & 0x08) == 0x08;
            OverVoltage = (generalStatus & 0x10) == 0x10;
            OverPower = (generalStatus & 0x20) == 0x20;

            switch (RegMode)
            {
                case 0x01:
                    SetPoint = (decimal)(BitConverter.ToUInt16(DataArray, 16)) / 1000;
                    break;
                case 0x02:
                    SetPoint = (decimal)(BitConverter.ToUInt16(DataArray, 18)) / 10; ;
                    break;
                case 0x03:
                    SetPoint = (decimal)(BitConverter.ToUInt16(DataArray, 20)) / 100;
                    break;
            }

        }

        //todo maybe create ToString methods for measurments and status info
    }
}
