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
Create or edit config.json in the executable folder with this content:
```json
{
  "HistorianClientConfiguration": {
  "ServerName": "<Historian server or environment variable>",
  "UserName": "<Windows domain user or environment variable>",
  "Password": "<Windows domain password or environment variable>",
  "TagsToSubscribe": [
    { "TagName": "<Tag name>" },
      "MinimumElapsedMilliSeconds": 1000
    }
  ]
  },
  "WebSocketServiceConfiguration": {
    "Address": "<Websocket address or environment variable, for example ws://0.0.0.0:15099>"
  }
}
```
MinimumElapsedMilliSeconds can be omitted, defauæt is 1000 ms.

## Connect to the service
To test the service you can use for example the Browser WebSocket Client Chrome extension: https://chrome.google.com/webstore/detail/browser-websocket-client/mdmlhchldhfnfnkfmljgeinlffmdgkjo