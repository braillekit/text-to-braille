using System.Collections.Generic;
using System.IO;
using System.Text;
using Huanlin.Common.Helpers;

/// <summary>
/// HttpContentParser
/// Reads an http data stream and returns the form parameters.
/// </summary>
namespace Huanlin.Common.Http
{
    public class HttpContentParser
    {
        public HttpContentParser(Stream stream)
        {
            this.Parse(stream, Encoding.UTF8);
        }

        public HttpContentParser(Stream stream, Encoding encoding)
        {
            this.Parse(stream, encoding);
        }

        private void Parse(Stream stream, Encoding encoding)
        {
            this.Success = false;

            // Read the stream into a byte array
            byte[] data = ArrayHelper.ToByteArray(stream);

            // Copy to a string for header parsing
            string content = encoding.GetString(data);

            string name = string.Empty;
            string value = string.Empty;
            bool lookForValue = false;
            int charCount = 0;

            foreach (var c in content)
            {
                if (c == '=')
                {
                    lookForValue = true;
                }
                else if (c == '&')
                {
                    lookForValue = false;
                    AddParameter(name, value);
                    name = string.Empty;
                    value = string.Empty;
                }
                else if (!lookForValue)
                {
                    name += c;
                }
                else
                {
                    value += c;
                }

                if (++charCount == content.Length)
                {
                    AddParameter(name, value);
                    break;
                }
            }

            // Get the start & end indexes of the file contents
            //int startIndex = nameMatch.Index + nameMatch.Length + "\r\n\r\n".Length;
            //Parameters.Add(name, s.Substring(startIndex).TrimEnd(new char[] { '\r', '\n' }).Trim());

            // If some data has been successfully received, set success to true
            if (Parameters.Count != 0)
                this.Success = true;
        }

        private void AddParameter(string name, string value)
        {
            if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(value))
                Parameters.Add(name.Trim(), value.Trim());
        }

        public IDictionary<string, string> Parameters = new Dictionary<string, string>();

        public bool Success
        {
            get;
            private set;
        }
    }
}