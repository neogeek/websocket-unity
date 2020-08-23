using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using WebSocketSharp;

namespace WebSocketUnity
{

    public abstract class WebSocketJsonController<T> : WebSocketController
    {

        public WebSocketJsonEventHandler JsonMessageHandler;

        private readonly Queue<T> _jsonMessageQueue = new Queue<T>();

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

                var message = JsonConvert.DeserializeObject<T>(e.Data);

                _jsonMessageQueue.Enqueue(message);

            }
            catch (Exception _)
            {

                _messageQueue.Enqueue(e.Data);

            }

        }

        public void Send(object message)
        {

            base.Send(JsonConvert.SerializeObject(message));

        }

        public IEnumerator SendAsync(object message)
        {

            return base.SendAsync(JsonConvert.SerializeObject(message));

        }

        public async Task SendAwait(object message)
        {

            await base.SendAwait(JsonConvert.SerializeObject(message));

        }

        [Serializable]
        public class WebSocketJsonEventHandler : UnityEvent<T>
        {

        }

    }

}
