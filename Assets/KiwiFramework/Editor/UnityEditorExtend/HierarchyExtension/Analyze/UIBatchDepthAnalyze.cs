using System.Collections.Generic;
using System.Linq;

using KiwiFramework.Runtime.UnityExtend;

using Sirenix.Utilities.Editor;

using TMPro;

using UnityEditor;

using UnityEngine;
using UnityEngine.UI;

namespace KiwiFramework.Editor
{
	internal partial class HierarchyExtension
	{
		/// <summary>
		/// 选中要分析的 Canvas,点击按钮,子对象会显示分析结果,B(Batch 合批ID)/D(Depth 深度)/M(Material 材质ID)/T(Texture 纹理ID)
		/// </summary>
		internal static class UIBatchDepthAnalyze
		{
			/// <summary>
			/// UI物体数据
			/// </summary>
			internal class Data
			{
				/// <summary>
				/// 物体
				/// </summary>
				public GameObject GO;

				/// <summary>
				/// 合批 ID
				/// </summary>
				public int BatchID;

				/// <summary>
				/// 深度
				/// </summary>
				public int Depth;

				/// <summary>
				/// 材质ID
				/// </summary>
				public int MaterialID;

				/// <summary>
				/// 贴图ID
				/// </summary>
				public int TextureID;

				/// <summary>
				/// Mask 次数
				/// </summary>
				public int MaskCount;
			}

			internal enum NoAnalyzeReason
			{
				/// <summary>
				/// 需要分析
				/// </summary>
				Null,

				/// <summary>
				/// 不是 Graphic 对象
				/// </summary>
				NotGraphic,

				/// <summary>
				/// CanvasGroup 的 Alpha 为 0
				/// </summary>
				CanvasGroupAlphaZero,

				/// <summary>
				/// 非激活状态
				/// </summary>
				Inactive,

				/// <summary>
				/// Graphic 的 alpha 为 0
				/// </summary>
				GraphicAlphaZero,

				/// <summary>
				/// 子 Canvas
				/// </summary>
				SubCanvas,

				/// <summary>
				/// TextMeshProUGUI 不用计算
				/// </summary>
				TextMeshProUGUI,

				/// <summary>
				/// TMP_SelectionCaret 不用计算(只是因为出现概率极低,游戏运行时就不考虑这个因素了,并不是不会引起破坏合批的操作)
				/// </summary>
				TMP_SelectionCaret,
			}

			///记录 UI 物体数据
			private static readonly Dictionary<GameObject, Data> GameObjectData = new Dictionary<GameObject, Data>();

			/// <summary>
			/// 记录的指定 Canvas 下全部 UI 对象
			/// </summary>
			private static readonly Dictionary<GameObject, int> AllUIElements = new Dictionary<GameObject, int>();

			/// <summary>
			/// 材质 ID 数据
			/// </summary>
			private static readonly Dictionary<int, int> MaterialMapDict = new Dictionary<int, int>();

			/// <summary>
			/// 贴图 ID 数据
			/// </summary>
			private static readonly Dictionary<int, int> TextureMapDict = new Dictionary<int, int>();

			//允许第一个Canvas开头深入, 但第二次遇到Canvas时，由于isStart为true, 因此不可继续深入第二个Canvas!
			private static bool _isAnalyzing = false;

			/// <summary>
			/// 绘制 Hierarchy 面板
			/// </summary>
			public static void Draw(Rect rect, GameObject go, ref int lastWidth)
			{
				if (!EditorApplication.isPlaying)
				{
					if (GameObjectData.Count > 0) GameObjectData.Clear();
					if (AllUIElements.Count > 0) AllUIElements.Clear();
					if (MaterialMapDict.Count > 0) MaterialMapDict.Clear();
					if (TextureMapDict.Count > 0) TextureMapDict.Clear();
					return;
				}

				rect      =  HierarchyExtension.GetRect(rect, 100, lastWidth);
				lastWidth += 100;

				if (go.TryGetComponent<Canvas>(out _) && Selection.activeGameObject == go)
				{
					if (GUI.Button(rect, "分析Canvas", SirenixGUIStyles.ButtonMid))
					{
						AnalyzeCanvas(go);
					}

					return;
				}

				// 绘制显示信息
				if (GameObjectData == null || GameObjectData.Count == 0) return;

				if (GameObjectData.TryGetValue(go, out var data))
				{
					GUI.Label(
						rect,
						$"B{data.BatchID:D2}/" +
						$"D{data.Depth:D2}/" +
						$"M{MaterialMapDict[data.MaterialID]:D2}/" +
						$"T{TextureMapDict[data.TextureID]:D2}",
						SirenixGUIStyles.RightAlignedGreyMiniLabel
					);
				}
			}

			/// <summary>
			/// 分析 Canvas
			/// </summary>
			private static void AnalyzeCanvas(GameObject canvasGO)
			{
				//获得选中 Canvas 下的全部 UI 对象
				var order = 0;
				AllUIElements.Clear();
				GameObjectData.Clear();

				_isAnalyzing = false;

				GetAllUIElements(canvasGO.transform, AllUIElements, ref order);

				//开始分析 计算 UI 深度、材质ID、贴图ID
				foreach (var uiElement in AllUIElements)
				{
					AnalyzeUIElement(uiElement.Key);
				}

				//排序UI物体数据
				var list = GameObjectData.Values.ToList();

				list.Sort((a, b) =>
				{
					//1.根据深度
					if (a.Depth != b.Depth)
						return a.Depth < b.Depth ? -1 : 1;

					//2.根据材质
					if (a.MaterialID != b.MaterialID)
						return a.MaterialID < b.MaterialID ? -1 : 1;

					//3.根据贴图
					if (a.TextureID != b.TextureID)
						return a.TextureID < b.TextureID ? -1 : 1;

					return 0;
				});

				//因为真实的贴图ID和材质ID都太长了,为了在 Hierarchy 能显示下, 制作材质 ID、贴图 ID 映射表
				var materialID = 0;
				var textureID = 0;

				MaterialMapDict.Clear();
				TextureMapDict.Clear();

				foreach (var data in list)
				{
					if (!MaterialMapDict.ContainsKey(data.MaterialID))
						MaterialMapDict.Add(data.MaterialID, materialID++);

					if (!TextureMapDict.ContainsKey(data.TextureID))
						TextureMapDict.Add(data.TextureID, textureID++);
				}

				//打印排序后的物体数据情况
				// {
				// 	var s = "";
				// 	foreach (var data in list)
				// 	{
				// 		s += $"[{data.GO.name}] : " +
				// 		     $"Depth : {data.Depth.ToString()} | " +
				// 		     $"Material : {data.MaterialID.ToString()} | " +
				// 		     $"Texture : {data.MaterialID.ToString()} | " +
				// 		     $"Order : {AllUIElements[data.GO].ToString()}\n";
				// 	}
				//
				// 	Debug.Log(s);
				// }

				//计算UI 合批ID
				var batchID = 0;
				if (list.Count > 0)
				{
					list[0].BatchID = batchID;
					for (var i = 1; i < list.Count; i++)
					{
						//按顺序判断材质和贴图是否一样 一样的则为一个合批ID
						//相邻层 材质ID和贴图ID相同时 为一个批次，即可合批！
						var data = list[i];
						var lastData = list[i - 1];

						if (data.MaterialID != lastData.MaterialID || data.TextureID != lastData.TextureID || data.MaskCount != lastData.MaskCount)
							batchID++;

						data.BatchID = batchID;
					}
				}

				HierarchyExtension.Utility.SetExpandedRecursive(Selection.activeGameObject, true);
			}

			/// <summary>
			/// 获取 Canvas 下的全部 UI 对象
			/// </summary>
			/// <param name="parent"></param>
			/// <param name="elements"></param>
			/// <param name="order"></param>
			private static void GetAllUIElements(Transform parent, Dictionary<GameObject, int> elements, ref int order)
			{
				if (parent == null) return;

				if (NeedAnalyze(parent.gameObject, out var reason))
					elements.Add(parent.gameObject, order++);

				switch (reason)
				{
					case NoAnalyzeReason.Null:
					case NoAnalyzeReason.NotGraphic:
					case NoAnalyzeReason.GraphicAlphaZero:
					case NoAnalyzeReason.TextMeshProUGUI:
					case NoAnalyzeReason.TMP_SelectionCaret:
						foreach (Transform child in parent)
							GetAllUIElements(child, elements, ref order);
						break;
					case NoAnalyzeReason.Inactive:
					case NoAnalyzeReason.CanvasGroupAlphaZero:
					case NoAnalyzeReason.SubCanvas:
						// 这几种情况子物体都不用遍历子物体了
						break;
				}
			}

			/// <summary>
			/// 是否需要分析
			/// </summary>
			/// <param name="target"></param>
			/// <param name="reason">不需要分析原因</param>
			/// <returns></returns>
			private static bool NeedAnalyze(GameObject target, out NoAnalyzeReason reason)
			{
				reason = NoAnalyzeReason.Null;

				if (target == null || !target.activeInHierarchy)
				{
					reason = NoAnalyzeReason.Inactive;
					return false;
				}

				if (target.TryGetComponent<Canvas>(out _))
				{
					if (_isAnalyzing)
					{
						reason = NoAnalyzeReason.SubCanvas;
						return false;
					}

					_isAnalyzing = true;
					return false;
				}

				if (target.TryGetComponent<TextMeshProUGUI>(out _))
				{
					reason = NoAnalyzeReason.TextMeshProUGUI;
					return false;
				}

				if (target.TryGetComponent<TMP_SelectionCaret>(out _))
				{
					reason = NoAnalyzeReason.TMP_SelectionCaret;
					return false;
				}

				if (!target.TryGetComponent<Graphic>(out var graphic))
				{
					reason = NoAnalyzeReason.NotGraphic;
					return false;
				}

				if (!graphic.enabled || graphic.color.a == 0)
				{
					reason = NoAnalyzeReason.GraphicAlphaZero;
					return false;
				}

				if (target.TryGetComponent<CanvasGroup>(out var canvasGroup) && (!canvasGroup.enabled || canvasGroup.alpha == 0))
				{
					reason = NoAnalyzeReason.CanvasGroupAlphaZero;
					return false;
				}

				return true;
			}

			/// <summary>
			/// 深度优先级遍历
			/// </summary>
			private static void AnalyzeUIElement(GameObject curElement)
			{
				var graphic = curElement.GetComponent<Graphic>();
				var data = new Data
				{
					GO = curElement,
					MaterialID = graphic.materialForRendering.GetInstanceID(),
					TextureID = graphic.mainTexture.GetNativeTexturePtr().ToInt32()
				};

				/*
					根据子物体和它自身的信息获取depth
					无下层物体时，深度为0
					有>0个子物体且相交时，根据可 Batch 时,深度为子物体深度,不可 Batch 时，深度为子物体深度 +1 的规则,
					计算所有 depth_i, 取max(depth_1, depth_2, ... depth_x)
				*/

				//获取下层物体列表

				var order = AllUIElements[curElement];
				var lowerElements = (from v in AllUIElements where v.Value < order select v.Key).ToArray();

				// 检测下层是否有与当前 Element 相交的对象,如果有计算深度
				var depthList = new List<int>();

				foreach (var lowerElement in lowerElements)
				{
					if (curElement.rectTransform().Overlaps(lowerElement.rectTransform()))
					{
						Debug.Log(curElement + " | " + lowerElement);
						depthList.Add(CalculateDepth(curElement, lowerElement));
					}
				}

				data.Depth = depthList.Count == 0 ? 0 : Mathf.Max(depthList.ToArray());

				var curMask2DCount = curElement.GetComponentsInParent<RectMask2D>().Length;
				var curMaskCount = curElement.GetComponentsInParent<Mask>().Length;
				data.MaskCount = curMask2DCount + curMaskCount;

				GameObjectData.Add(curElement, data);
			}

			/// <summary>
			/// 根据判断是否合批来计算深度
			/// </summary>
			/// <param name="curElement">要计算深度的对象</param>
			/// <param name="lowerElement">下层物体对象</param>
			/// <returns></returns>
			private static int CalculateDepth(GameObject curElement, GameObject lowerElement)
			{
				if (!GameObjectData.ContainsKey(lowerElement)) return -1;

				var targetDepth = GameObjectData[lowerElement].Depth;

				var lowerMask2DCount = lowerElement.GetComponentsInParent<RectMask2D>().Length;
				var lowerMaskCount = lowerElement.GetComponentsInParent<Mask>().Length;

				var curMask2DCount = curElement.GetComponentsInParent<RectMask2D>().Length;
				var curMaskCount = curElement.GetComponentsInParent<Mask>().Length;

				if (IsCanBatch(curElement, lowerElement))
				{
					if (lowerMask2DCount == curMask2DCount && lowerMaskCount == curMaskCount)
					{
						return targetDepth; //depth为子物体的深度
					}

					targetDepth += curMask2DCount + curMaskCount;
					return targetDepth;
				}

				targetDepth += curMask2DCount + curMaskCount;

				return targetDepth + 1;
			}

			/// <summary>
			/// 判断2个物体是否可合批
			/// 1、比较材质(材质本身、材质上的贴图）
			/// 2、若比较材质相同时，继续比较图片的贴图（非材质贴图）即 Image 的贴图
			/// </summary>
			/// <param name="curElement">要计算深度的对象</param>
			/// <param name="lowerElement">下层物体对象</param>
			/// <returns></returns>
			private static bool IsCanBatch(GameObject curElement, GameObject lowerElement)
			{
				//比较材质
				var curGraphic = curElement.GetComponent<Graphic>();
				var lowerGraphic = lowerElement.GetComponent<Graphic>();

				if (!curGraphic.materialForRendering.Equals(lowerGraphic.materialForRendering)) return false;

				//如果材质相同,则比较纹理(或者说是不是同一个图集)
				var canBatch = curGraphic.mainTexture.Equals(lowerGraphic.mainTexture);

				return canBatch;
			}
		}
	}
}