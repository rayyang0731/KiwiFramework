using KiwiFramework.Editor;

using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;

using UnityEngine;

using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.Toolbars;

using UnityEngine.UIElements;

namespace KiwiFramework.Editor
{
	[Overlay(typeof(SceneView), "Kiwi SceneView Tools")]
	public class KiwiSceneViewToolbar : ToolbarOverlay
	{
		private KiwiSceneViewToolbar() : base(AlignStartButton.id, AlignEndButton.id, AlignTopButton.id, AlignBottomButton.id,
		                                      AlignCenterButton.id, AlignMiddleButton.id, DistributeHorizontalButton.id, DistributeVerticalButton.id,
		                                      SizeMaxButton.id, SizeMinButton.id, MakeGroupButton.id, UnGroupButton.id, GraphicRaycastOutlineToggle.id)
		{
		}

		[EditorToolbarElement(id, typeof(SceneView))]
		internal class AlignStartButton : EditorToolbarButton
		{
			public const string id = "KiwiSceneViewToolbar/AlignStart";

			public AlignStartButton()
			{
				text = "左对齐";
				icon = SdfIcons.CreateTransparentIconTexture(SdfIconType.AlignStart, Color.white, 30, 30, 2);

				clicked += OnClick;
			}

			void OnClick() { RectTransformTools.AlignTool.Align(RectTransformTools.AlignType.Left); }
		}

		[EditorToolbarElement(id, typeof(SceneView))]
		internal class AlignEndButton : EditorToolbarButton
		{
			public const string id = "KiwiSceneViewToolbar/AlignEnd";

			public AlignEndButton()
			{
				text = "右对齐";
				icon = SdfIcons.CreateTransparentIconTexture(SdfIconType.AlignEnd, Color.white, 30, 30, 2);

				clicked += OnClick;
			}

			void OnClick() { RectTransformTools.AlignTool.Align(RectTransformTools.AlignType.Right); }
		}

		[EditorToolbarElement(id, typeof(SceneView))]
		internal class AlignTopButton : EditorToolbarButton
		{
			public const string id = "KiwiSceneViewToolbar/AlignTop";

			public AlignTopButton()
			{
				text = "上对齐";
				icon = SdfIcons.CreateTransparentIconTexture(SdfIconType.AlignTop, Color.white, 30, 30, 2);

				clicked += OnClick;
			}

			void OnClick() { RectTransformTools.AlignTool.Align(RectTransformTools.AlignType.Top); }
		}

		[EditorToolbarElement(id, typeof(SceneView))]
		internal class AlignBottomButton : EditorToolbarButton
		{
			public const string id = "KiwiSceneViewToolbar/AlignBottom";

			public AlignBottomButton()
			{
				text = "下对齐";
				icon = SdfIcons.CreateTransparentIconTexture(SdfIconType.AlignBottom, Color.white, 30, 30, 2);

				clicked += OnClick;
			}

			void OnClick() { RectTransformTools.AlignTool.Align(RectTransformTools.AlignType.Bottom); }
		}

		[EditorToolbarElement(id, typeof(SceneView))]
		internal class AlignCenterButton : EditorToolbarButton
		{
			public const string id = "KiwiSceneViewToolbar/AlignCenter";

			public AlignCenterButton()
			{
				text = "水平居中";
				icon = SdfIcons.CreateTransparentIconTexture(SdfIconType.AlignCenter, Color.white, 30, 30, 2);

				clicked += OnClick;
			}

			void OnClick() { RectTransformTools.AlignTool.Align(RectTransformTools.AlignType.HorizontalCenter); }
		}

		[EditorToolbarElement(id, typeof(SceneView))]
		internal class AlignMiddleButton : EditorToolbarButton
		{
			public const string id = "KiwiSceneViewToolbar/AlignMiddle";

			public AlignMiddleButton()
			{
				text = "垂直居中";
				icon = SdfIcons.CreateTransparentIconTexture(SdfIconType.AlignMiddle, Color.white, 30, 30, 2);

				clicked += OnClick;
			}

			void OnClick() { RectTransformTools.AlignTool.Align(RectTransformTools.AlignType.VerticalCenter); }
		}

		[EditorToolbarElement(id, typeof(SceneView))]
		internal class DistributeHorizontalButton : EditorToolbarButton
		{
			public const string id = "KiwiSceneViewToolbar/DistributeHorizontal";

			public DistributeHorizontalButton()
			{
				text = "水平平均";
				icon = SdfIcons.CreateTransparentIconTexture(SdfIconType.DistributeHorizontal, Color.white, 30, 30, 2);

				clicked += OnClick;
			}

			void OnClick() { RectTransformTools.AlignTool.Align(RectTransformTools.AlignType.Horizontal); }
		}

		[EditorToolbarElement(id, typeof(SceneView))]
		internal class DistributeVerticalButton : EditorToolbarButton
		{
			public const string id = "KiwiSceneViewToolbar/DistributeVertical";

			public DistributeVerticalButton()
			{
				text = "垂直平均";
				icon = SdfIcons.CreateTransparentIconTexture(SdfIconType.DistributeVertical, Color.white, 30, 30, 2);

				clicked += OnClick;
			}

			void OnClick() { RectTransformTools.AlignTool.Align(RectTransformTools.AlignType.Vertical); }
		}

		[EditorToolbarElement(id, typeof(SceneView))]
		internal class SizeMaxButton : EditorToolbarButton
		{
			public const string id = "KiwiSceneViewToolbar/SizeMax";

			public SizeMaxButton()
			{
				text = "等大";
				icon = SdfIcons.CreateTransparentIconTexture(SdfIconType.ArrowsAngleExpand, Color.white, 30, 30, 2);

				clicked += OnClick;
			}

			void OnClick() { RectTransformTools.AlignTool.Align(RectTransformTools.AlignType.SizeMax); }
		}

		[EditorToolbarElement(id, typeof(SceneView))]
		internal class SizeMinButton : EditorToolbarButton
		{
			public const string id = "KiwiSceneViewToolbar/SizeMin";

			public SizeMinButton()
			{
				text = "等小";
				icon = SdfIcons.CreateTransparentIconTexture(SdfIconType.ArrowsAngleContract, Color.white, 30, 30, 2);

				clicked += OnClick;
			}

			void OnClick() { RectTransformTools.AlignTool.Align(RectTransformTools.AlignType.SizeMin); }
		}

		[EditorToolbarElement(id, typeof(SceneView))]
		internal class MakeGroupButton : EditorToolbarButton
		{
			public const string id = "KiwiSceneViewToolbar/MakeGroup";

			public MakeGroupButton()
			{
				text = "组合";
				icon = SdfIcons.CreateTransparentIconTexture(SdfIconType.Box, Color.white, 30, 30, 2);

				clicked += OnClick;
			}

			void OnClick() { RectTransformTools.GroupTool.MakeGroup(); }
		}

		[EditorToolbarElement(id, typeof(SceneView))]
		internal class UnGroupButton : EditorToolbarButton
		{
			public const string id = "KiwiSceneViewToolbar/UnGroupButton";

			public UnGroupButton()
			{
				text = "组合";
				icon = SdfIcons.CreateTransparentIconTexture(SdfIconType.Boxes, Color.white, 30, 30, 2);

				clicked += OnClick;
			}

			void OnClick() { RectTransformTools.GroupTool.UnGroup(); }
		}

		[EditorToolbarElement(id, typeof(SceneView))]
		internal class GraphicRaycastOutlineToggle : EditorToolbarToggle
		{
			public const string id = "KiwiSceneViewToolbar/GraphicRaycastOutlineToggle";

			private GraphicRaycastOutlineToggle()
			{
				text = "Raycast 检测器";
				icon = SdfIcons.CreateTransparentIconTexture(SdfIconType.Image, Color.white, 30, 30, 2);

				this.RegisterValueChangedCallback(ValueChanged);
			}

			private void ValueChanged(ChangeEvent<bool> evt) { GraphicRaycastOutline.Display = evt.newValue; }
		}
	}
}