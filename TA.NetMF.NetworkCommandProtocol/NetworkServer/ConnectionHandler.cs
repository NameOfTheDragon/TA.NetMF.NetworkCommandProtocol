// This file is part of the TA.NetMF.NetworkCommandProtocol project
// 
// Copyright � 2014 TiGra Networks, all rights reserved.
// 
// File: ConnectionHandler.cs  Created: 2014-06-14@15:25
// Last modified: 2014-12-13@22:32 by Tim

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Microsoft.SPOT;
using TA.NetMF.NetworkCommandProtocol.Diagnostics;

namespace TA.NetMF.NetworkCommandProtocol.NetworkServer
    {
    internal class ConnectionHandler
        {
        const int DefaultMaximumConnections = 5; // Maximum number of active sockets.
        const int RxTxBufferSize = 1024;
        readonly IPEndPoint client;
        readonly Socket connection;

        public ConnectionHandler(Socket connection)
            {
            this.connection = connection;
            client = connection.RemoteEndPoint as IPEndPoint;
            }

        internal void HandleClientRequests()
            {
            var workerThread = CreateConnectionThread();
            workerThread.Start();
            // To prevent multiple simultaneous client connections, uncomment the following line:
            // workerThread.Join();
            }

        Thread CreateConnectionThread()
            {
            ThreadStart connectionHandler = HandleRequestsThread;
            return new Thread(connectionHandler);
            }

        /// <summary>
        ///     Handles requests from the connected client until either the client closes the connection
        ///     or a Socket related error occurs.
        /// </summary>
        void HandleRequestsThread()
            {
            Dbg.Trace("Accepting requests from " + client, Source.ConnectionHandler);
            while (!SocketClosed())
                {
                try
                    {
                    HandleOneRequest();
                    }
                catch (SocketException ex)
                    {
                    // Any socket related problems result in the thread terminating and the socket being closed.
                    Dbg.Trace("Closing connection from " + client + " due to a socket exception", Source.ConnectionHandler);
                    if (connection != null)
                        connection.Close(); // This should never fail.
                    return;
                    }
                catch (Exception ex)
                    {
                    // Last ditch exception handler. 
                    // Any non-socket exception from a request results in the request being ignored.
                    Dbg.Trace("Exception handling client request: "+ex.Message, Source.ConnectionHandler);
                    }
                }
            }

        /// <summary>
        ///     Detects whether the socket has been closed, reset or terminated by the client.
        /// </summary>
        /// <returns><c>true</c> if the socket has been closed, reset or terminated, <c>false</c> otherwise.</returns>
        /// <remarks>
        ///     see: How to tell if a socket is closed http://stackoverflow.com/a/2556369/98516
        ///     Socket.Poll() returns true if data is available for reading or if the connection has been closed, reset, or
        ///     terminated.
        ///     Therefore if it returns true and there is no data available, it must have been closed, reset or terminated.
        /// </remarks>
        bool SocketClosed()
            {
            var socketClosed = connection.Poll(100, SelectMode.SelectRead) && connection.Available == 0;
            return socketClosed;
            }

        /// <summary>
        ///   Handles one or more requests from a connected socket.
        ///   The connection is left open until the client closes it.
        /// </summary>
        void HandleOneRequest()
            {
            var commandParsedSuccessfully = false;
            var responseSent = false;
            var receiveBuffer = new byte[RxTxBufferSize];
            var bytesReceived = connection.Receive(receiveBuffer);
            if (bytesReceived == 0)
                {
                Dbg.Trace("Received 0 bytes from " + client + ", assuming the socket is broken.", Source.ConnectionHandler);
                throw new SocketException(SocketError.ConnectionAborted);
                }
            var requestString = receiveBuffer.GetString();
            Dbg.Trace("Received [" + requestString.ExpandASCII() + "] from " + client, Source.ConnectionHandler);
            if (requestString.Length < 6)
                {
                Dbg.Trace("Request is too short to be valid - ignoring", Source.ConnectionHandler);
                return; // Too short to be valid, ignore it.
                }
            Command command;
            Response response;
            try
                {
                command = CommandParser.ParseCommand(requestString);
                }
            catch (ArgumentException ex)
                {
                response = Response.FromInvalidCommand(requestString);
                SendResponse(response);
                return;
                }
            try
                {
                response = CommandDispatcher.Dispatch(command);
                SendResponse(response);
                }
            catch (SocketException ex)
                {
                throw; // Handle this up at the server level.
                }
            catch (Exception ex)
                {
                // Command parsed OK but could not be dispatched. Create and send a generic error response.
                var error = Response.FromException(ex, command);
                SendResponse(error);
                }
            }

        void SendResponse(Response response)
            {
            var responseString = response.ToPayloadString();
            var bytes = Encoding.UTF8.GetBytes(responseString);
            connection.Send(bytes);
            }
        }
    }
