using System;
using System.Collections.Generic;

namespace KiwiFramework.Runtime
{
	/// <summary>
	/// 对象池
	/// </summary>
	/// <typeparam name="T">缓存对象类型</typeparam>
	[Serializable]
	public class ObjectPool<T> where T : class, new()
	{
		/// <summary>
		/// 缓存最大数量
		/// </summary>
		private int _capacity;

		/// <summary>
		/// 缓存队列
		/// </summary>
		private Queue<T> _cacheQueue;

		/// <summary>
		/// 创建对象委托,对象池将会使用此委托创建对象,此委托不能为Null
		/// </summary>
		private Func<T> _onCreate;

		/// <summary>
		/// 销毁对象委托,对象池将会使用此委托销毁对象,此委托不能为Null
		/// </summary>
		private Action<T> _onDestroy;

		/// <summary>
		/// 获取对象委托,对象池将会在获取对象时调用此委托
		/// </summary>
		private Action<T> _onGet;

		/// <summary>
		/// 回收对象委托,对象池将会在回收对象时调用此委托
		/// </summary>
		private Action<T> _onRecycle;

		/// <summary>
		/// 返回缓存池中剩余对象的数量
		/// </summary>
		public int Count => _cacheQueue.Count;

		/// <summary>
		/// 缓存池最大容量
		/// </summary>
		public int Capacity
		{
			get => _capacity;
			set => _capacity = value;
		}

		/// <summary>
		/// 创建一个对象池
		/// </summary>
		/// <param name="capacity">对象池容量</param>
		/// <param name="onCreate">创建对象委托方法,这个参数不能为Null</param>
		/// <param name="onDestroy">销毁对象委托方法,这个参数不能为Null</param>
		public ObjectPool(int capacity, Func<T> onCreate, Action<T> onDestroy)
		{
			if (onCreate == null || onDestroy == null)
				throw new Exception("缓存池的创建对象委托与销毁对象委托不能为null.");
			if (capacity < 1)
				throw new Exception("缓存池目标容量不能小于 1.");

			_capacity = capacity;
			_cacheQueue = new Queue<T>(_capacity);
			_onCreate = onCreate;
			_onDestroy = onDestroy;
		}

		/// <summary>
		/// 创建一个对象池
		/// </summary>
		/// <param name="capacity">对象池容量</param>
		/// <param name="onCreate">创建对象委托方法,这个参数不能为Null</param>
		/// <param name="onDestroy">销毁对象委托方法,这个参数不能为Null</param>
		/// <param name="onGet">获取对象时调用此委托</param>
		/// <param name="onRecycle">回收对象时调用此委托</param>
		public ObjectPool(int capacity, Func<T> onCreate, Action<T> onDestroy, Action<T> onGet, Action<T> onRecycle)
			: this(capacity, onCreate, onDestroy)
		{
			_onGet = onGet;
			_onRecycle = onRecycle;
		}

		/// <summary>
		/// 从对象池中获取一个缓存对象,如果池中没有对象,将使用创建方法创建一个对象
		/// </summary>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public T Get()
		{
			T obj = null;

			if (_cacheQueue.Count > 0) obj = _cacheQueue.Dequeue();

			obj ??= _onCreate.Invoke();

			if (obj == null)
				throw new Exception($"创建对象失败:{typeof(T).Name}");

			_onGet?.Invoke(obj);
			return obj;
		}

		/// <summary>
		/// 缓存对象,对象池满时将会使用销毁方法销毁对象
		/// </summary>
		/// <param name="obj"></param>
		/// <returns>回收是否成功</returns>
		public bool Recycle(T obj)
		{
			if (obj == null || _cacheQueue.Contains(obj)) return false;

			_onRecycle?.Invoke(obj);

			if (_cacheQueue.Count < _capacity)
			{
				_cacheQueue.Enqueue(obj);
				return true;
			}

			_onDestroy?.Invoke(obj);
			return true;
		}

		/// <summary>
		/// 清空对象池,清空并销毁对象池中的对象
		/// </summary>
		public void Clear()
		{
			while (_cacheQueue.Count > 0)
			{
				_onDestroy?.Invoke(_cacheQueue.Dequeue());
			}
		}

		/// <summary>
		/// 预加载对象池,预先填满对象池
		/// </summary>
		public bool Preload() => Preload(_capacity);

		/// <summary>
		/// 预加载对象池,预先填满对象池
		/// </summary>
		/// <param name="number">预加载数量</param>
		public bool Preload(int number)
		{
			if (number <= _cacheQueue.Count || number > _capacity) return false;

			var preloadNum = number - _cacheQueue.Count;

			for (var i = 0; i < preloadNum; i++)
			{
				var obj = _onCreate.Invoke();
				if (obj == null)
					throw new Exception($"创建对象失败:{typeof(T).Name}");
				_onRecycle?.Invoke(obj);
				_cacheQueue.Enqueue(obj);
			}

			return true;
		}
	}
}
