using System;
using System.Collections.Generic;

using KiwiFramework.Runtime.Utility;

using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

namespace KiwiFramework.Runtime.UI
{
	/// <summary>
	/// 图像组件
	/// <para>继承自 Unity 的 Image 组件,支持翻转和镜像功能</para>
	/// </summary>
	[HideMonoScript, AddComponentMenu("KiwiUI/Native/RichImage")]
	public class RichImage : Image
	{
		/// <summary>
		/// 使用翻转或镜像
		/// </summary>
		[SerializeField]
		private ImageType imageType = ImageType.NONE;

		/// <summary>
		/// 使用翻转或镜像
		/// </summary>
		public ImageType ImageType
		{
			get => imageType;
			set
			{
				imageType = value;
				SetVerticesDirty();
			}
		}

		#region 翻转类型

		/// <summary>
		/// 翻转类型
		/// </summary>
		[SerializeField]
		private FlipType flipType;

		/// <summary>
		/// 翻转类型
		/// </summary>
		public FlipType FlipType
		{
			get => flipType;
			set
			{
				if (!SetPropertyUtility.SetStruct(ref flipType, value))
					return;

				UpdateGeometry();

#if UNITY_EDITOR
				if (!UnityEditor.EditorApplication.isPlaying)
					UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
			}
		}

		#endregion

		#region 镜像类型

		[SerializeField] private MirrorType mirrorType;

		/// <summary>
		/// 镜像类型
		/// </summary>
		public MirrorType MirrorType
		{
			get => mirrorType;
			set
			{
				if (!SetPropertyUtility.SetStruct(ref mirrorType, value))
					return;

				SetVerticesDirty();

#if UNITY_EDITOR
				if (!UnityEditor.EditorApplication.isPlaying)
					UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
			}
		}

		/// <summary>
		/// Simple模式,原始图片顶点左移
		/// </summary>
		/// <param name="rect">像素矫正过的尺寸(排除图片的 alpha 区域)</param>
		/// <param name="vertexes">原始的顶点数据</param>
		/// <param name="count">原始顶点数量</param>
		private void SimpleScale(Rect rect, List<UIVertex> vertexes, int count)
		{
			for (var i = 0; i < count; i++)
			{
				var vertex   = vertexes[i];
				var position = vertex.position;

				if (MirrorType is MirrorType.HORIZONTAL or MirrorType.QUARTER)
					position.x = (position.x + rect.x) * 0.5f;

				if (MirrorType is MirrorType.VERTICAL or MirrorType.QUARTER)
					position.y = (position.y + rect.y) * 0.5f;

				vertex.position = position;
				vertexes[i]     = vertex;
			}
		}

		/// <summary>
		/// Sliced模式,原始图片顶点左移
		/// </summary>
		/// <param name="rect">像素矫正过的尺寸(排除图片的 alpha 区域)</param>
		/// <param name="vertexes">原始的顶点数据</param>
		/// <param name="count">原始顶点数量</param>
		private void SlicedScale(Rect rect, List<UIVertex> vertexes, int count)
		{
			var border     = GetAdjustedBorders(rect);
			var halfWidth  = rect.width * 0.5f;
			var halfHeight = rect.height * 0.5f;
			for (var i = 0; i < count; i++)
			{
				var vertex   = vertexes[i];
				var position = vertex.position;
				if (MirrorType is MirrorType.HORIZONTAL or MirrorType.QUARTER)
				{
					if (halfWidth < border.x && position.x >= rect.center.x)
						position.x = rect.center.x;
					else if (position.x >= border.x)
						position.x = (position.x + rect.x) * 0.5f;
				}

				if (MirrorType is MirrorType.VERTICAL or MirrorType.QUARTER)
				{
					if (halfHeight < border.y && position.y >= rect.center.y)
						position.y = rect.center.y;
					else if (position.y >= border.y)
						position.y = (position.y + rect.y) * 0.5f;
				}

				vertex.position = position;
				vertexes[i]     = vertex;
			}
		}

		/// <summary>
		/// 返回Sprite切片区域
		/// (Copy From Image.GetAdjustedBorders)
		/// </summary>
		/// <param name="rect"></param>
		/// <returns></returns>
		private Vector4 GetAdjustedBorders(Rect rect)
		{
			var border = sprite.border;

			border /= pixelsPerUnit;

			for (var axis = 0; axis <= 1; axis++)
			{
				var combinedBorders = border[axis] + border[axis + 2];
				if (rect.size[axis] < combinedBorders && combinedBorders != 0)
				{
					var borderScaleRatio = rect.size[axis] / combinedBorders;
					border[axis]     *= borderScaleRatio;
					border[axis + 2] *= borderScaleRatio;
				}
			}

			return border;
		}

		/// <summary>
		/// 清理掉不能成三角面的顶点
		/// 判断三个顶点是否有重合，把无用的三角面挪到数组最后，
		/// 在遍历完之后，把结尾的无用三角面删掉
		/// </summary>
		/// <param name="vertexes">顶点数据</param>
		/// <param name="count">顶点数量</param>
		/// <returns>实际可用的顶点数量</returns>
		private static int SliceExcludeVerts(List<UIVertex> vertexes, int count)
		{
			var realCount = count;

			var i = 0;

			while (i < realCount)
			{
				var v1 = vertexes[i];
				var v2 = vertexes[i + 1];
				var v3 = vertexes[i + 2];

				if (v1.position == v2.position || v2.position == v3.position || v3.position == v1.position)
				{
					vertexes[i]     = vertexes[realCount - 3];
					vertexes[i + 1] = vertexes[realCount - 2];
					vertexes[i + 2] = vertexes[realCount - 1];

					realCount -= 3;
					continue;
				}

				i += 3;
			}

			if (realCount < count)
				vertexes.RemoveRange(realCount, count - realCount);

			return realCount;
		}


		/// <summary>
		/// 扩展容量(镜像图片的顶点)
		/// </summary>
		/// <param name="vertexes">顶点数据</param>
		/// <param name="addCount">要添加的数量</param>
		private static void ExtendCapacity(List<UIVertex> vertexes, int addCount)
		{
			var neededCapacity = vertexes.Count + addCount;
			if (vertexes.Capacity < neededCapacity)
				vertexes.Capacity = neededCapacity;
		}

		/// <summary>
		/// 计算镜像顶点
		/// </summary>
		/// <param name="rect">像素矫正过的尺寸(排除图片的 alpha 区域)</param>
		/// <param name="vertexes">顶点数据</param>
		/// <param name="count">镜像顶点数量</param>
		/// <param name="isHorizontal">是否是水平镜像</param>
		private static void MirrorVerts(Rect rect, List<UIVertex> vertexes, int count, bool isHorizontal)
		{
			for (var i = 0; i < count; i++)
			{
				var vertex   = vertexes[i];
				var position = vertex.position;
				if (isHorizontal)
					position.x = rect.center.x * 2 - position.x;
				else
					position.y = rect.center.y * 2 - position.y;
				vertex.position = position;
				vertexes.Add(vertex);
			}
		}

		/// <summary>
		/// 返回三个点的中心点
		/// </summary>
		/// <returns></returns>
		private static float GetCenter(float p1, float p2, float p3)
		{
			var max = Mathf.Max(Mathf.Max(p1, p2), p3);
			var min = Mathf.Min(Mathf.Min(p1, p2), p3);

			return (max + min) / 2;
		}

		/// <summary>
		/// 返回翻转UV坐标
		/// </summary>
		/// <param name="uv">原始 UV 坐标</param>
		/// <param name="start">起始值</param>
		/// <param name="length">长度</param>
		/// <param name="isHorizontal">是否是水平翻转</param>
		/// <returns></returns>
		private static Vector2 GetOverturnUV(Vector2 uv, float start, float length, bool isHorizontal = true)
		{
			if (isHorizontal)
				uv.x = length - uv.x + start;
			else
				uv.y = length - uv.y + start;

			return uv;
		}

		private void DrawSimple(List<UIVertex> vertexes, int count)
		{
			var rect = GetPixelAdjustedRect();
			SimpleScale(rect, vertexes, count);

			switch (MirrorType)
			{
				case MirrorType.HORIZONTAL:
					ExtendCapacity(vertexes, count);
					MirrorVerts(rect, vertexes, count, true);
					break;
				case MirrorType.VERTICAL:
					ExtendCapacity(vertexes, count);
					MirrorVerts(rect, vertexes, count, false);
					break;
				case MirrorType.QUARTER:
					ExtendCapacity(vertexes, count * 3);
					MirrorVerts(rect, vertexes, count, true);
					MirrorVerts(rect, vertexes, count * 2, false);
					break;
			}
		}

		private void DrawSliced(List<UIVertex> vertexes, int count)
		{
			if (!hasBorder)
			{
				DrawSimple(vertexes, count);
				return;
			}

			var rect = GetPixelAdjustedRect();

			SlicedScale(rect, vertexes, count);

			count = SliceExcludeVerts(vertexes, count);

			switch (MirrorType)
			{
				case MirrorType.HORIZONTAL:
					ExtendCapacity(vertexes, count);
					MirrorVerts(rect, vertexes, count, true);
					break;
				case MirrorType.VERTICAL:
					ExtendCapacity(vertexes, count);
					MirrorVerts(rect, vertexes, count, false);
					break;
				case MirrorType.QUARTER:
					ExtendCapacity(vertexes, count * 3);
					MirrorVerts(rect, vertexes, count, true);
					MirrorVerts(rect, vertexes, count * 2, false);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void DrawTiled(List<UIVertex> vertexes, int count)
		{
			var rect = GetPixelAdjustedRect();

			//此处使用 inner 是因为 Image 绘制 Tiled 时，会把透明区域也绘制了。
			var inner = DataUtility.GetInnerUV(sprite);

			var w = sprite.rect.width / pixelsPerUnit;
			var h = sprite.rect.height / pixelsPerUnit;

			var len = count / 3;

			for (var i = 0; i < len; i++)
			{
				var v1 = vertexes[i * 3];
				var v2 = vertexes[i * 3 + 1];
				var v3 = vertexes[i * 3 + 2];

				var centerX = GetCenter(v1.position.x, v2.position.x, v3.position.x);
				var centerY = GetCenter(v1.position.y, v2.position.y, v3.position.y);

				if (MirrorType is MirrorType.HORIZONTAL or MirrorType.QUARTER)
				{
					//判断三个点的水平位置是否在偶数矩形内，如果是，则把UV坐标水平翻转
					if (Mathf.FloorToInt((centerX - rect.xMin) / w) % 2 == 1)
					{
						v1.uv0 = GetOverturnUV(v1.uv0, inner.x, inner.z, true);
						v2.uv0 = GetOverturnUV(v2.uv0, inner.x, inner.z, true);
						v3.uv0 = GetOverturnUV(v3.uv0, inner.x, inner.z, true);
					}
				}

				if (MirrorType is MirrorType.VERTICAL or MirrorType.QUARTER)
				{
					//判断三个点的垂直位置是否在偶数矩形内，如果是，则把UV坐标垂直翻转
					if (Mathf.FloorToInt((centerY - rect.yMin) / h) % 2 == 1)
					{
						v1.uv0 = GetOverturnUV(v1.uv0, inner.y, inner.w, false);
						v2.uv0 = GetOverturnUV(v2.uv0, inner.y, inner.w, false);
						v3.uv0 = GetOverturnUV(v3.uv0, inner.y, inner.w, false);
					}
				}

				vertexes[i * 3]     = v1;
				vertexes[i * 3 + 1] = v2;
				vertexes[i * 3 + 2] = v3;
			}
		}

		#endregion

		private readonly List<UIVertex> _vertexes = new();

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			base.OnPopulateMesh(vh);

			if (sprite == null || !isActiveAndEnabled)
				return;

			switch (imageType)
			{
				case ImageType.NONE:
					return;
				case ImageType.FLIP:
					OnPopulateMeshForFlip(vh);
					break;
				case ImageType.MIRROR:
					OnPopulateMeshForMirror(vh);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void OnPopulateMeshForFlip(VertexHelper vh)
		{
			vh.GetUIVertexStream(_vertexes);

			var count      = _vertexes.Count;
			var rectCenter = rectTransform.rect.center;
			for (var i = 0; i < count; i++)
			{
				var vertex = _vertexes[i];
				var pos    = vertex.position;

				vertex.position = new Vector3(
					flipType is FlipType.HORIZONTAL or FlipType.DIAGONAL ? (pos.x + (rectCenter.x - pos.x) * 2) : pos.x,
					flipType is FlipType.VERTICAL or FlipType.DIAGONAL ? (pos.y + (rectCenter.y - pos.y) * 2) : pos.y,
					pos.z);

				_vertexes[i] = vertex;
			}

			vh.Clear();
			vh.AddUIVertexTriangleStream(_vertexes);
			_vertexes.Clear();
		}

		private void OnPopulateMeshForMirror(VertexHelper vh)
		{
			vh.GetUIVertexStream(_vertexes);

			var count = _vertexes.Count;

			switch (type)
			{
				case Type.Simple:
					DrawSimple(_vertexes, count);
					break;
				case Type.Sliced:
					DrawSliced(_vertexes, count);
					break;
				case Type.Tiled:
					DrawTiled(_vertexes, count);
					break;
				case Type.Filled:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			vh.Clear();
			vh.AddUIVertexTriangleStream(_vertexes);
			_vertexes.Clear();
		}

		/// <summary>
		/// 使用图片原始尺寸
		/// </summary>
		public override void SetNativeSize()
		{
			if (sprite == null) return;

			switch (ImageType)
			{
				case ImageType.NONE:
				case ImageType.FLIP:
					base.SetNativeSize();
					break;
				case ImageType.MIRROR:
					SetNativeSizeForMirror();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void SetNativeSizeForMirror()
		{
			var rt = rectTransform;
			var w  = sprite.rect.width / pixelsPerUnit;
			var h  = sprite.rect.height / pixelsPerUnit;
			rt.anchorMax = rt.anchorMin;

			rt.sizeDelta = MirrorType switch
			               {
				               MirrorType.HORIZONTAL => new Vector2(w * 2, h),
				               MirrorType.VERTICAL   => new Vector2(w, h * 2),
				               MirrorType.QUARTER    => new Vector2(w * 2, h * 2),
				               _                     => rt.sizeDelta
			               };

			SetVerticesDirty();
		}
	}

	/// <summary>
	/// 使用翻转或镜像
	/// </summary>
	public enum ImageType : byte
	{
		/// <summary>
		/// 原始 Image
		/// </summary>
		[InspectorName("原始")]
		NONE,

		/// <summary>
		/// 翻转
		/// </summary>
		[InspectorName("翻转")]
		FLIP,

		/// <summary>
		/// 镜像
		/// </summary>
		[InspectorName("镜像")]
		MIRROR,
	}

	/// <summary>
	/// 翻转类型
	/// </summary>
	public enum FlipType : byte
	{
		/// <summary>
		/// 水平翻转
		/// </summary>
		[InspectorName("水平翻转")]
		HORIZONTAL,

		/// <summary>
		/// 垂直翻转
		/// </summary>
		[InspectorName("垂直翻转")]
		VERTICAL,

		/// <summary>
		/// 对角翻转
		/// </summary>
		[InspectorName("对角翻转")]
		DIAGONAL,
	}

	/// <summary>
	/// 镜像类型
	/// </summary>
	public enum MirrorType : byte
	{
		/// <summary>
		/// 水平镜像
		/// </summary>
		[InspectorName("水平镜像")]
		HORIZONTAL,

		/// <summary>
		/// 垂直镜像
		/// </summary>
		[InspectorName("垂直镜像")]
		VERTICAL,

		/// <summary>
		/// 万花筒(四分之一图片)
		/// </summary>
		[InspectorName("万花筒")]
		QUARTER,
	}
}