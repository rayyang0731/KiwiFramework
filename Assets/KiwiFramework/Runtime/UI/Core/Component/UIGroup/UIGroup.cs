using System.Collections.Generic;

using Sirenix.OdinInspector;

using UnityEngine;

namespace KiwiFramework.Runtime.UI
{
	/// <summary>
	/// UI元素组
	/// </summary>
	[HideMonoScript, AddComponentMenu("KiwiUI/UIGroup")]
	public partial class UIGroup : UIElement
	{
		[SerializeField]
		[LabelText("组对象"), ListDrawerSettings, PropertyOrder(10)]
		public UIElement[] elements;
	}

#if UNITY_EDITOR
	public partial class UIGroup
	{
		[Button("收集对象", ButtonSizes.Large), GUIColor(0, 1, 0), PropertyOrder(10)]
		private void CollectChild()
		{
			List<UIElement> list = new();

			foreach (Transform child in transform)
			{
				if (child.TryGetComponent<UIElement>(out var element))
				{
					list.Add(element);
				}
			}

			elements = list.ToArray();
		}
	}
#endif
}