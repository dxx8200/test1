using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Net;

namespace Controller.IOConfig
{
    public class EtherConfig : SerialConfig
    {
        #region Attributes
        [XmlAttribute]
        public String SimIP;

        [XmlAttribute]
        public int SimPort;

        [XmlAttribute]
        public String SimMask;
        #endregion

        private const string DEFAULT_SIMIP = "192.168.1.1";
        private const int DEFAULT_SIMPORT = 1024;
        private const string DEFAULT_SIMMASK = "255.255.255.0";

        private const int DEFAULT_READ_RESLEN = 60;

        private Byte[] ConfirmCode = new Byte[DEFAULT_READ_RESLEN];
        EtherConfig()
        {
            InitialValues();
        }
        protected override void InitialValues()
        {
            base.InitialValues();
            SimIP = DEFAULT_SIMIP;
            SimPort = DEFAULT_SIMPORT;
            SimMask = DEFAULT_SIMMASK;

            LabelRead = new Byte[] { 0xED, 0xF2, 0xA3, 0x56, 0xCA, 0xDB, 0x91, 
                0x84, 0xB0, 0xD7, 0x00, 0x00, 0x3C };
            LabelWrite = new Byte[] { 0xED, 0xF2, 0xA3, 0x56, 0xCA, 0xDB, 0x91, 
                0x84, 0xB0, 0xD7, 0x03, 0x00, 0x3C };
            LabelResponse = new Byte[] { 0x01 };
        }
        private Byte[] PrepareConfig(Byte[] Result)
        {
            int Offset = LabelWrite.Length;

            IPAddress.Parse(SimIP).GetAddressBytes().CopyTo(Result, Offset + 0);
            IPAddress.Parse(SimMask).GetAddressBytes().CopyTo(Result, Offset + 4);
            Byte[] simport = BitConverter.GetBytes(SimPort);
            simport.Reverse().ToList().CopyTo(2, Result, Offset + 16, 2);
            Array.Copy(Result, Offset, ConfirmCode, 0, DEFAULT_READ_RESLEN);
            return Result;
        }

        public override Byte[] GetCmdValue(Cmd_Type CmdType, Byte[] InitialValue)
        {
            switch (CmdType)
            {
                case Cmd_Type.CMD_QUERY:
                    return base.GetCmdValue(CmdType, new Byte[170]);
                case Cmd_Type.CMD_READ:
                    Byte[] ResultRead = new Byte[73];
                    LabelRead.CopyTo(ResultRead, 0);
                    return ResultRead;
                case Cmd_Type.CMD_WRITE:
                    Byte[] ResultWrite = new Byte[73];
                    LabelWrite.CopyTo(ResultWrite, 0);
                    InitialValue.CopyTo(ResultWrite, LabelWrite.Length);
                    return PrepareConfig(ResultWrite);
            }
            return null;
        }

        public override bool ConfirmCmd(Cmd_Type CmdType, Byte[] InitialValue, int Length)
        {
            switch (CmdType)
            {
                case Cmd_Type.CMD_QUERY:
                    Byte[] name = new Byte[Name.Length];
                    Array.Copy(InitialValue, 41, name, 0, Name.Length);
                    if (!IsByteArrayEqual(name, 0, Encoding.ASCII.GetBytes(Name), 0, Name.Length))
                        return false;
                    if (ConfirmConfig(InitialValue, LabelHead.Length + LabelResponse.Length) == false)
                        return false;
                    ConfigPort = DataPort;
                    return true;
                case Cmd_Type.CMD_READ:
                    if (Length != DEFAULT_READ_RESLEN) return false;
                    return true;
                case Cmd_Type.CMD_WRITE:
                    if (Length != DEFAULT_READ_RESLEN) return false;
                    return IsByteArrayEqual(InitialValue, 0, ConfirmCode, 0, DEFAULT_READ_RESLEN);
            }
            return false;
        }
    }
}
