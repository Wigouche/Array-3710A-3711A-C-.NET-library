using System;

namespace Array371A
{
    public class LoadData
    {
        //measurments
        public float current;
        public float voltage;
        public float power;
        public float resistance;
        //settings
        public float MaxPower;
        public float MaxCurrent;
        public byte RegMode;
        public float SetPoint;
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
            current = (float)(BitConverter.ToUInt16(DataArray, 0)) / 1000;
            voltage = (float)(BitConverter.ToUInt32(DataArray, 2)) / 1000;
            power = (float)(BitConverter.ToUInt16(DataArray, 6)) / 10;
            resistance = (float)(BitConverter.ToUInt16(DataArray, 12)) / 100;
            //settings
            MaxPower = (float)(BitConverter.ToUInt16(DataArray, 10)) / 10; ;
            MaxCurrent = (float)(BitConverter.ToUInt16(DataArray, 8)) / 1000;
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
                    SetPoint = (float)(BitConverter.ToUInt16(DataArray, 16)) / 1000;
                    break;
                case 0x02:
                    SetPoint = (float)(BitConverter.ToUInt16(DataArray, 18)) / 10; ;
                    break;
                case 0x03:
                    SetPoint = (float)(BitConverter.ToUInt16(DataArray, 20)) / 100;
                    break;
            }

        }

        //todo maybe create ToString methods for measurments and status info
    }
}
