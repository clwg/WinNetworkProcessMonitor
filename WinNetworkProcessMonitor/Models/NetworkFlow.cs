using Microsoft.Extensions.Caching.Memory;
using System.Net;
using System.Text.Json.Serialization;

namespace WinNetworkProcessMonitor.Models
{
    public class NetworkFlow
    {

        /// UDP Cache to minimize logging overhead
        private static readonly MemoryCache _udpCache = new MemoryCache(new MemoryCacheOptions()
        {
            ExpirationScanFrequency = TimeSpan.FromSeconds(30)
        });

        /// <summary>
        /// Check UDP cache for the presence of Destination Addr, Destination Port and pid
        /// </summary>
        /// <param name="dstAddr"></param>
        /// <param name="dstPort"></param>
        /// <param name="pid"></param>
        /// <returns>bool</returns>
        public static bool CheckUDPCache(IPAddress dstAddr, int dstPort, long pid)
        {
            string cacheKey = String.Format("{0}:{1}:{2}", dstAddr.ToString(), dstPort.ToString(), pid.ToString());

            if (!_udpCache.TryGetValue(cacheKey, out bool cacheEntry))// Look for cache key.
            {
                MemoryCacheEntryOptions options = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddSeconds(30)
                };
                _udpCache.Set(cacheKey, true, options);
                return false;
            }
            else
            {
                return true;
            }
        }


        public class FlowRecord
        {
            [JsonPropertyName("DestAddr")]
            public string DestAddrStr
            {
                get
                {
                    string DstIpString = DestAddr.ToString();
                    return DstIpString;
                }
            }
            [JsonIgnore]
            public IPAddress DestAddr { get; set; } = null!;
            public int DestPort { get; set; }
            public string EventName { get; set; } = null!;
            public string MachineGuid { get; set; } = null!;
            public Int64 ProcessID { get; set; }
            public string ProcessName { get; set; } = null!;
            public string ProviderGuid { get; set; } = null!;
            public string ProviderName { get; set; } = null!;

            [JsonPropertyName("SrcAddr")]
            public string SrcAddrStr
            {
                get
                {
                    string SrcIpString = SrcAddr.ToString();
                    return SrcIpString;
                }
            }

            [JsonIgnore]
            public IPAddress SrcAddr { get; set; } = null!;
            public int SrcPort { get; set; }
            public string TaskGuid { get; set; } = null!;
            public string TaskName { get; set; } = null!;
            public Int64 ThreadID { get; set; }
            public DateTime TimeStamp { get; set; }
            public Double TimeStampRelativeMSec { get; set; }
            public Filename? Filename { get; set; }

        }
    }
}

