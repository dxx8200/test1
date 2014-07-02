using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Reflection;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Controller.IOConfig;
using Controller.TestCaseGen;
using Controller.Util;
using Controller.TestDataRecv;
using Controller;
using System.Collections;
using Controller.TestRunner;
using System.Diagnostics;

namespace TestDemo
{
    class Program
    {
        static IDictionary<string, IList<byte[]>> dVars = new Dictionary<string, IList<byte[]>>();

        private static string ByteArrayToString(byte[] ba)
        {
            if (ba.Length > 0)
            {
                StringBuilder hex = new StringBuilder(ba.Length * 4);
                foreach (byte b in ba)
                    hex.AppendFormat("\\x{0:x2}", b);
                return hex.ToString();
            }
            return String.Empty;
        }

        private static byte[] SetVarValue(byte[] DstArray, int DstIndex, int DstBitShift, int DstByteLen,
                                    byte[] SrcArray, int SrcIndex, int SrcBitShift, int SrcByteLen,
                                    int CopyByteLen, int CopyBitLen)
        {
            BitArray DstBitArray = new BitArray(DstArray);
            BitArray SrcBitArray = new BitArray(SrcArray);
            int CopyLen = CopyByteLen * 8 + CopyBitLen;
            int i = 0;
            for (; i < CopyLen && i < DstBitArray.Length && i < SrcBitArray.Length; i++)
            {
                DstBitArray[DstIndex * 8 + DstBitShift + i] = SrcBitArray[SrcIndex * 8 + SrcBitShift + i];
            }
            byte[] Dst = new Byte[DstByteLen];
            DstBitArray.CopyTo(Dst, 0);
            return Dst;
        }

        private static byte[] ParseValue(string datatype, string value)
        {
            switch (datatype.ToUpper())
            {
                case "BOOL":
                    return BitConverter.GetBytes(Convert.ToBoolean(value));
                case "CHAR":
                    return BitConverter.GetBytes(Convert.ToChar(value));
                case "UCHAR":
                case "UINT16":
                    return BitConverter.GetBytes(Convert.ToUInt16(value));
                case "INT16":
                    return BitConverter.GetBytes(Convert.ToInt16(value));
                case "UINT32":
                    return BitConverter.GetBytes(Convert.ToUInt32(value));
                case "INT32":
                    return BitConverter.GetBytes(Convert.ToInt32(value));
                case "UINT64":
                    return BitConverter.GetBytes(Convert.ToUInt64(value));
                case "INT64":
                    return BitConverter.GetBytes(Convert.ToInt64(value));
                case "FLOAT":
                    return BitConverter.GetBytes(Convert.ToSingle(value));
                case "DOUBLE":
                    return BitConverter.GetBytes(Convert.ToDouble(value));
            }
            return null;
        }

        private static string AddVariable(XPathNavigator v)
        {
            string key = v.SelectSingleNode("Key").Value;
            int length = v.SelectSingleNode("Length").ValueAsInt;
            string datatype = v.SelectSingleNode("DataType").Value;
            string datavalue = v.SelectSingleNode("InitValue").Value;

            IList<byte[]> value = new List<byte[]>();

            if (!String.IsNullOrWhiteSpace(key))
            {
                if (datatype.ToUpper() == "BLOCK")
                {
                    XPathNodeIterator itr = v.Select("Elements/Element");
                    while (itr.MoveNext())
                    {
                        XPathNavigator e = itr.Current;
                        value.Add(ParseValue(e.SelectSingleNode("DataType").Value,
                            e.SelectSingleNode("InitValue").Value));
                    }
                }
                else
                {
                    value.Add(ParseValue(datatype, datavalue));

                }
                dVars.Add(key, value);
                return FormatVariable(key);
            }
            return String.Empty;
        }

        private static string FormatVariable(string key)
        {
            IList<byte[]> value = dVars[key];
            if (value == null) return "";
            if (value.Count > 1)
            {
                StringBuilder sb = new StringBuilder();
                foreach (byte[] b in value)
                    sb.AppendFormat("\"{0}\"\n", ByteArrayToString(b));
                return sb.ToString();
            }
            else if (value.Count > 0)
                return String.Format("\"{0}\"\n", ByteArrayToString(value[0]));
            else
                return String.Empty;
        }

        public static string Evaluate(XPathNavigator stValue)
        {
            string key = stValue.SelectSingleNode("Variable/VariableKey").Value;
            string vType = stValue.SelectSingleNode("Variable/VType").Value;
            string dataType = stValue.SelectSingleNode("Variable/DataType").Value;
            string dataValue = stValue.SelectSingleNode("Value").Value;

            if (!dVars.ContainsKey(key)) return "";

            IList<byte[]> value = dVars[key];
            if (value == null) return "";
            switch (vType)
            {
                case "Variable":
                    dVars[key] = new List<byte[]>(1) { ParseValue(dataType, dataValue) };
                    break;
                case "Element":
                    dVars[key][stValue.SelectSingleNode("Variable/ElementIndex").ValueAsInt] 
                        = ParseValue(dataType, dataValue);
                    break;
                case "BitSegment":
                    int eleIndex = stValue.SelectSingleNode("Variable/ElementIndex").ValueAsInt;
                    int startBit = stValue.SelectSingleNode("Variable/StartBit").ValueAsInt;
                    int endBit = stValue.SelectSingleNode("Variable/EndBit").ValueAsInt;
                    byte[] valueBit = ParseValue(dataType, dataValue);
                    if(valueBit != null)
                        dVars[key][eleIndex] = SetVarValue(dVars[key][eleIndex], 0, startBit, dVars[key][eleIndex].Length,
                            valueBit, 0, 0, valueBit.Length, 0, endBit - startBit + 1); 
                    break;
            }
            return FormatVariable(key);

        }
        // Define the event handlers. 
        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed, created, or deleted.
            Console.WriteLine("File: " + e.FullPath + " " + e.ChangeType);
        }

        static void Main(string[] args)
        {
            Process p = new Process();
            p.StartInfo.FileName = @"C:\Program Files\Mercury\LoadRunner\bin\mdrv.exe";
            p.StartInfo.Arguments = @"-usr ""C:\Documents and Settings\Geste\桌面\1s交替发送\1s交替发送.usr"" -out ""C:\temp""";

            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.OutputDataReceived += new DataReceivedEventHandler(p_OutputDataReceived);
            p.ErrorDataReceived += new DataReceivedEventHandler(p_ErrorDataReceived);
            p.EnableRaisingEvents = true;
            p.Start();
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();

            p.Kill();
            p.WaitForExit();
            // Create a new FileSystemWatcher and set its properties.
//             FileSystemWatcher watcher = new FileSystemWatcher();
//             watcher.Path = @"C:\temp";
//             /* Watch for changes in LastAccess and LastWrite times, and
//                the renaming of files or directories. */
//             watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
//                | NotifyFilters.FileName | NotifyFilters.DirectoryName;
//             // Only watch text files.
//             watcher.Filter = "*.log";
// 
//             // Add event handlers.
//             watcher.Changed += new FileSystemEventHandler(OnChanged);
//             watcher.Created += new FileSystemEventHandler(OnChanged);
//             watcher.Deleted += new FileSystemEventHandler(OnChanged);
// 
//             // Begin watching.
//             watcher.EnableRaisingEvents = true;
//             Console.Read();
        }

        static void p_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine(e.Data);
        }

        static void p_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine(e.Data);
        }
//         static void RecvDataHandler(String PortName, Byte[] Buff, int Length)
//         {
// 
//         }
// 
//         static void Main(string[] args)
//         {
//             try
//             {
//                 TestController tc = new TestController();
//                 Console.WriteLine("Initializing Test.");
//                 tc.Initial(@"C:\Users\Xiaoxu\Desktop\model.xml", @"C:\Users\Xiaoxu\Desktop\case.xml", @"C:\aaa");
//                 Console.WriteLine("Starting Test.");
//                 tc.Start(RecvDataHandler);
//                 Console.WriteLine("Test is Running...");
//                 Thread.Sleep(10000);
//                 Console.WriteLine("Stopping Test.");
//                 tc.Stop();
//                 Console.WriteLine("Closing Test.");
//                 tc.Close();
//                 Console.WriteLine("Closed.");
//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine(ex.Message);
//             }
//             Console.Read();
//         }
//         static void Main(string[] args)
//         {
//             //             CaseGener.Generate(@"C:\Users\Xiaoxu\Desktop\case.xml", @"C:\Users\Xiaoxu\Desktop\model.xml",
//             //                 @"C:\Users\Xiaoxu\Desktop\TestCases");
//             XPathDocument doc = new XPathDocument(@"C:\Documents and Settings\Geste\桌面\model.xml");
//             XPathNavigator nav = doc.CreateNavigator();
//             StringBuilder sb = new StringBuilder();
// 
// 
//             foreach (XPathNavigator v in nav.Select("/LinkedModel/Links/Link/Variables/Variable"))
//             {
//                 Console.WriteLine("//{0}", v.SelectSingleNode("Name").Value);
//                 Console.WriteLine(AddVariable(v));
// 
//             }
// 
//             XPathDocument doc1 = new XPathDocument(@"C:\Documents and Settings\Geste\桌面\case.xml");
//             XPathNavigator nav1 = doc1.CreateNavigator();
//             foreach(XPathNavigator v in nav1.Select("/TestCase/Operations/TsNode/Operation/Steps/TsStep/Values/TsStepValue"))
//             {
//                 Console.WriteLine("//{0}", v.SelectSingleNode("Variable/Name").Value);
//                 Console.WriteLine(Evaluate(v));
//             }
//             Console.Read();
//         }

        //         static void Main(string[] args)
        //         {
        //             byte[] buf = new byte[170];
        //             //             buf[0] = 0xCD;
        //             //             buf[1] = 0x04;
        //             buf[0] = 0x5A;
        //             buf[1] = 0x4C;
        //             buf[2] = 0x04;
        //             SocketHelper s = new SocketHelper();
        //             IPHostEntry HostEntry = Dns.GetHostEntry("");
        //             foreach (IPAddress IP in HostEntry.AddressList)
        //             {
        //                 if (IP.AddressFamily != AddressFamily.InterNetwork) continue;
        //                 s.Create(IP.ToString(), 30001, "UDP");
        //                 s.BoardCast(1092, buf);
        //                 Thread.Sleep(80);
        //                 string ip = "";
        //                 int port = 0;
        // 
        //                 while (s.GetAvialable() > 0)
        //                 {
        //                     buf = new byte[s.GetAvialable()];
        //                     int recv = s.ReceiveFrom(ref ip, ref port, buf);
        //                     File.WriteAllBytes(@"C:\aaa\" + ip, buf);
        //                 }
        //                 s.Close();
        // 
        //             }
        // 
        // 
        //         }
        //         static void Main(string[] args)
        //         {
        //             byte[] buf = new byte[170];
        //             buf[0] = 0x5A;
        //             buf[1] = 0x4C;
        //             buf[2] = 0x04;
        //             SocketHelper s = new SocketHelper();
        //             s.Create("192.168.0.90", 30001, "UDP");
        //             s.Send("192.168.0.201", 1092, buf);
        //             Thread.Sleep(80);
        //             string ip = "";
        //             int port = 0;
        // 
        //             while (s.GetAvialable() > 0)
        //             {
        //                 buf = new byte[170];
        //                 int recv = s.ReceiveFrom(ref ip, ref port, buf);
        //                 Encoding.ASCII.GetBytes("192.168.0.4\0").CopyTo(buf, 69);
        //                 buf[2] = 0x02;
        //                 s.Send("192.168.0.201", 1092, buf);
        //                 Thread.Sleep(80);
        //                 buf[2] = 0x04;
        //                 s.Send("192.168.0.201", 1092, buf);
        //                 Thread.Sleep(80);
        //                 while(s.GetAvialable() > 0)
        //                 {
        //                     recv = s.ReceiveFrom(ref ip, ref port, buf);
        //                 }
        //             }
        //             s.Close();
        // 
        //         }



    }
}
