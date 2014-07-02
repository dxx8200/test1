using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Controller.IOConfig
{
    [XmlRoot]
    public class ConfigRoot
    {
        [XmlArray("SerialConfigs")]
        [XmlArrayItem("SerialConfig")]
        public List<SerialConfig> SerialConfig = new List<SerialConfig>();

        [XmlArray("CANConfigs")]
        [XmlArrayItem("CANConfig")]
        public List<CANConfig> CANConfig = new List<CANConfig>();

        [XmlArray("EtherConfigs")]
        [XmlArrayItem("EtherConfig")]
        public List<EtherConfig> EtherConfig = new List<EtherConfig>();

    }
}
