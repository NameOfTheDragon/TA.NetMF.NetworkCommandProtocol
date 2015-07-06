// This file is part of the TA.NetMF.NetworkCommandProtocol project
// 
// Copyright © 2015 Tigra Networks., all rights reserved.
// 
// File: Source.cs  Last modified: 2015-07-06@02:22 by Tim Long

namespace TA.NetMF.NetworkCommandProtocol.Diagnostics
    {
    /// <summary>
    ///     Class Source. An immutable type representing a source of diagnostic trace data.
    ///     Instances of this class can only be obtained using the static readonly fields containing
    ///     pre-built instances.
    /// </summary>
    public sealed class Source
        {
        public static readonly Source ConnectionHandler = new Source("Connection Handler");
        public static readonly Source NetworkServer = new Source("Network Server");
        public static readonly Source Dispatcher = new Source("Dispatcher");
        public static readonly Source Unspecified = new Source("Unspecified");
        public static readonly Source CommandProcessor = new Source("Cmd Processor");
        readonly string name;

        Source(string name)
            {
            this.name = name;
            var length = name.Length;
            if (length > LongestSource)
                LongestSource = length;
            }

        public string Name
        {
            get { return name; }
        }

        /// <summary>
        ///     Gets the length in characters of the longest source name.
        /// </summary>
        /// <value>The length of the longest source.</value>
        public static int LongestSource { get; private set; }
        }
    }