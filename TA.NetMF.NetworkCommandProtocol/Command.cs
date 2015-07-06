// This file is part of the TA.NetMF.NetworkCommandProtocol project
// 
// Copyright © 2014 TiGra Networks, all rights reserved.
// 
// File: Command.cs  Created: 2014-06-06@07:38
// Last modified: 2014-12-13@22:32 by Tim

using System.Text;

namespace TA.NetMF.NetworkCommandProtocol
    {
    /// <summary>
    ///   Struct Command - immutable. Represents a valid command received from a client.
    /// </summary>
    public struct Command
        {
        public const int NoPosition = -1;
        static readonly Command EmptyCommand = new Command(null, 0, null, null, 0, null);

        public Command(string deviceAddress, int transaction, string verb, string data, int position, string source)
            : this()
            {
            DeviceId = deviceAddress;
            TransactionId = transaction;
            Verb = verb;
            Payload = data;
            Position = position;
            Source = source;
            }

        public static Command Invalid { get { return EmptyCommand; } }
        public string Source { get; private set; }
        public string DeviceId { get; private set; }
        public int TransactionId { get; private set; }
        public string Verb { get; private set; }
        public int Position { get; private set; }
        public string Payload { get; private set; }
        public bool HasPayload { get { return (Payload != null && Payload.Length > 0); } }
        

        public override string ToString()
            {
            var builder = new StringBuilder();
            builder.Append("DeviceId=");
            builder.Append(DeviceId);
            builder.Append(" TransactionId=");
            builder.Append(TransactionId);
            builder.Append(" Verb=");
            builder.Append(Verb);
            builder.Append(" Payload=");
            builder.Append(Payload);
            return builder.ToString();
            }
        }
    }
