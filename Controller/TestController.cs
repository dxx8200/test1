using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using Controller.IOConfig;
using Controller.TestCaseGen;
using Controller.TestDataRecv;
using Controller.TestRunner;

namespace Controller
{
    public enum Msg_Type
    {
        Msg_Info,
        Msg_Warning,
        Msg_Error
    };

    public delegate void RecvDataHandler(String VarName, Byte[] Buff, int Length);
    public delegate void RecvMsgHandler(Msg_Type MsgType, String Message);

    public class TestController
    {
        private const string DEFAULT_SCRIPTNAME = "sock.usr";
        public static event RecvDataHandler recvDataHandler;
        public static event RecvMsgHandler recvMsgHandler;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ModelPath"></param>
        /// <param name="CasePath"></param>
        /// <param name="OutputPath"></param>
        public static void Initial(string ModelPath, string CasePath, string OutputPath = "")
        {
            OutputPath = FormatOutputPath(OutputPath);
            String ConfigPath = null;

            if(recvMsgHandler != null) recvMsgHandler(Msg_Type.Msg_Info, "Generating IO Config File.");
            try { ConfigPath = IOConfiger.GenerateConfig(ModelPath, OutputPath); }
            catch (Exception ex) { throw new Exception("Create IO Config Failed.\n" + ex.Message); }

            if (recvMsgHandler != null) recvMsgHandler(Msg_Type.Msg_Info, "Generating Test Case DataFile.");
            try { CaseGener.Generate(CasePath, ModelPath, ConfigPath, OutputPath); }
            catch (Exception ex) { throw new Exception("Create Test Case Failed.:\n" + ex.Message);}

            if (recvMsgHandler != null) recvMsgHandler(Msg_Type.Msg_Info, "Configuring IO Device.");
            try{IOConfiger.InitialTest(ConfigPath, false);}
            catch (Exception ex) { throw new Exception("Initial IO Device Failed.\n" + ex.Message); }

            if (recvMsgHandler != null) recvMsgHandler(Msg_Type.Msg_Info, "Initializing Data Receiver.");
            try { DataRecver.Initialize(IOConfiger.RootConfig); }
            catch (Exception ex) { throw new Exception("Initial Data Receiver Failed.\n" + ex.Message); }

            try 
            {
                if (recvMsgHandler != null) recvMsgHandler(Msg_Type.Msg_Info, "Opening LoadRunner...");
                LRController.OpenLR();
                LRController.InitScenario(new List<string>() { Path.Combine(OutputPath, DEFAULT_SCRIPTNAME) }); 
            }
            catch (Exception ex) 
            { 
                throw new Exception("Initial LoadRunner Failed.\n" + ex.Message); 
            }

        }

        public static void Start()
        {
            if (recvMsgHandler != null) recvMsgHandler(Msg_Type.Msg_Info, "Starting LoadRunner...");
            DataRecver.Start();
            LRController.Start();
        }

        public static void Stop()
        {
            if (recvMsgHandler != null) recvMsgHandler(Msg_Type.Msg_Info, "Stopping LoadRunner..."); 
            try { LRController.Stop(); }
            catch (Exception) { }
            try { DataRecver.Close(); }
            catch (Exception) { }
            try { IOConfiger.Close(); }
            catch (Exception) { }
        }

        public static void Close()
        {
            try { LRController.CloseLR(); }
            catch (Exception) { }
        }

        private static String FormatOutputPath(String OriginalPath)
        {
            if (String.IsNullOrWhiteSpace(OriginalPath))
                return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            else
            {
                if ((File.GetAttributes(OriginalPath) & FileAttributes.Directory) != FileAttributes.Directory)
                    return Path.GetDirectoryName(OriginalPath);
            }
            return OriginalPath;
        }

        private static void RecvData(String PortName, Byte[] Buff, int Length)
        {
            if(recvDataHandler != null) recvDataHandler(PortName, Buff, Length);
        }

    }
}
