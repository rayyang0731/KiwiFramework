using UnityEngine;
namespace KiwiFramework.Runtime
{
	/// <summary>
	/// GameObject 类方法扩展
	/// </summary>
	public static partial class UnityClassExtend
	{
		/// <summary>
		/// 获得 GameObject 的 RectTransform 组件
		/// </summary>
		/// <param name="go">被扩展的对象</param>
		/// <returns>获取成功返回 RectTransform 组件, 否则返回 null.</returns>
		public static RectTransform rectTransform(this GameObject go)
		{
			return go.transform as RectTransform;
		}

		/// <summary>
		/// 强制获得对象组件,如果对象未挂载目标组件,则为该对象添加组件
		/// </summary>
		/// <param name="go">被扩展的对象</param>
		/// <param name="exist">想要获取的组件是否存在</param>
		/// <typeparam name="T">目标组件必须继承自 Component</typeparam>
		/// <returns>目标组件</returns>
		public static T ForceGetComponent<T>(this GameObject go, out bool exist) where T : Component
		{
			var result = go.GetComponent<T>();
			if (result != null)
			{
				exist = true;
				return result;
			}

			result = go.AddComponent<T>();
			exist = false;
			return result;
		}

		/// <summary>
		/// 强制获得对象组件,如果对象未挂载目标组件,则为该对象添加组件
		/// </summary>
		/// <param name="go">被扩展的对象</param>
		/// <typeparam name="T">目标组件必须继承自 Component</typeparam>
		/// <returns>目标组件</returns>
		public static T ForceGetComponent<T>(this GameObject go) where T : Component
		{
			return go.ForceGetComponent<T>(out _);
		}

		/// <summary>
		/// 强制获得对象组件,如果对象未挂载目标组件,则为该对象添加组件
		/// </summary>
		/// <param name="go">被扩展的对象</param>
		/// <param name="type">目标组件必须继承自Component,需要带命名空间的完整名称</param>
		/// <param name="exist">想要获取的组件是否存在</param>
		/// <returns>目标组件</returns>
		public static Component ForceGetComponent(this GameObject go, string type, out bool exist)
		{

			var componentType = System.Type.GetType(type);

			var result = go.GetComponent(componentType);
			if (result != null)
			{
				exist = true;
				return result;
			}

			result = go.AddComponent(componentType);
			exist = false;

			return result;
		}

		/// <summary>
		/// 强制获得对象组件,如果对象未挂载目标组件,则为该对象添加组件
		/// </summary>
		/// <param name="go">被扩展的对象</param>
		/// <param name="type">目标组件必须继承自Component,需要带命名空间的完整名称</param>
		/// <returns>目标组件</returns>
		public static Component ForceGetComponent(this GameObject go, string type)
		{
			return go.ForceGetComponent(type, out _);
		}

		/// <summary>
		/// 强制获得对象组件,如果对象未挂载目标组件,则为该对象添加组件
		/// </summary>
		/// <param name="go">被扩展的对象</param>
		/// <param name="type">目标组件必须继承自Component</param>
		/// <param name="exist">想要获取的组件是否存在</param>
		/// <returns>目标组件</returns>
		public static Component ForceGetComponent(this GameObject go, System.Type type, out bool exist)
		{
			var result = go.GetComponent(type);
			if (result != null)
			{
				exist = true;
				return result;
			}

			result = go.AddComponent(type);
			exist = false;

			return result;
		}

		/// <summary>
		/// 强制获得对象组件,如果对象未挂载目标组件,则为该对象添加组件
		/// </summary>
		/// <param name="go">被扩展的对象</param>
		/// <param name="type">目标组件必须继承自Component</param>
		/// <returns>目标组件</returns>
		public static Component ForceGetComponent(this GameObject go, System.Type type)
		{
			return go.ForceGetComponent(type, out _);
		}
	}
}
