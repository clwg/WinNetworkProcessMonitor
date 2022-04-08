using System.Net;

namespace WinNetworkProcessMonitor.Handlers
{
    public class RuleSet
    {
        private static readonly List<IPAddress> _ruleList = new List<IPAddress>();

        public RuleSet()
        {
            Console.WriteLine("Initializing Ruleset");
            try
            {
                var reader = File.ReadAllLines("iprules.txt");
                foreach (string line in reader)
                {
                    Console.WriteLine("Adding Rule: {0}", line);
                    try
                    {
                        var ip = IPAddress.Parse(line);
                        _ruleList.Add(ip);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);

                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Unable to parse IP rule file\nAlerting disabled");
            }
        }

        public static bool CheckIpRule(IPAddress ip)
        {
            if (_ruleList.Contains(ip))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool CheckIpListRule(List<IPAddress> iplist)
        {
            if (_ruleList.Any(x => iplist.Contains(x)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
