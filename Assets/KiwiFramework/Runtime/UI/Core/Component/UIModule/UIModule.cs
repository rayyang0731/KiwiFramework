using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace KiwiFramework.Runtime.UI
{
	/// <summary>
	/// UI 模块
	/// </summary>
	[HideMonoScript, AddComponentMenu("KiwiUI/UIModule")]
	public partial class UIModule : UIContainer
	{
		public override bool ForceCallInCode => true;
	}

#if UNITY_EDITOR

	public partial class UIModule
	{
		[ResponsiveButtonGroup("collector/button"),
		 Button(SdfIconType.TerminalFill, "导出预设和代码", ButtonHeight = 40), GUIColor(0, 1, 0)]
		public void Generate()
		{
			if (DoCollect())
			{
				var prefabAssetType = PrefabUtility.GetPrefabAssetType(gameObject);
				if (prefabAssetType != PrefabAssetType.Regular) return; // 判断是否为一个普通Prefab

				EditorUtility.SetDirty(gameObject);
				AssetDatabase.SaveAssets();
			}
		}
	}

#endif
}