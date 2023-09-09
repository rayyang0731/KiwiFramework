using System;
using System.Collections.Generic;

using UnityEngine.Assertions;

namespace KiwiFramework.Runtime
{
	/// <summary>
	/// 对象池
	/// </summary>
	/// <typeparam name="T">缓存对象类型</typeparam>
	public class ObjectPool<T> where T : class, new()
	{
		/// <summary>
		/// 缓存队列
		/// </summary>
		private readonly Queue<T> _cacheQueue;

		/// <summary>
		/// 创建对象委托,对象池将会使用此委托创建对象,此委托不能为Null
		/// </summary>
		private readonly Func<T> _onCreate;

		/// <summary>
		/// 销毁对象委托,对象池将会使用此委托销毁对象,此委托不能为Null
		/// </summary>
		private readonly Action<T> _onDestroy;

		/// <summary>
		/// 激活对象委托,对象池将会使用此委托激活对象
		/// </summary>
		private readonly Action<T> _onActive;

		/// <summary>
		/// 取消激活对象委托,对象池将会使用此委托取消激活对象
		/// </summary>
		private readonly Action<T> _onInactive;

		/// <summary>
		/// 返回缓存池中剩余对象的数量
		/// </summary>
		public int Count => _cacheQueue.Count;

		/// <summary>
		/// 缓存池最大容量
		/// </summary>
		public int MaxSize { get; set; }

		/// <summary>
		/// 由池衍生出来的全部对象
		/// </summary>
		private List<T> allObjs { get; }

		/// <summary>
		/// 创建一个对象池
		/// </summary>
		/// <param name="capacity">对象池容量</param>
		/// <param name="onCreate">创建对象委托方法,这个参数不能为Null</param>
		/// <param name="onDestroy">销毁对象委托方法,这个参数不能为Null</param>
		public ObjectPool(int capacity, Func<T> onCreate, Action<T> onDestroy)
		{
			Assert.IsNotNull(onCreate, "缓存池的创建对象委托为 null.");
			Assert.IsNotNull(onDestroy, "缓存池的销毁对象委托为 null.");

			if (capacity < 1)
				throw new Exception("缓存池目标容量小于1.");

			allObjs     = new List<T>();
			MaxSize     = capacity;
			_cacheQueue = new Queue<T>(MaxSize);
			_onCreate   = onCreate;
			_onDestroy  = onDestroy;
		}

		/// <summary>
		/// 创建一个对象池
		/// </summary>
		/// <param name="capacity">对象池容量</param>
		/// <param name="onCreate">创建对象委托方法,这个参数不能为Null</param>
		/// <param name="onDestroy">销毁对象委托方法,这个参数不能为Null</param>
		/// <param name="onActive">激活对象委托方法</param>
		/// <param name="onInactive">取消激活对象委托方法</param>
		public ObjectPool(int capacity, Func<T> onCreate, Action<T> onDestroy,
		                  Action<T> onActive, Action<T> onInactive) :
			this(capacity, onCreate, onDestroy)
		{
			_onActive   = onActive;
			_onInactive = onInactive;
		}

		/// <summary>
		/// 从对象池中获取一个缓存对象,如果池中没有对象,将使用创建方法创建一个对象
		/// </summary>
		public T Get()
		{
			T obj = null;

			if (_cacheQueue.Count > 0)
				obj = _cacheQueue.Dequeue();

			obj ??= _onCreate.Invoke();

			if (obj == null)
				throw new Exception("创建对象失败");

			allObjs.Add(obj);

			_onActive?.Invoke(obj);

			return obj;
		}

		/// <summary>
		/// 缓存对象,对象池满时将会使用销毁方法销毁对象
		/// </summary>
		public bool Recycle(T obj)
		{
			if (obj == null || _cacheQueue.Contains(obj))
				return false;

			_onInactive?.Invoke(obj);

			if (_cacheQueue.Count < MaxSize)
			{
				_cacheQueue.Enqueue(obj);
				return true;
			}

			allObjs.Remove(obj);

			_onDestroy?.Invoke(obj);

			return true;
		}

		/// <summary>
		/// 清空对象池,清空并销毁对象池中的对象
		/// </summary>
		/// <param name="destroyAll">是否销毁从池中衍生出来的所有对象</param>
		public void Clear(bool destroyAll = true)
		{
			while (_cacheQueue.Count > 0)
			{
				_onDestroy.Invoke(_cacheQueue.Dequeue());
			}

			if (destroyAll)
			{
				allObjs.ForEach(obj =>
				{
					if (obj != null && !obj.Equals(null))
					{
						_onDestroy.Invoke(obj);
					}
				});
			}
		}

		/// <summary>
		/// 预加载对象池,预先填满对象池
		/// </summary>
		public bool Preload() { return Preload(MaxSize); }

		/// <summary>
		/// 预加载对象池,预先填满对象池
		/// </summary>
		/// <param name="number">预加载数量</param>
		public bool Preload(int number)
		{
			if (number <= _cacheQueue.Count || number > MaxSize) return false;

			var preloadNum = number - _cacheQueue.Count;

			for (var i = 0; i < preloadNum; i++)
			{
				var obj = _onCreate.Invoke();
				if (obj == null)
					throw new Exception("创建对象失败");

				_onInactive?.Invoke(obj);

				_cacheQueue.Enqueue(obj);

				allObjs.Add(obj);
			}

			return true;
		}

		/// <summary>
		/// 返回缓存池中是否存在这个对象
		/// </summary>
		public bool Contains(T obj) => _cacheQueue.Contains(obj);
	}
}