using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace KiwiFramework.Runtime
{
	/// <summary>
	/// Update管理器
	/// </summary>
	public class UpdateManager : MonoSingleton<UpdateManager>
	{
		private readonly List<IUpdate> _updateStore = new();

		private readonly List<IFixedUpdate> _fixedUpdateStore = new();

		private readonly List<ILateUpdate> _lateUpdateStore = new();

		#region Update

		private void AddUpdate(IUpdate update)
		{
			if (update == null || _updateStore.Contains(update)) return;

			_updateStore.Add(update);

			if (update.UpdateOrder > 0)
				_updateStore.Sort((x, y) => x.UpdateOrder.CompareTo(y.UpdateOrder));
		}

		private void RemoveUpdate(IUpdate update)
		{
			if (update == null) return;

			if (_updateStore.Contains(update))
				_updateStore.Remove(update);
		}

		#endregion

		#region FixedUpdate

		private void AddFixedUpdate(IFixedUpdate fixedUpdate)
		{
			if (fixedUpdate == null || _fixedUpdateStore.Contains(fixedUpdate)) return;

			_fixedUpdateStore.Add(fixedUpdate);

			if (fixedUpdate.FixedUpdateOrder > 0)
				_fixedUpdateStore.Sort((x, y) => x.FixedUpdateOrder.CompareTo(y.FixedUpdateOrder));
		}

		private void RemoveFixedUpdate(IFixedUpdate fixedUpdate)
		{
			if (fixedUpdate == null) return;

			if (_fixedUpdateStore.Contains(fixedUpdate))
				_fixedUpdateStore.Remove(fixedUpdate);
		}

		#endregion

		#region LateUpdate

		private void AddLateUpdate(ILateUpdate lateUpdate)
		{
			if (lateUpdate == null || _lateUpdateStore.Contains(lateUpdate)) return;

			_lateUpdateStore.Add(lateUpdate);

			if (lateUpdate.LateUpdateOrder > 0)
				_lateUpdateStore.Sort((x, y) => x.LateUpdateOrder.CompareTo(y.LateUpdateOrder));
		}

		private void RemoveLateUpdate(ILateUpdate lateUpdate)
		{
			if (lateUpdate == null) return;

			if (_lateUpdateStore.Contains(lateUpdate))
				_lateUpdateStore.Remove(lateUpdate);
		}

		#endregion

		/// <summary>
		/// 向更新列表中添加对象
		/// </summary>
		/// <param name="obj">要添加的更新对象</param>
		/// <typeparam name="T">实现IUpdate,IFixedUpdate或ILateUpdate接口的对象</typeparam>
		public void Add<T>(T obj)
		{
			switch (obj)
			{
				case IUpdate update:
					AddUpdate(update);
					break;
				case IFixedUpdate fixedUpdate:
					AddFixedUpdate(fixedUpdate);
					break;
				case ILateUpdate lateUpdate:
					AddLateUpdate(lateUpdate);
					break;
			}
		}

		/// <summary>
		/// 从更新列表中移除对象
		/// </summary>
		/// <param name="obj">要移除的更新对象</param>
		/// <typeparam name="T"></typeparam>
		public void Remove<T>(T obj)
		{
			switch (obj)
			{
				case IUpdate update:
					RemoveUpdate(update);
					break;
				case IFixedUpdate fixedUpdate:
					RemoveFixedUpdate(fixedUpdate);
					break;
				case ILateUpdate lateUpdate:
					RemoveLateUpdate(lateUpdate);
					break;
			}
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
				IUpdate update => _updateStore.Contains(update),
				IFixedUpdate fixedUpdate => _fixedUpdateStore.Contains(fixedUpdate),
				ILateUpdate lateUpdate => _lateUpdateStore.Contains(lateUpdate),
				_ => false
			};
		}

		private void Update()
		{
			if (_updateStore.Count == 0) return;

			var deltaTime = Time.deltaTime;

			foreach (var obj in _updateStore.Where(obj => obj != null))
				obj.OnUpdate(deltaTime);
		}

		private void FixedUpdate()
		{
			if (_fixedUpdateStore.Count == 0) return;

			var fixedDeltaTime = Time.fixedDeltaTime;
			foreach (var obj in _fixedUpdateStore.Where(obj => obj != null))
				obj.OnFixedUpdate(fixedDeltaTime);
		}

		private void LateUpdate()
		{
			if (_lateUpdateStore.Count == 0) return;

			var deltaTime = Time.deltaTime;

			foreach (var obj in _lateUpdateStore.Where(obj => obj != null))
				obj.OnLateUpdate(deltaTime);
		}

		protected override void OnClear()
		{
			base.OnClear();

			_updateStore.Clear();
			_fixedUpdateStore.Clear();
			_lateUpdateStore.Clear();
		}
	}
}
