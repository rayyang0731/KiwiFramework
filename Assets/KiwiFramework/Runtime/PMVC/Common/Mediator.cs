using System;

using JetBrains.Annotations;

namespace KiwiFramework.Runtime
{
	/// <summary>
	/// 中介器
	/// 主要职责：
	///     负责处理 Component 派发的事件和系统其他部分发出来的通知
	/// </summary>
	public abstract class Mediator : Notifier, IMediator
	{
		public string Name { get; private set; }

		/// <summary>
		/// 事件组
		/// </summary>
		private readonly EventGroup _eventGroup = new();

		/// <summary>
		/// 设置中介器名称
		/// </summary>
		/// <param name="name">中介器名称</param>
		internal void SetName(string name) { Name = name; }

		/// <summary>
		/// 加载时调用
		/// </summary>
		protected abstract void OnLoad();

		/// <summary>
		/// 卸载时调用
		/// </summary>
		protected abstract void OnUnload();

		/// <summary>
		/// 注册时调用
		/// </summary>
		public virtual void OnRegister() { OnLoad(); }

		/// <summary>
		/// 移除时调用
		/// </summary>
		public virtual void OnRemove()
		{
			OnUnload();
			RemoveAllListens();
		}

		/// <summary>
		/// 添加监听
		/// </summary>
		public void AddListener<TEvent>([NotNull] Action<IEventMessage> listener) where TEvent : IEventMessage
		{
			if (listener == null) throw new ArgumentNullException(nameof(listener));

			_eventGroup.AddListener<TEvent>(listener);
		}

		/// <summary>
		/// 移除监听
		/// </summary>
		private void RemoveListener<TEvent>([NotNull] Action<IEventMessage> listener) where TEvent : IEventMessage
		{
			if (listener == null) throw new ArgumentNullException(nameof(listener));

			_eventGroup.RemoveListener<TEvent>(listener);
		}

		/// <summary>
		/// 移除本次注册的所有监听
		/// </summary>
		public void RemoveAllListens() => _eventGroup.RemoveAllListener();
	}
}