using System;

using Cysharp.Threading.Tasks;

using UnityEngine;

using Object = UnityEngine.Object;

namespace KiwiFramework.Runtime
{
	public class UnityResourceHelper : BaseAssetHelper
	{
		public override T Load<T>(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				Debug.LogException(new ArgumentException(nameof(key)));
				return null;
			}

			var obj = Resources.Load<T>(key);
			if (obj != null) return obj;

			Debug.LogError($"资源同步加载失败 : {key}");
			return null;
		}

		public override async void LoadAsync<T>(string key, Action<T> callback)
		{
			var asset = await LoadAsync<T>(key);
			if (asset == null)
				return;

			callback.Invoke(asset);
		}

		/// <summary>
		/// 异步加载资源
		/// </summary>
		/// <param name="key">对象的key或者路径</param>
		public async UniTask<T> LoadAsync<T>(string key) where T : Object
		{
			if (string.IsNullOrEmpty(key))
			{
				Debug.LogException(new ArgumentException(nameof(key)));
				return null;
			}

			var request = Resources.LoadAsync<T>(key);
			await request;
			if (request.asset == null)
			{
				Debug.LogError($"异步加载资源失败 : {key}");
				return null;
			}

			return request.asset as T;
		}

		public override T Instantiate<T>(string key, Transform parent = null, bool instantiateInWorldSpace = false)
		{
			var obj = Load<T>(key);
			if (obj != null)
				return typeof(T) == typeof(GameObject)
					? Object.Instantiate(obj, parent, instantiateInWorldSpace)
					: Object.Instantiate(obj);

			Debug.LogError($"实例化资源失败 : {key}");
			return null;
		}

		public override async void InstantiateAsync<T>(string key, Action<T> callback, Transform parent = null, bool instantiateInWorldSpace = false)
		{
			var obj = await InstantiateAsync<T>(key, parent, instantiateInWorldSpace);
			if (obj == null)
				return;

			callback.Invoke(obj);
		}

		/// <summary>
		/// 异步加载并实例化对象
		/// </summary>
		/// <param name="key">对象的key或者路径</param>
		/// <param name="parent">父物体(T为GameObject类型时生效)</param>
		/// <param name="instantiateInWorldSpace">
		/// 如果为 true,当设置父对象时,直接在世界空间中定位新对象;
		/// 如果为 false ,设置对象相对于其新父对象的位置.
		/// (T为GameObject类型时生效)
		/// </param>
		/// <returns></returns>
		public async UniTask<T> InstantiateAsync<T>(string key, Transform parent = null, bool instantiateInWorldSpace = false) where T : Object
		{
			var obj = await LoadAsync<GameObject>(key);
			if (obj == null)
				return null;

			T clone;
			if (typeof(T) == typeof(GameObject))
				clone = Object.Instantiate(obj, parent, instantiateInWorldSpace) as T;
			else
				clone = Object.Instantiate(obj) as T;

			if (clone == null)
			{
				Debug.LogError($"异步实例化失败 : {key}");
				return null;
			}

			return clone;
		}

		public override void Unload(Object obj) { Resources.UnloadAsset(obj); }

		public override void UnloadUnusedAssets() { Resources.UnloadUnusedAssets(); }
	}
}