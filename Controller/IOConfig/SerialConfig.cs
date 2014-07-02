using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Net;

namespace Controller.IOConfig
{
    public class SerialConfig : TransConfig
    {
        #region Attributes
        [XmlAttribute]
        public int BaudRate;

        [XmlAttribute]
        public string Parity;

        [XmlAttribute]
        public int InterTime;

        [XmlAttribute]
        public int MaxFrameLen;

        [XmlAttribute]
        public int DataBits;

        [XmlAttribute]
        public int StopBits;

        #endregion
        private const int DEFAULT_BAUDRATE = 9600;
        private const string DEFAULT_PARITY = "NONE";
        private const int DEFAULT_INTERTIME = 200;
        private const int DEFAULT_MAXFRAMELEN = 1024;
        private const int DEFAULT_DATABITS = 8;
        private const int DEFAULT_STOPBITS = 1;
        private const int DEFAULT_FRAME_LEN = 170;

        private Byte[] ConfirmCode = new Byte[DEFAULT_FRAME_LEN];

        public SerialConfig()
        {
            InitialValues();
        }

        protected override void InitialValues()
        {
            base.InitialValues();
            BaudRate = DEFAULT_BAUDRATE;
            Parity = DEFAULT_PARITY;
            InterTime = DEFAULT_INTERTIME;
            MaxFrameLen = DEFAULT_MAXFRAMELEN;
            DataBits = DEFAULT_DATABITS;
            StopBits = DEFAULT_STOPBITS;

            LabelHead = new Byte[] { 0x5A, 0x4C };
            LabelQuery = new Byte[] { 0x04 };
            LabelRead = new Byte[] { 0x04 };
            LabelWrite = new Byte[] { 0x02 };
            LabelResponse = new Byte[] { 0x01 };
        }

        public override Byte[] GetCmdValue(Cmd_Type CmdType, Byte[] InitialValue)
        {
            Byte[] Result = null;
            if (InitialValue == null) Result = new Byte[170];
            else Result = InitialValue;
            Result = base.GetCmdValue(CmdType, Result);
            switch (CmdType)
            {
                case Cmd_Type.CMD_WRITE:
                    return PrepareConfig(Result, LabelHead.Length + LabelWrite.Length);
            }
            return Result;
        }

        private Byte GetBaudRate(int BaudRate)
        {
            switch (BaudRate)
            {
                case 1200:
                    return 0;
                case 2400:
                    return 1;
                case 4800:
                    return 2;
                case 7200:
                    return 3;
                case 9600:
                    return 4;
                case 14400:
                    return 5;
                case 19200:
                    return 6;
                case 28800:
                    return 7;
                case 38400:
                    return 8;
                case 57600:
                    return 9;
                case 78600:
                    return 10;
                case 115200:
                    return 11;
                default:
                    throw new Exception("Invalid BaudRate.");
            }
        }

        private Byte GetParity(String Parity)
        {
            switch (Parity.ToUpper())
            {
                case "NONE":
                    return 0;
                case "EVEN":
                    return 1;
                case "ODD":
                    return 2;
                default:
                    throw new Exception("Invalid Parity.");
            }
        }
        private Byte[] PrepareConfig(Byte[] Result, int Offset)
        {
            Encoding.ASCII.GetBytes(LocalIP + "\0").CopyTo(Result, Offset + 66);
            IPAddress.Parse(LocalIP).GetAddressBytes().CopyTo(Result, Offset + 12);
            Byte[] localport = BitConverter.GetBytes(LocalPort);
            localport.Reverse().ToList().CopyTo(2, Result, Offset + 18, 2);
            Result[Offset + 37] = GetBaudRate(BaudRate);
            Result[Offset + 48] = GetParity(Parity);
            Result.CopyTo(ConfirmCode, 0);
            return Result;
        }

        protected virtual bool ConfirmConfig(Byte[] Buff, int Offset)
        {
            try
            {
                Byte[] configip = new Byte[4];
                Array.Copy(Buff, Offset + 0, configip, 0, 4);
                ConfigIP = (new IPAddress(configip)).ToString();

                Byte[] maskip = new Byte[4];
                Array.Copy(Buff, Offset + 4, maskip, 0, 4);
                MaskIP = (new IPAddress(maskip)).ToString();

                Byte[] dataport = new Byte[4];
                Array.Copy(Buff, Offset + 16, dataport, 2, 2);
                DataPort = BitConverter.ToInt32(dataport.Reverse().ToArray(), 0);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public override bool ConfirmCmd(Cmd_Type CmdType, Byte[] InitialValue, int Length)
        {
            if (Length != DEFAULT_FRAME_LEN) return false;
            if (!base.ConfirmCmd(CmdType, InitialValue, Length)) return false;
            switch (CmdType)
            {
                case Cmd_Type.CMD_QUERY:
                case Cmd_Type.CMD_READ:
                    Byte[] name = new Byte[5];
                    Array.Copy(InitialValue, 41, name, 0, 5);
                    if (!IsByteArrayEqual(name, 0, Encoding.ASCII.GetBytes(Name), 0, 5))
                        return false;
                    return ConfirmConfig(InitialValue, LabelHead.Length + LabelResponse.Length);
                case Cmd_Type.CMD_WRITE:
                    return IsByteArrayEqual(InitialValue, LabelHead.Length + LabelResponse.Length,
                        ConfirmCode, LabelHead.Length + LabelWrite.Length, Length);
            }
            return false;
        }


    }
}
