using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{

    public enum StatusCode
    {
        Redirect = 301,
        OK = 200,
        InternalServerError = 500,
        NotFound = 404,
        BadRequest = 400
        
    }
    class Response
    {
        string responseString;
        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }
        StatusCode code;
        List<string> headerLines = new List<string>();
        public Response(StatusCode code, string contentType, string content, string redirectoinPath)
        {
            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])
            this.headerLines.Add("Content-Type: " + contentType + "\r\n");
            this.headerLines.Add("Content-Length: " + content.Length.ToString() + "\r\n");
            this.headerLines.Add("Date: " + DateTime.Now.ToString() + "\r\n");
            if (redirectoinPath != null)
                this.headerLines.Add("Location: " + redirectoinPath + "\r\n");

            // TODO: Create the request string
            if (code == StatusCode.BadRequest)
                this.responseString = this.GetStatusLine(code) + "Bad Request" + "\r\n";
            else if (code == StatusCode.NotFound)
                this.responseString = this.GetStatusLine(code) + "NotFound" + "\r\n";
            else if (code == StatusCode.OK)
                this.responseString = this.GetStatusLine(code) + "OK" + "\r\n";
            else if (code == StatusCode.InternalServerError)
                this.responseString = this.GetStatusLine(code) + "Internal Server Error" + "\r\n";
            else if (code == StatusCode.Redirect)
                this.responseString = this.GetStatusLine(code) + "Redirect" + "\r\n";

            foreach (string i in headerLines)
                this.responseString += i;
            this.responseString += "\r\n" + content;
        }

        private string GetStatusLine(StatusCode code)
        {
            // TODO: Create the response status line and return it
            return  Configuration.ServerHTTPVersion+((int)code).ToString();
        }
    }
}
