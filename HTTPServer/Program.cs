using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Program
    {
        static string RedirectionMatrixPath = "redirectionRules.txt";
        static void Main(string[] args)
        {
            Console.Title = "HTTP Server";
            Console.WriteLine("We started the server on port 1000");
            // TODO: Call CreateRedirectionRulesFile() function to create the rules of redirection 
            CreateRedirectionRulesFile();
            //Start server
            // 1) Make server object on port 1000
            Server JustServer = new Server(1000, RedirectionMatrixPath);
            // 2) Start Server
            JustServer.StartServer();
        }

        static void CreateRedirectionRulesFile()
        {
            // TODO: Create file named redirectionRules.txt
            // each line in the file specify a redirection rule
            // example: "aboutus.html,aboutus2.html"
            // means that when making request to aboustus.html,, it redirects me to aboutus2
            FileStream fs=new FileStream(RedirectionMatrixPath,FileMode.Create);
            StreamWriter sr = new StreamWriter(fs);
            sr.WriteLine("aboutus.html,aboutus2.html");
            sr.Close();
            fs.Close();
        }
         
    }
}
