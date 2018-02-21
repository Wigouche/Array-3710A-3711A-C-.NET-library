using System;


namespace Array371A
{
    public class LoadControlFrame
    {
        public byte[] frameData;

        public LoadControlFrame()
        {
            frameData = new byte[26];
        }

        public LoadControlFrame(byte adress, byte command)
        {
            frameData = new byte[] { 0xAA, adress, command, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            frameData[25] = CalcChecksum();
        }

        public LoadControlFrame(byte adress, byte command, byte[] payload)
        {
            frameData = new byte[] { 0xAA, adress, command, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            int frameByteNum = 3;
            if (payload.Length > 22)
            {
                throw (new ApplicationException("payload array is to large max payload is 22"));
            }
            foreach (byte dataByte in payload)
            {
                frameData[frameByteNum] = dataByte;
                frameByteNum++;
            }

            frameData[25] = CalcChecksum();
        }

        public LoadControlFrame(byte adress, byte command, byte payloadSingleByte)
        {
            frameData = new byte[] { 0xAA, adress, command, payloadSingleByte, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            frameData[25] = CalcChecksum();
        }

       /* public LoadControlFrame(byte[] inputFarmeData)
        {
            if (inputFarmeData.Length > 26)
            {
                throw (new ApplicationException("payload array is to large"));
            }
            frameData = inputFarmeData;
        }*/  // taken out as original use was reworked 

        private byte CalcChecksum()
        {
            byte checksum = 0;
            for (int i = 0; i < 25; i++)
            {
                checksum += frameData[i];
            }
            return checksum;
        }

        public byte[] GetPayload()
        {
            byte[] payload = new byte[22];
            for (int i = 0; i < 22; i++)
            {
                payload[i] = frameData[i + 3];
            }
            return payload;
        }

        public bool ValidateRxFrame(byte address, byte command)
        {
            if (frameData[0] != 0xAA || frameData[1] != address || frameData[2] != command)
                return false;
            if (CalcChecksum() != frameData[25])
                return false;

            return true;
        }
    }
}
