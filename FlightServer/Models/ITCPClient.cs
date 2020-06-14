using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightServer.Models
{
    public interface ITCPClient
    {
        void Connect(string ip, int port);
        void Write(string command);
        string Read(); // blocking call
        void Disconnect();
    }
}
