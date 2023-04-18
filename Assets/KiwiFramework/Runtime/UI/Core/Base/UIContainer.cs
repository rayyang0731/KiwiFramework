﻿using System.Collections.Generic;

using KiwiFramework.Runtime.UnityExtend;

using Sirenix.OdinInspector;

using UnityEditor;

using UnityEngine;

namespace KiwiFramework.Runtime.UI
{
	/// <summary>
	/// 容器类
	/// </summary>
	public partial class UIContainer : UIElement
	{
		/// <summary>
		/// 是否强制为代码中要调用的对象
		/// </summary>
		public override bool ForceCallInCode => true;

		/// <summary>
		/// 容器所管理的UI元素
		/// </summary>
		[SerializeField, LabelText(" "), ReadOnly, PropertyOrder(int.MaxValue), BoxGroup("collector", LabelText = "收集器"),
		 ListDrawerSettings(IsReadOnly = true, NumberOfItemsPerPage = int.MaxValue, Expanded = true)]
		private List<UIElement> _uiElements = new();

		/// <summary>
		/// 添加UI元素
		/// </summary>
		/// <param name="element">要添加的UI元素</param>
		public override void AddElement(UIElement element) { _uiElements.Add(element); }

		/// <summary>
		/// 移除UI元素
		/// </summary>
		/// <param name="element">要移除的UI元素</param>
		public override void RemoveElement(UIElement element) { _uiElements.Remove(element); }

		/// <summary>
		/// 获得UI元素
		/// </summary>
		/// <param name="elementName"></param>
		/// <returns></returns>
		public override UIElement GetElement(string elementName)
		{
			foreach (var child in _uiElements)
			{
				if (child.name == elementName) return child;

				if (child is not UIContainer container) continue;

				var result = container.GetElement(elementName);
				if (result != null)
					return result;
			}

			return null;
		}
	}

#if UNITY_EDITOR
	public partial class UIContainer
	{
		[ResponsiveButtonGroup("collector/button"),
		 Button(SdfIconType.Collection, "收集UI元素", ButtonHeight = 40), GUIColor(0, 1, 1)]
		public void Collect()
		{
			if (DoCollect())
			{
				var prefabAssetType = PrefabUtility.GetPrefabAssetType(gameObject);
				if (prefabAssetType != PrefabAssetType.Regular) return;

				EditorUtility.SetDirty(gameObject);
				AssetDatabase.SaveAssets();
			}
		}

		private static bool GetElements(Transform parent, List<UIElement> elements, Dictionary<string, Transform> nameList)
		{
			foreach (Transform child in parent)
			{
				if (!child.TryGetComponent<UIElement>(out var comp))
					goto isNotUIElement;

				if (nameList.ContainsKey(comp.FieldNameInCode))
				{
					if (EditorUtility.DisplayDialog("名称重复",
					                                "名称重复,请修改 : \n" +
					                                $"{child.GetPath()}\n" +
					                                $"{nameList[comp.FieldNameInCode].GetPath()}",
					                                "确定"))
					{
						Sirenix.Utilities.Editor.GUIHelper.PingObject(child);
					}

					return false;
				}

				var error = string.Empty;
				if (!comp.CheckValidate(ref error))
				{
					if (EditorUtility.DisplayDialog("组件自检",
					                                "组件自检发现问题,请修改 : \n" +
					                                $"{child.GetPath()}\n" +
					                                error,
					                                "确定"))
					{
						Sirenix.Utilities.Editor.GUIHelper.PingObject(child);
					}

					return false;
				}

				if (comp.CallInCode)
				{
					elements.Add(comp);
					nameList.Add(comp.FieldNameInCode, child);
				}

				if (comp is UIContainer)
					continue;

				isNotUIElement :

				if (child.childCount > 0)
				{
					var success = GetElements(child, elements, nameList);
					if (!success)
						return false;
				}
			}

			return true;
		}

		public bool DoCollect()
		{
			// 名称列表,用来检测重名
			var nameList = new Dictionary<string, Transform>();

			var elements = new List<UIElement>();

			var success = GetElements(rectTransform, elements, nameList);

			if (success)
				_uiElements = elements;
			else
				_uiElements.Clear();

			return success;
		}
	}
#endif
}