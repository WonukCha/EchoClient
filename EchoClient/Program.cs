using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Threading;

namespace EchoClient
{
    class Program
    {
        const string ipAdrr = "127.0.0.1";
        const int port = 10000;
        const int bufferSize = 256;

        static void Main(string[] args)
        {
            List<Thread> threadList = new List<Thread>();
            for (int i = 0; i < 100; i++)
            {
                Thread thread = new Thread(() => Run());
                threadList.Add(thread);
                thread.Start();
            }
            foreach (Thread thread in threadList)
            {
                thread.Join();
            }
        }
        public static void Run()
        {
            Socket sock = new Socket(
                   AddressFamily.InterNetwork,
                   SocketType.Stream,
                   ProtocolType.Tcp
                   );//소켓 생성
            //인터페이스 결합(옵션)
            //연결
            IPAddress addr = IPAddress.Parse(ipAdrr);
            IPEndPoint iep = new IPEndPoint(addr, port);
            sock.Connect(iep);
            string sendStr = "";
            string receiveStr = "";
            byte[] sendPacket = new byte[bufferSize];
            byte[] receivePacket = new byte[bufferSize];
            while (true)
            {
                System.Array.Clear(sendPacket, 0, bufferSize);
                System.Array.Clear(receivePacket, 0, bufferSize);

                sendStr = sendStr + '1';
                if (sendStr.Length > bufferSize - 2)
                    sendStr = "1";

                //Console.Write("전송한 메시지: {0}\r\n", sendStr);
                MemoryStream ms = new MemoryStream(sendPacket);
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(sendStr);
                bw.Close();
                ms.Close();
                sock.Send(sendPacket);

                Stopwatch stopwatch = Stopwatch.StartNew();
                stopwatch.Stop();

                if (sendStr == "exit")
                {
                    break;
                }
                sock.Receive(receivePacket);

                stopwatch.Stop();

                MemoryStream ms2 = new MemoryStream(receivePacket);
                BinaryReader br = new BinaryReader(ms2);
                receiveStr = br.ReadString();
                //Console.WriteLine("수신한 메시지:{0}\r\n", receiveStr);
                br.Close();
                ms2.Close();

                Console.WriteLine(stopwatch.ElapsedMilliseconds + "ms\r\n");

                if (receiveStr != sendStr)
                    break;
            }
            sock.Close();//소켓 닫기
        }
    }
}