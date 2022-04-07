using System.Management;
using System.Runtime.Versioning;

namespace WinNetworkProcessMonitor.Handlers
{
    public class MachineGUID
    {
        [SupportedOSPlatform("windows")]
        private static readonly ManagementObject _os = new ManagementObject("Win32_OperatingSystem=@");

        [SupportedOSPlatform("windows")]
        public static string GetGuid()
        {
            string serial = (string)_os["SerialNumber"];
            return serial;
        }

    }
}