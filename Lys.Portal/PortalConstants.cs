using System;
using System.IO;

namespace Lys.Portal
{
    public static class PortalConstants
    {
        public static class Paths
        {
            public static readonly string BaseDir = AppDomain.CurrentDomain.BaseDirectory;
            public static readonly string LogDir = Path.Combine(BaseDir, "Logs/");
            public static readonly string ErrorLog = Path.Combine(LogDir, "Error.txt");
            public static readonly string LogConfig = Path.Combine(BaseDir, "log4net.config");
        }
    }
}