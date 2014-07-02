using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Xml.Serialization;

namespace Controller.IOConfig
{
    public class CANConfig : TransConfig
    {
        #region Attributes
        [XmlAttribute]
        public int BaudRate;

        [XmlAttribute]
        public int MessageId;

        [XmlAttribute]
        public int FilterCode;

        [XmlAttribute]
        public int FilterMask;

        #endregion

        private const int DEFAULT_BAUDRATE = 250;
        private const int DEFAULT_MESSAGEID = 0;
        private const int DEFAULT_FILTERCODE = 0;
        private const int DEFAULT_FILTERMASK = 0;

        private const int DEFAULT_QUERY_RESLEN = 13;
        private const int DEFAULT_READ_RESLEN = 56;
        private const int DEFAULT_WRITE_RESLEN = 1;

        public override Byte[] GetCmdValue(Cmd_Type CmdType, Byte[] InitialValue = null)
        {
            switch(CmdType)
            {
                case Cmd_Type.CMD_QUERY:
                case Cmd_Type.CMD_READ:
                    InitialValue = new Byte[2];
                    break;
                case Cmd_Type.CMD_WRITE:
                    return PrepareConfig(InitialValue);
            }
            return base.GetCmdValue(CmdType, InitialValue);
        }

        private Byte[] PrepareConfig(Byte[] Buff)
        {
            Byte[] Result = new Byte[58];
            LabelHead.CopyTo(Result, 0);
            LabelWrite.CopyTo(Result, LabelHead.Length);
            int Offset = LabelHead.Length + LabelWrite.Length;
            Buff.CopyTo(Result, Offset);

            Byte[] filtercode = BitConverter.GetBytes(FilterCode);
            filtercode.Reverse().ToList().CopyTo(0, Result, Offset + 2 + 16*IOPort, 4);
            Byte[] filtermask = BitConverter.GetBytes(FilterMask);
            filtermask.Reverse().ToList().CopyTo(0, Result, Offset + 6 + 16 * IOPort, 4);
            GetBaudRate(BaudRate).CopyTo(Result, Offset + 10 + 16*IOPort);

            IPAddress.Parse(ConfigIP).GetAddressBytes().CopyTo(Result, Offset + 32);
            IPAddress.Parse(LocalIP).GetAddressBytes().CopyTo(Result, Offset + 36);
            Byte[] dataport = BitConverter.GetBytes(DataPort - IOPort);
            dataport.Reverse().ToList().CopyTo(2, Result, Offset + 40, 2);
            Byte[] localport = BitConverter.GetBytes(LocalPort);
            localport.Reverse().ToList().CopyTo(2, Result, Offset + 42, 2);

            return Result;
        }

        private bool ConfirmConfig(Byte[] Buff)
        {
            try
            {
                Byte[] configip = new Byte[4];
                Array.Copy(Buff, 32, configip, 0, 4);
                ConfigIP = (new IPAddress(configip)).ToString();

                Byte[] dataport = new Byte[4];
                Array.Copy(Buff, 40, dataport, 2, 2);
                DataPort = BitConverter.ToInt32(dataport.Reverse().ToArray(), 0) + IOPort;

            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
        private bool ConfirmQuery(Byte[] Buff)
        {
            try
            {
                if (!IsByteArrayEqual(Buff, 0, LabelResponse, 0, LabelResponse.Length))
                    return false;

                int Offset = LabelResponse.Length;

                Byte[] configip = new Byte[4];
                Array.Copy(Buff, Offset + 0, configip, 0, 4);
                ConfigIP = (new IPAddress(configip)).ToString();

                Byte[] dataport = new Byte[4];
                Array.Copy(Buff, Offset + 8, dataport, 2, 2);
                DataPort = BitConverter.ToInt32(dataport, 0) + IOPort;

            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
        public override bool ConfirmCmd(Cmd_Type CmdType, Byte[] InitialValue, int Length)
        {
            switch (CmdType)
            {
                case Cmd_Type.CMD_QUERY:
                    if (Length != DEFAULT_QUERY_RESLEN) return false;
                    return ConfirmQuery(InitialValue);
                case Cmd_Type.CMD_READ:
                    if (Length != DEFAULT_READ_RESLEN) return false;
                    return ConfirmConfig(InitialValue);
                case Cmd_Type.CMD_WRITE:
                    return base.ConfirmCmd(CmdType, InitialValue, Length);
            }
            return false;
        }

        protected override void InitialValues()
        {
            base.InitialValues();
            BaudRate = DEFAULT_BAUDRATE;
            MessageId = DEFAULT_MESSAGEID;
            FilterCode = DEFAULT_FILTERCODE;
            FilterMask = DEFAULT_FILTERMASK;

            LabelHead = new Byte[] { 0xCD };
            LabelQuery = new Byte[] { 0x0D };
            LabelRead = new Byte[] { 0x04 };
            LabelWrite = new Byte[] { 0x05 };
            LabelResponse = new Byte[] { 0xAA };
        }

        private Byte[] GetBaudRate(int BaudRate)
        {
            switch (BaudRate)
            {
                case 5:
                    return new Byte[]{0x01, 0xBF, 0xFF};
                case 10:
                    return new Byte[] { 0x02, 0x31, 0x1C };
                case 20:
                    return new Byte[] { 0x03, 0x18, 0x1C };
                case 40:
                    return new Byte[] { 0x04, 0x87, 0xFF };
                case 50:
                    return new Byte[] { 0x05, 0x09, 0x1C };
                case 80:
                    return new Byte[] { 0x06, 0x83, 0xFF };
                case 100:
                    return new Byte[] { 0x07, 0x04, 0x1C };
                case 125:
                    return new Byte[] { 0x08, 0x03, 0x1C };
                case 200:
                    return new Byte[] { 0x09, 0x81, 0xFA };
                case 250:
                    return new Byte[] { 0x0A, 0x01, 0x1C };
                case 400:
                    return new Byte[] { 0x0B, 0x80, 0xFA };
                case 500:
                    return new Byte[] { 0x0C, 0x00, 0x1C };
                case 666:
                    return new Byte[] { 0x0D, 0x80, 0xB6 };
                case 800:
                    return new Byte[] { 0x0E, 0x00, 0x16 };
                case 1000:
                    return new Byte[] { 0x0F, 0x00, 0x14 };
                default:
                    throw new Exception("Invalid BaudRate.");
            }
        }
    }
}
