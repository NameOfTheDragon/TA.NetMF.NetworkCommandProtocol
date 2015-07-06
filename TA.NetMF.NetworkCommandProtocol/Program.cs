// This file is part of the TA.NetMF.NetworkCommandProtocol project
// 
// Copyright © 2014 TiGra Networks, all rights reserved.
// 
// File: Program.cs  Created: 2014-06-05@17:39
// Last modified: 2014-12-13@22:32 by Tim

using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Net.NetworkInformation;
using TA.NetMF.NetworkCommandProtocol.CommandTargets;
using TA.NetMF.NetworkCommandProtocol.Diagnostics;
using TA.NetMF.NetworkCommandProtocol.NetworkServer;

namespace TA.NetMF.NetworkCommandProtocol
    {
    public class Program
        {
        static ManualResetEvent networkAvailableEvent = new ManualResetEvent(false);
        const int RxTxBufferSize = 1024;

        public static void Main()
            {
            ConfigureCommandTargets();
            WaitForNetwork();
            while (true)
                {
                try
                    {
                    Server.ListenForConnections(); // should never return.
                    }
                catch (Exception ex)
                    {
                    Dbg.Trace("Exception caught in Main loop (attempting to continue):", Source.Unspecified);
                    Dbg.Trace(ex.ToString(), Source.Unspecified);
                    }
                }
            }

        static void WaitForNetwork()
            {
            NetworkChange.NetworkAvailabilityChanged += HandleNetworkAvailabilityChanged;
            networkAvailableEvent.WaitOne();
            WaitForValidIpAddress();
            }

        static void HandleNetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
            {
            if (e.IsAvailable)
                {
                Debug.Print("ConnectionHandler available");
                networkAvailableEvent.Set();
                }
            else
                {
                Debug.Print("ConnectionHandler down");
                networkAvailableEvent.Reset();
                }
            }

        static void WaitForValidIpAddress()
            {
            NetworkInterface[] networkInterfaces;
            Dbg.Trace("Waiting for valid IP address", Source.NetworkServer);
            do
                {
                Thread.Sleep(100);
                networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
                } while (networkInterfaces[0].IPAddress == "0.0.0.0");

            Dbg.Trace("Found " + networkInterfaces.Length + " network interfaces - details as follows...", Source.NetworkServer);
            for (int i = 0; i < networkInterfaces.Length; i++)
                {
                var nic = networkInterfaces[i];
                Dbg.Trace("Network interface: " + i, Source.NetworkServer);
                Dbg.Trace("  Interface type: " + nic.NetworkInterfaceType, Source.NetworkServer);
                Dbg.Trace("  IPv4 address: " + nic.IPAddress, Source.NetworkServer);
                Dbg.Trace("  Default gateway: " + nic.GatewayAddress, Source.NetworkServer);
                Dbg.Trace("  Subnet mask: " + nic.SubnetMask, Source.NetworkServer);

                var dnsServers = nic.DnsAddresses;
                Dbg.Trace("  DNS servers:", Source.NetworkServer);
                foreach (var dnsServer in dnsServers)
                    {
                    Dbg.Trace("    " + dnsServer, Source.NetworkServer);
                    }
                }
            }
        static void ConfigureCommandTargets()
            {
            // Add your command targets here.

            // Temperature probe (example)
            var probe = new TemperatureProbe("T1");
            CommandDispatcher.RegisterCommandTarget(probe);
            }
        }
    }
