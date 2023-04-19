using UnityEngine;
using UnityEngine.UI;

namespace KiwiFramework.Runtime.UI
{
	/// <summary>
	/// 模糊效果辅助器
	/// </summary>
	public static partial class UIEffectHelper
	{
		private static Material _blurMat;

		public static Material BlurMaterial
		{
			get
			{
				if (_blurMat == null)
				{
					var mat = Resources.Load<Material>("UIBlur");
					_blurMat      = Object.Instantiate(mat);
					_blurMat.name = "Kiwi.UIBlur";
				}

				return _blurMat;
			}
		}


		public static void SetBlur(Graphic target, bool value) { target.material = value ? BlurMaterial : null; }

		/// <summary>
		/// 释放UI模糊材质
		/// </summary>
		public static void ReleaseBlurMaterial()
		{
			_blurMat    = null;
		}

		public static Texture2D GetBlurTexture2D(Texture2D tex2D, int blurSize = 3)
		{
			var blurred = new Texture2D(tex2D.width, tex2D.height) {hideFlags = HideFlags.DontSaveInBuild, wrapMode = TextureWrapMode.Clamp, filterMode = FilterMode.Bilinear,};

			var avgColor = Color.clear;

			// 遍历图片的每个像素
			for (var imageX = 0; imageX < tex2D.width; imageX++)
			{
				for (var imageY = 0; imageY < tex2D.height; imageY++)
				{
					// 清空颜色平均值
					avgColor.r = avgColor.g = avgColor.b = avgColor.a = 0;

					// 模糊次数
					var blurPixelCount = 0;

					// 确保不会越界,平均每个像素的颜色
					for (var x = imageX; (x < imageX + blurSize && x < tex2D.width); x++)
					{
						for (var y = imageY; (y < imageY + blurSize && y < tex2D.height); y++)
						{
							var pixel = tex2D.GetPixel(x, y);

							avgColor.r += pixel.r;
							avgColor.g += pixel.g;
							avgColor.b += pixel.b;
							avgColor.a += pixel.a;

							blurPixelCount++;
						}
					}

					avgColor.r /= blurPixelCount;
					avgColor.g /= blurPixelCount;
					avgColor.b /= blurPixelCount;
					avgColor.a /= blurPixelCount;

					blurred.SetPixel(imageX, imageY, avgColor);
				}
			}

			blurred.Apply();

			Object.DestroyImmediate(tex2D, true);

			return blurred;
		}
	}
}