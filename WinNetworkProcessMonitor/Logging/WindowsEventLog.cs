using System.Diagnostics;
using System.Runtime.Versioning;

namespace WinNetworkProcessMonitor.Logging
{
    [SupportedOSPlatform("windows")]
    public class WindowsEventLog
    {
        private static EventLog _winLogger = new EventLog();
        private static string? _eventSourceName;

        public WindowsEventLog(string eventSource = "NetProcMonLog")
        {
            _eventSourceName = eventSource;
            if (!EventLog.SourceExists(eventSource))
            {
                EventLog.CreateEventSource(eventSource, eventSource);

                Console.WriteLine("Created Event Source");
            }
            _winLogger.Source = _eventSourceName;
            _winLogger.EnableRaisingEvents = true;
        }

        public static void WriteLogEvent(string message, int eventId, bool rulehit)
        {
            if (rulehit == true)
            {
                _winLogger.WriteEntry(message, EventLogEntryType.Warning, eventId, 2);
            }
            else
            {
                _winLogger.WriteEntry(message, EventLogEntryType.Information, eventId, 1);
            }
        }
    }
}

