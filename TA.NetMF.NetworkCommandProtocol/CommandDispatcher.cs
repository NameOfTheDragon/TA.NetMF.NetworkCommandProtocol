// This file is part of the TA.NetMF.NetworkCommandProtocol project
// 
// Copyright © 2014 TiGra Networks, all rights reserved.
// 
// File: CommandDispatcher.cs  Created: 2014-06-12@20:15
// Last modified: 2014-12-13@22:32 by Tim

using System;
using System.Collections;
using System.Text;
using TA.NetMF.NetworkCommandProtocol.CommandProcessors;
using TA.NetMF.NetworkCommandProtocol.CommandTargets;
using TA.NetMF.NetworkCommandProtocol.Diagnostics;

namespace TA.NetMF.NetworkCommandProtocol
    {
    /// <summary>
    ///   Class CommandDispatcher. When commands arrive, they are dispatched to an <see cref="ICommandProcessor" /> that
    ///   can process the command. The command processors are searched based on the device address and command verb contained
    ///   in the <see cref="Command" /> structure. For a command to be considered valid, there must be an
    ///   <see cref="ICommandProcessor" />    instance with matching device address and command verb.
    /// </summary>
    internal static class CommandDispatcher
        {
        static readonly IList CommandProcessors = new ArrayList();

        /// <summary>
        ///   Dispatches the specified command and returns an appropriate response.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>Response.</returns>
        internal static Response Dispatch(Command command)
            {
            Dbg.Trace("Dispatching command: " + command, Source.Dispatcher);
            try
                {
                var processor = GetCommandProcessorForCommand(command);
                var response = processor.Execute(command);
                return response;
                }
            catch (InvalidAddressException ex)
                {
                // Device address not valid
                var response = Response.FromException(ex, command);
                response.Payload.Add("Valid addresses", GetValidDeviceAddresses());
                return response;
                }
            catch (InvalidCommandVerbException ex)
                {
                // The device address is OK but the device rejected the command verb
                var response = Response.FromException(ex, command);
                response.Payload.Add("Valid commands", GetValidCommandsForDevice(command.DeviceId));
                return response;
                }
            catch (CommandException ex)
                {
                return Response.FromException(ex, command);
                }
            }

        /// <summary>
        ///   Gets a command processor for the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>an <see cref="ICommandProcessor" /> that can process the command.</returns>
        /// <exception cref="CommandException">Thrown if no command processor could be found.</exception>
        /// <remarks>
        ///   Searches for a command processor that matches both the Device Address and Command Verb.
        /// </remarks>
        static ICommandProcessor GetCommandProcessorForCommand(Command command)
            {
            var deviceValid = false;
            foreach (ICommandProcessor candidate in CommandProcessors)
                {
                if (candidate.DeviceAddress != command.DeviceId)
                    continue;
                deviceValid = true; // Found at least 1 command processor with this device address.
                if (candidate.Verb.CaseInsensitiveEquals(command.Verb))
                    return candidate;
                }
            // The command can't be processed and we must raise an exception.
            if (deviceValid)
                throw new InvalidCommandVerbException(command, "The command is not valid for the addressed device");
            throw new InvalidAddressException(command, "No such device address");
            }

        /// <summary>
        ///   Registers a command target with the Command Dispatcher. The command target is expected to supply a list of
        ///   <see cref="ICommandProcessor" /> instances that handle its commands. Each <see cref="ICommandProcessor" />
        ///   instance must be unique, that is, the <see cref="ICommandProcessor.DeviceAddress" /> and
        ///   <see cref="ICommandProcessor.Verb" />
        ///   combination must be unique, otherwise an exception is thrown.
        /// </summary>
        /// <param name="target">The object that is to be the target of commands.</param>
        /// <exception cref="InvalidOperationException">Thrown if a duplicate command target is registered.</exception>
        internal static void RegisterCommandTarget(ICommandTarget target)
            {
            var processors = target.GetCommandProcessors();
            foreach (var commandProcessor in processors)
                {
                if (CommandProcessors.Contains(commandProcessor))
                    {
                    var builder = new StringBuilder();
                    builder.Append("Duplicate command processor registration. [device=");
                    builder.Append(commandProcessor.DeviceAddress);
                    builder.Append("; verb=");
                    builder.Append(commandProcessor.Verb);
                    builder.Append("]");
                    throw new InvalidOperationException(builder.ToString());
                    }
                CommandProcessors.Add(commandProcessor);
                }
            }

        /// <summary>
        ///   Unregisters all command targets. Primarily intended for unit testing.
        /// </summary>
        public static void UnregisterAllCommandTargets()
            {
            CommandProcessors.Clear();
            }

        static string GetValidDeviceAddresses()
            {
            var addressListBuilder = new StringBuilder();
            bool separatorRequired = false;
            var processedAddresses = new ArrayList();
            foreach (ICommandProcessor commandProcessor in CommandProcessors)
                {
                var deviceAddress = commandProcessor.DeviceAddress;
                if (!processedAddresses.Contains(deviceAddress))
                    {
                    if (separatorRequired)
                        addressListBuilder.Append(' ');
                    addressListBuilder.Append(deviceAddress);
                    processedAddresses.Add(deviceAddress);
                    separatorRequired = true;
                    }
                }
            return addressListBuilder.ToString();
            }

        static string GetValidCommandsForDevice(string deviceId)
            {
            var builder = new StringBuilder();
            bool separatorRequired = false;
            foreach (ICommandProcessor commandProcessor in CommandProcessors)
                {
                if (commandProcessor.DeviceAddress != deviceId)
                    continue;
                if (separatorRequired)
                    builder.Append(' ');
                builder.Append(commandProcessor.Verb);
                separatorRequired = true;
                }
            return builder.ToString();
            }
        }
    }
