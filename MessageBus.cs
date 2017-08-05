using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Becerra.UnityMB
{
    public class MessageBus : IMessageBus
    {
        readonly IDictionary<Type, IList<object>> _subscribers = new Dictionary<Type, IList<object>>();
        readonly IList<object> _newSubscriptions = new List<object>();
        readonly IList<object> _removedSubscriptions = new List<object>();
        int _publishingDepth;

        public bool IsPublishing { get { return _publishingDepth > 0; } }

        public void Subscribe(object subscriber)
        {
            _newSubscriptions.Add(subscriber);
            UpdateSubscriptions();
        }

        public void Unsubscribe(object subscriber)
        {
            _removedSubscriptions.Add(subscriber);
            UpdateSubscriptions();
        }

        public void Publish<T>(T message) where T : IMessage
        {
            if (!_subscribers.ContainsKey(typeof (T)) || !_subscribers[typeof (T)].Any())
            {
                Debug.LogWarning(string.Format("Message {0} has no subscribers", typeof(T).FullName));
                return;
            }

            _publishingDepth++;

            foreach (var handler in _subscribers[typeof(T)])
                ((IHandler<T>)handler).Handle(message);

            _publishingDepth--;
            UpdateSubscriptions();
        }

        List<Type> GetHandledMessageTypes(object subscriber)
        {
            return subscriber.GetType()
                .GetInterfaces()
                .Where(handler => handler.IsGenericType && handler.GetGenericTypeDefinition() == typeof (IHandler<>))
                .Select(handler => handler.GetGenericArguments().Single())
                .ToList();
        }

        void UpdateSubscriptions()
        {
            if (IsPublishing) return;

            foreach (var subscriber in _newSubscriptions)
            {
                GetHandledMessageTypes(subscriber).ForEach(message => Subscribe(message, subscriber));
            }

            foreach (var subscriber in _removedSubscriptions)
            {
                GetHandledMessageTypes(subscriber).ForEach(message => Unsubscribe(message, subscriber));
            }

            _newSubscriptions.Clear();
            _removedSubscriptions.Clear();
        }

        void Subscribe(Type messageType, object subscriber)
        {
            if (!_subscribers.ContainsKey(messageType))
                _subscribers[messageType] = new List<object>();

            _subscribers[messageType].Add(subscriber);
        }

        void Unsubscribe(Type messageType, object subscriber)
        {
            if (!_subscribers.ContainsKey(messageType) || !_subscribers[messageType].Contains(subscriber))
            {
                Debug.LogWarning(string.Format("Unsubscribing {0} but it was never subscribed", subscriber.GetType().FullName));
                return;
            }

            _subscribers[messageType].Remove(subscriber);
        }
    }
}
