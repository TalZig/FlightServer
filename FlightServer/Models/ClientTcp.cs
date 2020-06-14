using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Web;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace FlightServer.Models
{
    public class ClientTcp : ITCPClient
    {
        TcpClient tcpclnt;
        NetworkStream stm;
        bool connect;

        public ClientTcp()
        {
            this.tcpclnt = new TcpClient();
            connect = false;
        }

        public void Connect(string ip, int port)
        {
            if (IsConnect()) { return; }
            try
            {
                tcpclnt.Connect(ip, port);
                this.stm = this.tcpclnt.GetStream();
                connect = true;
            }
            catch (Exception e)
            {
                connect = false;
                Console.WriteLine(e);
                throw new Exception("There is aproblem with connecting to the server");
            }
        }
        public void Disconnect()
        {
            tcpclnt.GetStream().Close();
            tcpclnt.Close();
            tcpclnt = null;
        }

        public string Read()
        {
            if (tcpclnt != null)
            {
                // Time out of 10 seconds.
                tcpclnt.ReceiveTimeout = 10000;
                this.stm.ReadTimeout = 10000;
                // Only if the ReceiveBufferSize not empty so we want to convert the message to string and return it.
                if (tcpclnt.ReceiveBufferSize > 0)
                {
                    byte[] bb = new byte[tcpclnt.ReceiveBufferSize];
                    int k = this.stm.Read(bb, 0, 100);
                    string massage = "";
                    for (int i = 0; i < k; i++)
                    {
                        massage += (Convert.ToChar(bb[i]));
                    }
                    return massage;
                }
            }
            return "ERR";
        }
        public void Write(string command)
        {
            try
            {
                this.stm = this.tcpclnt.GetStream();
                ASCIIEncoding asen = new ASCIIEncoding();
                byte[] ba = asen.GetBytes(command);

                stm.Write(ba, 0, ba.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine("The sever is stoped");
                Thread.Sleep(2000);
            }
        }
        private bool IsConnect()
        {
            return connect;
        }
    }
}
