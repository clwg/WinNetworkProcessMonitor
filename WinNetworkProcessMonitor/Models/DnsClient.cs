using System.Text.Json.Serialization;
using System.Net;

namespace WinNetworkProcessMonitor.Models
{
    public class DnsClient
    {
        public class DnsResponse
        {
            private string _queryResults = null!;
            public string MachineGuid { get; set; } = null!;
            public string EventName { get; set; } = null!;
            public Filename? Hashes { get; set; }
            [JsonIgnore]
            public List<IPAddress> IpEntities { get; set; } = null!;

            [JsonPropertyName("IpEntities")]
            public List<string> IpStrEntities
            {
                get
                {
                    List<string> ipStringList = IpEntities.Select(s => s.ToString()).ToList();
                    return ipStringList;
                }
            }
            
            public Int64 ProcessID { get; set; }
            public Int64 ThreadID { get; set; }
            public string QueryName { get; set; } = null!;
            public string QueryOptions { get; set; } = null!;
            
            [JsonIgnore]
            public string QueryResults
            {
                get { return _queryResults; }
                set
                {
                    _queryResults = value;
                    string[] resultsSplit = value.Split(';');
                    List<IPAddress> _ipEntities = new List<IPAddress>();
                    List<DnsRname> _rnames = new List<DnsRname>();

                    foreach (string rec in resultsSplit)
                    {
                        if (rec.Contains("type"))
                        {
                            string[] rnameSplit = rec.Split(' ');
                            DnsRname _rnameRec = new DnsRname
                            {
                                Name = rnameSplit[^1],
                                Type = rnameSplit[^2]
                            };
                            _rnames.Add(_rnameRec);
                        }
                        else if (rec != "")
                        {
                            _ipEntities.Add(IPAddress.Parse(rec));
                        }
                    }
                    IpEntities = _ipEntities;
                    Rnames = _rnames;
                }
            }

            public Int64 QueryStatus { get; set; }
            public List<DnsRname> Rnames { get; set; } = null!;
            public DateTime TimeStamp { get; set; }
            public Double TimeStampRelativeMSec { get; set; }

        }

        public class DnsRname
        {
            public string Type { get; set; } = null!;
            public string Name { get; set; } = null!;
        }
    }
}
