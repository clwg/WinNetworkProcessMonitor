# Windows Network Process Monitor

Network process monitor for Windows systems using .net 6 and event tracing.

- Monitors outbound TCP, UDP and DNS traffic on a Windows system.
- Enumerates underlying process information (file path, md5, sha1, sha256)
- Provides alerting from ip rule sets
- Logs results to Windows eventlog with a json eventdata payload
- Lightweight with internal caching mechanisms to minimize system resource utilization

## Quickstart
Extract the zip file to the directory you wish to run the service from and execute install_service.bat from a command prompt.

Alternatively you can install the service normally with sc.exe and a administrative terminal

```
C:\windows\system32\sc.exe create "NetworkProcessMonitor" binpath="C:\path\to\WinNetworkProcessMonitor.exe" start=auto
C:\windows\system32\sc.exe start "NetworkProcessMonitor"
```

To uninstall the application execute the following from an administrative terminal
```
sc.exe stop "NetworkProcessMonitor" 
sc.exe delete "NetworkProcessMonitor" 
```

You can then delete the application folder.

## Logging

All logging outputs to Windows Event Log facilities so integration with secondary systems such as Elastic or Splunk should be realtively straight forward.  ![Event Viewer Output](/img/eventviewer.png)

### "Alerting"

Alerting is driven by the newline deliminted iprules.txt file, all network flows and dns records are then processed and if there is a match the log level of the message is increased.

While you can use this to feed a ip blacklist into the system, using a DNS firewall and alerting on pre-defined rewrite locations will probably provide more utility and be easier to maintain.


## Building

The application is built in .net 6 and can target both x86 and x64 Windows editions.  
The application will work with .net 5 but requires modification to the csproj file in order to pack to a single exe.

```
dotnet restore
dotnet build --configuration Release --no-restore
dotnet publish -r win-x64 /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true --self-contained true
```

## Gotchas

The application is meant to monitor endpoint systems.  Monitoring server systems that provide UDP services, such as DNS servers will result in a extreme amount of event log entries.
UDP record caching code can be customized to deal with these scenarios.

Additionally the application must run with administrative privlideges - you shouldn't blindly trust this code and you should be taken in production enviornments to adequately protect the application.



## Analysis Considerations

When analyzing these logs it's important to remember that the majority of DNS traffic will originate from internal Windows DNS Client stub resolver.  This has several properties that should be considered.

- Most DNS records are not directly correlated to the process that initiated them.
- Some applications that implment their own resolution will be seen communciating outbound on port 53 which is it's own signature.

From a correlation or ontology perspective the data collected can be correlated to provide a represenatative view of the data.

Domain Name(s) <-> IP Address(es) <-> Network Flow(s) <-> Hash <-> Filename(s)

evt_to_gexf. 


![Gephi Output](/img/graph.png)

