using System;

using Cysharp.Threading.Tasks;

using KiwiFramework.Runtime.AssetSystem;
using KiwiFramework.Runtime.Utility;

using UnityEngine;

using Object = UnityEngine.Object;

namespace KiwiFramework.Runtime
{
	/// <summary>
	/// 资源加载器
	/// </summary>
	public class AssetLoader
	{
		/// <summary>
		/// 加载资源成功回调函数
		/// </summary>
		/// <param name="assetName">要加载的资源名称</param>
		/// <param name="asset">已加载的资源</param>
		/// <param name="duration">加载持续时间</param>
		/// <param name="userData">用户自定义数据</param>
		public delegate void LoadSuccessCallback<in T>(string assetName, T asset, float duration, object userData);

		/// <summary>
		/// 加载资源失败回调函数
		/// </summary>
		/// <param name="assetName">要加载的资源名称</param>
		/// <param name="errorMessage">错误信息</param>
		/// <param name="userData">用户自定义数据</param>
		public delegate void LoadFailureCallback(string assetName, string errorMessage, object userData);

		/// <summary>
		/// 游戏资源加载辅助器
		/// </summary>
		private static BaseAssetHelper _assetHelper;

		/// <summary>
		/// 游戏资源加载辅助器
		/// </summary>
		public static BaseAssetHelper assetHelper
		{
			get
			{
				if (_assetHelper == null)
					Debug.LogError($"游戏资源加载辅助器无效.请先调用{nameof(SetAssetHelper)}方法,进行初始化.");
				return _assetHelper;
			}
		}

		/// <summary>
		/// 是否打印加载耗时
		/// </summary>
		public static bool DebugElapsedTime = true;

		private static UnityResourceHelper _resource;

		/// <summary>
		/// Unity Resource 加载辅助器
		/// </summary>
		public static UnityResourceHelper Resource => _resource ??= new UnityResourceHelper();

#if UNITY_EDITOR
		private static UnityEditorHelper _editor;

		/// <summary>
		/// Unity Editor 方式使用的加载辅助器
		/// </summary>
		public static UnityEditorHelper Editor => _editor ??= new UnityEditorHelper();
#endif

		/// <summary>
		/// 是否已经初始化完毕
		/// </summary>
		public static bool Initialized => _assetHelper != null;

		/// <summary>
		/// 设置游戏资源加载辅助器
		/// </summary>
		/// <param name="helper">游戏资源加载辅助器</param>
		/// <param name="userData">用户自定义数据</param>
		/// <param name="callback">初始化完成回调</param>
		public static void SetAssetHelper(BaseAssetHelper helper, object userData, Action callback)
		{
			_assetHelper = helper ?? throw new Exception("游戏资源加载辅助器无效.");
			_assetHelper.Initialize(userData, callback);
		}

		/// <summary>
		/// 同步加载泛型
		/// </summary>
		/// <param name="key">对象的key或者路径</param>
		/// <returns></returns>
		public static T Load<T>(string key) where T : Object
		{
			try
			{
				var stopwatch = new Stopwatch();
				var obj       = assetHelper?.Load<T>(key);
				stopwatch.Stop(DebugElapsedTime, key);
				return obj;
			}
			catch (Exception e)
			{
				throw new Exception("同步加载资源报错 : ", e);
			}
		}

		/// <summary>
		/// 异步加载泛型
		/// </summary>
		/// <param name="key">对象的key或者路径</param>
		/// <param name="successCallback">加载资源成功回调函数</param>
		/// <param name="failureCallback">加载资源失败回调函数</param>
		/// <param name="userData">用户自定义数据</param>
		public static void LoadAsync<T>(string key, LoadSuccessCallback<T> successCallback,
		                                LoadFailureCallback failureCallback = null,
		                                object userData = null) where T : Object
		{
			try
			{
				if (assetHelper == null)
					return;

				if (successCallback == null)
				{
					Debug.LogException(new ArgumentException("加载资源回调函数无效", nameof(successCallback)));
					return;
				}

				if (string.IsNullOrEmpty(key))
				{
					failureCallback?.Invoke(
						key,
						"资源名称不能为 Null 或 string.Empty",
						userData
					);

					return;
				}

				var stopwatch = new Stopwatch();

				assetHelper.LoadAsync<T>(key, obj =>
				{
					stopwatch.Stop(DebugElapsedTime, key);

					if (obj == null)
						failureCallback?.Invoke(key, $"{key} 未成功加载到资源.", userData);
					else
					{
						successCallback.Invoke(key, obj, stopwatch.ElapseMilliseconds, userData);
					}
				});
			}
			catch (Exception e)
			{
				throw new Exception("异步加载资源报错 : ", e);
			}
		}

		/// <summary>
		/// 异步加载泛型
		/// </summary>
		/// <param name="key">对象的key或者路径</param>
		public static async UniTask<T> LoadAsync<T>(string key) where T : Object
		{
			try
			{
				if (assetHelper == null)
					return null;

				if (string.IsNullOrEmpty(key))
				{
					Debug.LogError("资源名称不能为 Null 或 string.Empty");
					return null;
				}

				var stopwatch = new Stopwatch();
				var obj       = await assetHelper.LoadAsync<T>(key);
				stopwatch.Stop(DebugElapsedTime, key);
				return obj;
			}
			catch (Exception e)
			{
				throw new Exception("异步加载泛型资源报错 : ", e);
			}
		}

		/// <summary>
		/// 同步加载并实例化对象
		/// </summary>
		/// <param name="key">对象的key或者路径</param>
		/// <param name="parent">父物体(<typeparamref name="T"/>为GameObject类型时生效)</param>
		/// <param name="instantiateInWorldSpace">
		/// 如果为 true,当设置父对象时,直接在世界空间中定位新对象;
		/// 如果为 false ,设置对象相对于其新父对象的位置.
		/// (<typeparamref name="T"/>为GameObject类型时生效)
		/// </param>
		/// <returns></returns>
		public static T Instantiate<T>(string key, Transform parent = null, bool instantiateInWorldSpace = false) where T : Object
		{
			try
			{
				if (string.IsNullOrEmpty(key))
				{
					Debug.LogError("资源名称不能为 Null 或 string.Empty");
					return null;
				}

				var stopwatch = new Stopwatch();
				var clone     = assetHelper?.Instantiate<T>(key, parent, instantiateInWorldSpace);
				stopwatch.Stop(DebugElapsedTime, key);
				return clone;
			}
			catch (Exception e)
			{
				throw new Exception("同步实例化资源报错 : ", e);
			}
		}

		/// <summary>
		/// 异步加载并实例化对象
		/// </summary>
		/// <param name="key">对象的key或者路径</param>
		/// <param name="successCallback">加载资源成功回调函数</param>
		/// <param name="failureCallback">加载资源失败回调函数</param>
		/// <param name="userData">用户自定义数据</param>
		/// <param name="parent">父物体(<typeparamref name="T"/>为GameObject类型时生效)</param>
		/// <param name="instantiateInWorldSpace">
		/// 如果为 true,当设置父对象时,直接在世界空间中定位新对象;
		/// 如果为 false ,设置对象相对于其新父对象的位置.
		/// (<typeparamref name="T"/>为GameObject类型时生效)
		/// </param>
		public static void InstantiateAsync<T>(string key, LoadSuccessCallback<T> successCallback,
		                                       LoadFailureCallback failureCallback = null,
		                                       object userData = null, Transform parent = null, bool instantiateInWorldSpace = false) where T : Object
		{
			try
			{
				if (assetHelper == null)
					return;

				if (successCallback == null)
				{
					Debug.LogException(new ArgumentException("加载资源回调函数无效", nameof(successCallback)));
					return;
				}

				if (string.IsNullOrEmpty(key))
				{
					failureCallback?.Invoke(
						key,
						"资源名称不能为 Null 或 string.Empty",
						userData
					);

					return;
				}

				var stopwatch = new Stopwatch();

				assetHelper.InstantiateAsync<T>(key, obj =>
				                                {
					                                stopwatch.Stop(DebugElapsedTime, key);

					                                if (obj == null)
						                                failureCallback?.Invoke(key, $"{key} 未成功加载到资源.", userData);
					                                else
						                                successCallback.Invoke(key, obj, stopwatch.ElapseMilliseconds, userData);
				                                },
				                                parent,
				                                instantiateInWorldSpace);
			}
			catch (Exception e)
			{
				throw new Exception("异步实例化资源报错 : ", e);
			}
		}

		/// <summary>
		/// 同步加载文本数据
		/// </summary>
		/// <param name="key">对象的key或者路径</param>
		/// <returns></returns>
		public static string LoadRawFileToText(string key)
		{
			try
			{
				if (string.IsNullOrEmpty(key))
				{
					Debug.LogError("资源名称不能为 Null 或 string.Empty");
					return null;
				}

				var stopwatch = new Stopwatch();
				var text      = assetHelper?.LoadRawFileToText(key);
				stopwatch.Stop(DebugElapsedTime, key);
				return text;
			}
			catch (Exception e)
			{
				throw new Exception("同步加载文本数据失败 : ", e);
			}
		}

		/// <summary>
		/// 异步加载文本数据
		/// </summary>
		/// <param name="key">对象的key或者路径</param>
		/// <param name="successCallback">加载资源成功回调函数</param>
		/// <param name="failureCallback">加载资源失败回调函数</param>
		/// <param name="userData">用户自定义数据</param>
		public static void LoadRawFileToTextAsync(string key, LoadSuccessCallback<string> successCallback, LoadFailureCallback failureCallback = null, object userData = null)
		{
			try
			{
				if (assetHelper == null)
					return;

				if (successCallback == null)
				{
					Debug.LogException(new ArgumentException("加载资源回调函数无效", nameof(successCallback)));
					return;
				}

				if (string.IsNullOrEmpty(key))
				{
					failureCallback?.Invoke(
						key,
						"资源名称不能为 Null 或 string.Empty",
						userData
					);

					return;
				}

				var stopwatch = new Stopwatch();

				assetHelper.LoadRawFileToTextAsync(key, text =>
					{
						stopwatch.Stop(DebugElapsedTime, key);

						if (text == null)
							failureCallback?.Invoke(key, $"{key} 未成功加载到资源.", userData);
						else
							successCallback.Invoke(key, text, stopwatch.ElapseMilliseconds, userData);
					}
				);
			}
			catch (Exception e)
			{
				throw new Exception("异步加载文本数据失败 : ", e);
			}
		}

		/// <summary>
		/// 同步加载 byte[] 数据
		/// </summary>
		/// <param name="key">对象的key或者路径</param>
		/// <returns></returns>
		public static byte[] LoadRawFileToBytes(string key)
		{
			try
			{
				if (string.IsNullOrEmpty(key))
				{
					Debug.LogError("资源名称不能为 Null 或 string.Empty");
					return null;
				}

				var stopwatch = new Stopwatch();
				var bytes     = assetHelper?.LoadRawFileToBytes(key);
				stopwatch.Stop(DebugElapsedTime, key);
				return bytes;
			}
			catch (Exception e)
			{
				throw new Exception("同步加载 byte[] 数据失败 : ", e);
			}
		}

		/// <summary>
		/// 异步加载 byte[] 数据
		/// </summary>
		/// <param name="key">对象的key或者路径</param>
		/// <param name="successCallback">加载资源成功回调函数</param>
		/// <param name="failureCallback">加载资源失败回调函数</param>
		/// <param name="userData">用户自定义数据</param>
		public static void LoadRawFileToBytesAsync(string key, LoadSuccessCallback<byte[]> successCallback,
		                                           LoadFailureCallback failureCallback = null,
		                                           object userData = null)
		{
			try
			{
				if (assetHelper == null)
					return;

				if (successCallback == null)
				{
					Debug.LogException(new ArgumentException("加载 byte[] 资源回调函数无效", nameof(successCallback)));
					return;
				}

				if (string.IsNullOrEmpty(key))
				{
					failureCallback?.Invoke(
						key,
						"资源名称不能为 Null 或 string.Empty",
						userData
					);

					return;
				}

				var stopwatch = new Stopwatch();

				assetHelper.LoadRawFileToBytesAsync(key, text =>
				{
					stopwatch.Stop(DebugElapsedTime, key);

					if (text == null)
						failureCallback?.Invoke(key, $"{key} 未成功加载到 byte[] 资源.", userData);
					else
						successCallback.Invoke(key, text, stopwatch.ElapseMilliseconds, userData);
				});
			}
			catch (Exception e)
			{
				throw new Exception("异步加载 byte[] 资源报错 : ", e);
			}
		}

		/// <summary>
		/// 卸载资源对象
		/// </summary>
		/// <param name="obj">要卸载的资源对象</param>
		public static void Unload(Object obj)
		{
			try
			{
				if (obj != null)
					assetHelper?.Unload(obj);
			}
			catch (Exception e)
			{
				throw new Exception("卸载资源失败 : ", e);
			}
		}

		/// <summary>
		/// 卸载未使用的资源对象
		/// </summary>
		public static void UnloadUnusedAssets()
		{
			try
			{
				assetHelper?.UnloadUnusedAssets();
			}
			catch (Exception e)
			{
				throw new Exception("卸载未使用的资源对象失败 : ", e);
			}
		}
	}
}