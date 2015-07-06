// This file is part of the TA.NetMF.NetworkCommandProtocol project
// 
// Copyright © 2015 Tigra Networks., all rights reserved.
// 
// File: Dbg.cs  Last modified: 2015-07-06@02:22 by Tim Long

using System.Collections;
using System.Diagnostics;
using System.Text;
using Microsoft.SPOT;

namespace TA.NetMF.NetworkCommandProtocol.Diagnostics
    {
    internal static class Dbg
        {
        static readonly IDictionary traceSwitches = new Hashtable();

        public static bool AutoEnable { get; set; }

        static Dbg()
            {
            AutoEnable = true;
            }

        [Conditional("DEBUG")]
        public static void Trace(string message, Source source)
            {
            if (!traceSwitches.Contains(source))
                {
                if (AutoEnable)
                    {
                    traceSwitches.Add(source, true);
                    }
                else
                    return;
                }
            var enabled = (bool) traceSwitches[source];
            if (!enabled)
                return;
            var builder = new StringBuilder();
            var sourceLength = source.Name.Length;
            builder.Append('[');
            builder.Append(source.Name);
            if (sourceLength < Source.LongestSource)
                builder.Append(new string(' ', Source.LongestSource - sourceLength));
            builder.Append(']');
            builder.Append(' ');
            builder.Append(message);
            Debug.Print(builder.ToString());
            }

        [Conditional("DEBUG")]
        static void SetSource(Source source, bool enabled)
            {
            traceSwitches[source] = enabled; // Hashtable allows overwrite
            }
        }
    }