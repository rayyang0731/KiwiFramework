using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEditor;

using UnityEngine;

using Object = UnityEngine.Object;

namespace KiwiFramework.Editor.Utility
{
	public static class EditorUtility
	{
		public static IEnumerable<T> GetAssets<T>(string directory, string filter) where T : Object
		{
			if (string.IsNullOrEmpty(directory) || !directory.StartsWith("Assets"))
				throw new Exception("请使用以 Assets 为开头的路径.");

			var subFolders = Directory.GetDirectories(directory);
			var allObjs = new List<T>();

			var guids = AssetDatabase.FindAssets(filter, new[] {directory});
			allObjs.AddRange(guids.Select(guid => AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid))));

			guids = AssetDatabase.FindAssets(filter, subFolders);
			allObjs.AddRange(guids.Select(guid => AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid))));

			return allObjs;
		}

		/// <summary>
		/// 获取对象边框
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static Bounds GetBounds(GameObject obj)
		{
			var min = new Vector3(99999, 99999, 99999);
			var max = new Vector3(-99999, -99999, -99999);
			var renders = obj.GetComponentsInChildren<MeshRenderer>();
			if (renders.Length > 0)
			{
				foreach (var render in renders)
				{
					if (render.bounds.min.x < min.x)
						min.x = render.bounds.min.x;
					if (render.bounds.min.y < min.y)
						min.y = render.bounds.min.y;
					if (render.bounds.min.z < min.z)
						min.z = render.bounds.min.z;

					if (render.bounds.max.x > max.x)
						max.x = render.bounds.max.x;
					if (render.bounds.max.y > max.y)
						max.y = render.bounds.max.y;
					if (render.bounds.max.z > max.z)
						max.z = render.bounds.max.z;
				}
			}
			else
			{
				var rectTrans = obj.GetComponentsInChildren<RectTransform>();
				var corner = new Vector3[4];
				foreach (var rt in rectTrans)
				{
					//获取节点的四个角的世界坐标，分别按顺序为左下左上，右上右下
					rt.GetWorldCorners(corner);
					if (corner[0].x < min.x)
						min.x = corner[0].x;
					if (corner[0].y < min.y)
						min.y = corner[0].y;
					if (corner[0].z < min.z)
						min.z = corner[0].z;

					if (corner[2].x > max.x)
						max.x = corner[2].x;
					if (corner[2].y > max.y)
						max.y = corner[2].y;
					if (corner[2].z > max.z)
						max.z = corner[2].z;
				}
			}

			var center = (min + max) / 2;
			var size = new Vector3(max.x - min.x, max.y - min.y, max.z - min.z);
			return new Bounds(center, size);
		}
	}
}