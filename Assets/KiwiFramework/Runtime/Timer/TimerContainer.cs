using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace KiwiFramework.Runtime
{
	/// <summary>
	/// 计时器容器
	/// </summary>
	public sealed class TimerContainer : IUpdate
	{
		/// <summary>
		/// 对象池容量
		/// </summary>
		private const int PoolCapacity = 10;

		/// <summary>
		/// 计时器池
		/// </summary>
		private readonly ObjectPool<Timer> _timerPool;

		/// <summary>
		/// 正在使用的计时器
		/// </summary>
		private readonly SortedList<int, Timer> _timers;

		/// <summary>
		/// 要移除的计时器
		/// </summary>
		private readonly List<Timer> _removes;

		/// <summary>
		/// Update队列排序
		/// </summary>
		public int UpdateOrder => 0;

		public TimerContainer()
		{
			_timerPool = new ObjectPool<Timer>(PoolCapacity, () => new Timer(), timer => timer.Dispose());

			_timers = new SortedList<int, Timer>();
			_removes = new List<Timer>();

			UpdateManager.Instance.Add(this);
		}

		/// <summary>
		/// 添加计时器
		/// </summary>
		/// <param name="timer">计时器</param>
		public void AddTimer(Timer timer)
		{
			if (timer == null || _timers.ContainsKey(timer.Guid)) return;

			_timers.Add(timer.Guid, timer);
		}

		/// <summary>
		/// 移除计时器
		/// </summary>
		/// <param name="timer">计时器</param>
		public void RemoveTimer(Timer timer)
		{
			if (timer == null || !_timers.ContainsKey(timer.Guid)) return;

			_removes.Add(timer);
		}

		/// <summary>
		/// 获得计时器
		/// </summary>
		/// <param name="guid">计时器唯一标识符</param>
		/// <returns></returns>
		public Timer GetTimer(int guid) => _timers.TryGetValue(guid, out var timer) ? timer : null;

		/// <summary>
		/// 获得计时器
		/// </summary>
		/// <param name="guid">计时器唯一标识符</param>
		/// <returns></returns>
		public bool TryGetTimer(int guid, out Timer timer)
		{
			return _timers.TryGetValue(guid, out timer);
		}

		/// <summary>
		/// 是否包含这个计时器
		/// </summary>
		/// <param name="guid">计时器唯一标识符</param>
		/// <returns></returns>
		public bool ExistTimer(int guid) => _timers.ContainsKey(guid);

		/// <summary>
		/// 从池中获得一个计时器
		/// </summary>
		/// <returns></returns>
		public Timer Pop() => _timerPool.Get();

		/// <summary>
		/// 回收计时器
		/// </summary>
		/// <param name="timer">要回收的计时器对象</param>
		/// <returns></returns>
		public bool Recycle(Timer timer) => _timerPool.Recycle(timer);

		public void OnUpdate(float deltaTime)
		{
			//移除已经停止的Timer
			if (_removes.Count > 0)
			{
				foreach (var removeTimer in _removes)
				{
					Recycle(removeTimer);
					_timers.Remove(removeTimer.Guid);
				}

				_removes.Clear();
			}

			if (_timers.Count <= 0) return;
			foreach (var timer in _timers.Values.Where(timer => !timer.IsPause))
			{
				timer.Tick(timer.IgnoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime);
			}
		}
	}
}
