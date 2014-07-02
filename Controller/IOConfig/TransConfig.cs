using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Controller.IOConfig
{
    public class TransConfig
    {
        #region Attributes
        [XmlAttribute(DataType="hexBinary")]
        public Byte[] LabelHead;

        [XmlAttribute(DataType = "hexBinary")]
        public Byte[] LabelQuery;

        [XmlAttribute(DataType = "hexBinary")]
        public Byte[] LabelRead;

        [XmlAttribute(DataType = "hexBinary")]
        public Byte[] LabelWrite;

        [XmlAttribute(DataType = "hexBinary")]
        public Byte[] LabelResponse;

        [XmlAttribute]
        public int IOPort;

        [XmlAttribute]
        public String Name;

        [XmlAttribute]
        public String ConfigIP;

        [XmlAttribute]
        public String MaskIP;

        [XmlAttribute]
        public int ConfigPort;

        [XmlAttribute]
        public int DataPort;

        [XmlAttribute]
        public String Protocol;

        [XmlAttribute]
        public String LocalIP;

        [XmlAttribute]
        public int LocalPort;

        [XmlAttribute]
        public int LRPort;

        [XmlAttribute]
        public int WaitTime;

        public bool IsOnline;

        #endregion

        public enum Cmd_Type
        {
            CMD_QUERY,
            CMD_READ,
            CMD_WRITE
        };

        public TransConfig()
        {
            InitialValues();
        }

        public virtual Byte[] GetCmdTypeValue(Cmd_Type CmdType)
        {
            switch (CmdType)
            {
                case TransConfig.Cmd_Type.CMD_QUERY:
                    return LabelQuery;
                case TransConfig.Cmd_Type.CMD_READ:
                    return LabelRead;
                case TransConfig.Cmd_Type.CMD_WRITE:
                    return LabelWrite;
                default:
                    return null;
            }
        }

        public virtual Byte[] GetCmdValue(Cmd_Type CmdType, Byte[] InitialValue = null)
        {
            if (InitialValue == null) return null;
            LabelHead.CopyTo(InitialValue, 0);
            switch (CmdType)
            {
                case Cmd_Type.CMD_QUERY:                    
                    LabelQuery.CopyTo(InitialValue, LabelHead.Length);
                    break;
                case Cmd_Type.CMD_READ:
                    LabelRead.CopyTo(InitialValue, LabelHead.Length);
                    break;
                case Cmd_Type.CMD_WRITE:
                    LabelWrite.CopyTo(InitialValue, LabelHead.Length);
                    break;
            }
            return InitialValue;
        }

        public virtual bool ConfirmCmd(Cmd_Type CmdType, Byte[] InitialValue = null, int Length = 0)
        {
            if (!IsByteArrayEqual(LabelHead, 0, InitialValue, 0, LabelHead.Length)) return false;
            switch (CmdType)
            {
                case Cmd_Type.CMD_QUERY:
                case Cmd_Type.CMD_READ:
                case Cmd_Type.CMD_WRITE:
                    return IsByteArrayEqual(LabelResponse, 0, InitialValue, LabelHead.Length, LabelResponse.Length);

            }
            return true;
        }

        protected virtual void InitialValues()
        {
            IsOnline = false;
        }
        protected bool IsByteArrayEqual(Byte[] Buf1, int Start1, Byte[] Buf2, int Start2, int Length)
        {
            for (int i = 0; i < Length && Start1+i < Buf1.Length && Start2+i < Buf2.Length; i++)
            {
                if (Buf1[Start1 + i] != Buf2[Start2 + i]) return false;
            }
            return true;
        }


    }
}
