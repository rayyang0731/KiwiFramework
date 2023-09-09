#if UNITY_EDITOR

using System;
using System.IO;

using UnityEditor;

using UnityEngine;
using UnityEngine.Assertions;

using Object = UnityEngine.Object;

namespace KiwiFramework.Runtime.AssetSystem
{
	public class UnityEditorHelper : BaseAssetHelper
	{
		public override T Load<T>(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				Debug.LogException(new ArgumentNullException(nameof(path)));
				return null;
			}

			if (path.Contains("unity_builtin_extra"))
			{
				Debug.LogError($"不能加载 Unity 内置资源 : {path}");
				return null;
			}

			var obj = AssetDatabase.LoadAssetAtPath<T>(path);
			if (obj != null) return obj;

			Debug.LogError($"同步加载资源失败 : {path}");
			return null;
		}

		public T LoadByGUID<T>(string guid) where T : Object
		{
			if (string.IsNullOrEmpty(guid))
			{
				Debug.LogException(new ArgumentNullException(nameof(guid)));
				return null;
			}

			var path = AssetDatabase.GUIDToAssetPath(guid);
			if (string.IsNullOrEmpty(path))
			{
				Debug.LogError($"GUID [{guid}] 的资源不存在.");
				return null;
			}

			if (path.Contains("unity_builtin_extra"))
			{
				Debug.LogError($"不能加载 Unity 内置资源 : {path}");
				return null;
			}

			return AssetDatabase.LoadAssetAtPath<T>(path);
		}

		public string GetGUID(Object obj)
		{
			if (obj == null)
			{
				Debug.LogException(new ArgumentNullException(nameof(obj)));
				return string.Empty;
			}

			var path = AssetDatabase.GetAssetPath(obj);
			if (string.IsNullOrEmpty(path))
			{
				Debug.LogError($"资源对象 [{obj}] 未找到资源路径.");
				return null;
			}

			return AssetDatabase.AssetPathToGUID(path);
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

		public override string LoadRawFileToText(string key)
		{
			Assert.IsTrue(File.Exists(key), $"同步加载文本数据的文件未找到 : {key}");

			return File.ReadAllText(key);
		}

		public override async void LoadRawFileToTextAsync(string key, Action<string> callback)
		{
			Assert.IsTrue(File.Exists(key), $"异步加载文本数据的文件未找到 : {key}");

			var text = await File.ReadAllTextAsync(key);
			callback.Invoke(text);
		}

		public override byte[] LoadRawFileToBytes(string key)
		{
			Assert.IsTrue(File.Exists(key), $"同步加载 byte[] 数据的文件未找到 : {key}");

			return File.ReadAllBytes(key);
		}

		public override async void LoadRawFileToBytesAsync(string key, Action<byte[]> callback)
		{
			Assert.IsTrue(File.Exists(key), $"异步加载 byte[] 数据的文件未找到 : {key}");

			var bytes = await File.ReadAllBytesAsync(key);
			callback.Invoke(bytes);
		}

		public override void Unload(Object obj) { Resources.UnloadAsset(obj); }

		public override void UnloadUnusedAssets() { Resources.UnloadUnusedAssets(); }
	}
}

#endif