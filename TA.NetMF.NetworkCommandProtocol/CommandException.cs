// This file is part of the TA.NetMF.NetworkCommandProtocol project
// 
// Copyright © 2014 TiGra Networks, all rights reserved.
// 
// File: CommandException.cs  Created: 2014-06-14@12:31
// Last modified: 2014-12-13@22:32 by Tim

using System;
using System.Text;

namespace TA.NetMF.NetworkCommandProtocol
    {
    /// <summary>
    ///   Exception thrown when no command processor could be found to handle a command
    /// </summary>
    /// <remarks>
    ///   This exception does not have any custom properties,
    ///   thus it does not implement ISerializable.
    /// </remarks>
    [Serializable]
    internal class CommandException : Exception
        {
        /// <summary>
        ///   Initializes a new instance of the <see cref="CommandException" /> class.
        /// </summary>
        /// <param name="command">The attempted command that gave rise to the exception.</param>
        /// <param name="message">The message describing what went wrong.</param>
        public CommandException(Command command, string message) : base(FormatMessage(command, message)) {}

        /// <summary>
        ///   Formats the message to include the command details.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="message">The message.</param>
        /// <returns>System.String.</returns>
        static string FormatMessage(Command command, string message)
            {
            var builder = new StringBuilder(message);
            builder.Append(" [transaction=");
            builder.Append(command.TransactionId);
            builder.Append("; device=");
            builder.Append(command.DeviceId);
            builder.Append("; verb=");
            builder.Append(command.Verb);
            builder.Append("]");
            return builder.ToString();
            }
        }

    /// <summary>
    ///   Class InvalidCommandVerbException. This class cannot be inherited.
    ///   An exception used when the device rejects the command verb.
    /// </summary>
    internal sealed class InvalidCommandVerbException : CommandException
        {
        /// <summary>
        ///   Initializes a new instance of the <see cref="InvalidCommandVerbException" /> class.
        /// </summary>
        /// <param name="command">The attempted command that gave rise to the exception.</param>
        /// <param name="message">The message describing what went wrong.</param>
        public InvalidCommandVerbException(Command command, string message) : base(command, message) {}
        }

    /// <summary>
    ///   Class InvalidAddressException. This class cannot be inherited.
    ///   An exception thrown when the device address is invalid (no device recognizes the address).
    /// </summary>
    internal sealed class InvalidAddressException : CommandException
        {
        /// <summary>
        ///   Initializes a new instance of the <see cref="InvalidAddressException" /> class.
        /// </summary>
        /// <param name="command">The attempted command that gave rise to the exception.</param>
        /// <param name="message">The message describing what went wrong.</param>
        public InvalidAddressException(Command command, string message) : base(command, message) {}
        }
    }
