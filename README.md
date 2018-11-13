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
   "RabbitMQConfiguration": {
        "Hostname": "<RabbitMQ server or environment variable>",
        "Username": "<RabbitMQ user name or environment variable>",
        "Password": "<RabbitMQ password or environment variable>",
        "SensorDataEventQueue": "<Name of the queue that holds sensor data messages or environment variable>",
        "ConfigurationEventQueue": "<Name of the queue that holds configuration messages or environment variable>"
    },
  "WebSocketServiceConfiguration": {
    "Address": "<Websocket address or environment variable, for example ws://0.0.0.0:15099>"
  }
}
```
## Change which tags are subscribed to
You can change which tag data event subscribtion by sending this message from the web socket client, or adding it to the configuration event queue:
```json
{
    "SubscribeMessage": {
        "Tags": [
            { 
                "TagName": "<Tag name to subscribe to>",
                "MinimumElapsedMilliSeconds": <The minimum number of milliseconds between successive updates. Used to throttle high frequency tags.>
            }
        ]
    },
    "UnsubscribeMessage": {
        "Tagnames": [
            "<Tag name to unsubscribe>"
            ]
    }
}
```
## Sensor data message format
```json
{
    "SensorData": [
        {
            "Tagname": "<Tag name>",
            "Value": <data point value>,
            "Time": <unix date time (milliseconds since January 1st 1970)>,
            "Quality": "<ata quality, i.e. Good>"
        }
    ]
}
```