using System;
using System.Collections.Generic;

using UnityEngine;

namespace KiwiFramework.Runtime
{
	public class EventGroup
	{
		private readonly Dictionary<Type, List<Action<IEventMessage>>> _cachedListener = new();

		/// <summary>
		/// 添加一个监听
		/// </summary>
		public void AddListener<TEvent>(Action<IEventMessage> listener) where TEvent : IEventMessage
		{
			var eventType = typeof(TEvent);
			if (_cachedListener.ContainsKey(eventType) == false)
				_cachedListener.Add(eventType, new List<Action<IEventMessage>>());

			if (_cachedListener[eventType].Contains(listener) == false)
			{
				_cachedListener[eventType].Add(listener);
				EventSystem.AddListener(eventType, listener);
			}
			else
			{
				Debug.LogWarning($"事件监听已经存在 : {eventType}");
			}
		}

		/// <summary>
		/// 移除所有缓存的监听
		/// </summary>
		public void RemoveAllListener()
		{
			foreach (var (eventType, value) in _cachedListener)
			{
				for (var i = 0; i < value.Count; i++)
				{
					EventSystem.RemoveListener(eventType, value[i]);
				}

				value.Clear();
			}

			_cachedListener.Clear();
		}
	}
}