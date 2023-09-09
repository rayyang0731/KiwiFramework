using UnityEngine;

namespace KiwiFramework.Runtime
{
	/// <summary>
	/// 托管 TickManager 执行 FixedUpdate/Update/LateUpdate 的 Mono 基类
	/// <para>需要哪个 Update 方法,请自行继承对应接口</para>>
	/// </summary>
	public abstract class BaseMonoBehaviour : MonoBehaviour
	{
		private void Awake()
		{
			TickManager.Instance.Add(this);
			OnAwake();
		}

		private void OnDestroy()
		{
			TickManager.Instance.Remove(this);
			OnDestroyed();
		}

		/// <summary>
		/// 取代 Mono 的 Awake 方法
		/// </summary>
		protected virtual void OnAwake() { }

		/// <summary>
		/// 取代 Mono 的 OnDestroy 方法
		/// </summary>
		protected virtual void OnDestroyed() { }
	}
}