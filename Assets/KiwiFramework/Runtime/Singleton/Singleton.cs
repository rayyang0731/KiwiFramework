using System;
using UnityEngine;

namespace KiwiFramework.Runtime
{
	/// <summary>
	/// 单例基类
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class Singleton<T> : IDisposable where T : Singleton<T>
	{
		private static T _instance;
		public static T Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = Activator.CreateInstance<T>();
					if (_instance == null) throw new Exception("创建单例失败");
					Debug.Log($"创建单例:{typeof(T).Name}");
				}

				return _instance;
			}
		}

		public static T GetInstance() => Instance;

		/// <summary>
		/// 释放标记
		/// </summary>
		private bool _disposed;

		~Singleton() => Dispose(false);

		/// <summary>
		/// 释放
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// 释放
		/// </summary>
		/// <param name="disposing">是否已经释放过</param>
		private void Dispose(bool disposing)
		{
			if (_disposed) return;

			//清理托管资源
			if (disposing)
			{
				_instance = null;
				ReleaseManagedResources();
			}

			//清理非托管资源
			ReleaseUnmanagedResources();

			_disposed = true;
		}

		/// <summary>
		/// 清理托管资源
		/// </summary>
		protected virtual void ReleaseManagedResources() { }

		/// <summary>
		/// 清理非托管资源
		/// </summary>
		protected virtual void ReleaseUnmanagedResources() { }
	}
}
