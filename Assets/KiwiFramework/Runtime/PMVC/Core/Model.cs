using System;
using System.Collections.Generic;

using JetBrains.Annotations;

using UnityEngine;

namespace KiwiFramework.Runtime
{
	/// <summary>
	/// 数据层管理器
	/// </summary>
	public class Model : Singleton<Model>, IModel
	{
		/// <summary>
		/// 代理字典
		/// </summary>
		private readonly Dictionary<Type, IProxy> _proxyMap = new();

		/// <summary>
		/// 注册代理
		/// </summary>
		/// <param name="proxy">代理对象</param>
		public void RegisterProxy([NotNull] IProxy proxy)
		{
			if (proxy == null) throw new ArgumentNullException(nameof(proxy));

			var type = proxy.GetType();

			if (_proxyMap.ContainsKey(type)) return;
			_proxyMap[type] = proxy;

			proxy.OnRegister();

			Debug.Log($"Register proxy: {proxy}");
		}

		/// <summary>
		/// 获得代理
		/// </summary>
		/// <typeparam name="T">代理对象</typeparam>
		/// <returns></returns>
		public T GetProxy<T>() where T : class, IProxy
		{
			var type = typeof(T);

			if (_proxyMap.TryGetValue(type, out var proxy))
				return proxy as T;

			proxy = Activator.CreateInstance(type) as Proxy;
			if (proxy != null)
			{
				RegisterProxy(proxy);

				return proxy as T;
			}

			Debug.LogError($"获取代理失败 : {type}");
			return null;
		}

		/// <summary>
		/// 尝试获得代理
		/// </summary>
		/// <param name="proxy">代理对象</param>
		/// <typeparam name="T">代理类型</typeparam>
		/// <returns>是否成功获得代理</returns>
		public bool TryGetProxy<T>(out T proxy) where T : class, IProxy
		{
			if (_proxyMap.TryGetValue(typeof(T), out var temp))
			{
				proxy = temp as T;
				return proxy != null;
			}

			proxy = null;
			return false;
		}

		/// <summary>
		/// 移除代理
		/// </summary>
		/// <typeparam name="T">代理类型</typeparam>
		/// <returns>是否成功移除代理</returns>
		public bool RemoveProxy<T>() where T : class, IProxy
		{
			var type = typeof(T);

			if (!_proxyMap.ContainsKey(type))
				return false;

			_proxyMap[type].OnRemove();

			_proxyMap.Remove(type);

			Debug.Log($"Remove proxy: {type.Name}");

			return true;
		}

		/// <summary>
		/// 移除全部代理
		/// </summary>
		public void RemoveAllProxy()
		{
			foreach (var (_, item) in _proxyMap)
			{
				item.OnRemove();
			}

			_proxyMap.Clear();
		}
	}
}