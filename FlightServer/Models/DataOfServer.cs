﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightServer.Models
{
    public class DataOfServer
    {
       public string Ip { get; set; }
       public int Port { get; set; }
        public string HttpAddress { get; set; }
        /*public Host(string IpNew, string PortNew, string HttpAddressNew)
        {
            Ip = IpNew;
            Port = PortNew;
            HttpAddress = HttpAddressNew;
        }*/
    }
}
