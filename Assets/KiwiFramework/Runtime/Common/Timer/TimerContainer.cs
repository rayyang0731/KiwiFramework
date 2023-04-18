using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace KiwiFramework.Runtime
{
	/// <summary>
	/// 计时器容器
	/// </summary>
	public sealed class TimerContainer : Singleton<TimerContainer>
	{
		protected override bool useDriver => true;

		/// <summary>
		/// 对象池容量
		/// </summary>
		private const int CONST_POOL_CAPACITY = 32;

		/// <summary>
		/// 计时器池
		/// </summary>
		private ObjectPool<Timer> _timerPool;

		/// <summary>
		/// 正在使用的计时器
		/// </summary>
		private List<Timer> _timers;

		/// <summary>
		/// 要移除的计时器
		/// </summary>
		private List<Timer> _removes;

		protected override void SingletonUpdate()
		{
			if (_timers.Count <= 0)
				return;

			//每帧开始 移除并回收被标脏的已经停止的Timer
			if (_removes.Count > 0)
			{
				foreach (var timer in _removes)
				{
					Recycle(timer);
					_timers.Remove(timer);
				}

				_removes.Clear();
			}

			foreach (var timer in _timers.Where(timer => !timer.IsPause))
			{
				timer.Tick(timer.IgnoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime);
			}
		}

		protected override void OnSingletonInit()
		{
			_timerPool = new ObjectPool<Timer>(CONST_POOL_CAPACITY, CreateTimer, DestroyTimer);
			_timers    = new List<Timer>();
			_removes   = new List<Timer>();
		}

		protected override void OnSingletonReset() { Clear(); }
		protected override void OnSingletonRelease() { Clear(); }

		/// <summary>
		/// 获得计时器
		/// </summary>
		/// <param name="guid">计时器唯一标识符</param>
		/// <returns></returns>
		public Timer GetTimer(int guid) => _timers.Find(timer => timer.ID == guid);

		/// <summary>
		/// 是否包含这个计时器
		/// </summary>
		/// <param name="guid">计时器唯一标识符</param>
		/// <returns></returns>
		public bool ExistTimer(int guid) => _timers.Exists(timer => timer.ID == guid);

		/// <summary>
		/// 是否包含这个计时器
		/// </summary>
		/// <param name="timer">要检测的 Timer 对象</param>
		/// <returns></returns>
		public bool ExistTimer(Timer timer) => _timers.Contains(timer);

		/// <summary>
		/// 添加计时器
		/// </summary>
		/// <param name="timer">计时器</param>
		internal void AddTimer(Timer timer)
		{
			if (_timers.Contains(timer)) return;

			_timers.Add(timer);
		}

		/// <summary>
		/// 移除计时器
		/// </summary>
		/// <param name="timer">计时器</param>
		internal void RemoveTimer(Timer timer)
		{
			if (_timers.Contains(timer))
				_removes.Add(timer);
		}

		/// <summary>
		/// 从池中获得一个计时器
		/// </summary>
		/// <returns></returns>
		internal Timer Get() => _timerPool.Get();

		/// <summary>
		/// 回收计时器
		/// </summary>
		/// <param name="timer">要回收的计时器对象</param>
		/// <returns></returns>
		private bool Recycle(Timer timer) => _timerPool.Recycle(timer);

		/// <summary>
		/// 删除计时器
		/// </summary>
		/// <param name="obj">要删除的计时器</param>
		private void DestroyTimer(Timer obj) => obj.Dispose();

		/// <summary>
		/// 创建计时器
		/// </summary>
		/// <returns></returns>
		private Timer CreateTimer() => new();

		/// <summary>
		/// 清除全部计时器
		/// </summary>
		private void Clear()
		{
			_timerPool.Clear();
			_timers.Clear();
			_removes.Clear();
		}
	}
}