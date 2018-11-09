# Proficy Historian Gateway
Gateway to GE Proficy Historian.
This service works as a gateway between a historian running in an OT network and exposes tag data changed events as web socket messages to applications running in an IT network.
GET Proficy Historian data changed events are forwarded to web socket clients and a RabbitMQ queue.

# Running the Application from the Command Line
Simply run 
```
Proficy.Historian.Gateway.Service.exe
```
# Installing the Service
To install the application as a Windows service:
```
Proficy.Historian.Gateway.Service.exe install -servicename "Proficy.Historian.Gateway.Service" -displayname "Proficy Historian Gateway Service" -description "Websocket gateway to GE Proficy Historian"
```
# Uninstalling the Service
To uninstall the Windows service:
```
Proficy.Historian.Gateway.Service.exe uninstall 
```
## Configuration
Create or edit config.json in the executable folder with this content:
```json
{
  "HistorianClientConfiguration": {
  "ServerName": "<Historian server or environment variable>",
  "UserName": "<Windows domain user or environment variable>",
  "Password": "<Windows domain password or environment variable>"
  },
  "WebSocketServiceConfiguration": {
    "Address": "<Websocket address or environment variable, for example ws://0.0.0.0:15099>"
  }
}
```
## Connect to the service
To test the service you can use for example the Browser WebSocket Client Chrome extension: https://chrome.google.com/webstore/detail/browser-websocket-client/mdmlhchldhfnfnkfmljgeinlffmdgkjo

## Subscribe or Unsubscribe to Tag Data Changed Messages
To subscribe or unsubscribe to tag data changed messages, send a message containing a historian message:
```json
{
  "SubscribeMessage": {
    "Tags": [
	{
		"TagName": "<Tag name>" ,
		"MinimumElapsedMilliSeconds": 1000
	}]
  },
  "UnsubscribeMessage": {
    "TagNames": [
		"<Tag name>"
	]
  }
}
```
MinimumElapsedMilliSeconds : Minimum elapsed milliseconds between data changed messages, can be omitted, default is 1000 ms.