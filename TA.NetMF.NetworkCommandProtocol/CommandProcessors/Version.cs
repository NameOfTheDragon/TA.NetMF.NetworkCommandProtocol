// This file is part of the TA.NetMF.NetworkCommandProtocol project
// 
// Copyright © 2014 TiGra Networks, all rights reserved.
// 
// File: Version.cs  Created: 2014-06-12@19:30
// Last modified: 2014-12-13@22:32 by Tim

using System;
using System.Reflection;

namespace TA.NetMF.NetworkCommandProtocol.CommandProcessors
    {
    internal class Version : ICommandProcessor
        {
        public Version(string deviceAddress)
            {
            DeviceAddress = deviceAddress;
            }

        /// <summary>
        ///   Gets the device address that this command processor operates on.
        /// </summary>
        /// <value>The device address.</value>
        public string DeviceAddress { get; private set; }

        /// <summary>
        ///   Gets the command verb that this command processor handles.
        /// </summary>
        /// <value>The verb.</value>
        public string Verb { get { return Protocol.CmdGetFirmwareVersion; } }

        /// <summary>
        ///   Executes the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>A <see cref="Response" /> structure containing either the command's response, or an error response.</returns>
        /// <exception cref="ArgumentException">An inappropriate command was passed to the command processor</exception>
        public Response Execute(Command command)
            {
            if (command.Verb != Verb)
                throw new ArgumentException("An inappropriate command was passed to the command processor");
            if (command.HasPayload)
                return ErrorResponse(command, "Version is read-only");
            var assembly = Assembly.GetExecutingAssembly();
            var assemblyName = assembly.GetName();
            var assemblyVersion = assemblyName.Version;
            var versionString = assemblyVersion.ToString();
            var builder = new ResponseBuilder(command);
            builder.AddPayloadItem("Version", versionString);
            return builder.ToResponse();
            }

        Response ErrorResponse(Command command, string errorText)
            {
            var builder = new ResponseBuilder(command);
            builder.AddPayloadItem("Error", errorText);
            return builder.ToResponse();
            }
        }
    }
