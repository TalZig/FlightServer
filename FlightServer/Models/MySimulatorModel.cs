﻿using FlightMobileAppServer.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace FlightServer.Models
{
    public class MySimulatorModel
    {
        ITCPClient client;
        private readonly BlockingCollection<AsyncCommand> _queueCommand;
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

        public MySimulatorModel(ITCPClient tcpClient)
        {
            _queueCommand = new BlockingCollection<AsyncCommand>();
            this.client = tcpClient;
            Start();
        }

        // Called by the WebApi Controller, it will await on the returned Task<>
        // This is not an async method, since it does not await anything.
        public Task<Result> Execute(Command cmd)
        {
            var asyncCommand = new AsyncCommand(cmd);
            _queueCommand.Add(asyncCommand);
            return asyncCommand.Task;
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
                readSucceed = false;
                return ReadObjectDisposedException;
            }
            catch (InvalidOperationException)
            {
                readSucceed = false;
                return ReadInvalidOperationException;
            }
            catch (TimeoutException)
            {
                readSucceed = false;
                return ReadTimeoutException;
            }
            catch (IOException e)
            {
                readSucceed = false;
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
                readSucceed = false;
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
            while (client.IsConnect())
            {
                foreach (AsyncCommand command in _queueCommand.GetConsumingEnumerable())
                {
                    OneIterationOfProcessCommands(command);
                }
            }            
        }

        private void OneIterationOfProcessCommands(AsyncCommand command)
        {
            Result aileronAction = OneActionOfWriteAndRead(AileronLocation, 
                command.Command.Aileron);
            if (!CheckReturnOfAction(aileronAction, command)) { return; }
            Result elevatorAction = OneActionOfWriteAndRead(ElevatorLocation, 
                command.Command.Elevator);
            if (!CheckReturnOfAction(elevatorAction, command)) { return; }
            Result rudderAction = OneActionOfWriteAndRead(RudderLocation, 
                command.Command.Rudder);
            if (!CheckReturnOfAction(rudderAction, command)) { return; }
            Result throttleAction = OneActionOfWriteAndRead(ThrottleLocation, 
                command.Command.Throttle);
            if (!CheckReturnOfAction(throttleAction, command)) { return; }
            command.Completion.SetResult(Result.Ok);
        }
        private bool CheckReturnOfAction(Result oneAction, AsyncCommand command)
        {
            if (oneAction != Result.Ok) 
            {
                command.Completion.SetResult(oneAction);
                return false; 
            }
            return true;
        }

        private Result SetResultAccordingToException(string exceptionToChange)
        {
            if (exceptionToChange == WriteObjectDisposedException)
            {
                return Result.WriteObjectDisposedException;
            }
            if (exceptionToChange == WriteInvalidOperationException)
            {
                return Result.WriteInvalidOperationException;
            }
            if (exceptionToChange == ReadObjectDisposedException)
            {
                return Result.ReadObjectDisposedException;
            }
            if (exceptionToChange == ReadInvalidOperationException)
            {
                return Result.ReadInvalidOperationException;
            }
            if (exceptionToChange == ReadTimeoutException)
            {
                return Result.ReadTimeoutException;
            }
            if (exceptionToChange == ReadIOException)
            {
                return Result.ReadIOException;
            }
            if (exceptionToChange == RegularException)
            {
                return Result.RegularException;
            }
            return Result.Ok;

        }

        private Result OneActionOfWriteAndRead(string locationOfVariable, 
            double valueOfVariable)
        {
            string messageToServerWithSet = RequestFromServer(true, locationOfVariable, 
                valueOfVariable);
            string statusOfWriteToServer = WriteToServer(messageToServerWithSet);
            if (statusOfWriteToServer != EverythingIsGood) 
            { 
                return SetResultAccordingToException(statusOfWriteToServer); 
            }
            string messageToServerInGet = RequestFromServer(false, locationOfVariable, 
                valueOfVariable);
            client.Write(messageToServerInGet);
            string statusOfReadFromServer = ReadFromServer();
            if (!IsValidInput(statusOfReadFromServer, valueOfVariable)) 
            { 
                return SetResultAccordingToException(statusOfReadFromServer); 
            }
            return Result.Ok;
        }
        private bool IsValidInput(string strRead, double valueFromJSON)
        {
            if (!readSucceed) { return false; }
            double numberOfRead;
            try
            {
                numberOfRead = Double.Parse(strRead);
            }
            catch (Exception)
            {
                return false;
            }
            if (valueFromJSON != numberOfRead)
            {
                return false;
            }
            return true;
        }

        private string RequestFromServer(bool isSet, string locationInServer, double val)
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