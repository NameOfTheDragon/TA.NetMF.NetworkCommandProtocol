// This file is part of the TA.NetMF.NetworkCommandProtocol project
// 
// Copyright © 2014 TiGra Networks, all rights reserved.
// 
// File: Temperature.cs  Created: 2014-10-19@02:09
// Last modified: 2014-12-13@22:32 by Tim

using TA.NetMF.NetworkCommandProtocol.CommandTargets;

namespace TA.NetMF.NetworkCommandProtocol.CommandProcessors
    {
    internal class Temperature : ICommandProcessor
        {
        readonly string deviceAddress;
        readonly TemperatureProbe temperatureProbe;

        public Temperature(string deviceAddress, TemperatureProbe temperatureProbe)
            {
            this.deviceAddress = deviceAddress;
            this.temperatureProbe = temperatureProbe;
            }

        public string DeviceAddress { get { return deviceAddress; } }
        public string Verb { get { return "Temperature"; } }

        public Response Execute(Command command)
            {
            var temperatureC = temperatureProbe.Temperature;
            var temperatureF = 1.8*temperatureC + 32;
            var temperatureK = temperatureC + 273;
            var builder = new ResponseBuilder(command);
            builder.AddPayloadItem("Celsius", temperatureC.ToString());
            builder.AddPayloadItem("Farenheit", temperatureF.ToString());
            builder.AddPayloadItem("Kelvin", temperatureK.ToString());
            return builder.ToResponse();
            }
        }
    }
