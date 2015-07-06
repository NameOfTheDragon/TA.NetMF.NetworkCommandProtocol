// This file is part of the TA.NetMF.NetworkCommandProtocol project
// 
// Copyright © 2014 TiGra Networks, all rights reserved.
// 
// File: CommandParser.cs  Created: 2014-06-05@18:01
// Last modified: 2014-12-13@22:32 by Tim

 //using Microsoft.SPOT;

using System;
using System.Text.RegularExpressions;

namespace TA.NetMF.NetworkCommandProtocol
    {
    internal class CommandParser
        {
        /*
         * Regex matches command strings in format "<Dn,TT,CommandVerb=Payload>
         * D is the Device class, always 'F' for this driver.
         * n is the device index, always '1' for this driver.
         * TT is a numeric transaction ID of at least 2 digits.
         * CommandVerb is, obviously, the command verb ;-)
         * Payload is optional and is used to supply any parameter values to the command.
         * 
         * N.B. Micro Framework doesn't support named captures and will throw exceptions if they are used.
         */

        const string CommandRegex = @"<(\w\d),(\d+),([A-Za-z]\w+)(=((\d+)|(.+)))?>";
        static readonly Regex Parser = new Regex(CommandRegex);

        internal static Command ParseCommand(string text)
            {
            var matches = Parser.Match(text);
            if (!matches.Success)
                throw new ArgumentException("Invalid command format");
            /* Command with positional data has 6 groups
             * Group[0] contains [<F1,234,Move=12345>]
             * Group[1] contains [F1]
             * Group[2] contains [234]
             * Group[3] contains [Move]
             * Group[4] contains [=12345]
             * Group[5] contains [12345]
             * Group[6] contains [12345]  
             * -----
             * Command without positional data has 5 groups
             * Group[0] contains [<F1,234,Nickname=Fred>]
             * Group[1] contains [F1]
             * Group[2] contains [234]
             * Group[3] contains [Nickname]
             * Group[4] contains [=Fred]
             * Group[5] contains [Fred]
             */
            var deviceAddress = matches.Groups[1].Value;
            var transaction = int.Parse(matches.Groups[2].Value);
            var verb = matches.Groups[3].Value;
            var position = Command.NoPosition;
            var data = string.Empty;
            if (matches.Groups.Count >= 6 && matches.Groups[6].Success)
                position = int.Parse(matches.Groups[6].Value);
            if (matches.Groups[5].Success)
                data = matches.Groups[5].Value;
            var source = matches.Groups[0].Success ? matches.Groups[0].Value : text;
            var command = new Command(deviceAddress, transaction, verb, data, position, source);
            return command;
            }
        }
    }
