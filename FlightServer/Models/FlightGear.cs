using FlightMobileAppServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlightServer.Models
{
    public class FlightGear
    {
        Dictionary<string, string> valuesAndPlaces = new Dictionary<string, string>();
        Queue<string> setReqeusts;
        ITCPClient tcp;
        string ipTcp;
        int portTcp;

        public FlightGear(ITCPClient server)
        {        
            tcp = server;
            portTcp = 5402;     
            ipTcp = "127.0.0.1";
            setReqeusts = new Queue<string>();
            tcp.Connect(ipTcp, portTcp);
            tcp.Write("data\n");
            tcp.Read();
            //new thread of Update etc.
        }

    

        public void Start()
        {
            try
            {
                tcp.Connect(ipTcp, portTcp);
            }
            catch (Exception)
            {
                Console.WriteLine("Problem in connect to tcp");
            }
        }

        public void setValues(Command command)
        {
            setReqeusts.Enqueue(setRequest("aileron", command.Aileron));
            setReqeusts.Enqueue(setRequest("rudder", command.Rudder));
            setReqeusts.Enqueue(setRequest("elevator", command.Elevator));
            setReqeusts.Enqueue(setRequestThrottle("throttle", command.Throttle));
        }

        public bool UpdateTcpSetValues(Command command)
        {             
            string temp;
            while (setReqeusts.Count > 0)
            {
                temp = setReqeusts.Dequeue();
                tcp.Write(temp);
                string strRead = tcp.Read();
                if(!IsValidInput(strRead, temp)) { return false; }
                
            }
            return true;
            //while there is no values in queue then send set request to flightGear
        }

        public bool IsValidInput(string strRead, string strWrite)
        {
            if (strRead == "ERR")
            {
                return false;
            }
            string[] arr = strWrite.Split(null);
            string strCompare = arr[2] + "\n";
            if (strCompare != strRead)
            {
                return false;
            }         
            return true;
        }

        public string setRequest(string variable, double val)
        {
            string str = "set /controls/flight/" + variable + " " + val;
            str += " \n";
            return str;
        }

        public string setRequestThrottle(string variable, double val)
        {
            string str = "set /controls/engines/current-engine/" + variable + " " + val;
            str += "\n";
            return str;
        }

        public bool checkSuccessInPost(Command command)
        {
            // Get from the FlightGear the 4 values. recieve.
            // Check that the values correct.
            // If not - return false.
            return true;
        }
    }
}