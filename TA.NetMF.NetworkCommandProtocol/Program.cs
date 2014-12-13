// This file is part of the TA.NetMF.NetworkCommandProtocol project
// 
// Copyright © 2014 TiGra Networks, all rights reserved.
// 
// File: Program.cs  Created: 2014-06-05@17:39
// Last modified: 2014-12-13@22:32 by Tim

using System;
using Microsoft.SPOT;
using TA.NetMF.NetworkCommandProtocol.CommandTargets;
using TA.NetMF.NetworkCommandProtocol.NetworkServer;

namespace TA.NetMF.NetworkCommandProtocol
    {
    public class Program
        {
        const int RxTxBufferSize = 1024;

        public static void Main()
            {
            ConfigureCommandTargets();
            while (true)
                {
                try
                    {
                    Server.ListenForConnections(); // should never return.
                    }
                catch (Exception ex)
                    {
                    Debug.Print("Exception caught in Main loop (attempting to continue):");
                    Debug.Print(ex.ToString());
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
