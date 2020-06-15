using FlightMobileAppServer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace FlightServer.Models
{
    public class FlightGearClient
    {
        Queue<string> setReqeusts;
        ITCPClient tcpClient;
        string ipTcp;
        int portTcp;
        bool readSucceed;

        public const string WriteObjectDisposedException = "The server has been closed. Please check your connection";
        public const string WriteInvalidOperationException = "The server is not connected to a remote host. Please check your connection";
        public const string WriteIOException = "An error occurred when accessing the socket. Please check your connection";
        public const string ReadObjectDisposedException = "The NetworkStream is closed. Please check your connection";
        public const string ReadInvalidOperationException = "The NetworkStream does not support reading. Please check your connection";
        public const string ReadTimeoutException = "The operation has timed-out. Please wait few seconds";
        public const string ReadIOException = "An error occurred when accessing the socket. Please check your connection";
        public const string RegularException = "Something got wrong. Please check your connection";
        public const string EverythingIsGood = "Ok";



        public FlightGearClient(ITCPClient server)
        {
            readSucceed = false;
            tcpClient = server;
            portTcp = 5402;
            ipTcp = "127.0.0.1";
            setReqeusts = new Queue<string>();
            //new thread of Update etc.
        }

        // Function that call to read function of tcpClient and catches all the exceptions that can happen.
        private string ReadFromServer()
        {
            try
            {
                string strFromServer = tcpClient.Read();
                readSucceed = true;
                return strFromServer;
            }
            catch (ObjectDisposedException)
            {
                return ReadObjectDisposedException;
            }
            catch (InvalidOperationException)
            {
                return ReadInvalidOperationException;
            }
            catch (TimeoutException)
            {
                return ReadTimeoutException;
            }
            catch (IOException e)
            {
                string msg;
                // Sometimes there is timeout but this exception belongs to IOException.
                if (e.Message.Contains("Unable to read data from the transport connection: " +
                    "A connection attempt failed because the connected party did not properly " +
                    "respond after a period of time, or established connection failed because " +
                    "connected host has failed to respond."))
                {
                    msg = ReadTimeoutException;
                }
                else
                {
                    // Regular IOException.
                    msg = ReadIOException;
                }
                return msg;
            }
            catch (Exception)
            {
                return RegularException;
            }
        }

        // Function that call to write function of tcpClient and catches all the exceptions that can happen.
        private string WriteToServer(string variable)
        {
            try
            {
                tcpClient.Write(variable);
                return EverythingIsGood;
            }
            catch (ObjectDisposedException)
            {
                return WriteObjectDisposedException;
            }
            catch (InvalidOperationException)
            {
                return WriteInvalidOperationException;
            }
            catch (IOException)
            {
                return WriteIOException;
            }
            catch (Exception)
            {
                return RegularException;
            }
        }


        public void Start()
        {
            try
            {
                tcpClient.Connect(ipTcp, portTcp);
            }
            catch (Exception)
            {
                Console.WriteLine("Problem in connect to tcp");
            }
        }

        public string UpdateTcpSetValues(string locationOfVariable, double valueOfVariable)
        {
            Start();
            string messageToServerWithSet = RequestFromServer(true, locationOfVariable, valueOfVariable);
            string statusOfWriteToServer = WriteToServer(messageToServerWithSet);
            if (statusOfWriteToServer != EverythingIsGood) { return statusOfWriteToServer; }
            string messageToServerInGet = RequestFromServer(false, locationOfVariable, valueOfVariable);
            tcpClient.Write(messageToServerInGet);
            string statusOfReadFromServer = ReadFromServer();
            if (!IsValidInput(statusOfReadFromServer, valueOfVariable)) { return statusOfReadFromServer; }
            return EverythingIsGood;
        }

        public bool IsValidInput(string strRead, double valueFromJSON)
        {
            if (!readSucceed) { return false; }
            if (valueFromJSON != Double.Parse(strRead))
            {
                return false;
            }
            return true;
        }


        public string RequestFromServer(bool isSet, string locationInServer, double val)
        {
            string messageToServer;
            if (isSet)
            {
                messageToServer = "set ";
                messageToServer += locationInServer + " " + val;
            }
            else
            {
                messageToServer = "get ";
                messageToServer += locationInServer;
            }
            messageToServer += "\r\n";
            return messageToServer;
        }
    }
}