using System;
using System.IO;

namespace Lys.Portal
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                Directory.CreateDirectory(PortalConstants.Paths.LogDir);
                new MainHost().Run();
                Console.Read();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                File.AppendAllText(PortalConstants.Paths.ErrorLog, ex + "\r\n");
            }
        }
    }
}
