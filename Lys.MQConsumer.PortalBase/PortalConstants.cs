using System;
using System.IO;

namespace Lys.MQConsumer.PortalBase
{
    public static class PortalConstants
    {
        public static class Paths
        {
            public static readonly string BaseDir = AppDomain.CurrentDomain.BaseDirectory;
            public static readonly string LogDir = Path.Combine(BaseDir, "Logs/");
            public static readonly string ErrorLog = Path.Combine(LogDir, "Error.txt");
        }
    }
}