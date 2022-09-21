using UnityEngine;
namespace KiwiFramework.Runtime
{
	/// <summary>
	/// Component 类方法扩展
	/// </summary>
	public static partial class UnityClassExtend
	{
		/// <summary>
		/// 获得 GameObject 的 RectTransform 组件
		/// </summary>
		/// <param name="cp">被扩展的对象</param>
		/// <returns>获取成功返回 RectTransform 组件, 否则返回 null.</returns>
		public static RectTransform rectTransform(this Component cp)
		{
			return cp.transform as RectTransform;
		}

		/// <summary>
		/// 强制获得对象组件,如果对象未挂载目标组件,则为该对象添加组件
		/// </summary>
		/// <param name="cp">被扩展的对象</param>
		/// <param name="exist">想要获取的组件是否存在</param>
		/// <typeparam name="T">目标组件必须继承自 Component</typeparam>
		/// <returns>目标组件</returns>
		public static T ForceGetComponent<T>(this Component cp, out bool exist) where T : Component
		{
			var result = cp.GetComponent<T>();
			if (result != null)
			{
				exist = true;
				return result;
			}

			result = cp.gameObject.AddComponent<T>();
			exist = false;
			return result;
		}

		/// <summary>
		/// 强制获得对象组件,如果对象未挂载目标组件,则为该对象添加组件
		/// </summary>
		/// <param name="cp">被扩展的对象</param>
		/// <typeparam name="T">目标组件必须继承自 Component</typeparam>
		/// <returns>目标组件</returns>
		public static T ForceGetComponent<T>(this Component cp) where T : Component
		{
			return cp.ForceGetComponent<T>(out _);
		}

		/// <summary>
		/// 强制获得对象组件,如果对象未挂载目标组件,则为该对象添加组件
		/// </summary>
		/// <param name="cp">被扩展的对象</param>
		/// <param name="type">目标组件必须继承自Component,需要带命名空间的完整名称</param>
		/// <param name="exist">想要获取的组件是否存在</param>
		/// <returns>目标组件</returns>
		public static Component ForceGetComponent(this Component cp, string type, out bool exist)
		{

			var componentType = System.Type.GetType(type);

			var result = cp.GetComponent(componentType);
			if (result != null)
			{
				exist = true;
				return result;
			}

			result = cp.gameObject.AddComponent(componentType);
			exist = false;

			return result;
		}

		/// <summary>
		/// 强制获得对象组件,如果对象未挂载目标组件,则为该对象添加组件
		/// </summary>
		/// <param name="cp">被扩展的对象</param>
		/// <param name="type">目标组件必须继承自Component,需要带命名空间的完整名称</param>
		/// <returns>目标组件</returns>
		public static Component ForceGetComponent(this Component cp, string type)
		{
			return cp.ForceGetComponent(type, out _);
		}

		/// <summary>
		/// 强制获得对象组件,如果对象未挂载目标组件,则为该对象添加组件
		/// </summary>
		/// <param name="cp">被扩展的对象</param>
		/// <param name="type">目标组件必须继承自Component</param>
		/// <param name="exist">想要获取的组件是否存在</param>
		/// <returns>目标组件</returns>
		public static Component ForceGetComponent(this Component cp, System.Type type, out bool exist)
		{
			var result = cp.GetComponent(type);
			if (result != null)
			{
				exist = true;
				return result;
			}

			result = cp.gameObject.AddComponent(type);
			exist = false;

			return result;
		}

		/// <summary>
		/// 强制获得对象组件,如果对象未挂载目标组件,则为该对象添加组件
		/// </summary>
		/// <param name="cp">被扩展的对象</param>
		/// <param name="type">目标组件必须继承自Component</param>
		/// <returns>目标组件</returns>
		public static Component ForceGetComponent(this Component cp, System.Type type)
		{
			return cp.ForceGetComponent(type, out _);
		}

	}
}