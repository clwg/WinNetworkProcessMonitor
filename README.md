# Windows Network Process Monitor

Network process monitor for Windows systems using .net 6 and event tracing.

- Monitors outbound TCP, UDP and DNS traffic on a Windows system.
- Enumerates underlying process information (file path, md5, sha1, sha256)
- Provides alerting from ip rule sets
- Logs results to Windows eventlog with a json eventdata payload
- Lightweight with internal caching mechanisms to minimize system resource utilization

## Quickstart
[Download the package](https://github.com/clwg/WinNetworkProcessMonitor/releases/download/0.0.1/WinNetworkProcessMonitor.zip)
Extract the zip file to the directory you wish to run the service from and execute install_service.bat from a command prompt.

Alternatively, you can install the service normally with sc.exe and an administrative terminal.

```
C:\windows\system32\sc.exe create "NetworkProcessMonitor" binpath="C:\path\to\WinNetworkProcessMonitor.exe" start=auto
C:\windows\system32\sc.exe start "NetworkProcessMonitor"
```
### Uninstall

To uninstall the application execute the following from an administrative terminal
```
sc.exe stop "NetworkProcessMonitor" 
sc.exe delete "NetworkProcessMonitor" 
```

You can then delete the application folder.

## Logging

All logging outputs to Windows Event Log facilities, so integration with secondary systems such as Elastic or Splunk should be relatively straightforward.  ![Event Viewer Output](/img/eventviewer.png)

### "Alerting"

Alerting is driven by the newline-delimited iprules.txt file; all network flows and DNS records are then processed, and if there is a match, the log level of the message is increased.

While you can use this to feed an IP blacklist into the system, using a DNS firewall and alerting on predefined rewrite locations will probably provide more utility and be easier to maintain.


## Building

The application is built in .NET 6 and can target both x86 and x64 Windows editions.
The application will work with .NET 5 but requires modification to the csproj file in order to pack to a single exe.

```
dotnet restore
dotnet build --configuration Release --no-restore
dotnet publish -r win-x64 /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true --self-contained true
```

## Gotchas

The application is meant to monitor endpoint systems. Monitoring server systems that provide UDP services, such as DNS servers, will result in an extreme amount of event log entries.
UDP record caching code can be customized to deal with these scenarios.

Additionally, the application must run with administrative privileges - you shouldn't blindly trust this code, and precautions should be taken in production environments to adequately protect the application.



## Analysis Considerations

When analyzing these logs, it's important to remember that the majority of DNS traffic will originate from the internal Windows DNS Client stub resolver. This has several properties that should be considered.

Most DNS records are not directly correlated to the process that initiated them, but rather they are the records that were in cache when the process initiated communication.
Some applications that implement their own resolution will be seen communicating outbound on port 53, which is its own signature.
From a correlation or ontology perspective, the data collected can be correlated to provide a representative view of the data as follows.

Domain Name(s) <-> IP Address(es) <-> Network Flow(s) <-> Hash <-> Filename(s)

The script evt_to_gexf.py in the Python scripts folder processes the event log in the above manner.

![Gephi Output](/img/graph.png)

