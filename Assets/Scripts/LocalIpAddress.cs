using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace Assets.Scripts
{
    public class LocalIpAddress
    {
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new System.Exception("No network adapters with an IPv4 address in the system!");
        }

        public static string GetNetWork()
        {
            var strings = GetLocalIPAddress().Split("."[0]);
            return strings[0] + "." + strings[1] + "." + strings[2] + ".";
        }
    }
}