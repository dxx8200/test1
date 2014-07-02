using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Controller.Util;
using Controller.IOConfig;

namespace Controller.TestDataRecv
{
    class DataRecver
    {
        
        private static IDictionary<string, SocketHelper> sockets = new Dictionary<string, SocketHelper>();
        private static IDictionary<string, TransConfig> configs = new Dictionary<string, TransConfig>();

        public delegate void RecvDataHandler(String PortName, Byte[] Buff, int Length);
        public static event RecvDataHandler recvDataHandler;

        private static bool IsInitialized = false;

        public static void Recv(string SrcIP, int SrcPort, Byte[] Buff, int Length)
        {
            string key = CreateKey(SrcIP, SrcPort);
            TransConfig tc = configs[key];
            if (tc != null) recvDataHandler(tc.Name, Buff, Length);
        }

        private static void InitSocket(TransConfig tc)
        {
            SocketHelper sh = new SocketHelper();
            sh.Create(tc.LocalIP, tc.LocalPort, "UDP");
            string key = CreateKey(tc.ConfigIP, tc.DataPort);
            if (!sockets.ContainsKey(key)) sockets.Add(key, sh);
            else
            {
                sockets[key].Close();
                sockets[key] = sh;
            }
            if (!configs.ContainsKey(key)) configs.Add(key, tc);
            else
            {
                configs[key] = tc;
            }
        }

        public static void Initialize(ConfigRoot config)
        {
            if (IsInitialized) return;

            foreach(SerialConfig sc in config.SerialConfig)
            {
                InitSocket(sc);
            }
            foreach(CANConfig cc in config.CANConfig)
            {
                InitSocket(cc);
            }
            foreach(EtherConfig ec in config.EtherConfig)
            {
                InitSocket(ec);
            }
            IsInitialized = true;
        }

        public static void Start()
        {
            if (!IsInitialized) throw new Exception("Should Initialize Test First.");
            foreach(KeyValuePair<string, SocketHelper> k in sockets)
            {
                k.Value.StartRecv(Recv, 1024);
            }

        }

        public static void Close()
        {
            foreach (KeyValuePair<string, SocketHelper> k in sockets)
            {
                k.Value.Close();
            }
            sockets.Clear();
            IsInitialized = false;
        }

        private static string CreateKey(string IP, int port)
        {
            return IP + ":" + port;
        }
    }
}
