# websocket-unity

> Simple wrapper for the websocket-csharp library.

## Installation

### Unity Package Manager

<https://docs.unity3d.com/Packages/com.unity.package-manager-ui@2.0/manual/index.html>

#### Git

```json
{
  "dependencies": {
    "com.neogeek.websocket-unity": "https://github.com/neogeek/websocket-unity.git#v1.1.0",
    ...
  }
}
```

## Usage

### WebSocketController

Add the `WebSocketController` component to any GameObject.

#### Properties

- **URL** - The WebSocket URL, including the protocol (ws or wss) and the port.
- **Log Events in Editor** - For debugging purposes only. Will only work in editor and in builds marked as development.
- **Keep-Alive Timeout** - WebSockets timeout after around 60 seconds. This timeout is used to ping the server to maintain a connection.
- **Message Handler** - Attach an event to this either via code or the inspector to receive messages from the WebSocket server.

<img src="https://i.imgur.com/8POoQnB.png" width="400">

### WebSocketJsonController

`WebSocketJsonController<T>` is an abstract class for handling custom JSON responses from a WebSocket server.

#### Example

```csharp
using WebSocketUnity;

public struct Message
{

    public string type;

    public string gameId;

    public string gameCode;

    public string playerId;

}

public class WebSocketGameLobbyClient : WebSocketJsonController<Message>
{

    public void SendMessage()
    {

        Send(new Message { type = "ping", gameId = "1", gameCode = "ABCD", playerId = "2" });

    }

}
```

<img src="https://i.imgur.com/84XU6vp.png" width="400">
