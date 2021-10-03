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
#pragma warning disable CS0649
        [SerializeField] protected string _url;

        [SerializeField] protected bool _logEventsInEditor;

        [SerializeField] protected float _keepAliveTimeout = 30;
#pragma warning restore CS0649

        public WebSocketEventHandler MessageHandler;

        protected readonly Queue<string> _messageQueue = new Queue<string>();

        protected float _keepAliveNextTick;

        protected WebSocket _webSocket;

        public bool isConnected { get; protected set; }

        protected void Awake()
        {
            _webSocket = new WebSocket(_url);

            _webSocket.OnOpen += HandleOpen;
            _webSocket.OnMessage += HandleMessage;
            _webSocket.OnClose += HandleClose;
            _webSocket.OnError += HandleError;
        }

        protected virtual void Update()
        {
            while (_messageQueue.Count > 0)
            {
                var message = _messageQueue.Dequeue();

                if (_logEventsInEditor && Debug.isDebugBuild)
                {
                    Debug.Log($"Message received: {message}");
                }

                MessageHandler?.Invoke(message);
            }

            if (Time.time > _keepAliveNextTick)
            {
                Send("ping");

                _keepAliveNextTick = Time.time + _keepAliveTimeout;
            }
        }

        protected void OnEnable()
        {
            _webSocket.Connect();
        }

        protected void OnDisable()
        {
            _webSocket.Close();
        }

        protected void HandleOpen(object sender, EventArgs e)
        {
            if (_logEventsInEditor && Debug.isDebugBuild)
            {
                Debug.Log($"Connected to {_url}");
            }

            isConnected = true;
        }

        protected virtual void HandleMessage(object sender, MessageEventArgs e)
        {
            _messageQueue.Enqueue(e.Data);
        }

        protected void HandleClose(object sender, CloseEventArgs e)
        {
            if (_logEventsInEditor && Debug.isDebugBuild)
            {
                Debug.Log($"Disconnected from {_url}");
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
        public class WebSocketEventHandler : UnityEvent<string>
        {
        }
    }
}