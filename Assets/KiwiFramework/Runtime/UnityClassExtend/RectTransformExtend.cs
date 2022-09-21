using System;
using System.Linq;
using UnityEngine;
namespace KiwiFramework.Runtime
{
	/// <summary>
	/// 水平 anchor 枚举
	/// </summary>
	public enum AnchorHorizontal
	{
		/// <summary>
		/// 左边
		/// </summary>
		Left,
		/// <summary>
		/// 中间
		/// </summary>
		Center,
		/// <summary>
		/// 右边
		/// </summary>
		Right,
		/// <summary>
		/// 拉伸
		/// </summary>
		Stretch
	}

	/// <summary>
	/// 垂直 anchor 枚举
	/// </summary>
	public enum AnchorVertical
	{
		/// <summary>
		/// 顶部
		/// </summary>
		Top,
		/// <summary>
		/// 中间
		/// </summary>
		Middle,
		/// <summary>
		/// 底部
		/// </summary>
		Bottom,
		/// <summary>
		/// 拉伸
		/// </summary>
		Stretch
	}

	/// <summary>
	/// 锚点预设
	/// </summary>
	public enum AnchorPresets
	{
		/// <summary>
		/// 左上
		/// </summary>
		TopLeft = 1,
		/// <summary>
		/// 中上
		/// </summary>
		TopCenter,
		/// <summary>
		/// 右上
		/// </summary>
		TopRight,

		/// <summary>
		/// 左中
		/// </summary>
		MiddleLeft,
		/// <summary>
		/// 正中
		/// </summary>
		MiddleCenter,
		/// <summary>
		/// 右中
		/// </summary>
		MiddleRight,

		/// <summary>
		/// 左下
		/// </summary>
		BottomLeft,
		/// <summary>
		/// 中下
		/// </summary>
		BottomCenter,
		/// <summary>
		/// 右下
		/// </summary>
		BottomRight,

		/// <summary>
		/// 靠左垂直拉伸
		/// </summary>
		VertStretchLeft,
		/// <summary>
		/// 靠右垂直拉伸
		/// </summary>
		VertStretchRight,
		/// <summary>
		/// 中间垂直拉伸
		/// </summary>
		VertStretchCenter,

		/// <summary>
		/// 靠上水平拉伸
		/// </summary>
		HorStretchTop,
		/// <summary>
		/// 中间水平拉伸
		/// </summary>
		HorStretchMiddle,
		/// <summary>
		/// 靠下水平拉伸
		/// </summary>
		HorStretchBottom,

		/// <summary>
		/// 完全拉伸
		/// </summary>
		StretchAll
	}

	/// <summary>
	/// 轴心点预设
	/// </summary>
	public enum PivotPresets
	{
		/// <summary>
		/// 左上
		/// </summary>
		TopLeft = 1,
		/// <summary>
		/// 中上
		/// </summary>
		TopCenter,
		/// <summary>
		/// 右上
		/// </summary>
		TopRight,

		/// <summary>
		/// 左中
		/// </summary>
		MiddleLeft,
		/// <summary>
		/// 正中
		/// </summary>
		MiddleCenter,
		/// <summary>
		/// 右中
		/// </summary>
		MiddleRight,

		/// <summary>
		/// 左下
		/// </summary>
		BottomLeft,
		/// <summary>
		/// 中下
		/// </summary>
		BottomCenter,
		/// <summary>
		/// 右下
		/// </summary>
		BottomRight,
	}

	/// <summary>
	/// RectTransform 类方法扩展
	/// </summary>
	public static partial class UnityClassExtend
	{
		/// <summary>
		/// 将位置,宽高,锚点,轴心点全部重置
		/// </summary>
		/// <param name="rectTransform">被扩展的对象</param>
		/// <param name="horizontal">水平 anchor 位置</param>
		/// <param name="vertical">垂直 anchor 位置</param>
		/// <param name="parent">父物体</param>
		public static void ResetZero(this RectTransform rectTransform, AnchorHorizontal horizontal,
			AnchorVertical vertical, Transform parent = null)
		{
			SetAnchorPresets(rectTransform, horizontal, vertical);
			if (parent != null)
				rectTransform.SetParent(parent);
			rectTransform.anchoredPosition = Vector2.zero;
			rectTransform.sizeDelta = Vector2.zero;
			rectTransform.pivot = Vector2.one * 0.5f;
		}

		/// <summary>
		/// 设置 anchor
		/// </summary>
		/// <param name="rectTransform">被扩展的对象</param>
		/// <param name="horizontal">水平 anchor 位置 [0-Left,1-Center,2-Right,3-Stretch]</param>
		/// <param name="vertical">垂直 anchor 位置 [0-Top,1-Middle,2-Bottom,3-Stretch]</param>
		public static void SetAnchorPresets(this RectTransform rectTransform, int horizontal, int vertical)
		{
			rectTransform.SetAnchorPresets((AnchorHorizontal)horizontal, (AnchorVertical)vertical);
		}

		/// <summary>
		/// 设置 anchor
		/// </summary>
		/// <param name="rectTransform">被扩展的对象</param>
		/// <param name="horizontal">水平 anchor 位置</param>
		/// <param name="vertical">垂直 anchor 位置</param>
		public static void SetAnchorPresets(this RectTransform rectTransform, AnchorHorizontal horizontal,
			AnchorVertical vertical)
		{
			var min = rectTransform.anchorMin;
			var max = rectTransform.anchorMax;

			var anchorHor = GetAnchorHorizontalValue(horizontal);
			min.x = anchorHor.x;
			max.x = anchorHor.y;

			var anchorVer = GetAnchorVerticalValue(vertical);
			min.y = anchorVer.x;
			max.y = anchorVer.y;

			rectTransform.anchorMin = min;
			rectTransform.anchorMax = max;

			rectTransform.pivot = new Vector2(min.x, max.y);
		}


		/// <summary>
		/// 设置水平 anchor
		/// </summary>
		/// <param name="rectTransform">被扩展的对象</param>
		/// <param name="horizontal">水平 anchor 位置,请参考<see cref="AnchorHorizontal"/></param>
		public static void SetAnchorHorizontal(this RectTransform rectTransform, int horizontal)
		{
			rectTransform.SetAnchorHorizontal((AnchorHorizontal)horizontal);
		}

		/// <summary>
		/// 设置水平 anchor
		/// </summary>
		/// <param name="rectTransform">被扩展的对象</param>
		/// <param name="horizontal">水平 anchor 位置</param>
		public static void SetAnchorHorizontal(this RectTransform rectTransform, AnchorHorizontal horizontal)
		{
			var min = rectTransform.anchorMin;
			var max = rectTransform.anchorMax;
			var anchor = GetAnchorHorizontalValue(horizontal);
			min.x = anchor.x;
			max.x = anchor.y;
			rectTransform.anchorMin = min;
			rectTransform.anchorMax = max;

			rectTransform.pivot = anchor;
		}

		/// <summary>
		/// 设置垂直 anchor
		/// </summary>
		/// <param name="rectTransform">被扩展的对象</param>
		/// <param name="vertical">垂直 anchor 位置,请参考<see cref="AnchorVertical"/></param>
		public static void SetAnchorVertical(this RectTransform rectTransform, int vertical)
		{
			rectTransform.SetAnchorVertical((AnchorVertical)vertical);
		}

		/// <summary>
		/// 设置垂直 anchor
		/// </summary>
		/// <param name="rectTransform">被扩展的对象</param>
		/// <param name="vertical">垂直 anchor 位置</param>
		public static void SetAnchorVertical(this RectTransform rectTransform, AnchorVertical vertical)
		{
			var min = rectTransform.anchorMin;
			var max = rectTransform.anchorMax;
			var anchor = GetAnchorVerticalValue(vertical);
			min.x = anchor.x;
			max.x = anchor.y;
			rectTransform.anchorMin = min;
			rectTransform.anchorMax = max;

			rectTransform.pivot = anchor;
		}

		/// <summary>
		/// 设置锚点
		/// </summary>
		/// <param name="rectTransform">被扩展的对象</param>
		/// <param name="anchorPresets">锚点预设,请参考<see cref="AnchorPresets"/></param>
		/// <param name="x">当更改完锚点后要设置的 anchorPosition x 轴坐标</param>
		/// <param name="y">当更改完锚点后要设置的 anchorPosition y 轴坐标</param>
		public static void SetAnchor(this RectTransform rectTransform, int anchorPresets, float x = 0, float y = 0)
		{
			rectTransform.SetAnchor((AnchorPresets)anchorPresets, x, y);
		}

		/// <summary>
		/// 设置锚点
		/// </summary>
		/// <param name="rectTransform">被扩展的对象</param>
		/// <param name="anchorPresets">锚点预设</param>
		/// <param name="x">当更改完锚点后要设置的 anchorPosition x 轴坐标</param>
		/// <param name="y">当更改完锚点后要设置的 anchorPosition y 轴坐标</param>
		public static void SetAnchor(this RectTransform rectTransform, AnchorPresets anchorPresets, float x = 0, float y = 0)
		{
			var anchorMin = rectTransform.anchorMin;
			var anchorMax = rectTransform.anchorMax;

			switch (anchorPresets)
			{
				case (AnchorPresets.TopLeft):
					anchorMin.x = 0;
					anchorMin.y = 1;

					anchorMax.x = 0;
					anchorMax.y = 1;
					break;

				case (AnchorPresets.TopCenter):
					anchorMin.x = 0.5f;
					anchorMin.y = 1;

					anchorMax.x = 0.5f;
					anchorMax.y = 1;
					break;

				case (AnchorPresets.TopRight):
					anchorMin.x = 1;
					anchorMin.y = 1;

					anchorMax.x = 1;
					anchorMax.y = 1;
					break;

				case (AnchorPresets.MiddleLeft):
					anchorMin.x = 0;
					anchorMin.y = 0.5f;

					anchorMax.x = 0;
					anchorMax.y = 0.5f;
					break;

				case (AnchorPresets.MiddleCenter):
					anchorMin.x = 0.5f;
					anchorMin.y = 0.5f;

					anchorMax.x = 0.5f;
					anchorMax.y = 0.5f;
					break;

				case (AnchorPresets.MiddleRight):
					anchorMin.x = 1;
					anchorMin.y = 0.5f;

					anchorMax.x = 1;
					anchorMax.y = 0.5f;
					break;

				case (AnchorPresets.BottomLeft):
					anchorMin.x = 0;
					anchorMin.y = 0;

					anchorMax.x = 0;
					anchorMax.y = 0;
					break;

				case (AnchorPresets.BottomCenter):
					anchorMin.x = 0.5f;
					anchorMin.y = 0;

					anchorMax.x = 0.5f;
					anchorMax.y = 0;
					break;

				case (AnchorPresets.BottomRight):
					anchorMin.x = 1;
					anchorMin.y = 0;

					anchorMax.x = 1;
					anchorMax.y = 0;
					break;

				case (AnchorPresets.HorStretchTop):
					anchorMin.x = 0;
					anchorMin.y = 1;

					anchorMax.x = 1;
					anchorMax.y = 1;
					break;

				case (AnchorPresets.HorStretchMiddle):
					anchorMin.x = 0;
					anchorMin.y = 0.5f;

					anchorMax.x = 1;
					anchorMax.y = 0.5f;
					break;

				case (AnchorPresets.HorStretchBottom):
					anchorMin.x = 0;
					anchorMin.y = 0;

					anchorMax.x = 1;
					anchorMax.y = 0;
					break;

				case (AnchorPresets.VertStretchLeft):
					anchorMin.x = 0;
					anchorMin.y = 0;

					anchorMax.x = 0;
					anchorMax.y = 1;
					break;

				case (AnchorPresets.VertStretchCenter):
					anchorMin.x = 0.5f;
					anchorMin.y = 0;

					anchorMax.x = 0.5f;
					anchorMax.y = 1;
					break;

				case (AnchorPresets.VertStretchRight):
					anchorMin.x = 1;
					anchorMin.y = 0;

					anchorMax.x = 1;
					anchorMax.y = 1;
					break;

				case (AnchorPresets.StretchAll):
					anchorMin.x = 0;
					anchorMin.y = 0;

					anchorMax.x = 1;
					anchorMax.y = 1;
					break;

				default:
					throw new ArgumentOutOfRangeException(nameof(anchorPresets), anchorPresets, null);
			}

			rectTransform.anchorMin = anchorMin;
			rectTransform.anchorMax = anchorMax;

			var anchoredPos = rectTransform.anchoredPosition;
			anchoredPos.Set(x, y);
			rectTransform.anchoredPosition = anchoredPos;
		}

		/// <summary>
		/// 设置轴心点
		/// </summary>
		/// <param name="rectTransform">被扩展的对象</param>
		/// <param name="x">x 轴坐标</param>
		/// <param name="y">y 轴坐标</param>
		public static void SetPivot(this RectTransform rectTransform, float x, float y)
		{
			var pivot = rectTransform.pivot;
			pivot.Set(x, y);
			rectTransform.pivot = pivot;
		}

		/// <summary>
		/// 设置轴心点
		/// </summary>
		/// <param name="rectTransform">被扩展的对象</param>
		/// <param name="preset">轴心点预设,请参考<see cref="PivotPresets"/></param>
		public static void SetPivot(this RectTransform rectTransform, int preset)
		{
			rectTransform.SetPivot((PivotPresets)preset);
		}

		/// <summary>
		/// 设置轴心点
		/// </summary>
		/// <param name="rectTransform">被扩展的对象</param>
		/// <param name="preset">轴心点预设</param>
		public static void SetPivot(this RectTransform rectTransform, PivotPresets preset)
		{
			var pivot = rectTransform.pivot;
			switch (preset)
			{
				case (PivotPresets.TopLeft):
				{
					pivot.x = 0;
					pivot.y = 1;
					break;
				}
				case (PivotPresets.TopCenter):
				{
					pivot.x = 0.5f;
					pivot.y = 1;
					break;
				}
				case (PivotPresets.TopRight):
				{
					pivot.x = 1;
					pivot.y = 1;
					break;
				}

				case (PivotPresets.MiddleLeft):
				{
					pivot.x = 0;
					pivot.y = 0.5f;
					break;
				}
				case (PivotPresets.MiddleCenter):
				{
					pivot.x = 0.5f;
					pivot.y = 0.5f;
					break;
				}
				case (PivotPresets.MiddleRight):
				{
					pivot.x = 1;
					pivot.y = 0.5f;
					break;
				}

				case (PivotPresets.BottomLeft):
				{
					pivot.x = 0;
					pivot.y = 0;
					break;
				}
				case (PivotPresets.BottomCenter):
				{
					pivot.x = 0.5f;
					pivot.y = 0;
					break;
				}
				case (PivotPresets.BottomRight):
				{
					pivot.x = 1;
					pivot.y = 0;
					break;
				}
			}

			rectTransform.pivot = pivot;
		}

		/// <summary>
		/// 获得指定水平 anchor 位置的值
		/// </summary>
		/// <param name="horizontal">水平 anchor 位置</param>
		/// <returns></returns>
		private static Vector2 GetAnchorHorizontalValue(AnchorHorizontal horizontal)
		{
			return horizontal switch
			{
				AnchorHorizontal.Center => Vector2.one * 0.5f,
				AnchorHorizontal.Left => Vector2.zero,
				AnchorHorizontal.Right => Vector2.one,
				AnchorHorizontal.Stretch => Vector2.up,
				_ => Vector2.one * 0.5f
			};
		}

		/// <summary>
		/// 获得指定垂直 anchor 位置的值
		/// </summary>
		/// <param name="vertical">垂直 anchor 位置</param>
		/// <returns></returns>
		private static Vector2 GetAnchorVerticalValue(AnchorVertical vertical)
		{
			return vertical switch
			{
				AnchorVertical.Middle => Vector2.one * 0.5f,
				AnchorVertical.Top => Vector2.one,
				AnchorVertical.Bottom => Vector2.zero,
				AnchorVertical.Stretch => Vector2.up,
				_ => Vector2.one * 0.5f
			};
		}

		/// <summary>
		/// 是否与指定的 anchor 值相等
		/// </summary>
		/// <param name="rectTransform">被扩展的对象</param>
		/// <param name="horizontal">水平 anchor 位置 [0-Left,1-Center,2-Right,3-Stretch]</param>
		/// <param name="vertical">垂直 anchor 位置 [0-Top,1-Middle,2-Bottom,3-Stretch]</param>
		/// <returns></returns>
		public static bool IsAnchorPresets(this RectTransform rectTransform, int horizontal, int vertical)
		{
			return rectTransform.IsAnchorPresets((AnchorHorizontal)horizontal, (AnchorVertical)vertical);
		}

		/// <summary>
		/// 是否与指定的 anchor 值相等
		/// </summary>
		/// <param name="rectTransform">被扩展的对象</param>
		/// <param name="horizontal">水平 anchor 位置</param>
		/// <param name="vertical">垂直 anchor 位置</param>
		/// <returns></returns>
		public static bool IsAnchorPresets(this RectTransform rectTransform, AnchorHorizontal horizontal,
			AnchorVertical vertical)
		{
			return IsAnchorHorizontal(rectTransform, horizontal) && IsAnchorVertical(rectTransform, vertical);
		}

		/// <summary>
		/// 是否与指定的水平 anchor 值相等
		/// </summary>
		/// <param name="rectTransform">被扩展的对象</param>
		/// <param name="horizontal">水平 anchor 位置 [0-Left,1-Center,2-Right,3-Stretch]</param>
		/// <returns></returns>
		public static bool IsAnchorHorizontal(this RectTransform rectTransform, int horizontal)
		{
			return rectTransform.IsAnchorHorizontal((AnchorHorizontal)horizontal);
		}

		/// <summary>
		/// 是否与指定的水平 anchor 值相等
		/// </summary>
		/// <param name="rectTransform">被扩展的对象</param>
		/// <param name="horizontal">水平 anchor 位置</param>
		/// <returns></returns>
		public static bool IsAnchorHorizontal(this RectTransform rectTransform, AnchorHorizontal horizontal)
		{
			var anchor = GetAnchorHorizontalValue(horizontal);
			return Mathf.Approximately(rectTransform.anchorMin.x, anchor.x) && Mathf.Approximately(rectTransform.anchorMax.x, anchor.y);
		}

		/// <summary>
		/// 是否与指定的垂直 anchor 值相等
		/// </summary>
		/// <param name="rectTransform">被扩展的对象</param>
		/// <param name="vertical">垂直 anchor 位置 [0-Top,1-Middle,2-Bottom,3-Stretch]</param>
		/// <returns></returns>
		public static bool IsAnchorVertical(this RectTransform rectTransform, int vertical)
		{
			return rectTransform.IsAnchorVertical((AnchorVertical)vertical);
		}

		/// <summary>
		/// 是否与指定的垂直 anchor 值相等
		/// </summary>
		/// <param name="rectTransform">被扩展的对象</param>
		/// <param name="vertical">垂直 anchor 位置</param>
		/// <returns></returns>
		public static bool IsAnchorVertical(this RectTransform rectTransform, AnchorVertical vertical)
		{
			var anchor = GetAnchorVerticalValue(vertical);
			return Mathf.Approximately(rectTransform.anchorMin.y, anchor.x) && Mathf.Approximately(rectTransform.anchorMax.y, anchor.y);
		}

		/// <summary>
		/// 设置 RectTransform 的 AnchoredPosition
		/// </summary>
		/// <param name="rectTransform"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public static void SetAnchoredPosition(this RectTransform rectTransform, float x, float y)
		{
			var pos = rectTransform.anchoredPosition;

			if (!Mathf.Approximately(pos.x, x))
				pos.x = x;
			if (!Mathf.Approximately(pos.y, y))
				pos.y = y;

			rectTransform.anchoredPosition = pos;
		}

		/// <summary>
		/// 设置 RectTransform 的 AnchoredPosition 的 X
		/// </summary>
		/// <param name="rectTransform"></param>
		/// <param name="x"></param>
		public static void SetAnchoredPositionX(this RectTransform rectTransform, float x)
		{
			var pos = rectTransform.anchoredPosition;

			if (!Mathf.Approximately(pos.x, x))
				pos.x = x;

			rectTransform.anchoredPosition = pos;
		}

		/// <summary>
		/// 设置 RectTransform 的 AnchoredPosition 的 Y
		/// </summary>
		/// <param name="rectTransform"></param>
		/// <param name="y"></param>
		public static void SetAnchoredPositionY(this RectTransform rectTransform, float y)
		{
			var pos = rectTransform.anchoredPosition;

			if (!Mathf.Approximately(pos.y, y))
				pos.y = y;

			rectTransform.anchoredPosition = pos;
		}

		/// <summary>
		/// 设置 RectTransform 的 AnchoredPosition3D
		/// </summary>
		/// <param name="rectTransform"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		public static void SetAnchoredPosition3D(this RectTransform rectTransform, float x, float y, float z)
		{
			var pos = rectTransform.anchoredPosition3D;

			if (!Mathf.Approximately(pos.x, x))
				pos.x = x;
			if (!Mathf.Approximately(pos.y, y))
				pos.y = y;
			if (!Mathf.Approximately(pos.z, z))
				pos.z = z;

			rectTransform.anchoredPosition3D = pos;
		}

		/// <summary>
		/// 获取宽度
		/// </summary>
		/// <param name="rectTransform">被扩展的对象</param>
		/// <returns></returns>
		public static float GetWidth(this RectTransform rectTransform)
		{
			return rectTransform.rect.width;
		}

		/// <summary>
		/// 获取高度
		/// </summary>
		/// <param name="rectTransform">被扩展的对象</param>
		/// <returns></returns>
		public static float GetHeight(this RectTransform rectTransform)
		{
			return rectTransform.rect.height;
		}

		/// <summary>
		/// 获取尺寸
		/// </summary>
		/// <param name="rectTransform">被扩展的对象</param>
		/// <returns></returns>
		public static Vector2 GetSize(this RectTransform rectTransform)
		{
			return rectTransform.rect.size;
		}

		/// <summary>
		/// 设置宽度
		/// </summary>
		/// <param name="rectTransform">被扩展的对象</param>
		/// <param name="width">要设置的宽度</param>
		public static void SetWidth(this RectTransform rectTransform, float width)
		{
			rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
		}

		/// <summary>
		/// 设置高度
		/// </summary>
		/// <param name="rectTransform">被扩展的对象</param>
		/// <param name="height">要设置的高度</param>
		public static void SetHeight(this RectTransform rectTransform, float height)
		{
			rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
		}

		/// <summary>
		/// 设置尺寸
		/// </summary>
		/// <param name="rectTransform">被扩展的对象</param>
		/// <param name="width">要设置的宽度</param>
		/// <param name="height">要设置的高度</param>
		public static void SetSize(this RectTransform rectTransform, float width, float height)
		{
			rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
			rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
		}

		/// <summary>
		/// 设置尺寸
		/// </summary>
		/// <param name="rectTransform">被扩展的对象</param>
		/// <param name="size">要设置的尺寸</param>
		public static void SetSize(this RectTransform rectTransform, Vector2 size)
		{
			rectTransform.SetSize(size.x, size.y);
		}

		/// <summary>
		/// 判断两个 RectTransform 是否相交
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="allowInverse">是否可以有负值</param>
		/// <returns></returns>
		public static bool Overlaps(this RectTransform a, RectTransform b, bool allowInverse = false)
		{
			var corner1 = new Vector3[4];
			a.GetWorldCorners(corner1);

			var corner2 = new Vector3[4];
			b.GetWorldCorners(corner2);

			bool IsPointInRect(Vector3 lb, Vector3 rt, Vector3 point) => point.x <= rt.x && point.x >= lb.x && point.y <= rt.y && point.y >= lb.y;

			return corner1.Any(point => IsPointInRect(corner2[0], corner2[2], point)) ||
				corner2.Any(point => IsPointInRect(corner1[0], corner1[2], point));
		}
	}
}
