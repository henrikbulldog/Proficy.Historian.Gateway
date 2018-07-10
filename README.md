# Proficy Historian Gateway
Websocket gateway to GE Proficy Historian

# Running the Application from the Command Line
Simply run Proficy.Historian.Gateway.Service.exe

# Installing the Service
To install the application as a Windows service:

Proficy.Historian.Gateway.Service.exe install -servicename "Proficy.Historian.Gateway.ServiceClient" -displayname "GE Proficy Histoeian Gateway Service" -description "Websocket gateway to GE Proficy Historian"

# Uninstalling the Service
To uninstall the Windows service:

Proficy.Historian.Gateway..exe uninstall 


## Configuration
Create a file named historian-config.json in the executable folder:

{
  "ServerName": "<Historian server>",
  "UserName": "<Windows domain user>",
  "Password": "<Windows domain password>",
  "PrintToConsole": True,
  "TagsToSubscribe": [
    { "TagName": "<Tag name>" },
      "MinimumElapsedMilliSeconds": <Elapsed milliseconds, can be omitted>
    }
  ]
}


