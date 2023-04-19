using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Cysharp.Threading.Tasks;

using JetBrains.Annotations;

using UnityEngine;

using YooAsset;

using Object = UnityEngine.Object;

namespace KiwiFramework.Runtime
{
	public class YooAssetHelper : BaseAssetHelper
	{
		public class Data
		{
			public readonly string PackageName;
			public readonly EPlayMode PlayMode;

			public Data(string packageName, EPlayMode playMode)
			{
				PackageName = packageName;
				PlayMode    = playMode;
			}
		}

		public string currentPackageName { get; private set; }
		public EPlayMode playMode { get; private set; }

		public override async void Initialize(object userData, Action callback)
		{
			if (userData is Data data)
			{
				currentPackageName = data.PackageName;
				playMode           = data.PlayMode;
			}
			else
			{
				throw new ArgumentException($"初始化资源加载系统失败,参数类型必须为 {typeof(Data).FullName} ", nameof(userData));
			}

			YooAssets.Initialize();
			YooAssets.SetOperationSystemMaxTimeSlice(30);

			var package = YooAssets.TryGetPackage(currentPackageName);
			if (package == null)
			{
				package = YooAssets.CreatePackage(currentPackageName);
				YooAssets.SetDefaultPackage(package);
			}

			InitializationOperation initializationOperation = null;

			switch (playMode)
			{
				// 编辑器下的模拟模式
				case EPlayMode.EditorSimulateMode:
				{
					var createParameters = new EditorSimulateModeParameters {SimulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild(currentPackageName),};
					initializationOperation = package.InitializeAsync(createParameters);
					break;
				}
				// 单机运行模式
				case EPlayMode.OfflinePlayMode:
				{
					var createParameters = new OfflinePlayModeParameters {DecryptionServices = new GameDecryptionServices(),};
					initializationOperation = package.InitializeAsync(createParameters);
					break;
				}
				// 联机运行模式
				case EPlayMode.HostPlayMode:
				{
					var createParameters = new HostPlayModeParameters
					                       {
						                       DecryptionServices = new GameDecryptionServices(),
						                       QueryServices      = new GameQueryServices(),
						                       DefaultHostServer  = GetHostServerURL(),
						                       FallbackHostServer = GetHostServerURL(),
					                       };
					initializationOperation = package.InitializeAsync(createParameters);
					break;
				}
			}

			await initializationOperation;

			callback.Invoke();
		}

		public override T Load<T>(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				Debug.LogError("要加载的资源的Key不能为 null 或 empty.");
				return null;
			}

			var handle = YooAssets.LoadAssetSync<T>(key);
			if (handle.Status == EOperationStatus.Succeed)
				return handle.GetAssetObject<T>();

			Debug.LogError($"资源同步加载失败 : {key} , error : {handle.LastError}");
			return null;
		}

		public override async void LoadAsync<T>(string key, [NotNull] Action<T> callback)
		{
			if (callback == null) throw new ArgumentNullException(nameof(callback));

			var obj = await LoadAsync<T>(key);
			if (obj == null)
				return;

			callback.Invoke(obj);
		}
		
		public override async UniTask<T> LoadAsync<T>(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				Debug.LogError("要加载的资源的Key不能为 null 或 empty.");
				return null;
			}

			var handle = YooAssets.LoadAssetAsync<T>(key);
			await handle;
			if (handle.Status == EOperationStatus.Succeed)
				return handle.GetAssetObject<T>();

			Debug.LogError($"资源异步加载失败 : {key} , error : {handle.LastError}");
			return null;
		}

		public override T Instantiate<T>(string key, Transform parent = null, bool instantiateInWorldSpace = false)
		{
			if (string.IsNullOrEmpty(key))
			{
				Debug.LogError("要实例化的资源的Key不能为 null 或 empty.");
				return null;
			}

			var handle = YooAssets.LoadAssetSync<T>(key);
			if (handle.Status == EOperationStatus.Succeed)
			{
				if (typeof(T) == typeof(GameObject))
					return handle.InstantiateSync(parent, instantiateInWorldSpace) as T;

				var obj = handle.GetAssetObject<T>();
				if (obj != null)
					return Object.Instantiate(obj);
			}

			Debug.LogError($"同步实例化资源失败 : {key} , error : {handle.LastError}");
			return null;
		}

		public override async void InstantiateAsync<T>(string key, [NotNull] Action<T> callback, Transform parent = null, bool instantiateInWorldSpace = false)
		{
			if (callback == null) throw new ArgumentNullException(nameof(callback));

			var obj = await InstantiateAsync<T>(key, parent, instantiateInWorldSpace);
			if (obj != null)
				return;

			callback.Invoke(obj);
		}

		public override async UniTask<T> InstantiateAsync<T>(string key, Transform parent = null, bool instantiateInWorldSpace = false)
		{
			if (string.IsNullOrEmpty(key))
			{
				Debug.LogError("要实例化的资源的Key不能为 null 或 empty.");
				return null;
			}

			var handle = YooAssets.LoadAssetAsync<T>(key);
			await handle;
			if (handle.Status == EOperationStatus.Succeed)
			{
				if (typeof(T) == typeof(GameObject))
				{
					var instantiateHandle = handle.InstantiateAsync(parent, instantiateInWorldSpace);
					await instantiateHandle;
					if (instantiateHandle.Status == EOperationStatus.Succeed)
						return instantiateHandle.Result as T;

					Debug.LogError($"异步实例化资源失败 : {key} , error : {instantiateHandle.Error}");
					return null;
				}

				var obj = handle.GetAssetObject<T>();
				if (obj != null)
					return Object.Instantiate(obj);
			}

			Debug.LogError($"异步实例化资源失败 : {key} , error : {handle.LastError}");
			return null;
		}

		public override string LoadRawFileToText(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				Debug.LogError("要加载的资源的Key不能为 null 或 empty.");
				return null;
			}

			var handle = YooAssets.LoadRawFileSync(key);
			if (handle.Status == EOperationStatus.Succeed)
				return handle.GetRawFileText();

			Debug.LogError($"同步加载文本数据失败 : {key} , error : {handle.LastError}");
			return null;
		}

		public override async void LoadRawFileToTextAsync(string key, [NotNull] Action<string> callback)
		{
			if (callback == null) throw new ArgumentNullException(nameof(callback));

			var text = await LoadRawFileToTextAsync(key);
			if (text == null)
				return;

			callback.Invoke(text);
		}

		public override async UniTask<string> LoadRawFileToTextAsync(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				Debug.LogError("要加载的资源的Key不能为 null 或 empty.");
				return null;
			}

			var handle = YooAssets.LoadRawFileAsync(key);
			await handle;
			if (handle.Status == EOperationStatus.Succeed)
				return handle.GetRawFileText();

			Debug.LogError($"同步加载文本数据失败 : {key} , error : {handle.LastError}");
			return null;
		}

		public override byte[] LoadRawFileToBytes(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				Debug.LogError("要加载的资源的Key不能为 null 或 empty.");
				return null;
			}

			var handle = YooAssets.LoadRawFileSync(key);
			if (handle.Status == EOperationStatus.Succeed)
				return handle.GetRawFileData();

			Debug.LogError($"同步加载byte[]数据失败 : {key} , error : {handle.LastError}");
			return null;
		}

		public override async void LoadRawFileToBytesAsync(string key, [NotNull] Action<byte[]> callback)
		{
			if (callback == null) throw new ArgumentNullException(nameof(callback));

			var bytes = await LoadRawFileToBytesAsync(key);
			if (bytes == null)
				return;

			callback.Invoke(bytes);
		}
		
		public override async UniTask<byte[]> LoadRawFileToBytesAsync(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				Debug.LogError("要加载的资源的Key不能为 null 或 empty.");
				return null;
			}

			var handle = YooAssets.LoadRawFileAsync(key);
			await handle;
			if (handle.Status == EOperationStatus.Succeed)
				return handle.GetRawFileData();

			Debug.LogError($"同步加载文本数据失败 : {key} , error : {handle.LastError}");
			return null;
		}

		public override void Unload(Object obj)
		{
			
		}

		public override void UnloadUnusedAssets()
		{
			var package = YooAssets.GetPackage(currentPackageName);
			package?.UnloadUnusedAssets();
		}

		/// <summary>
		/// 获取资源服务器地址
		/// </summary>
		private string GetHostServerURL()
		{
			//string hostServerIP = "http://10.0.2.2"; //安卓模拟器地址
			const string hostServerIP = "http://127.0.0.1";
			const string gameVersion  = "v1.0";

#if UNITY_EDITOR
			return UnityEditor.EditorUserBuildSettings.activeBuildTarget switch
			       {
				       UnityEditor.BuildTarget.Android => $"{hostServerIP}/CDN/Android/{gameVersion}",
				       UnityEditor.BuildTarget.iOS     => $"{hostServerIP}/CDN/IPhone/{gameVersion}",
				       UnityEditor.BuildTarget.WebGL   => $"{hostServerIP}/CDN/WebGL/{gameVersion}",
				       _                               => $"{hostServerIP}/CDN/PC/{gameVersion}"
			       };
#else
		return Application.platform switch
		       {
			       RuntimePlatform.Android      => $"{hostServerIP}/CDN/Android/{gameVersion}",
			       RuntimePlatform.IPhonePlayer => $"{hostServerIP}/CDN/IPhone/{gameVersion}",
			       RuntimePlatform.WebGLPlayer  => $"{hostServerIP}/CDN/WebGL/{gameVersion}",
			       _                            => $"{hostServerIP}/CDN/PC/{gameVersion}"
		       };
#endif
		}

		/// <summary>
		/// 内置文件查询服务类
		/// </summary>
		private class GameQueryServices : IQueryServices
		{
			private static readonly Dictionary<string, bool> _cacheData = new(1024);

			public bool QueryStreamingAssets(string fileName)
			{
				var buildinFolderName = YooAssets.GetStreamingAssetBuildinFolderName();

				var filePath = $"{buildinFolderName}/{fileName}";

				if (!_cacheData.TryGetValue(filePath, out var result))
				{
					result = File.Exists(Path.Combine(Application.streamingAssetsPath, filePath));
					_cacheData.Add(filePath, result);
				}

				return result;
			}
		}

		/// <summary>
		/// 资源文件解密服务类
		/// </summary>
		private class GameDecryptionServices : IDecryptionServices
		{
			public ulong LoadFromFileOffset(DecryptFileInfo fileInfo) { return 32; }

			public byte[] LoadFromMemory(DecryptFileInfo fileInfo) { throw new NotImplementedException(); }

			public Stream LoadFromStream(DecryptFileInfo fileInfo)
			{
				BundleStream bundleStream = new BundleStream(fileInfo.FilePath, FileMode.Open);
				return bundleStream;
			}

			public uint GetManagedReadBufferSize() { return 1024; }
		}

		public class BundleStream : FileStream
		{
			public const byte KEY = 64;

			public BundleStream(string path, FileMode mode) : base(path, mode) { }

			public override int Read(byte[] array, int offset, int count)
			{
				var index = base.Read(array, offset, count);
				for (var i = 0; i < array.Length; i++)
				{
					array[i] ^= KEY;
				}

				return index;
			}
		}
	}
}