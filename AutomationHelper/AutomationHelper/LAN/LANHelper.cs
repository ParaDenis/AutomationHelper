using System.Diagnostics;
using System.Net.NetworkInformation;
using AutomationHelper.Extensions;
using AutomationHelper.Waiters;

namespace AutomationHelper.LAN
{
    public class LanHelper
    {
        //protected static readonly ILog Log = LogManager.GetLogger(typeof(LANMethods));
        private static string interfaceName;

        public static void Enable()
        {
            Wait.UntilNoException(() =>
            {
                if (IsNetworkUp()) return;
                enable(interfaceName);
                Wait.UntilTrue(IsNetworkUp,"Network is not up",10*1000);
            },timeout: 180*1000);
        }

        private static void enable(string interfaceName)
        {
            //Log.Info("Enabling ethernet connection: " + interfaceName);
            var psi = new ProcessStartInfo("netsh",
                "interface set interface \"" + interfaceName + "\" enable");
            var p = new Process { StartInfo = psi };
            p.Start();
        }

        public static void Disable()
        {
            interfaceName = NetworkInterface.GetAllNetworkInterfaces()
                .First(
                    i =>
                        i.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
                        i.OperationalStatus == OperationalStatus.Up, "Can't find ethernet connection which is up")
                .Name;
            //Log.Info("Ethernet connection name is '" + interfaceName + "'");
            Wait.UntilNoException(() =>
            {
                if (!IsNetworkUp()) return;
                disable(interfaceName);
                Wait.UntilTrue(() => !IsNetworkUp(), "Network is not down", 10 * 1000);
            }, timeout: 180 * 1000);
        }

        private static void disable(string interfaceName)
        {
            //Log.Info("Disabling ethernet connection: "+interfaceName);
            var psi = new ProcessStartInfo("netsh",
                "interface set interface \"" + interfaceName + "\" disable");
            var p = new Process { StartInfo = psi };
            p.Start();
        }

        public static bool IsNetworkUp()
        {
            var result = false;
            try
            {
                return NetworkInterface.GetAllNetworkInterfaces()
                    .FirstUntilNumberOfException(i => i.Name == interfaceName, "Can't find ethernet connection with name: " + interfaceName)
                    .OperationalStatus == OperationalStatus.Up;
            }
            catch{}
            return result;
        }
    }
}
