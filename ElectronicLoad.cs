using System;
using System.IO.Ports;

namespace Array371A
{
    /* 
     * class librarry for controlling and getting data from array3710A or array3711A DC electronic load 
     * manages serial port and packet structure used by electronic load
     * 
     * TODO 
     *  - some additional useful methods could be added (inline todos)
     *  - improved exeption handling, define own exceptions
     * 
     */
     
    public class ElectronicLoad : IDisposable
    {
        //todo add methods for changing/updating max vlaues 
        //todo add method for changing load address

        public SerialPort ComPort;
        public byte Address;
        public float maxPower = 200.0F;
        public float maxCurrent = 30.000F;

        public ElectronicLoad(string PortName, byte loadAddress)
        {
            Address = loadAddress;
            ComPort = new SerialPort
            {
                PortName = PortName,
                BaudRate = 9600,
                ReadTimeout = 1000,
                WriteTimeout = 1000,
            };
            ComPort.Open();
            //todo handle port opening and creation exceptions
            ComPort.ReadExisting(); // flush port incase exiting data is available
        }

        public ElectronicLoad(string PortName, byte loadAddress, int loadBaudRate)
        {
            Address = loadAddress;
            ComPort = new SerialPort
            {
                PortName = PortName,
                BaudRate = loadBaudRate,
                ReadTimeout = 1000,
                WriteTimeout = 1000,
            };
            ComPort.Open();
            //todo handle port opening and creation exceptions
            ComPort.ReadExisting(); // flush port incase exiting data is available 
        }

        public void SendFrame(LoadControlFrame frameToSend)
        {
            ComPort.Write(frameToSend.frameData, 0, 26);
            //todo may want to handle exception here
        }

        public LoadControlFrame RecieveFrame()
        {
            //todo add managment of dataflow and timout exception
            try
            {
                LoadControlFrame recivedFrame = new LoadControlFrame();
                while (recivedFrame.frameData[0] != 0xAA)
                {
                    recivedFrame.frameData[0] = (byte)ComPort.ReadByte();
                }
                for (int i = 1; i < 26; i++)
                {
                    recivedFrame.frameData[i] = (byte)ComPort.ReadByte();
                }
                return recivedFrame;
            }
            catch (TimeoutException)
            {
                throw (new ApplicationException("data retrival timout"));
            }
        }

        public void Dispose()
        {
            LocalControl();
            ComPort.Close();
            ComPort.Dispose();
        }

        public void PCControl()
        {
            SendFrame(new LoadControlFrame(Address, 0x92, (byte)0x02));
        }

        public void LocalControl()
        {
            SendFrame(new LoadControlFrame(Address, 0x92, (byte)0x00));
        }

        public void On()
        {
            SendFrame(new LoadControlFrame(Address, 0x92, (byte)0x03));
        }

        public void Off()
        {
            SendFrame(new LoadControlFrame(Address, 0x92, (byte)0x02));
        }

        public void SetProgramSequence(LoadSequence newProgram)
        {
            SendFrame(new LoadControlFrame(Address, 0x93, newProgram.FirstPayload()));
            SendFrame(new LoadControlFrame(Address, 0x94, newProgram.SecondPayload()));
        }

        public void StartProgramSequence()
        {
            SendFrame(new LoadControlFrame(Address, 0x95));
        }

        public void StopProgramSequence()
        {
            SendFrame(new LoadControlFrame(Address, 0x96));
        }

        public void SetLoadCurrent(float current)
        {
            if (current <= 30 || current >= 0)
                SetLoad(0x01, (ushort)(current * 1000));
            else
                throw (new ApplicationException("invalid input current can only be between 0 and 30A"));
        }

        public void SetLoadPower(float power)
        {
            if (power <= 200 || power >= 0)
                SetLoad(0x02, (ushort)(power * 10));
            else
                throw (new ApplicationException("invalid input current can only be between 0 and 200W"));
        }

        public void SetLoadResistance(float resistance)
        {
            if (resistance <= 500 || resistance >= 0)
                SetLoad(0x03, (ushort)(resistance));
                        else
                throw (new ApplicationException("invalid input current can only be between 0 and 500Oms"));
        }

        public void SetLoad(byte mode, ushort value)
        {
            byte[] splitValue = BitConverter.GetBytes(value);
            byte[] splitMaxCurrent = BitConverter.GetBytes((ushort)maxCurrent * 1000);
            byte[] splitMaxPower = BitConverter.GetBytes((ushort)maxPower * 10);
            SendFrame(new LoadControlFrame(Address, 0x90, new byte[] { splitMaxCurrent[0], splitMaxCurrent[1], splitMaxPower[0], splitMaxPower[1], 0xFF, mode, splitValue[0], splitValue[1] }));
        }

        public LoadData GetData()
        {
            const byte command = 0x91;
            SendFrame(new LoadControlFrame(Address, command));
            try
            {
                LoadControlFrame recieveData = RecieveFrame();

                if (recieveData.ValidateRxFrame(Address, command))
                {
                    return new LoadData(recieveData);
                }
                else
                {
                    throw (new ApplicationException("Recieve data validation error"));
                }
            }
            catch (ApplicationException)
            {
                throw (new ApplicationException("Load status info failed")); 
            }
        }
    }
}
