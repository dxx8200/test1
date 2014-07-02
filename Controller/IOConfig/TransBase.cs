using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Controller.Util;

namespace Controller.IOConfig
{
    class TransBase
    {

        public static void StartConfig(ConfigRoot config)
        {
            try
            {
                foreach (SerialConfig sc in config.SerialConfig)
                {
                    ConfigFromSocket(sc);
                }
                foreach (CANConfig cc in config.CANConfig)
                {
                    ConfigFromSocket(cc);
                }
                foreach (EtherConfig ec in config.EtherConfig)
                {
                    ConfigFromSocket(ec);
                }
            }
            catch (Exception)
            {
                try
                {
                    if (IODetector.Detect(config)) StartConfig(config);
                    else throw new Exception("Can not find IO Device.\n");
                }
                catch(Exception ex1)
                {
                    throw new Exception("Detect IO Device Failed.\n" + ex1.Message);
                }
            }
        }

        private static bool ConfigFromSocket(TransConfig TransConfig)
        {
            using (SocketHelper sock = new SocketHelper())
            {

                sock.Create(TransConfig.LocalIP, TransConfig.LocalPort, TransConfig.Protocol);
                sock.Send(TransConfig.ConfigIP, TransConfig.ConfigPort,
                    TransConfig.GetCmdValue(TransConfig.Cmd_Type.CMD_READ));

                Thread.Sleep(TransConfig.WaitTime);

                while (sock.GetAvialable() > 0)
                {
                    String SrcIP = "";
                    int SrcPort = 0;
                    Byte[] Buff = new Byte[sock.GetAvialable()];
                    int Received = sock.ReceiveFrom(ref SrcIP, ref SrcPort, Buff);
                    if (TransConfig.ConfirmCmd(TransConfig.Cmd_Type.CMD_READ, Buff, Received))
                    {
                        sock.Send(TransConfig.ConfigIP, TransConfig.ConfigPort,
                               TransConfig.GetCmdValue(TransConfig.Cmd_Type.CMD_WRITE, Buff));
                        Thread.Sleep(TransConfig.WaitTime);
                        if (sock.GetAvialable() > 0)
                        {
                            Byte[] BuffConfirm = new Byte[sock.GetAvialable()];
                            int RecvConfirm = sock.ReceiveFrom(ref SrcIP, ref SrcPort, BuffConfirm);
                            return TransConfig.ConfirmCmd(TransConfig.Cmd_Type.CMD_WRITE, BuffConfirm, RecvConfirm);
                        }
                        else
                        {
                            sock.Send(TransConfig.ConfigIP, TransConfig.ConfigPort,
                                TransConfig.GetCmdValue(TransConfig.Cmd_Type.CMD_READ));
                            Thread.Sleep(TransConfig.WaitTime);

                            while (sock.GetAvialable() > 0)
                            {
                                String SrcIPConfirm = "";
                                int SrcPortConfirm = 0;
                                Byte[] BuffConfirm = new Byte[sock.GetAvialable()];
                                int ReceivedConfirm = sock.ReceiveFrom(ref SrcIPConfirm, ref SrcPortConfirm, BuffConfirm);
                                return TransConfig.ConfirmCmd(TransConfig.Cmd_Type.CMD_WRITE, BuffConfirm, ReceivedConfirm);

                            }
                        }
                    }
                }
            }
            return false;
        }

    }
}
