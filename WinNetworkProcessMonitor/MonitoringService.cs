using Microsoft.Diagnostics.Tracing.Session;
using System.Text.Json;
using System.Runtime.Versioning;

namespace WinNetworkProcessMonitor
{
    public class MonitoringService
    {
        [SupportedOSPlatform("windows")]
        public void Initialize()
        {
            var _machineGuid = Handlers.MachineGUID.GetGuid();
            var _pidDescriptorCache = new Handlers.ProcessEnumeration();

            var _ipRules = new Handlers.RuleSet();
            var _eventLog = new Logging.WindowsEventLog();

            var monitor = Task.Run(() =>
            {
                using var session = new TraceEventSession("NetworkCaptureSession");
                session.EnableKernelProvider(Microsoft.Diagnostics.Tracing.Parsers.KernelTraceEventParser.Keywords.NetworkTCPIP);
                session.EnableProvider("Microsoft-Windows-DNS-Client", Microsoft.Diagnostics.Tracing.TraceEventLevel.Informational, 0x00);

                var dynamicProviders = session.Source.Dynamic;

                session.Source.Dynamic.All += (data) =>
                {
                    if (data.ProviderName == "Microsoft-Windows-DNS-Client")
                    {
                        try
                        {
                            if (data.PayloadStringByName("QueryResults") != "" && data.PayloadStringByName("QueryResults") != null)
                            {
                                string queryResultsStr = data.PayloadStringByName("QueryResults");

                                Models.DnsClient.DnsResponse dnsRecord = new Models.DnsClient.DnsResponse
                                {
                                    MachineGuid = _machineGuid,
                                    EventName = "Dns/QueryResults",
                                    QueryName = data.PayloadStringByName("QueryName"),
                                    QueryResults = data.PayloadStringByName("QueryResults"),
                                    QueryOptions = data.PayloadStringByName("QueryOptions"),
                                    ProcessID = data.ProcessID,
                                    ThreadID = data.ThreadID,
                                    TimeStamp = data.TimeStamp,
                                    TimeStampRelativeMSec = data.TimeStampRelativeMSec
                                };

                                var dnsPidPath = Handlers.ProcessEnumeration.GetProcessPath(data.ProcessID);
                                var dnspathHashes = Handlers.ProcessEnumeration.GetFilepathHashes(dnsPidPath);
                                dnsRecord.Hashes = dnspathHashes;

                                string jsonString = JsonSerializer.Serialize(dnsRecord);

                                bool dnsRule = Handlers.RuleSet.CheckIpListRule(dnsRecord.IpEntities);

                                Logging.WindowsEventLog.WriteLogEvent(jsonString, 3, dnsRule);

                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                };

                session.Source.Kernel.UdpIpSend += (data) =>
                {
                    var record = new Models.NetworkFlow.FlowRecord
                    {
                        DestAddr = data.daddr,
                        DestPort = data.dport,
                        EventName = data.EventName,
                        MachineGuid = _machineGuid,
                        ProcessID = data.ProcessID,
                        ProcessName = data.ProcessName,
                        ProviderGuid = data.ProviderGuid.ToString(),
                        ProviderName = data.ProviderName,
                        SrcAddr = data.saddr,
                        SrcPort = data.sport,
                        TaskGuid = data.TaskGuid.ToString(),
                        TaskName = data.TaskName,
                        ThreadID = data.ThreadID,
                        TimeStamp = data.TimeStamp,
                        TimeStampRelativeMSec = data.TimeStampRelativeMSec
                    };

                    var inCache = Models.NetworkFlow.CheckUDPCache(record.DestAddr, record.DestPort, record.ProcessID);
                    if (inCache == false)
                    {
                        var udpPidPath = Handlers.ProcessEnumeration.GetProcessPath(data.ProcessID);
                        var udppathHashes = Handlers.ProcessEnumeration.GetFilepathHashes(udpPidPath);
                        record.Filename = udppathHashes;

                        string jsonString = JsonSerializer.Serialize(record);


                        bool ipRule = Handlers.RuleSet.CheckIpRule(record.DestAddr);

                        Logging.WindowsEventLog.WriteLogEvent(jsonString, 2, ipRule);

                    }
                };

                session.Source.Kernel.TcpIpConnect += (data) =>
                {
                    var record = new Models.NetworkFlow.FlowRecord
                    {
                        DestAddr = data.daddr,
                        DestPort = data.dport,
                        EventName = data.EventName,
                        MachineGuid = _machineGuid,
                        ProcessID = data.ProcessID,
                        ProcessName = data.ProcessName,
                        ProviderGuid = data.ProviderGuid.ToString(),
                        ProviderName = data.ProviderName,
                        SrcAddr = data.saddr,
                        SrcPort = data.sport,
                        TaskGuid = data.TaskGuid.ToString(),
                        TaskName = data.TaskName,
                        TimeStamp = data.TimeStamp,
                        TimeStampRelativeMSec = data.TimeStampRelativeMSec
                    };

                    var tcpPidPath = Handlers.ProcessEnumeration.GetProcessPath(data.ProcessID);
                    var tcppathHashes = Handlers.ProcessEnumeration.GetFilepathHashes(tcpPidPath);
                    record.Filename = tcppathHashes;
                    string jsonString = JsonSerializer.Serialize(record);

                    bool ipRule = Handlers.RuleSet.CheckIpRule(record.DestAddr);

                    Logging.WindowsEventLog.WriteLogEvent(jsonString, 1, ipRule);

                };
            
                session.Source.Process();

            });

            monitor.Wait();

        }
    }
}
