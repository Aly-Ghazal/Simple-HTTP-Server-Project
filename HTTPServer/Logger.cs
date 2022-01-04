using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;

namespace HTTPServer
{
    class Logger
    {
        
        public static void LogException(Exception ex)
        {
             FileStream fs = new FileStream("../../log.txt", FileMode.OpenOrCreate);
             StreamWriter sr = new StreamWriter(fs);
            // TODO: Create log file named log.txt to log exception details in it
            //Datetime:
            //message:
            // for each exception write its details associated with datetime 
            string dateTime = DateTime.Now.ToString();
            string error = ex.Message;
            sr.WriteLine("Datetime: " + dateTime);
            sr.WriteLine("message: " + error);
            sr.Close();
        }
    }
}
