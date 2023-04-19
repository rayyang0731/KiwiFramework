using System;
using System.Threading.Tasks;

using Cysharp.Threading.Tasks;

using UnityEngine;

using Object = UnityEngine.Object;

namespace KiwiFramework.Runtime
{
	/// <summary>
	/// 资源加载辅助器接口
	/// </summary>
	public interface IAssetHelper
	{
		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="userData">用户自定义数据</param>
		/// <param name="callback">初始化完成回调</param>
		void Initialize(object userData, Action callback);

		/// <summary>
		/// 同步加载泛型
		/// </summary>
		/// <param name="key">对象的key或者路径</param>
		/// <returns></returns>
		T Load<T>(string key) where T : Object;

		/// <summary>
		/// 异步加载泛型
		/// </summary>
		/// <param name="key">对象的key或者路径</param>
		/// <param name="callback">对象加载完成回调</param>
		void LoadAsync<T>(string key, Action<T> callback) where T : Object;

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
		T Instantiate<T>(string key, Transform parent = null, bool instantiateInWorldSpace = false) where T : Object;

		/// <summary>
		/// 异步加载并实例化对象
		/// </summary>
		/// <param name="key">对象的key或者路径</param>
		/// <param name="callback">对象实例化完成回调</param>
		/// <param name="parent">父物体(<typeparamref name="T"/>为GameObject类型时生效)</param>
		/// <param name="instantiateInWorldSpace">
		/// 如果为 true,当设置父对象时,直接在世界空间中定位新对象;
		/// 如果为 false ,设置对象相对于其新父对象的位置.
		/// (<typeparamref name="T"/>为GameObject类型时生效)
		/// </param>
		void InstantiateAsync<T>(string key, Action<T> callback, Transform parent = null, bool instantiateInWorldSpace = false) where T : Object;

		/// <summary>
		/// 同步加载文本数据
		/// </summary>
		/// <param name="key">对象的key或者路径</param>
		/// <returns></returns>
		string LoadRawFileToText(string key);

		/// <summary>
		/// 异步加载文本数据
		/// </summary>
		/// <param name="key">对象的key或者路径</param>
		/// <param name="callback">加载完成回调</param>
		void LoadRawFileToTextAsync(string key, Action<string> callback);

		/// <summary>
		/// 同步加载 byte[] 数据
		/// </summary>
		/// <param name="key">对象的key或者路径</param>
		/// <returns></returns>
		byte[] LoadRawFileToBytes(string key);

		/// <summary>
		/// 异步加载 byte[] 数据
		/// </summary>
		/// <param name="key">对象的key或者路径</param>
		/// <param name="callback">加载完成回调</param>
		/// <returns></returns>
		void LoadRawFileToBytesAsync(string key, Action<byte[]> callback);

		/// <summary>
		/// 卸载资源对象
		/// </summary>
		/// <param name="obj"></param>
		void Unload(Object obj);

		/// <summary>
		/// 删除实例资源对象
		/// </summary>
		/// <param name="clone"></param>
		void Destroy(Object clone);

		/// <summary>
		/// 卸载未使用的资源对象
		/// </summary>
		void UnloadUnusedAssets();
	}

	/// <summary>
	/// 资源加载辅助器基类
	/// </summary>
	public abstract class BaseAssetHelper : IAssetHelper
	{
		public virtual void Initialize(object userData, Action callback) { throw new NotImplementedException(); }

		public virtual T Load<T>(string key) where T : Object => throw new NotImplementedException();

		public virtual void LoadAsync<T>(string key, Action<T> callback) where T : Object { throw new NotImplementedException(); }

		/// <summary>
		/// 异步加载泛型
		/// </summary>
		/// <param name="key">对象的key或者路径</param>
		public virtual UniTask<T> LoadAsync<T>(string key) where T : Object => throw new NotImplementedException();

		public virtual T Instantiate<T>(string key, Transform parent = null, bool instantiateInWorldSpace = false) where T : Object => throw new NotImplementedException();

		public virtual void InstantiateAsync<T>(string key, Action<T> callback, Transform parent = null, bool instantiateInWorldSpace = false) where T : Object =>
			throw new NotImplementedException();

		/// <summary>
		/// 异步加载并实例化对象
		/// </summary>
		/// <param name="key">对象的key或者路径</param>
		/// <param name="parent">父物体(<typeparamref name="T"/>为GameObject类型时生效)</param>
		/// <param name="instantiateInWorldSpace">
		/// 如果为 true,当设置父对象时,直接在世界空间中定位新对象;
		/// 如果为 false ,设置对象相对于其新父对象的位置.
		/// (<typeparamref name="T"/>为GameObject类型时生效)
		/// </param>
		public virtual UniTask<T> InstantiateAsync<T>(string key, Transform parent = null, bool instantiateInWorldSpace = false) where T : Object
			=> throw new NotImplementedException();

		public virtual string LoadRawFileToText(string key) => throw new NotImplementedException();

		public virtual void LoadRawFileToTextAsync(string key, Action<string> callback) { throw new NotImplementedException(); }

		/// <summary>
		/// 异步加载文本数据
		/// </summary>
		/// <param name="key">对象的key或者路径</param>
		public virtual UniTask<string> LoadRawFileToTextAsync(string key) { throw new NotImplementedException(); }

		public virtual byte[] LoadRawFileToBytes(string key) => throw new NotImplementedException();

		public virtual void LoadRawFileToBytesAsync(string key, Action<byte[]> callback) { throw new NotImplementedException(); }

		/// <summary>
		/// 异步加载 byte[] 数据
		/// </summary>
		/// <param name="key">对象的key或者路径</param>
		public virtual UniTask<byte[]> LoadRawFileToBytesAsync(string key) => throw new NotImplementedException();

		public virtual void Unload(Object obj) { throw new NotImplementedException(); }
		public virtual void Destroy(Object clone) { throw new NotImplementedException(); }

		public virtual void UnloadUnusedAssets() { throw new NotImplementedException(); }
	}
}