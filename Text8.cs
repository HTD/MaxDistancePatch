using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MaxDistancePatch {

    /// <summary>
    /// 8-bit text processing class.
    /// Used to operate on 8-bit character encoded texts directly.
    /// </summary>
    class Text8 {

        /// <summary>
        /// ASCII values of numeric strings.
        /// </summary>
        private static byte[] NumberMatch = new Text8("-.0123456789").Data;

        /// <summary>
        /// ASCII values of common text separators.
        /// </summary>
        private static byte[] SeparatorMatch = new byte[] { 9, 32, 59 };

        /// <summary>
        /// Internal 8-bit character encoding.
        /// </summary>
        private Encoding Encoding = Encoding.ASCII;

        /// <summary>
        /// Byte array representing the text.
        /// </summary>
        private byte[] Data;


        /// <summary>
        /// Gets the length of the byte array equal to original string length.
        /// </summary>
        public int Length => Data.Length;

        /// <summary>
        /// Creates an empty instance of <see cref="Text8"/>.
        /// </summary>
        public Text8() { }

        /// <summary>
        /// Creates an empty instance with specified encoding.
        /// </summary>
        /// <param name="encoding">8-bit character encoding.</param>
        public Text8(Encoding encoding) { Encoding = encoding; }

        /// <summary>
        /// Creates instance from string.
        /// </summary>
        /// <param name="text">String to build the byte array.</param>
        public Text8(string text) { Data = Encoding.GetBytes(text); }

        /// <summary>
        /// Creates instance from string with specified encoding.
        /// </summary>
        /// <param name="text">String to build the byte array.</param>
        /// <param name="encoding">8-bit character encoding.</param>
        public Text8(string text, Encoding encoding) { Encoding = encoding; Data = Encoding.GetBytes(text); }

        /// <summary>
        /// Loads the byte array from file.
        /// </summary>
        /// <param name="path">File path.</param>
        public void Load(string path) => Data = File.ReadAllBytes(path);

        /// <summary>
        /// Saves the byte array to file.
        /// </summary>
        /// <param name="path">File path.</param>
        public void Save(string path) => File.WriteAllBytes(path, Data);

        /// <summary>
        /// Decodes the byte array to string.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Encoding.GetString(Data);

        /// <summary>
        /// Finds specified byte sequence in byte array.
        /// </summary>
        /// <param name="data">Byte sequence to find.</param>
        /// <param name="offset">Offset the search begins at, default 0 for searching at the beginning.</param>
        /// <param name="limit">Maximum number of bytes to search, default 0 for searching to the end of the data.</param>
        /// <returns>Absolute index of the first sequence occurance or -1 if the sequence was not found.</returns>
        public int FindSequence(byte[] data, int offset = 0, int limit = 0) {
            var l = data.Length;
            var n = Data.Length;
            var x = offset;
            var y = 0;
            var z = -1;
            while (x < n && y < l) {
                if (limit > 0 && x >= offset + limit) break;
                if (Data[x].Equals(data[y])) {
                    if (z < 0) z = x;
                    y++;
                    if (y == l) break;
                } else {
                    z = -1;
                    y = 0;
                }
                x++;
            }
            return z;
        }

        /// <summary>
        /// Finds any continuous numeric sequence.
        /// </summary>
        /// <param name="offset">Offset the search begins at, default 0 for searching at the beginning.</param>
        /// <returns><see cref="Match"/> object containing offset, length and the data found.</returns>
        public Match FindNumber(int offset = 0) => FindAny(NumberMatch, offset);

        /// <summary>
        /// Finds any continuous separators sequence.
        /// </summary>
        /// <param name="offset">Offset the search begins at, default 0 for searching at the beginning.</param>
        /// <returns><see cref="Match"/> object containing offset, length and the data found.</returns>
        public Match FindSeparator(int offset = 0) => FindAny(SeparatorMatch, offset);

        /// <summary>
        /// Finds a continuous sequence of bytes from specified byte array.
        /// </summary>
        /// <param name="data">Byte array containing elements to search.</param>
        /// <param name="offset">Offset the search begins at, default 0 for searching at the beginning.</param>
        /// <returns><see cref="Match"/> object containing offset, length and the data found.</returns>
        private Match FindAny(byte[] data, int offset = 0) {
            var matchOffset = -1;
            var matchLength = 0;
            var l = data.Length;
            var n = Data.Length;
            int x, y, z;
            for (x = offset; x < n; x++) {
                for (y = 0, z = -1; y < l; y++) if (Data[x].Equals(data[y])) { z = y; break; }
                if (z >= 0) {
                    if (matchOffset < 0) matchOffset = x;
                    matchLength++;
                } else {
                    if (matchOffset >= 0) break;
                }
            }
            return new Match { Offset = matchOffset, Length = matchLength };
        }

        /// <summary>
        /// Applies predefined replacements.
        /// </summary>
        /// <param name="replacements">Any collection of replacements.</param>
        public void Replace(IEnumerable<Replacement> replacements) {
            int delta = 0, n = 0;
            foreach (var r in replacements) { delta += r.Data.Length - r.Length; n++; }
            int targetLength = Data.Length + delta;
            byte[] target = new byte[targetLength];
            int lastIndex = Data.Length - 1;
            int lastReplacement = n - 1;
            int srcOffset = 0;
            int dstOffset = 0;
            foreach (var replacement in replacements) {
                var padding = replacement.Offset - srcOffset;
                Buffer.BlockCopy(Data, srcOffset, target, dstOffset, padding);
                srcOffset += padding;
                dstOffset += padding;
                Buffer.BlockCopy(replacement.Data, 0, target, dstOffset, replacement.Data.Length);
                srcOffset += replacement.Length;
                dstOffset += replacement.Data.Length;
            }
            if (srcOffset < lastIndex) Buffer.BlockCopy(Data, srcOffset, target, dstOffset, lastIndex - srcOffset + 1);
            Data = target;
        }

        /// <summary>
        /// <see cref="string"/> to <see cref="Text8"/> implicit conversion.
        /// </summary>
        /// <param name="text">Input string.</param>
        public static implicit operator Text8(string text) => new Text8(text);

        /// <summary>
        /// Byte array to <see cref="Text8"/> implicit conversion.
        /// </summary>
        /// <param name="text">Input byte array.</param>
        public static implicit operator Text8(byte[] text) => new Text8 { Data = text };

        /// <summary>
        /// <see cref="Text8"/> to <see cref="string"/> implicit conversion.
        /// </summary>
        /// <param name="text">Input <see cref="Text8"/> object.</param>
        public static implicit operator string(Text8 text) => text.ToString();

        /// <summary>
        /// <see cref="Text8"/> to byte array implicit conversion.
        /// </summary>
        /// <param name="text">Input <see cref="Text8"/> object.</param>
        public static implicit operator byte[](Text8 text) => text.Data;
        
        /// <summary>
        /// Match structure.
        /// </summary>
        public struct Match {
            
            /// <summary>
            /// Zero-based index of the match.
            /// </summary>
            public int Offset;
            
            /// <summary>
            /// Length of matched data.
            /// </summary>
            public int Length;

        }

        /// <summary>
        /// Replacement structure.
        /// </summary>
        public struct Replacement {

            /// <summary>
            /// Zero-based index of the sequence to replace.
            /// </summary>
            public int Offset;

            /// <summary>
            /// Length of the data to be replaced.
            /// </summary>
            public int Length;

            /// <summary>
            /// New sequence to replace the fragment with.
            /// </summary>
            public byte[] Data;

        }

    }

}