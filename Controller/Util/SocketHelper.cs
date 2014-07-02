using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Security.Permissions;

namespace Controller.Util
{
    class SocketHelper : IDisposable
    {
        public class StateObject
        {
            // Client socket.
            public Socket workSocket = null;
            // Receive buffer.
            public byte[] buffer = null;
            
        };
        public delegate void RecvHandler(string SrcIP, int SrcPort, Byte[] Buff, int Length);
        public event RecvHandler RecvEvent;

        private Socket socket = null;
        private Byte[] buffer = null;

        public void BoardCast(int port, byte[] buff)
        {
            if (socket == null) throw new Exception("Create Socket First.");
            socket.EnableBroadcast = true;

            Send(IPAddress.Broadcast.ToString(), port, buff);
        }

        public void Create(String LocalIP, int LocalPort, String Protocol)
        {
            if (Protocol.ToUpper() != "UDP") throw new Exception("Nonsupport Protocol: " + Protocol);

            try
            {
                IPHostEntry HostEntry = Dns.GetHostEntry("");
                IPAddress Address = IPAddress.Parse(LocalIP);
                IPEndPoint EndPoint = new IPEndPoint(Address, LocalPort);
                SocketPermission _permission = new SocketPermission(PermissionState.None);
                _permission.AddPermission(NetworkAccess.Accept, TransportType.Udp, HostEntry.HostName, LocalPort);
                _permission.AddPermission(NetworkAccess.Connect, TransportType.Udp, HostEntry.HostName, LocalPort);
                _permission.Demand();

                socket = new Socket(EndPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
                uint IOC_IN = 0x80000000;
                uint IOC_VENDOR = 0x18000000;
                uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;
                socket.IOControl((int)SIO_UDP_CONNRESET, new byte[] { Convert.ToByte(false) }, null);
                //socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.PacketInformation, true);
                socket.Bind(EndPoint);
                
            }
            catch (Exception ex)
            {
                throw new Exception("Create Socket Failed.\n" + ex.Message);
            }

        }

        public void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                EndPoint ep = (EndPoint)ar.AsyncState;
                int bytesRead = socket.EndReceiveFrom(ar, ref ep);
                IPEndPoint endPoint = (IPEndPoint)ep;
                if (bytesRead > 0)
                {                    
                    try
                    {                       
                        RecvEvent(endPoint.Address.ToString(), endPoint.Port, buffer, bytesRead);
                    }
                    catch (Exception) { }
                    endPoint.Address = IPAddress.Any;
                    endPoint.Port = 0;
                    socket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref ep,
                        new AsyncCallback(ReceiveCallback), ep);
                }
            }
            catch(Exception)
            {

            }
        }

        public void StartRecv(RecvHandler handler, int MaxBuffLen)
        {
            buffer = new Byte[MaxBuffLen];
            RecvEvent += handler;
            IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);
            EndPoint endPoint = (EndPoint)ep;
            socket.BeginReceiveFrom(buffer, 0, MaxBuffLen, SocketFlags.None, ref endPoint,
                new AsyncCallback(ReceiveCallback), ep);
        }

        public int GetAvialable()
        {
            if (socket == null) return 0;
            return socket.Available;
        }

        public int Send(String DestIP, int DestPort, byte[] Buff)
        {
            if (socket == null) throw new Exception("Create Socket First.");
            if (Buff == null) return 0;
            int Sended = 0;
            while (Sended < Buff.Length)
            {
                Sended += socket.SendTo(Buff, Sended, Buff.Length, SocketFlags.None,
                    new IPEndPoint(IPAddress.Parse(DestIP), DestPort));
            }
            return Sended;
        }

        public int ReceiveFrom(ref string SrcIP, ref int SrcPort, byte[] Buff)
        {
            if (socket == null) throw new Exception("Create Socket First.");
            EndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);

            int Received = socket.ReceiveFrom(Buff, Buff.Length, SocketFlags.None, ref endPoint);
            IPEndPoint ipEndPoint = (IPEndPoint)endPoint;
            SrcIP = ipEndPoint.Address.ToString();
            SrcPort = ipEndPoint.Port;
            return Received;
        }

        public void Close()
        {
            if (socket != null)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
        }

        public void Dispose()
        {
            Close();
        }
    }
}
