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

`WebSocketJsonController` is similar to the base `WebSocketController` class, but this class also handles custom JSON responses from a WebSocket server.

#### Example

```csharp
using Newtonsoft.Json.Linq;
using UnityEngine;
using WebSocketUnity;

public struct Message
{

    public string type;

    public string gameId;

    public string gameCode;

    public string playerId;

}

public class WebSocketGameLobbyClient : MonoBehaviour
{

    [SerializeField]
    private WebSocketJsonController _webSocketJsonController;

    public void HandleMessage(JObject message)
    {

        Debug.Log(message["game"]?["gameId"]);

    }

    public void CreateGame()
    {

        _webSocketJsonController.Send(new Message { type = "create" });

    }

    public void JoinGame()
    {

        _webSocketJsonController.Send(new Message { type = "join" });

    }

}
```

<img src="https://i.imgur.com/YBfdwAX.png" width="400">
