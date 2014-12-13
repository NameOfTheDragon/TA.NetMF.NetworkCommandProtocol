// This file is part of the TA.NetMF.NetworkCommandProtocol project
// 
// Copyright © 2014 TiGra Networks, all rights reserved.
// 
// File: Server.cs  Created: 2014-06-14@15:25
// Last modified: 2014-12-13@22:32 by Tim

using System;
using System.Net;
using System.Net.Sockets;

namespace TA.NetMF.NetworkCommandProtocol.NetworkServer
    {
    internal static class Server
        {
        const int ListenPort = 3564;

        public static void ListenForConnections()
            {
            Socket socket = null;
            // This is essentially the main application loop
            while (true)
                {
                try
                    {
                    // Create & configure a new socket if we don't yet have one.
                    if (socket == null)
                        {
                        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        var endpoint = new IPEndPoint(IPAddress.Any, ListenPort);
                        socket.Bind(endpoint);
                        }
                    // Wait for a connection to come in.
                    socket.Listen(3);
                    // Accept the connection and hand it off to a worker thread.
                    // This leaves the original socket ready to accept a new connection.
                    AcceptConnection(socket);
                    }
                catch (SocketException ex)
                    {
                    // If anything went wrong with our socket, we simply throw it away and start again.
                    if (socket != null)
                        socket.Close();
                    socket = null;
                    }
                }
            }

        static void AcceptConnection(Socket socket)
            {
            var connection = socket.Accept(); // An error accepting the connection will propagate up.
            try
                {
                // The connection may be used to conduct zero or more requests.
                var handler = new ConnectionHandler(connection);
                handler.StartConnectionThread();
                }
            catch (SocketException ex)
                {
                // Any socket exception terminates the connection but not the listening socket.
                connection.Close();
                }
            catch (Exception ex)
                {
                // Any other (non-socket) exception terminates the request, but leaves teh connection open
                }
            }
        }
    }
