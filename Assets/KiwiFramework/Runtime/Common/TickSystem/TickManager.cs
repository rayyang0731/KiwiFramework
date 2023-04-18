using System;
using System.Collections.Generic;

using Sirenix.OdinInspector;

using UnityEngine;

namespace KiwiFramework.Runtime
{
	/// <summary>
	/// Tick 管理器
	/// </summary>
	public class TickManager : Singleton<TickManager>
	{
		protected override bool useDriver => true;

		/// <summary>
		/// Update 队列
		/// </summary>
		[ShowInInspector,
		 TitleGroup("Tick Runtime", "Centralized processing tick object", TitleAlignments.Centered),
		 LabelText("Tick Pool"), ListDrawerSettings(IsReadOnly = true),]
		private readonly List<ITick> _tickStore = new();

		/// <summary>
		/// FixedTick 队列
		/// </summary>
		[ShowInInspector, LabelText("FixedTick Pool"), ListDrawerSettings(IsReadOnly = true),]
		private readonly List<IFixedTick> _fixedTickStore = new();

		/// <summary>
		/// LateTick 队列
		/// </summary>
		[ShowInInspector, LabelText("LateTick Pool"), ListDrawerSettings(IsReadOnly = true),]
		private readonly List<ILateTick> _lateUpdateStore = new();

		/// <summary>
		/// 每帧间隔时间差
		/// </summary>
		private float _deltaTime;

		/// <summary>
		/// 准备移除的 Tick 对象
		/// </summary>
		private readonly List<ITick> _wannaRemoveTicks = new();

		/// <summary>
		/// 准备移除的 FixedTick 对象
		/// </summary>
		private readonly List<IFixedTick> _wannaRemoveFixedTicks = new();

		/// <summary>
		/// 准备移除的 LateTick 对象
		/// </summary>
		private readonly List<ILateTick> _wannaRemoveLateTicks = new();

		protected override void OnSingletonReset() { DestroyAll(); }
		protected override void OnSingletonRelease() { DestroyAll(); }

		protected override void SingletonUpdate()
		{
			var count = _wannaRemoveTicks.Count;
			if (count > 0)
			{
				for (var i = 0; i < count; i++)
				{
					var tick = _wannaRemoveTicks[i];
					if (_tickStore.Contains(tick))
						_tickStore.Remove(tick);
				}

				_wannaRemoveTicks.Clear();
			}

			if (_tickStore.Count == 0) return;

			_deltaTime = Time.deltaTime;

			for (var i = _tickStore.Count - 1; i >= 0; i--)
			{
				var obj = _tickStore[i];
				if (obj == null || obj.Equals(null))
				{
					_tickStore.RemoveAt(i);
					continue;
				}

				var canTick = obj.runInBackground || obj is Behaviour {isActiveAndEnabled: true};
				if (!canTick) continue;

				try
				{
					obj.OnTick(_deltaTime);
				}
				catch (Exception e)
				{
					Debug.LogException(new Exception($"{obj}运行 Tick 方法时发生错误", e));
				}
			}
		}

		protected override void SingletonLateUpdate()
		{
			var count = _wannaRemoveLateTicks.Count;
			if (count > 0)
			{
				for (var i = 0; i < count; i++)
				{
					var tick = _wannaRemoveLateTicks[i];
					if (_lateUpdateStore.Contains(tick))
						_lateUpdateStore.Remove(tick);
				}

				_wannaRemoveLateTicks.Clear();
			}

			if (_lateUpdateStore.Count == 0) return;

			for (var i = _lateUpdateStore.Count - 1; i >= 0; i--)
			{
				var obj = _lateUpdateStore[i];
				if (obj == null || obj.Equals(null))
				{
					_lateUpdateStore.RemoveAt(i);
					continue;
				}

				var canTick = obj.runInBackground || obj is Behaviour {isActiveAndEnabled: true};
				if (!canTick) continue;

				try
				{
					obj.OnLateTick(_deltaTime);
				}
				catch (Exception e)
				{
					Debug.LogException(new Exception($"{obj}运行 LateTick 方法时发生错误", e));
				}
			}
		}

		protected override void SingletonFixedUpdate()
		{
			var count = _wannaRemoveFixedTicks.Count;
			if (count > 0)
			{
				for (var i = 0; i < count; i++)
				{
					var tick = _wannaRemoveFixedTicks[i];
					if (_fixedTickStore.Contains(tick))
						_fixedTickStore.Remove(tick);
				}

				_wannaRemoveFixedTicks.Clear();
			}

			if (_fixedTickStore.Count == 0) return;

			var fixedDeltaTime = Time.fixedDeltaTime;

			for (var i = _fixedTickStore.Count - 1; i >= 0; i--)
			{
				var obj = _fixedTickStore[i];
				if (obj == null || obj.Equals(null))
				{
					_fixedTickStore.RemoveAt(i);
					continue;
				}

				var canTick = obj.runInBackground || obj is Behaviour {isActiveAndEnabled: true};
				if (!canTick) continue;

				try
				{
					obj.OnFixedTick(fixedDeltaTime);
				}
				catch (Exception e)
				{
					Debug.LogException(new Exception($"{obj}运行 FixedTick 方法时发生错误", e));
				}
			}
		}

		/// <summary>
		/// 向更新列表中添加对象
		/// </summary>
		/// <param name="obj">要添加的更新对象</param>
		/// <typeparam name="T">实现 ITick,IFixedTick 或 ILateTick 接口的对象</typeparam>
		public void Add<T>(T obj)
		{
			if (obj is ITick tick)
				AddTick(tick);

			if (obj is IFixedTick fixedTick)
				AddFixedTick(fixedTick);

			if (obj is ILateTick lateTick)
				AddLateTick(lateTick);
		}

		/// <summary>
		/// 从更新列表中移除对象
		/// </summary>
		/// <param name="obj">要移除的更新对象</param>
		/// <typeparam name="T"></typeparam>
		public void Remove<T>(T obj)
		{
			if (obj is ITick tick)
				RemoveTick(tick);

			if (obj is IFixedTick fixedTick)
				RemoveFixedTick(fixedTick);

			if (obj is ILateTick lateTick)
				RemoveLateTick(lateTick);
		}

		/// <summary>
		/// 是否存在此实例
		/// </summary>
		/// <param name="obj">要判断的更新对象</param>
		/// <returns></returns>
		public bool Exist<T>(T obj)
		{
			return obj switch
			       {
				       ITick tick           => _tickStore.Contains(tick),
				       IFixedTick fixedTick => _fixedTickStore.Contains(fixedTick),
				       ILateTick lateTick   => _lateUpdateStore.Contains(lateTick),
				       _                    => false,
			       };
		}

		private void DestroyAll()
		{
			_tickStore.Clear();
			_fixedTickStore.Clear();
			_lateUpdateStore.Clear();

			_wannaRemoveTicks.Clear();
			_wannaRemoveFixedTicks.Clear();
			_wannaRemoveLateTicks.Clear();
		}

		/// <summary>
		/// 添加要执行 Tick 方法的对象
		/// </summary>
		/// <param name="tick">要执行 Tick 方法的对象</param>
		private void AddTick(ITick tick)
		{
			if (_tickStore.Contains(tick)) return;
			_tickStore.Add(tick);

			if (tick.TickOrder > 0)
				_tickStore.Sort((x, y) => x.TickOrder.CompareTo(y.TickOrder));
		}

		/// <summary>
		/// 移除正在执行 Tick 方法的对象
		/// </summary>
		/// <param name="tick">正在执行 Tick 方法的对象</param>
		private void RemoveTick(ITick tick) { _wannaRemoveTicks.Add(tick); }

		/// <summary>
		/// 添加要执行 FixedTick 方法的对象
		/// </summary>
		/// <param name="fixedTick">要执行 FixedTick 方法的对象</param>
		private void AddFixedTick(IFixedTick fixedTick)
		{
			if (_fixedTickStore.Contains(fixedTick)) return;
			_fixedTickStore.Add(fixedTick);

			if (fixedTick.FixedTickOrder > 0)
				_fixedTickStore.Sort((x, y) => x.FixedTickOrder.CompareTo(y.FixedTickOrder));
		}

		/// <summary>
		/// 移除正在执行 FixedTick 方法的对象
		/// </summary>
		/// <param name="fixedTick">正在执行 FixedTick 方法的对象</param>
		private void RemoveFixedTick(IFixedTick fixedTick) { _wannaRemoveFixedTicks.Add(fixedTick); }

		/// <summary>
		/// 添加要执行 LateTick 方法的对象
		/// </summary>
		/// <param name="lateTick">要执行 LateTick 方法的对象</param>
		private void AddLateTick(ILateTick lateTick)
		{
			if (_lateUpdateStore.Contains(lateTick)) return;
			_lateUpdateStore.Add(lateTick);

			if (lateTick.LateTickOrder > 0)
				_lateUpdateStore.Sort((x, y) => x.LateTickOrder.CompareTo(y.LateTickOrder));
		}

		/// <summary>
		/// 移除正在执行 LateTick 方法的对象
		/// </summary>
		/// <param name="lateTick">正在执行 LateTick 方法的对象</param>
		private void RemoveLateTick(ILateTick lateTick) { _wannaRemoveLateTicks.Add(lateTick); }
	}
}