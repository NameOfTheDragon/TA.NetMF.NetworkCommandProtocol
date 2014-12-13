// This file is part of the TA.NetMF.NetworkCommandProtocol project
// 
// Copyright © 2014 TiGra Networks, all rights reserved.
// 
// File: ResponseBuilder.cs  Created: 2014-06-12@20:08
// Last modified: 2014-12-13@22:32 by Tim

using System.Collections;

namespace TA.NetMF.NetworkCommandProtocol
    {
    /// <summary>
    ///   Class ResponseBuilder - a factory class to assist with building responses.
    ///   Once a response is built, it is immutable.
    /// </summary>
    public class ResponseBuilder
        {
        readonly Hashtable payloadItems = new Hashtable(5);
        readonly int transaction = -1;

        public ResponseBuilder(Command command)
            {
            transaction = command.TransactionId;
            }

        public void AddPayloadItem(string key, string value)
            {
            payloadItems.Add(key, value);
            }

        public Response ToResponse()
            {
            return new Response(transaction, payloadItems);
            }
        }
    }
