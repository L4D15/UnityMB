using System;
using System.Collections.Generic;
using UnityEngine;


namespace Becerra.UnityMB
{
    public class MessageBus : IMessageBus
    {
        readonly IDictionary<Type, IList<object>> _subscribers = new Dictionary<Type, IList<object>>();

        public void Publish<T>(T message) where T : IMessage
        {
            var type = typeof(T);

            if (_subscribers.ContainsKey(type) == false) return;

            var list = _subscribers[type];

            for (int i = 0; i < list.Count; i++)
            {
                (list[i] as IHandler<T>).Handle(message);
            }
        }

        public void Subscribe<T>(IHandler<T> subscriber) where T : IMessage
        {
            var type = typeof(T);

            if (_subscribers.ContainsKey(type) == false)
            {
                var list = new List<object>();
                _subscribers[type] = list;
            }

            _subscribers[type].Add(subscriber);
        }

        public void Unsubscribe<T>(IHandler<T> subscriber) where T : IMessage
        {
            var type = typeof(T);

            if (_subscribers.ContainsKey(type) == false || _subscribers[type].Contains(subscriber) == false)
            {
                Debug.LogWarning("Unsubscribing from " + type + " but it was never subscribed to.");
                return;
            }

            _subscribers[type].Remove(subscriber);
        }
    }
}
