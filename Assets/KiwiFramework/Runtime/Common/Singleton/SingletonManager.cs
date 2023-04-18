using System;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;

namespace KiwiFramework.Runtime
{
	/// <summary>
	/// 单例管理器
	/// </summary>
	public static class SingletonManager
	{
		static SingletonManager()
		{
#if UNITY_EDITOR
			EditorApplication.playModeStateChanged += state =>
			{
				if (state is PlayModeStateChange.ExitingPlayMode or PlayModeStateChange.ExitingEditMode)
					ReleaseAllSingletons();
			};
#endif
		}

		/// <summary>
		/// 全部单例
		/// </summary>
		private static readonly Dictionary<Type, ISingleton> Singletons = new();

		/// <summary>
		/// 创建单例
		/// </summary>
		private static T CreateSingleton<T>() where T : class, ISingleton, new()
		{
			var singleton = new T();

			singleton.SingletonPreInit();
			Singletons[typeof(T)] = singleton;
			singleton.SingletonInit();

			return singleton;
		}

		/// <summary>
		/// 获取单例
		/// </summary>
		public static T GetSingleton<T>() where T : class, ISingleton, new()
		{
			if (Singletons.TryGetValue(typeof(T), out var singleton))
				return singleton as T;

			return CreateSingleton<T>();
		}

		/// <summary>
		/// 重置所有单例
		/// </summary>
		public static void ResetAllSingletons()
		{
			foreach (var singleton in Singletons.Values.Where(singleton => singleton.canResetSingleton))
				singleton.SingletonReset();
		}

		/// <summary>
		/// 释放所有单例
		/// </summary>
		public static void ReleaseAllSingletons()
		{
			foreach (var singleton in Singletons.Values)
			{
				singleton.SingletonRelease();
				singleton.Dispose();
			}

			Singletons.Clear();
		}
	}
}