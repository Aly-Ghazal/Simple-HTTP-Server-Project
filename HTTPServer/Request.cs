using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTTPServer
{
    public enum RequestMethod
    {
        GET,
        POST,
        HEAD
    }

    public enum HTTPVersion
    {
        HTTP10,
        HTTP11,
        HTTP09
    }

    class Request
    {
        string[] requestLines;
        RequestMethod method;
        public string relativeURI;
        Dictionary<string, string> headerLines;

        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }

        HTTPVersion httpVersion;
        string requestString;
        string[] contentLines;

        public Request(string requestString)
        {
            this.requestString = requestString;
        }
        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>
        public bool ParseRequest()
        {

            //TODO: parse the receivedRequest using the \r\n delimeter   
            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)
            // Parse Request line
            // Validate blank line exists
            // Load header lines into HeaderLines dictionary
            if (!ParseRequestLine())
                return false;
            else if (!ValidateBlankLine())
                return false;
            else if (!LoadHeaderLines())
                return false;
            else
                return true;
        }

        private bool ParseRequestLine()
        {
            this.contentLines = this.requestString.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            if (this.contentLines.Length < 4)
                return false;
            else 
            {
                this.requestLines = this.contentLines[0].Split(' ');
                if (this.requestLines.Length != 3)
                    return false;
                else 
                {
                    if (this.requestLines[0] == "GET")
                        method = RequestMethod.GET;
                    else if (this.requestLines[0] == "POST")
                        method = RequestMethod.POST;
                    else if (this.requestLines[0] == "HEAD")
                        method = RequestMethod.HEAD;

                    if (!this.ValidateIsURI(requestLines[1]))
                        return false;
                    this.relativeURI = this.requestLines[1].Remove(0,1);//something will happen here
                    if (this.requestLines[2] == "HTTP/1.0")
                        httpVersion = HTTPVersion.HTTP10;
                    else if (this.requestLines[2] == "HTTP/1.1")
                        httpVersion = HTTPVersion.HTTP11;
                    else
                      httpVersion = HTTPVersion.HTTP09;

                    return true;
                }
            }
        }

        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }

        private bool LoadHeaderLines()
        {
            headerLines = new Dictionary<string, string>();
            for (int i = 1; i < contentLines.Length; i++)
            {
                string[] header = contentLines[i].Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);
                if (header.Length > 0)
                {
                    this.headerLines.Add(header[0], header[1]);
                }
            }
            return this.headerLines.Count > 1;
        }

        private bool ValidateBlankLine()
        {
            return this.requestString.EndsWith("\r\n\r\n");
        }

    }
}