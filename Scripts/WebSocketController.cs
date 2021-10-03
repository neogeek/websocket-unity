using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using WebSocketSharp;

namespace WebSocketUnity
{
    public class WebSocketController : MonoBehaviour
    {
        public const float DEFAULT_KEEP_ALIVE_TIMEOUT = 30;

        public string url { get; protected set; }

        public bool isConnected { get; protected set; }

        public bool logEventsInEditor;

        public float keepAliveTimeout = DEFAULT_KEEP_ALIVE_TIMEOUT;

        public delegate void WebSocketStringEvent(string data);

        public event WebSocketStringEvent StringMessageHandler;

        [SerializeField] public WebSocketStringUnityEvent _stringMessageHandler;

        protected readonly Queue<string> _messageQueue = new Queue<string>();

        protected float _keepAliveNextTick;

        protected WebSocket _webSocket;

        protected void Awake()
        {
            if (url != null)
            {
                Connect(url);
            }
        }

        public void Connect(string url)
        {
            this.url = url;

            if (_webSocket != null)
            {
                Disconnect();
            }

            _webSocket = new WebSocket(url);

            _webSocket.OnOpen += HandleOpen;
            _webSocket.OnMessage += HandleMessage;
            _webSocket.OnClose += HandleClose;
            _webSocket.OnError += HandleError;

            _webSocket.Connect();
        }

        public void Disconnect()
        {
            if (_webSocket == null)
            {
                return;
            }

            _webSocket.OnOpen -= HandleOpen;
            _webSocket.OnMessage -= HandleMessage;
            _webSocket.OnClose -= HandleClose;
            _webSocket.OnError -= HandleError;

            _webSocket.Close();

            _webSocket = null;
        }

        protected virtual void Update()
        {
            while (_messageQueue.Count > 0)
            {
                var message = _messageQueue.Dequeue();

                if (logEventsInEditor && Debug.isDebugBuild)
                {
                    Debug.Log($"Message received: {message}");
                }

                _stringMessageHandler?.Invoke(message);

                StringMessageHandler?.Invoke(message);
            }

            if (Time.time > _keepAliveNextTick)
            {
                Send("ping");

                _keepAliveNextTick = Time.time + keepAliveTimeout;
            }
        }

        protected void OnEnable()
        {
            _webSocket?.Connect();
        }

        protected void OnDisable()
        {
            _webSocket?.Close();
        }

        protected void HandleOpen(object sender, EventArgs e)
        {
            if (logEventsInEditor && Debug.isDebugBuild)
            {
                Debug.Log($"Connected to {url}");
            }

            isConnected = true;
        }

        protected virtual void HandleMessage(object sender, MessageEventArgs e)
        {
            _messageQueue.Enqueue(e.Data);
        }

        protected void HandleClose(object sender, CloseEventArgs e)
        {
            if (logEventsInEditor && Debug.isDebugBuild)
            {
                Debug.Log($"Disconnected from {url}");
            }

            isConnected = false;
        }

        public static void HandleError(object sender, ErrorEventArgs e)
        {
            Debug.LogError(e.Message);
        }

        public void Send(string message)
        {
            if (isConnected && _webSocket.IsAlive)
            {
                _webSocket.Send(message);
            }
        }

        public IEnumerator SendAsync(string message)
        {
            if (isConnected && _webSocket.IsAlive)
            {
                var completed = false;

                _webSocket.SendAsync(message, _ => completed = true);

                while (completed == false)
                {
                    yield return null;
                }
            }
        }

        public async Task SendAwait(string message)
        {
            if (isConnected && _webSocket.IsAlive)
            {
                var completed = false;

                _webSocket.SendAsync(message, _ => completed = true);

                while (completed == false)
                {
                    await Task.Yield();
                }
            }
        }

        [Serializable]
        public class WebSocketStringUnityEvent : UnityEvent<string>
        {
        }
    }
}