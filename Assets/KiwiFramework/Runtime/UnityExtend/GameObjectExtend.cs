using UnityEngine;

namespace KiwiFramework.Runtime.UnityExtend
{
	/// <summary>
	/// GameObject 类方法扩展
	/// </summary>
	public static partial class Extend
	{
		/// <summary>
		/// 获得 GameObject 的 RectTransform 组件
		/// </summary>
		/// <param name="go">被扩展的对象</param>
		/// <returns>获取成功返回 RectTransform 组件, 否则返回 null.</returns>
		public static RectTransform rectTransform(this GameObject go) => go.transform as RectTransform;

		/// <summary>
		/// 强制获得对象组件,如果对象未挂载目标组件,则为该对象添加组件
		/// </summary>
		/// <param name="go">被扩展的对象</param>
		/// <param name="exist">想要获取的组件是否存在</param>
		/// <typeparam name="T">目标组件必须继承自 Component</typeparam>
		/// <returns>目标组件</returns>
		public static T ForceGetComponent<T>(this GameObject go, out bool exist) where T : UnityEngine.Component
		{
			var result = go.GetComponent<T>();
			if (result != null)
			{
				exist = true;
				return result;
			}

			result = go.AddComponent<T>();
			exist  = false;
			return result;
		}

		/// <summary>
		/// 强制获得对象组件,如果对象未挂载目标组件,则为该对象添加组件
		/// </summary>
		/// <param name="go">被扩展的对象</param>
		/// <typeparam name="T">目标组件必须继承自 Component</typeparam>
		/// <returns>目标组件</returns>
		public static T ForceGetComponent<T>(this GameObject go) where T : UnityEngine.Component => go.ForceGetComponent<T>(out _);

		/// <summary>
		/// 强制获得对象组件,如果对象未挂载目标组件,则为该对象添加组件
		/// </summary>
		/// <param name="go">被扩展的对象</param>
		/// <param name="type">目标组件必须继承自Component,需要带命名空间的完整名称</param>
		/// <param name="exist">想要获取的组件是否存在</param>
		/// <returns>目标组件</returns>
		public static UnityEngine.Component ForceGetComponent(this GameObject go, string type, out bool exist)
		{
			var componentType = System.Type.GetType(type);

			var result = go.GetComponent(componentType);
			if (result != null)
			{
				exist = true;
				return result;
			}

			result = go.AddComponent(componentType);
			exist  = false;

			return result;
		}

		/// <summary>
		/// 强制获得对象组件,如果对象未挂载目标组件,则为该对象添加组件
		/// </summary>
		/// <param name="go">被扩展的对象</param>
		/// <param name="type">目标组件必须继承自Component,需要带命名空间的完整名称</param>
		/// <returns>目标组件</returns>
		public static UnityEngine.Component ForceGetComponent(this GameObject go, string type) => go.ForceGetComponent(type, out _);

		/// <summary>
		/// 强制获得对象组件,如果对象未挂载目标组件,则为该对象添加组件
		/// </summary>
		/// <param name="go">被扩展的对象</param>
		/// <param name="type">目标组件必须继承自Component</param>
		/// <param name="exist">想要获取的组件是否存在</param>
		/// <returns>目标组件</returns>
		public static UnityEngine.Component ForceGetComponent(this GameObject go, System.Type type, out bool exist)
		{
			var result = go.GetComponent(type);
			if (result != null)
			{
				exist = true;
				return result;
			}

			result = go.AddComponent(type);
			exist  = false;

			return result;
		}

		/// <summary>
		/// 强制获得对象组件,如果对象未挂载目标组件,则为该对象添加组件
		/// </summary>
		/// <param name="go">被扩展的对象</param>
		/// <param name="type">目标组件必须继承自Component</param>
		/// <returns>目标组件</returns>
		public static UnityEngine.Component ForceGetComponent(this GameObject go, System.Type type) => go.ForceGetComponent(type, out _);

		/// <summary>
		/// 设置 GameObject 的 Layer
		/// </summary>
		/// <param name="go">被扩展的对象</param>
		/// <param name="layer">要设置的 Layer</param>
		/// <param name="syncChildren">是否同时设置子对象的 Layer</param>
		/// <param name="recursion">是否递归获得所有子对象</param>
		/// <param name="includeInactive">是否获取未激活的子对象</param>
		public static void SetLayer(this GameObject go, int layer, bool syncChildren = false, bool recursion = true, bool includeInactive = true)
		{
			go.layer = layer;

			if (!syncChildren) return;

			var children = recursion ? go.transform.GetChildrenRecursion(includeInactive) : go.transform.GetChildren(includeInactive);
			foreach (var child in children)
			{
				child.gameObject.layer = layer;
			}
		}

		/// <summary>
		/// 根据指定 tag 查找子对象
		/// </summary>
		/// <param name="go">被扩展的对象</param>
		/// <param name="tag">指定的 tag</param>
		/// <param name="targetChild">返回查找到的目标子对象</param>
		/// <returns>如果查找到指定对象返回 True,否则返回 false</returns>
		public static bool FindChildWithTag(this GameObject go, string tag, out GameObject targetChild)
		{
			targetChild = null;
			if (go.CompareTag(tag))
			{
				targetChild = go;
				return true;
			}

			var children = go.transform.GetChildrenRecursion(true);
			foreach (var child in children)
			{
				if (!child.CompareTag(tag)) continue;

				targetChild = child.gameObject;
				return true;
			}

			return false;
		}

		/// <summary>
		/// 控制对象显隐
		/// </summary>
		/// <param name="go">被扩展的对象</param>
		/// <param name="val">是否要显示</param>
		public static void SetDisplay(this GameObject go, bool val)
		{
			var renderers = go.GetComponentsInChildren<Renderer>();

			foreach (var renderer in renderers)
			{
				renderer.enabled = val;
			}
		}
	}
}