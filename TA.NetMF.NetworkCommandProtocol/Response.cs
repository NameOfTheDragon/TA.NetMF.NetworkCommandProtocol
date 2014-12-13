// This file is part of the TA.NetMF.NetworkCommandProtocol project
// 
// Copyright © 2014 TiGra Networks, all rights reserved.
// 
// File: Response.cs  Created: 2014-06-06@07:35
// Last modified: 2014-12-13@22:32 by Tim

using System;
using System.Collections;

namespace TA.NetMF.NetworkCommandProtocol
    {
    /// <summary>
    ///   Struct Response. Immutable. Represents the data for a response to the client.
    /// </summary>
    public struct Response
        {
        readonly IDictionary payloadItems;

        internal Response(int transaction, IDictionary payloadItems) : this()
            {
            TransactionId = transaction;
            this.payloadItems = payloadItems;
            }

        /// <summary>
        ///   Gets the transaction identifier.
        /// </summary>
        /// <value>The transaction identifier.</value>
        public int TransactionId { get; private set; }

        /// <summary>
        ///   Gets the payload (a collection of key-value pairs).
        /// </summary>
        /// <value>The payload.</value>
        public IDictionary Payload { get { return payloadItems; } }

        /// <summary>
        ///   Creates an error response from an exception.
        ///   The error code is the exception message.
        /// </summary>
        /// <param name="ex">The caught exception.</param>
        /// <returns>A <see cref="Response" /> populated with the error details.</returns>
        public static Response FromException(Exception ex)
            {
            return FromException(ex, Command.Invalid);
            }

        /// <summary>
        ///   Creates an error response from an exception and the command that gave rise to it.
        ///   The error code is the exception message.
        /// </summary>
        /// <param name="ex">The caught exception.</param>
        /// <param name="command">The command that gave rise to the exception.</param>
        /// <returns>A <see cref="Response" /> populated with the error details and information about the command.</returns>
        public static Response FromException(Exception ex, Command command)
            {
            var builder = new ResponseBuilder(command);
            builder.AddPayloadItem("Error", ex.Message);
            builder.AddPayloadItem("Device", command.DeviceId);
            builder.AddPayloadItem("Verb", command.Verb);
            if (command.HasPayload)
                builder.AddPayloadItem("Payload", command.Payload);
            return builder.ToResponse();
            }

        public static Response FromInvalidCommand(string commandFragment)
            {
            var builder = new ResponseBuilder(Command.Invalid);
            builder.AddPayloadItem("Error", "Unable to parse command ");
            builder.AddPayloadItem("Reason",
                "[" + commandFragment + "] is not in the correct format <Fn,t,Verb=Payload>");
            return builder.ToResponse();
            }

        /// <summary>
        ///   Creates an empty acknowledgement response from a command that successfully executed to completion.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>Response.</returns>
        public static Response FromSuccessfulCommand(Command command)
            {
            var builder = new ResponseBuilder(command);
            return builder.ToResponse();
            }
        }
    }
