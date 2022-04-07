# Windows Network Process Monitor

.NET 6 Network Process Monitor for Windows systems

	- Monitors TCP, UDP and DNS traffic on a Windows system.
	- Enumerates underlying process information (file path, md5, sha1, sha256)
	- Logs results to Windows eventlog with a json payload.



## Quickstart
Extract the zip file to the directory you wish to run the service from and execute install_service.bat from a command prompt.

To uninstall the application execute the following from an administrative terminal

sc.exe stop "NetworkProcessMonitor" 
sc.exe delete "NetworkProcessMonitor" 

You can then delete the application folder.

## Building

This application is built against .net 6 and will probably require Visual Studio 2022.

## Capabilities

