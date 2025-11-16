using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Huanlin.Common.Helpers;

/// <summary>
/// HttpMultipartParser
/// Reads a multipart http data stream and returns the file name, content type and file content.
/// Also, it returns any additional form parameters in a Dictionary.
/// </summary>
namespace Huanlin.Common.Http
{
    public delegate void MultiPartFileHandler(string filename, string contentType, byte[] content);

    public class HttpMultipartParser
    {
        public HttpMultipartParser(Stream stream, string filePartName)
        {
            FilePartName = filePartName;
            this.Parse(stream, Encoding.UTF8);
        }

        public HttpMultipartParser(Stream stream, Encoding encoding, string filePartName)
        {
            FilePartName = filePartName;
            this.Parse(stream, encoding);
        }

        private void Parse(Stream stream, Encoding encoding)
        {
            this.Success = false;

            // Read the stream into a byte array
            byte[] data = ArrayHelper.ToByteArray(stream);

            // Copy to a string for header parsing
            string content = encoding.GetString(data);

            // The first line should contain the delimiter
            int delimiterEndIndex = content.IndexOf("\r\n");

            if (delimiterEndIndex > -1)
            {
                string delimiter = content.Substring(0, content.IndexOf("\r\n"));

                string[] sections = content.Split(new string[] { delimiter }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string s in sections)
                {
                    if (s.Contains("Content-Disposition"))
                    {
                        // If we find "Content-Disposition", this is a valid multi-part section
                        // Now, look for the "name" parameter
                        Match nameMatch = new Regex(@"(?<=name\=\"")(.*?)(?=\"")").Match(s);
                        string name = nameMatch.Value.Trim().ToLower();

                        if (name == FilePartName.ToLower())
                        {
                            // Look for Content-Type
                            Regex re = new Regex(@"(?<=Content\-Type:)(.*?)(?=\r\n\r\n)");
                            Match contentTypeMatch = re.Match(content);

                            // Look for filename
                            re = new Regex(@"(?<=filename\=\"")(.*?)(?=\"")");
                            Match filenameMatch = re.Match(content);

                            // Did we find the required values?
                            if (contentTypeMatch.Success && filenameMatch.Success)
                            {
                                // Set properties
                                this.ContentType = contentTypeMatch.Value.Trim();
                                this.FileName = filenameMatch.Value.Trim();

                                // Get the start & end indexes of the file contents
                                int startIndex = contentTypeMatch.Index + contentTypeMatch.Length + "\r\n\r\n".Length;

                                byte[] delimiterBytes = encoding.GetBytes("\r\n" + delimiter);
                                int endIndex = ArrayHelper.IndexOf(data, delimiterBytes, startIndex);

                                int contentLength = endIndex - startIndex;

                                // Extract the file contents from the byte array
                                byte[] fileData = new byte[contentLength];

                                Buffer.BlockCopy(data, startIndex, fileData, 0, contentLength);

                                this.FileContents = fileData;
                            }
                        }
                        else if (!string.IsNullOrWhiteSpace(name))
                        {
                            // Get the start & end indexes of the file contents
                            int startIndex = nameMatch.Index + nameMatch.Length + "\r\n\r\n".Length;
                            Parameters.Add(name, s.Substring(startIndex).TrimEnd(new char[] { '\r', '\n' }).Trim());
                        }
                    }
                }

                // If some data has been successfully received, set success to true
                if (FileContents != null || Parameters.Count != 0)
                    this.Success = true;
            }
        }

        public IDictionary<string, string> Parameters = new Dictionary<string, string>();

        public string FilePartName
        {
            get;
            private set;
        }

        public bool Success
        {
            get;
            private set;
        }

        public string ContentType
        {
            get;
            private set;
        }

        public string FileName
        {
            get;
            private set;
        }

        public byte[] FileContents
        {
            get;
            private set;
        }

        public static void ParseFiles(Stream stream, string[] filenames, Encoding encoding,
            MultiPartFileHandler multiPartHandler)
        {
            List<string> lowerCasedFileNames = new List<string>();
            foreach (string fname in filenames)
            {
                lowerCasedFileNames.Add(fname.ToLower());
            }

            // Read the stream into a byte array
            byte[] data = ArrayHelper.ToByteArray(stream);

            // Copy to a string for header parsing
            string content = encoding.GetString(data);

            System.IO.File.WriteAllText(@"C:\debug.txt", content);

            // The first line should contain the delimiter
            int delimiterEndIndex = content.IndexOf("\r\n");

            if (delimiterEndIndex > -1)
            {
                string delimiter = content.Substring(0, content.IndexOf("\r\n"));

                string[] sections = content.Split(new string[] { delimiter }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string currentSection in sections)
                {
                    if (currentSection.Contains("Content-Disposition"))
                    {
                        // If we find "Content-Disposition", this is a valid multi-part section
                        // Now, look for the "name" parameter
                        Match nameMatch = new Regex(@"(?<=name\=\"")(.*?)(?=\"")").Match(currentSection);
                        string name = nameMatch.Value.Trim();

                        int idx = lowerCasedFileNames.IndexOf(name.ToLower());
                        if (idx >= 0)
                        {
                            // Look for Content-Type
                            Regex re = new Regex(@"(?<=Content\-Type:)(.*?)(?=\r\n\r\n)");
                            Match contentTypeMatch = re.Match(currentSection);

                            // Look for filename
                            re = new Regex(@"(?<=filename\=\"")(.*?)(?=\"")");
                            Match filenameMatch = re.Match(currentSection);

                            // Did we find the required values?
                            if (contentTypeMatch.Success && filenameMatch.Success)
                            {
                                // Set properties
                                string contentType = contentTypeMatch.Value.Trim();
                                string fileName = filenameMatch.Value.Trim();

                                System.IO.File.WriteAllText(@"C:\debug2.txt", fileName);

                                if (multiPartHandler != null)
                                {
                                    // Get the start & end indexes of the file contents
                                    int startIndex = contentTypeMatch.Index + contentTypeMatch.Length + "\r\n\r\n".Length;

                                    byte[] delimiterBytes = encoding.GetBytes("\r\n" + delimiter);
                                    int endIndex = ArrayHelper.IndexOf(data, delimiterBytes, startIndex);

                                    int contentLength = endIndex - startIndex;

                                    // Extract the file contents from the byte array
                                    byte[] fileData = new byte[contentLength];

                                    Buffer.BlockCopy(data, startIndex, fileData, 0, contentLength);

                                    multiPartHandler(fileName, contentType, fileData);
                                }
                            }
                        }
                        //else if (!string.IsNullOrWhiteSpace(name))
                        //{
                        //    // Get the start & end indexes of the file contents
                        //    int startIndex = nameMatch.Index + nameMatch.Length + "\r\n\r\n".Length;
                        //    Parameters.Add(name, s.Substring(startIndex).TrimEnd(new char[] { '\r', '\n' }).Trim());
                        //}
                    }
                }
            }
        }
    }
}