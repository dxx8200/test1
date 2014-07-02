using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Controller.Util;

namespace Controller.IOConfig
{
    class IODetector
    {
        private static bool DetectFromSock(IPAddress IP, TransConfig tc)
        {
            bool Result = false;
            using (SocketHelper sock = new SocketHelper())
            {
                sock.Create(IP.ToString(), tc.LocalPort, "UDP");
                sock.BoardCast(tc.ConfigPort, tc.GetCmdValue(TransConfig.Cmd_Type.CMD_QUERY));
                Thread.Sleep(80);
                while (sock.GetAvialable() > 0)
                {
                    string SourceIP = "";
                    int SrcPort = 0;
                    byte[] buff = new byte[sock.GetAvialable()];
                    int Received = sock.ReceiveFrom(ref SourceIP, ref SrcPort, buff);
                    if (tc.ConfirmCmd(TransConfig.Cmd_Type.CMD_QUERY, buff, Received))
                    {
                        Result = true; break;
                    }
                }
            }
            return Result;
        }

        private static bool DetectConfigs(IPAddress IP, IList<TransConfig> configs)
        {
            try
            {
                foreach (TransConfig tc in configs)
                {
                    if (DetectFromSock(IP, tc) == false) return false;
                    else
                    {
                        tc.LocalIP = IP.ToString();
                        tc.IsOnline = true;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;

        }

        private static bool DetectSerial(IPAddress IP, ConfigRoot root)
        {
            return DetectConfigs(IP, root.SerialConfig.Cast<TransConfig>().ToList());
        }
        private static bool DetectCAN(IPAddress IP, ConfigRoot root)
        {
            return DetectConfigs(IP, root.CANConfig.Cast<TransConfig>().ToList());
        }

        private static bool DetectEther(IPAddress IP, ConfigRoot root)
        {
            return DetectConfigs(IP, root.EtherConfig.Cast<TransConfig>().ToList());
        }

        public static bool Detect(ConfigRoot root)
        {
                IPHostEntry hosts = Dns.GetHostEntry("");
                foreach (IPAddress IP in hosts.AddressList)
                {
                    if (IP.AddressFamily != AddressFamily.InterNetwork) continue;
                    try
                    {
                        if (DetectSerial(IP, root) &&
                            DetectCAN(IP, root) &&
                            DetectEther(IP, root))
                            return true;
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                }
            return false;
        }

    }
}
