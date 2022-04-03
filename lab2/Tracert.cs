using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace lab2
{
    public class Tracert
    {
        private static readonly int _maxHops = 30;
        private static readonly int _attempts = 3;
        private static readonly int _timeout = 300;
        private static readonly int _bufSize = 4096;

        public string RemoteIPStr { get; set; }


        public Tracert(string remoteIPStr)
        {
            RemoteIPStr = remoteIPStr;
        }


        public List<TracertFrame> Run()
        {
            List<TracertFrame> res = new List<TracertFrame>();

            Socket hostSocket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Icmp);
            hostSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, _timeout);

            IPAddress remoteIP = IPAddress.Parse(RemoteIPStr);
            IPEndPoint remotePoint = new IPEndPoint(remoteIP, 0);
            EndPoint endPoint = remotePoint;

            ushort number = 0;

            for (int i = 0; i < _maxHops; ++i)
            {
                ICMPPacket packet = new ICMPPacket();
                ICMPPacket recieved = null;

                TracertFrame frame = new TracertFrame
                {
                    HopNumber = i + 1,
                    Success = false
                };
                Console.Write("{0, 3}:", frame.HopNumber);
                for (int j = 0; j < _attempts; ++j)
                {
                    packet.SequenceNumber = number++;

                    hostSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.IpTimeToLive, i + 1);
                    hostSocket.SendTo(packet.CreateByteArray(), remotePoint);

                    DateTime startTime = DateTime.Now;
                    frame.RemoteIP = endPoint.ToString();
                    recieved = null;


                    try
                    {
                        byte[] buf = new byte[_bufSize];
                        int len = hostSocket.ReceiveFrom(buf, ref endPoint);
                        TimeSpan timeSpan = DateTime.Now - startTime;
                        recieved = new ICMPPacket(buf, len);

                        if ((recieved != null) && ((recieved.Type == 11) || (recieved.Type == 0)))
                        {
                            frame.Success = true;
                            frame.Attempts.Add(timeSpan.Milliseconds);
                        }
                    }
                    catch (SocketException e)
                    {
                        frame.Attempts.Add(-1);
                    }
                }

                res.Add(frame);
                foreach (int a in frame.Attempts)
                    if (a != -1)
                        Console.Write("{0, 8}", a + " mc");
                    else
                        Console.Write("{0, 8}", "***");
                if (!frame.Success)
                    Console.WriteLine("    Response timed out");
                else
                    Console.WriteLine("    {0, 22}", frame.RemoteIP.PadRight(22));

                if ((recieved != null) && (recieved.Type == 0))
                    break;
            }

            hostSocket.Close();

            return res;
        }
    }
}