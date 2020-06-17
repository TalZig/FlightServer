using FlightMobileAppServer.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace FlightServer.Models
{
    public class FlightGearClient
    {
        ITCPClient client;
        private readonly BlockingCollection<AsyncCommand> _queue;
        private bool readSucceed;

        public const string WriteObjectDisposedException = "The server has been " +
            "closed. Please check your connection";
        public const string WriteInvalidOperationException = "The server is not " +
            "connected to a remote host. Please check your connection";
        public const string WriteIOException = "An error occurred when accessing" +
            " the socket. Please check your connection";
        public const string ReadObjectDisposedException = "The NetworkStream is " +
            "closed. Please check your connection";
        public const string ReadInvalidOperationException = "The NetworkStream does" +
            " not support reading. Please check your connection";
        public const string ReadTimeoutException = "The operation has timed-out. " +
            "Please wait few seconds";
        public const string ReadIOException = "An error occurred when accessing the" +
            " socket. Please check your connection";
        public const string RegularException = "Something got wrong. Please check " +
            "your connection";
        public const string EverythingIsGood = "Ok";

        public const string ThrottleLocation = "/controls/engines/current-engine/" +
            "throttle";
        public const string ElevatorLocation = "/controls/flight/elevator";
        public const string AileronLocation = "/controls/flight/aileron";
        public const string RudderLocation = "/controls/flight/rudder";

        // ShouldStop for ShouldStop the thread in the staet method

        public FlightGearClient(ITCPClient tcpClient)
        {
            _queue = new BlockingCollection<AsyncCommand>();
            this.client = tcpClient;
            Start();
        }

        // Called by the WebApi Controller, it will await on the returned Task<>
        // This is not an async method, since it does not await anything.
        public Task<Result> Execute(Command cmd)
        {
            var asyncCommand = new AsyncCommand(cmd);
            _queue.Add(asyncCommand);
            return asyncCommand.Task;
        }

        //create connection with the server(simulator)
        // catch the exp if the server ip or port does not exsit 
        public void Connect(string ip, int port)
        {
            this.client.Connect(ip, port);
            Start();
        }

        // ShouldStop the thread and log out
        public void Disconnect()
        {
            this.client.Disconnect();
        }

        public void Start()
        {
            Task.Factory.StartNew(ProcessCommands);
        }
        // Function that call to read function of tcpClient and catches all 
        // The exceptions that can happen.
        private string ReadFromServer()
        {
            try
            {
                string strFromServer = client.Read();
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
                if (e.Message.Contains("Unable to read data from the transport " +
                    "connection: A connection attempt failed because the connected" +
                    " party did not properly respond after a period of time, or" +
                    " established connection failed because connected host has " +
                    "failed to respond."))
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

        // Function that call to write function of tcpClient and catches all 
        // The exceptions that can happen.
        private string WriteToServer(string variable)
        {
            try
            {
                client.Write(variable);
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

        public void ProcessCommands()
        {
            while (!client.IsConnect())
            {
                foreach (AsyncCommand command in _queue.GetConsumingEnumerable())
                {
                    OneIterationOfProcessCommands(command);
                }
            }            
        }

        private void OneIterationOfProcessCommands(AsyncCommand command)
        {
            string aileronAction = OneActionOfWriteAndRead(AileronLocation, 
                command.Command.Aileron);
            string elevatorAction = OneActionOfWriteAndRead(ElevatorLocation, 
                command.Command.Elevator);
            string rudderAction = OneActionOfWriteAndRead(RudderLocation, 
                command.Command.Rudder);
            string throttleAction = OneActionOfWriteAndRead(ThrottleLocation, 
                command.Command.Throttle);
            Result res;
            if (CheckReturnOfAction(aileronAction, elevatorAction, 
                rudderAction, throttleAction))
            {
                res = Result.Ok;
            }
            else
            {
                res = Result.NotOk;
            }
            command.Completion.SetResult(res);
        }

        private bool CheckReturnOfAction(string aileronAction, string elevatorAction, 
            string rudderAction, string throttleAction)
        {
            if (aileronAction != EverythingIsGood) { return false; }
            if (elevatorAction != EverythingIsGood) { return false; }
            if (rudderAction != EverythingIsGood) { return false; }
            if (throttleAction != EverythingIsGood) { return false; }
            return true;
        }

        private string OneActionOfWriteAndRead(string locationOfVariable, 
            double valueOfVariable)
        {
            string messageToServerWithSet = RequestFromServer(true, locationOfVariable, 
                valueOfVariable);
            string statusOfWriteToServer = WriteToServer(messageToServerWithSet);
            if (statusOfWriteToServer != EverythingIsGood) 
            { 
                return statusOfWriteToServer; 
            }
            string messageToServerInGet = RequestFromServer(false, locationOfVariable, 
                valueOfVariable);
            client.Write(messageToServerInGet);
            string statusOfReadFromServer = ReadFromServer();
            if (!IsValidInput(statusOfReadFromServer, valueOfVariable)) 
            { 
                return statusOfReadFromServer; 
            }
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