using UnityEngine;
using System.Text;
using System.Collections.Generic;

namespace Framework
{
    public class ByteReader
    {
        byte[] mBuffer;
        int mOffset = 0;

        public ByteReader(byte[] bytes) { mBuffer = bytes; }
        public ByteReader(TextAsset asset) { mBuffer = asset.bytes; }

        /// <summary>
        /// Whether the buffer is readable.
        /// </summary>

        public bool canRead { get { return (mBuffer != null && mOffset < mBuffer.Length); } }

        /// <summary>
        /// Read a single line from the buffer.
        /// </summary>

        static string ReadLine(byte[] buffer, int start, int count)
        {
            return Encoding.UTF8.GetString(buffer, start, count);
        }

        /// <summary>
        /// Read a single line from the buffer.
        /// </summary>

        public string ReadLine()
        {
            int max = mBuffer.Length;

            // Skip empty characters
            while (mOffset < max && mBuffer[mOffset] < 32) ++mOffset;

            int end = mOffset;

            if (end < max)
            {
                for (;;)
                {
                    if (end < max)
                    {
                        int ch = mBuffer[end++];
                        if (ch != '\n' && ch != '\r') continue;
                    }
                    else ++end;

                    string line = ReadLine(mBuffer, mOffset, end - mOffset - 1);
                    mOffset = end;
                    return line;
                }
            }
            mOffset = max;
            return null;
        }

        /// <summary>
        /// Assume that the entire file is a collection of key/value pairs.
        /// </summary>

        public Dictionary<string, string> ReadDictionary()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            char[] separator = new char[] { '=' };

            while (canRead)
            {
                string line = ReadLine();
                if (line == null) break;
                string[] split = line.Split(separator, 2, System.StringSplitOptions.RemoveEmptyEntries);
                if (split.Length == 2)
                {
                    string key = split[0].Trim();
                    string val = split[1].Trim();
                    dict[key] = val;
                }
            }
            return dict;
        }
    }
}
