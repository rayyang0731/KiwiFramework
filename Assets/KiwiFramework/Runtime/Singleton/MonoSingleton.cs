using UnityEngine;

namespace KiwiFramework.Runtime
{
	public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
	{
		private static T _instance;

		public static T Instance
		{
			get
			{
				if (_instance != null) return _instance;

				var objs = FindObjectsOfType<T>();
				if (objs.Length >= 1)
				{
					Debug.Assert(objs.Length == 1, $"{typeof(T)}实例对象大于 1 个.");
					_instance = objs[0];
					return _instance;
				}

#if UNITY_EDITOR
				var typeT = typeof(T);
				var typeName = typeT.Name;

				if (Application.isPlaying)
				{
					var instanceGO = new GameObject($"[ {typeName} ]", typeT);
					DontDestroyOnLoad(instanceGO);

					_instance = instanceGO.ForceGetComponent<T>();
					Debug.Log($"创建单例:{typeName}");
					return _instance;
				}

				Debug.LogWarning($"非运行模式下不能使用 Mono 单例. Class:{typeName}");
				return null;
#else
				var instanceGO = new GameObject();
				DontDestroyOnLoad(instanceGO);
				_instance = instanceGO.ForceGetComponent<T>();
				return _instance;
#endif
			}
		}

		public static T GetInstance() => Instance;

		/// <summary>
		/// 实例是否存在
		/// <para>
		/// 主要为了解决关闭 Unity 播放时,刚好有组件在 OnDisable 或者 OnDestroy 中调用了单例,导致引擎会报错的问题.
		/// </para>
		/// </summary>
		/// <returns></returns>
		public static bool HasInstance => _instance != null;

		private void Awake()
		{
			if (_instance != null) return;

			_instance = gameObject.ForceGetComponent<T>();
			if (_instance != null)
			{
				DontDestroyOnLoad(gameObject);
			}
			else
			{
				Debug.LogError($"创建{typeof(T).Name}失败,自动删除.顺便问一句你这是怎么办到的?");
				Destroy(gameObject);
				return;
			}

			OnAwake();
		}

		private void OnDestroy()
		{
			OnClear();
			_instance = null;
		}

		protected virtual void OnAwake() { }

		protected virtual void OnClear() { }
	}
}
