using Lys.MQConsumer.PortalBase;
using System;
using System.IO;

namespace Lys.MQConsumer.Sample.Portal
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                Directory.CreateDirectory(PortalConstants.Paths.LogDir);
                new MainHost().Run();
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                File.AppendAllText(PortalConstants.Paths.ErrorLog, ex + "\r\n");
            }
        }
    }
}
