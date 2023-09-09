using System;
using System.Collections.Generic;

using UnityEngine;

namespace KiwiFramework.Runtime
{
	/// <summary>
	/// 事件系统
	/// </summary>
	public class EventSystem : Singleton<EventSystem>
	{
		/// <summary>
		/// 延迟事件包装层
		/// </summary>
		private class PostWrapper
		{
			public int PostFrame;
			public int EventID;
			public IEventMessage Message;
		}

		protected override bool useDriver => true;

		/// <summary>
		/// 全部同步事件列表
		/// </summary>
		private readonly Dictionary<int, LinkedList<Action<IEventMessage>>> _listeners = new(1024);

		/// <summary>
		/// 延迟事件列表
		/// </summary>
		private readonly List<PostWrapper> _postingList = new(128);

		protected override void SingletonUpdate()
		{
			for (var i = _postingList.Count - 1; i >= 0; i--)
			{
				var wrapper = _postingList[i];
				if (Time.frameCount > wrapper.PostFrame)
				{
					SendMessage(wrapper.EventID, wrapper.Message);
					_postingList.RemoveAt(i);
				}
			}
		}

		protected override void OnSingletonReset() => ClearAll();

		protected override void OnSingletonRelease() => ClearAll();

		/// <summary>
		/// 清空所有监听
		/// </summary>
		public static void ClearAll()
		{
			foreach (var eventId in Instance._listeners.Keys)
			{
				Instance._listeners[eventId].Clear();
			}

			Instance._listeners.Clear();
			Instance._postingList.Clear();
		}

		/// <summary>
		/// 添加监听
		/// </summary>
		public static void AddListener<TEvent>(Action<IEventMessage> listener) where TEvent : IEventMessage
		{
			var eventType = typeof(TEvent);
			var eventId   = eventType.GetHashCode();
			AddListener(eventId, listener);
		}

		/// <summary>
		/// 添加监听
		/// </summary>
		public static void AddListener(Type eventType, Action<IEventMessage> listener)
		{
			var eventId = eventType.GetHashCode();
			AddListener(eventId, listener);
		}

		/// <summary>
		/// 添加监听
		/// </summary>
		public static void AddListener(int eventId, Action<IEventMessage> listener)
		{
			if (Instance._listeners.ContainsKey(eventId) == false)
				Instance._listeners.Add(eventId, new LinkedList<Action<IEventMessage>>());
			if (Instance._listeners[eventId].Contains(listener) == false)
				Instance._listeners[eventId].AddLast(listener);
		}


		/// <summary>
		/// 移除监听
		/// </summary>
		public static void RemoveListener<TEvent>(Action<IEventMessage> listener) where TEvent : IEventMessage
		{
			var eventType = typeof(TEvent);
			var eventId   = eventType.GetHashCode();
			RemoveListener(eventId, listener);
		}

		/// <summary>
		/// 移除监听
		/// </summary>
		public static void RemoveListener(Type eventType, Action<IEventMessage> listener)
		{
			var eventId = eventType.GetHashCode();
			RemoveListener(eventId, listener);
		}

		/// <summary>
		/// 移除监听
		/// </summary>
		public static void RemoveListener(int eventId, Action<IEventMessage> listener)
		{
			if (Instance._listeners.ContainsKey(eventId))
			{
				if (Instance._listeners[eventId].Contains(listener))
					Instance._listeners[eventId].Remove(listener);
			}
		}


		/// <summary>
		/// 实时广播事件
		/// </summary>
		public static void SendMessage(IEventMessage message)
		{
			int eventId = message.GetType().GetHashCode();
			SendMessage(eventId, message);
		}

		/// <summary>
		/// 实时广播事件
		/// </summary>
		public static void SendMessage(int eventId, IEventMessage message)
		{
			if (Instance._listeners.ContainsKey(eventId) == false)
				return;

			var listeners = Instance._listeners[eventId];
			if (listeners.Count > 0)
			{
				var currentNode = listeners.Last;
				while (currentNode != null)
				{
					currentNode.Value.Invoke(message);
					currentNode = currentNode.Previous;
				}
			}
		}

		/// <summary>
		/// 延迟广播事件
		/// </summary>
		public static void PostMessage(IEventMessage message)
		{
			var eventId = message.GetType().GetHashCode();
			PostMessage(eventId, message);
		}

		/// <summary>
		/// 延迟广播事件
		/// </summary>
		public static void PostMessage(int eventId, IEventMessage message)
		{
			var wrapper = new PostWrapper {PostFrame = Time.frameCount, EventID = eventId, Message = message,};
			Instance._postingList.Add(wrapper);
		}
	}
}