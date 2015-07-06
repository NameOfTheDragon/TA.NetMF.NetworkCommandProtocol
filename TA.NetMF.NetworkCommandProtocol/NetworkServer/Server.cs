// This file is part of the TA.NetMF.NetworkCommandProtocol project
// 
// Copyright © 2014 TiGra Networks, all rights reserved.
// 
// File: Server.cs  Created: 2014-06-14@15:25
// Last modified: 2014-12-13@22:32 by Tim

using System;
using System.Net;
using System.Net.Sockets;
using TA.NetMF.NetworkCommandProtocol.Diagnostics;

namespace TA.NetMF.NetworkCommandProtocol.NetworkServer
    {
    internal static class Server
        {
        const int ListenPort = 3564;
        const int MaxConnectionBacklog = 1;

        public static void ListenForConnections()
            {
            Socket socket = null;
            /*
             * This is essentially the main application loop and its job is to keep a server socket alive and well.
             */
            while (true) // Server socket loop
                {
                try
                    {
                    // Create & configure a new socket if we don't yet have one.
                    if (socket == null)
                        {
                        Dbg.Trace("Creating new server socket", Source.NetworkServer);
                        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        var endpoint = new IPEndPoint(IPAddress.Any, ListenPort);
                        socket.Bind(endpoint);
                        Dbg.Trace("Socket successfully bound to " + endpoint.ToString(), Source.NetworkServer);
                        }
                    // Put the socket in Listen mode, making it a server socket.
                    socket.Listen(MaxConnectionBacklog);
                    AcceptClientConnections(socket);
                    }
                catch (SocketException ex)
                    {
                    /* 
                     * A SocketException here must relate to the server socket, because once a connection
                     * is accepted, that creates a new socket and exceptions on that will be handled on
                     * a worker thread. So, this means our server socket is broken
                     * and we should throw it away and start again with a new one.
                     */
                    Dbg.Trace("Server socket exception: " + ex.Message, Source.NetworkServer);
                    if (socket != null) socket.Close();
                    socket = null;
                    }
                catch (Exception ex)
                    {
                    /*
                     * Any other type of exception could be fatal, but we will try to keep going.
                     * We must close the server socket though, because it will be opened again at the
                     * top of the main loop.
                     */
                    Dbg.Trace("Exception in main server loop: " + ex.Message, Source.NetworkServer);
                    socket.Close();
                    }
                }
            }

        /// <summary>
        ///     Accepts client connections from the server socket and hands the connections off to a worker thread. Each
        ///     connection runs in its own thread.
        /// </summary>
        /// <param name="socket">The server socket which must be configured to listen for connections.</param>
        static void AcceptClientConnections(Socket socket)
            {
            while (true)
                {
                Dbg.Trace("Listening for client connections.", Source.NetworkServer);
                var clientConnection = socket.Accept(); // Blocks until a connection request is received. Allow exceptions to bubble up.
                Dbg.Trace("Client connection request; creating handler.", Source.NetworkServer);
                var handler = new ConnectionHandler(clientConnection);    // Create a handler for the client connection.
                try
                    {
                    /*
                     * The connection represents a single client and may be used to conduct
                     * zero or more requests on a worker thread. Each connected client uses up one worker thread.
                     */
                    handler.HandleClientRequests(); // Handles all communications with this client.
                    }
                catch (Exception ex)
                    {
                    // Any exception terminates the client connection and its request handler, but not the server socket.
                    Dbg.Trace("Exception starting client connection handler: " + ex.Message, Source.NetworkServer);
                    clientConnection.Close();
                    }
                finally
                    {
                    handler = null;
                    clientConnection = null;
                    }
                }
            }
        }
    }
