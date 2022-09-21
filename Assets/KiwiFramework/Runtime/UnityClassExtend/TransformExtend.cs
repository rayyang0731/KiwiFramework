using System.Collections.Generic;
using UnityEngine;
namespace KiwiFramework.Runtime
{
	/// <summary>
	/// Transform 类方法扩展
	/// </summary>
	public static partial class UnityClassExtend
	{
		/// <summary>
		/// 设置世界坐标
		/// </summary>
		/// <param name="transform">被扩展的对象</param>
		/// <param name="x">x 轴坐标</param>
		/// <param name="y">y 轴坐标</param>
		/// <param name="z">z 轴坐标</param>
		public static void SetPosition(this Transform transform, float x, float y, float z)
		{
			var pos = transform.position;
			pos.Set(x, y, z);
			transform.position = pos;
		}

		/// <summary>
		/// 设置本地坐标
		/// </summary>
		/// <param name="transform">被扩展的对象</param>
		/// <param name="x">x 轴相对坐标</param>
		/// <param name="y">y 轴相对坐标</param>
		/// <param name="z">z 轴相对坐标</param>
		public static void SetLocalPosition(this Transform transform, float x, float y, float z)
		{
			var localPos = transform.localPosition;
			localPos.Set(x, y, z);
			transform.localPosition = localPos;
		}

		/// <summary>
		/// 设置欧拉角
		/// </summary>
		/// <param name="transform">被扩展的对象</param>
		/// <param name="x">x 轴角度</param>
		/// <param name="y">y 轴角度</param>
		/// <param name="z">z 轴角度</param>
		public static void SetEulerAngle(this Transform transform, float x, float y, float z)
		{
			var euler = transform.eulerAngles;
			euler.Set(x, y, z);
			transform.eulerAngles = euler;
		}

		/// <summary>
		/// 设置相对欧拉角
		/// </summary>
		/// <param name="transform">被扩展的对象</param>
		/// <param name="x">x 轴相对角度</param>
		/// <param name="y">y 轴相对角度</param>
		/// <param name="z">z 轴相对角度</param>
		public static void SetLocalEulerAngle(this Transform transform, float x, float y, float z)
		{
			var localEuler = transform.localEulerAngles;
			localEuler.Set(x, y, z);
			transform.localEulerAngles = localEuler;
		}

		/// <summary>
		/// 设置 Transform 的父物体,并对相对坐标赋值
		/// </summary>
		/// <param name="transform">被扩展的对象</param>
		/// <param name="parent">父物体目标</param>
		/// <param name="localPosition">父物体下的相对坐标</param>
		public static void SetParentAndPos(this Transform transform, Transform parent, Vector3 localPosition)
		{
			transform.SetParent(parent, true);
			transform.localPosition = localPosition;
		}

		/// <summary>
		/// 设置 Transform 的父物体,并对相对坐标赋值
		/// </summary>
		/// <param name="transform">被扩展的对象</param>
		/// <param name="parent">父物体目标</param>
		/// <param name="x">父物体下 x 轴的相对坐标</param>
		/// <param name="y">父物体下 y 轴的相对坐标</param>
		/// <param name="z">父物体下 z 轴的相对坐标</param>
		public static void SetParentAndPos(this Transform transform, Transform parent, float x, float y, float z)
		{
			transform.SetParent(parent, true);
			transform.SetLocalPosition(x, y, z);
		}

		/// <summary>
		/// 获得 Transform 的子物体(仅 Transform 自己的子对象,不获取子对象的子对象)
		/// </summary>
		/// <param name="transform">被扩展的对象</param>
		/// <returns>Transform 自己的子物体对象,如果没有子物体返回 null</returns>
		public static Transform[] GetChildren(this Transform transform)
		{
			if (transform.childCount == 0) return null;

			var newChildren = new Transform[transform.childCount];
			for (var i = 0; i < newChildren.Length; i++)
				newChildren[i] = transform.GetChild(i);

			return newChildren;
		}

		/// <summary>
		/// 获得 Transform 的所有子物体(Transform 自己的子对象,以及子对象的子对象)
		/// </summary>
		/// <param name="transform">被扩展的对象</param>
		/// <returns>Transform 自己的子对象,以及子对象的子对象,如果没有子物体返回 null</returns>
		public static List<Transform> GetAllChildren(this Transform transform)
		{
			if (transform.childCount == 0) return null;

			var children = new List<Transform>();

			void Get(Transform parent)
			{
				if (parent.childCount == 0) return;

				for (var i = 0; i < parent.childCount; i++)
				{
					var child = parent.GetChild(i);
					children.Add(child);
					Get(child);
				}
			}

			Get(transform);

			return children;
		}
	}
}
