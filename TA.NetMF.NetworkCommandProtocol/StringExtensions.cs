// This file is part of the TA.NetMF.NetworkCommandProtocol project
// 
// Copyright © 2014 TiGra Networks, all rights reserved.
// 
// File: StringExtensions.cs  Created: 2014-06-13@01:42
// Last modified: 2014-12-13@22:32 by Tim

using System.Collections;
using System.Text;

namespace TA.NetMF.NetworkCommandProtocol
    {
    public static class StringExtensions
        {
        static readonly string[] AsciiEncoding =
            {
            "", "", "", "", "", "", "", "\a", "\b", "\t", "\n", "\v", "\f", "\r", "", "",
            "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "",
            " ", "!", "\"", "#", "$", "%", "&", "\'", "(", ")", "*", "+", ",", "-", ".", "/",
            "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", ":", ";", "<", "=", ">", "?",
            "@", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O",
            "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "[", "\\", "]", "^", "_",
            "`", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o",
            "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "{", "|", "}", "~", ""
            };

        static readonly Hashtable asciiMnemonics = new Hashtable
            {
            {0x00, "<NULL>"},
            {0x01, "<SOH>"},
            {0x02, "<STH>"},
            {0x03, "<ETX>"},
            {0x04, "<EOT>"},
            {0x05, "<ENQ>"},
            {0x06, "<ACK>"},
            {0x07, "<BELL>"},
            {0x08, "<BS>"},
            {0x09, "<HT>"},
            {0x0A, "<LF>"},
            {0x0B, "<VT>"},
            {0x0C, "<FF>"},
            {0x0D, "<CR>"},
            {0x0E, "<SO>"},
            {0x0F, "<SI>"},
            {0x11, "<DC1>"},
            {0x12, "<DC2>"},
            {0x13, "<DC3>"},
            {0x14, "<DC4>"},
            {0x15, "<NAK>"},
            {0x16, "<SYN>"},
            {0x17, "<ETB>"},
            {0x18, "<CAN>"},
            {0x19, "<EM>"},
            {0x1A, "<SUB>"},
            {0x1B, "<ESC>"},
            {0x1C, "<FS>"},
            {0x1D, "<GS>"},
            {0x1E, "<RS>"},
            {0x1F, "<US>"},
            //{ 0x20, "<SP>" },
            {0x7F, "<DEL>"}
            };

        public static string GetString(this byte[] bytes)
            {
            var builder = new StringBuilder(bytes.Length);
            for (var i = 0; i < bytes.Length; i++)
                builder.Append(AsciiEncoding[bytes[i] & 0x7F]);
            return builder.ToString();
            }

        /// <summary>
        ///   Utility function. Expands non-printable ASCII characters into mnemonic human-readable form.
        /// </summary>
        /// <returns>
        ///   Returns a new string with non-printing characters replaced by human-readable mnemonics.
        /// </returns>
        public static string ExpandASCII(this string inputString)
            {
            var expanded = new StringBuilder(inputString.Length);
            foreach (char c in inputString)
                expanded.Append(c.ExpandASCII());
            return expanded.ToString();
            }

        /// <summary>
        ///   Utility function. Expands non-printable ASCII characters into mnemonic human-readable form.
        ///   printable characters are returned unmodified.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns>
        ///   If the original character is a non-printing ASCII character, then returns a string containing a
        ///   human-readable mnemonic for that ASCII character,
        ///   <example>
        ///     0x07 returns &lt;BELL&gt;
        ///   </example>
        ///   .
        ///   Otherwise, returns the original character converted to a string.
        /// </returns>
        public static string ExpandASCII(this char c)
            {
            int asciiCode = c;
            return asciiMnemonics.Contains(asciiCode) ? (string)asciiMnemonics[asciiCode] : c.ToString();
            }

        public static bool CaseInsensitiveEquals(this string lhs, string rhs)
            {
            return string.Equals(lhs.ToLower(), rhs.ToLower());
            }
        }
    }
