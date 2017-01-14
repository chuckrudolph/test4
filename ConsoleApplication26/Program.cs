using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Drawing;
using System.Text.RegularExpressions;
using log4net;
[assembly: log4net.Config.XmlConfigurator(Watch = true)]


namespace ConsoleApplication26
{
    class Program
    {
        static int Main(string[] args) {
            //do this once
            //log4net.Config.XmlConfigurator.Configure();
            //OR put this attribute someplace, 
            //[assembly: log4net.Config.XmlConfigurator(Watch = true)]

            //StringBuilder sbx = new StringBuilder(1000);
            //string sx = sbx.ToString();


            Color test = Color.LightSteelBlue;
            


        ILog logger = LogManager.GetLogger("programtest");
            logger.Info("info message from program");
            
            testme x = new testme();
            
            newMethod();
        
            Console.ReadLine();
            return 10;

        }

        static int newMethod() {
            ILog logger = LogManager.GetLogger(typeof(Program));
            logger.Warn("warnming message from program in newmethod");

            string justnumbers = @"[^\d]";

            Console.WriteLine(Regex.Replace("5555-6666", justnumbers, ""));
            Console.WriteLine(Regex.Replace("A- 5555-6666 dd", justnumbers, ""));
            Console.WriteLine(Regex.Replace("5d555-666a6", justnumbers, ""));
            return 5;
        }
    }

    class testme
    {
        public testme() {
            ILog l = LogManager.GetLogger(typeof(testme));
            try {
                ApplicationException aex = new ApplicationException("this is a test application exception");
                throw aex;
            }
            catch (Exception ex) {
                l.Error("Error message in testme", ex);
            }

        }
    }
}
