# Windows Network Process Monitor

.NET 6 Event Tracing based network process monitor for Windows based systems.

- Monitors TCP, UDP and DNS traffic on a Windows system.
- Enumerates underlying process information (file path, md5, sha1, sha256)
- Logs results to Windows eventlog with a json payload.
- Lightweight with internal caching mechanisms to minimize system resource utilization.

Fundementally the application attempts to answer the first question when a security observation is made on the wire - primarily what caused this traffic to occur.



## Quickstart
Extract the zip file to the directory you wish to run the service from and execute install_service.bat from a command prompt.

Alternatively you can install the service normally with sc.exe

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


## Building

The application is built in .net 6 and can target both x86 and x64 Windows editions.  
The application will work with .net 5 but requires modification to the csproj file in order to pack to a single exe.

```
dotnet restore
dotnet build --configuration Release --no-restore
dotnet publish -r win-x64 /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true --self-contained true
```

## Gotchas

The application as is meant to monitor endpoint systems.  Monitoring server systems that provide UDP services, such as DNS servers will result in a extreme amount of event log entries.
UDP record caching code can be customized to deal with these scenarios.

## Internals

The application creates a keyword filtered trace sessions on the kernel and the Windows DNS client.

Speed and performance have been valued over fidelity.  The application makes no attempt to maintain a accurate audit trail of all network and process activity.  
Process and UDP tracing implment caching to minimize data collection,



## Analysis Considerations

When analyzing these logs it's important to remember that the majority of DNS traffic will originate from internal Windows DNS Client stub resolver.  This has several properties that should be considered.

- DNS records are not directly correlated to the process that initiated them.
- Some applications that implment their own resolution will be seen communciating outbound on port 53
- 