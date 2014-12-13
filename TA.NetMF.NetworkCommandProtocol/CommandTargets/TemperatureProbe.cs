// This file is part of the TA.NetMF.NetworkCommandProtocol project
// 
// Copyright © 2014 TiGra Networks, all rights reserved.
// 
// File: TemperatureProbe.cs  Created: 2014-10-19@01:52
// Last modified: 2014-12-13@22:32 by Tim

using TA.NetMF.NetworkCommandProtocol.CommandProcessors;

namespace TA.NetMF.NetworkCommandProtocol.CommandTargets
    {
    internal class TemperatureProbe : ICommandTarget
        {
        readonly string deviceAddress;

        public TemperatureProbe(string deviceAddress)
            {
            this.deviceAddress = deviceAddress;
            }

        internal double Temperature { get { return 12.5; } }

        public ICommandProcessor[] GetCommandProcessors()
            {
            var temperature = new Temperature(deviceAddress, this);
            var processors = new ICommandProcessor[] {temperature};
            return processors;
            }
        }
    }
