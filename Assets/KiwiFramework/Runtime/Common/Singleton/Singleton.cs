using System;
using System.Collections;

using UnityEngine;

using Object = UnityEngine.Object;

namespace KiwiFramework.Runtime
{
	/// <summary>
	/// 单例基类
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class Singleton<T> : ISingleton where T : Singleton<T>, ISingleton, new()
	{
		/// <summary>
		/// 单例驱动器
		/// </summary>
		public class Driver : MonoBehaviour
		{
			private void Update() { Instance.SingletonUpdate(); }

			private void LateUpdate() { Instance.SingletonLateUpdate(); }

			private void FixedUpdate() { Instance.SingletonFixedUpdate(); }
		}

		/// <summary>
		/// 单例对象
		/// </summary>
		private static T _instance;

		/// <summary>
		/// 单例对象
		/// </summary>
		public static T Instance => _instance ??= SingletonManager.GetSingleton<T>();

		/// <summary>
		/// 获得单例对象
		/// </summary>
		public static T GetInstance() => Instance;

		/// <summary>
		/// 实例是否存在
		/// <para>照理说单例不应该有这个判断,但是主要为了解决关闭 Unity 播放时,
		/// 刚好有组件在 OnDisable 或者 OnDestroy 中调用了单例,
		/// 导致引擎会报错的问题.</para>
		/// </summary>
		public static bool Exists => _instance != null;

		/// <summary>
		/// 是否使用驱动器
		/// </summary>
		protected virtual bool useDriver { get; }

		/// <summary>
		/// 单例驱动器
		/// </summary>
		protected Driver driver;

		/// <summary>
		/// 单例是否可以被重置
		/// </summary>
		public virtual bool canResetSingleton => true;

		/// <summary>
		/// 构造方法
		/// </summary>
		protected Singleton() { }


		void ISingleton.SingletonPreInit()
		{
			if (!useDriver) return;

			var driverGO = new GameObject($"[{GetType().Name} Driver]", typeof(Driver));
			driver = driverGO.GetComponent<Driver>();
			Object.DontDestroyOnLoad(driverGO);
		}

		void ISingleton.SingletonInit() => OnSingletonInit();
		void ISingleton.SingletonReset() => OnSingletonReset();
		void ISingleton.SingletonRelease() => OnSingletonRelease();

		protected virtual void OnSingletonInit() { }
		protected virtual void OnSingletonReset() { }
		protected virtual void OnSingletonRelease() { }

		protected virtual void SingletonUpdate() { }
		protected virtual void SingletonLateUpdate() { }
		protected virtual void SingletonFixedUpdate() { }

		/// <summary>
		/// 开启一个协程
		/// </summary>
		public Coroutine StartCoroutine(IEnumerator coroutine) => driver.StartCoroutine(coroutine);

		/// <summary>
		/// 开启一个协程
		/// </summary>
		public Coroutine StartCoroutine(string methodName) => driver.StartCoroutine(methodName);

		/// <summary>
		/// 停止一个协程
		/// </summary>
		public void StopCoroutine(Coroutine coroutine) => driver.StopCoroutine(coroutine);

		/// <summary>
		/// 停止一个协程
		/// </summary>
		public void StopCoroutine(string methodName) => driver.StopCoroutine(methodName);

		/// <summary>
		/// 停止所有协程
		/// </summary>
		public void StopAllCoroutines() => driver.StopAllCoroutines();

		// public 

		void IDisposable.Dispose()
		{
			_instance = null;

			if (driver == null) return;

#if UNITY_EDITOR
			Object.DestroyImmediate(driver.gameObject);
#else
			Object.Destroy(driver.gameObject);
#endif
			driver = null;
		}
	}
}