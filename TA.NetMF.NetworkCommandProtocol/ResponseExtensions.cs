// This file is part of the TA.NetMF.NetworkCommandProtocol project
// 
// Copyright © 2014 TiGra Networks, all rights reserved.
// 
// File: ResponseExtensions.cs  Created: 2014-06-06@06:54
// Last modified: 2014-12-13@22:32 by Tim

using System.Text;

namespace TA.NetMF.NetworkCommandProtocol
    {
    /// <summary>
    ///   Class ResponseExtensions. Produces responses to be transmitted to the client.
    /// </summary>
    internal static class ResponseExtensions
        {
        public static string ToPayloadString(this Response response)
            {
            var builder = new StringBuilder();
            builder.Append('!');
            builder.AppendLine(response.TransactionId.ToString());
            foreach (var key in response.Payload.Keys)
                {
                builder.Append(key);
                builder.Append(" = ");
                builder.AppendLine(response.Payload[key].ToString());
                }
            builder.AppendLine("END");
            return builder.ToString();
            }
        }
    }
