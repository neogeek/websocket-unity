using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Events;
using WebSocketSharp;

namespace WebSocketUnity
{
    public class WebSocketJsonController : WebSocketController
    {
        public WebSocketJsonEventHandler JsonMessageHandler;

        private static readonly JsonSerializerSettings JSON_SETTINGS =
            new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore};

        private readonly Queue<JObject> _jsonMessageQueue = new Queue<JObject>();

        protected override void Update()
        {
            base.Update();

            while (_jsonMessageQueue.Count > 0)
            {
                var message = _jsonMessageQueue.Dequeue();

                if (_logEventsInEditor && Debug.isDebugBuild)
                {
                    Debug.Log($"Message received: {JsonConvert.SerializeObject(message)}");
                }

                JsonMessageHandler?.Invoke(message);
            }
        }

        protected override void HandleMessage(object sender, MessageEventArgs e)
        {
            try
            {
                var message = JObject.Parse(e.Data);

                _jsonMessageQueue.Enqueue(message);
            }
            catch (Exception)
            {
                _messageQueue.Enqueue(e.Data);
            }
        }

        public void Send(object message)
        {
            base.Send(JsonConvert.SerializeObject(message, Formatting.None, JSON_SETTINGS));
        }

        public IEnumerator SendAsync(object message)
        {
            return base.SendAsync(JsonConvert.SerializeObject(message, Formatting.None, JSON_SETTINGS));
        }

        public async Task SendAwait(object message)
        {
            await base.SendAwait(JsonConvert.SerializeObject(message, Formatting.None, JSON_SETTINGS));
        }

        [Serializable]
        public class WebSocketJsonEventHandler : UnityEvent<JObject>
        {
        }
    }
}