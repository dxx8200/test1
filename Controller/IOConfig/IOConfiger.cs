using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;
using System.Xml.Serialization;
using System.IO;
using Controller.Util;

namespace Controller.IOConfig
{
    class IOConfiger
    {
        private const string DEFAULT_USER_PATH = @"UsrConfig.xml";
        private const string DEFAULT_SHEET_PATH = @"TransConfig\TransConfig.xslt";
        private const string DEFAULT_CONFIG_PATH = @"TransConfig\CtrlConfig.xml";

        public static ConfigRoot RootConfig = null;

        public static string GenerateConfig(string modelpath, string outputpath = "")
        {
            string Location = Assembly.GetEntryAssembly().Location;

            if (String.IsNullOrWhiteSpace(outputpath))
                outputpath = Path.Combine(Path.GetDirectoryName(Location), DEFAULT_USER_PATH);
            else
            {
                if ((File.GetAttributes(outputpath) & FileAttributes.Directory) == FileAttributes.Directory)
                    outputpath = Path.Combine(outputpath, DEFAULT_USER_PATH);
            }
            outputpath = Path.GetFullPath(outputpath);

            modelpath = Path.GetFullPath(modelpath);

            string xsltpath = Path.Combine(Path.GetDirectoryName(Location), DEFAULT_SHEET_PATH);
            string xmlpath = Path.Combine(Path.GetDirectoryName(Location), DEFAULT_CONFIG_PATH);

            XsltTransformer.TransformFile(xsltpath, xmlpath, outputpath, true,
                new Dictionary<string, object>() { { "p_model", modelpath } });
            return outputpath;
        }

        public static void InitialTest(string configpath, bool IsConfigDevice = true)
        {
            RootConfig = Controller.Util.XmlSerializer.DeserializeFile<ConfigRoot>(configpath);
            if(IsConfigDevice) TransBase.StartConfig(RootConfig);
        }

        public static void Close()
        {

        }

    }
}
