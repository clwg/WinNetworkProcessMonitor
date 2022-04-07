# Windows Network Process Monitor

.NET 6 Network Process Monitor for Windows systems

	- Monitors TCP, UDP and DNS traffic on a Windows system.
	- Enumerates underlying process information (file path, md5, sha1, sha256)
	- Logs results to Windows eventlog with a json payload.



## Quickstart
```
@echo OFF
SET currentdir=%cd%
SET path=%currentdir%\WinNetworkProcessMonitor.exe
echo installing to %path%
C:\windows\system32\sc.exe create "NetworkProcessMonitor" binpath="%path%" start=auto
```

sc.exe stop "NetworkProcessMonitor" 
sc.exe delete "NetworkProcessMonitor" 


## Building


## Capabilities

