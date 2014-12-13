// This file is part of the TA.NetMF.NetworkCommandProtocol project
// 
// Copyright © 2014 TiGra Networks, all rights reserved.
// 
// File: ICommandProcessor.cs  Created: 2014-06-12@19:24
// Last modified: 2014-12-13@22:32 by Tim

namespace TA.NetMF.NetworkCommandProtocol.CommandProcessors
    {
    /// <summary>
    ///   Interface ICommandProcessor.
    ///   There should be an ICommandProcessor instance for each command supported by each device.
    /// </summary>
    public interface ICommandProcessor
        {
        /// <summary>
        ///   Gets the device address that this command processor operates on.
        /// </summary>
        /// <value>The device address.</value>
        string DeviceAddress { get; }

        /// <summary>
        ///   Gets the command verb that this command processor handles.
        /// </summary>
        /// <value>The verb.</value>
        string Verb { get; }

        /// <summary>
        ///   Executes the specified command and returns an appropriate response.
        ///   If the command can be fully and correctly executed, then a <see cref="Response" /> structure should be returned
        ///   (responses can be conveniently built using the <see cref="ResponseBuilder" /> class).
        ///   If the command cannot be executed, then a <see cref="CommandException" /> should be thrown. This type of exception
        ///   will be caught by the dispatcher and converted into an error response.
        /// </summary>
        /// <param name="command">
        ///   The command to be executed. It is assumed that the dispatcher has already verified that the
        ///   device address and verb are correct for this command processor.
        /// </param>
        /// <returns>A <see cref="Response" /> structure containing either the command's response, or an error response.</returns>
        /// <exception cref="CommandException">
        ///   Thrown if the command cannot be successfully and fully completed. This exception type causes the details to be sent
        ///   back to the client
        ///   in an error response.
        /// </exception>
        Response Execute(Command command);
        }
    }
