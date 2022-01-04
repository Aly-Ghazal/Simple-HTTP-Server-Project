using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace HTTPServer
{
    class Server
    {
        Socket serverSocket;
        byte []data;
        int receivedLength;
        Request request;
        Response response;
        string content_type = "Text/HTML";
        public Server(int portNumber, string redirectionMatrixPath)
        {
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            this.LoadRedirectionRules(redirectionMatrixPath);
            //TODO: initialize this.serverSocket
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint hostEndPoint = new IPEndPoint(IPAddress.Any, portNumber);
            this.serverSocket.Bind(hostEndPoint);
        }

        public void StartServer()
        {
            // TODO: Listen to connections, with large backlog.
            this.serverSocket.Listen(1000);
            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            while (true)
            {
                //TODO: accept connections and start thread for each accepted connection.
                Socket clientSocket = this.serverSocket.Accept();
                Console.WriteLine("New client accepted: {0}", clientSocket.RemoteEndPoint);
                Thread newthread = new Thread(new ParameterizedThreadStart(HandleConnection));
                newthread.Start(clientSocket);
            }
        }

        public void HandleConnection(object obj)
        {
            // TODO: Create client socket 
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            Socket clientSock = (Socket)obj;
            clientSock.ReceiveTimeout = 0;
            // TODO: receive requests in while true until remote client closes the socket.
            while (true)
            {
                try
                {
                    // TODO: Receive request
                    this.data = new byte[1024];
                    this.receivedLength = clientSock.Receive(data);
                    // TODO: break the while loop if receivedLen==0
                    if (receivedLength == 0)
                    {
                        Console.WriteLine("Client: {0} ended the connection", clientSock.RemoteEndPoint);
                        break;
                    }
                    // TODO: Create a Request object using received request string
                    string display = Encoding.ASCII.GetString(data, 0, receivedLength);
                    this.request = new Request(display);
                    // TODO: Call HandleRequest Method that returns the response
                    this.response = this.HandleRequest(request);
                    // TODO: Send Response back to client
                    clientSock.Send(Encoding.ASCII.GetBytes(response.ResponseString));
                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex);
                }
            }
            // TODO: close client socket
            clientSock.Shutdown(SocketShutdown.Both);
            clientSock.Close();
        }
        Response HandleRequest(Request request)
        {
            string content;
            try
            {
               
                //TODO: check for bad request 
                if (!request.ParseRequest())
                {
                    content = LoadDefaultPage(Configuration.BadRequestDefaultPageName);
                    return new Response(StatusCode.BadRequest, this.content_type, content, null);
                }
                //TODO: map the relativeURI in request to get the physical path of the resource.
                
               //TODO: check for redirect
                if (Configuration.RedirectionRules.ContainsKey(request.relativeURI))
                {
                    string redirction_path = Configuration.RedirectionRules[request.relativeURI];
                    content = LoadDefaultPage(GetRedirectionPagePathIFExist(redirction_path));
                    return new Response(StatusCode.Redirect, this.content_type, content, redirction_path);
                }
                //TODO: check file exists
                //TODO: read the physical file
                content = LoadDefaultPage(request.relativeURI);

                if (content == "")
                {
                    content = LoadDefaultPage(Configuration.NotFoundDefaultPageName);
                    return new Response(StatusCode.NotFound, this.content_type, content, null);
                }
                
                
                // Create OK response
                return new Response(StatusCode.OK, this.content_type, content, null);

            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                // TODO: in case of exception, return Internal Server Error. 
                content = LoadDefaultPage(Configuration.InternalErrorDefaultPageName);
                return new Response(StatusCode.InternalServerError, this.content_type, content, null);
            }
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            if (File.Exists(Configuration.RootPath+"\\"+relativePath)) 
            {
                return relativePath;
            }
            else
            return string.Empty;
        }

        private string LoadDefaultPage(string defaultPageName)
        {
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
            // TODO: check if filepath not exist log exception using Logger class and return empty string
            if (!File.Exists(filePath))
            { 
                Logger.LogException(new Exception(defaultPageName + " Dosen't exist"));
                return string.Empty;
            }
            // else read file and return its content
            else 
            {
                return File.ReadAllText(filePath);
            }
        }

        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                Configuration.RedirectionRules = new Dictionary<string, string>();
                // TODO: using the filepath paramter read the redirection rules from file
                string[] redirectionRules = File.ReadAllText(filePath).Split(new string[]{"\r\n"},StringSplitOptions.RemoveEmptyEntries);
                // then fill Configuration.RedirectionRules dictionary 
                for (int i = 0; i < redirectionRules.Length;i++ )
                {
                    string[]rule=redirectionRules[i].Split(',');
                    Configuration.RedirectionRules.Add(rule[0],rule[1]);
                }

            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                Environment.Exit(1);
            }
        }
    }
}
