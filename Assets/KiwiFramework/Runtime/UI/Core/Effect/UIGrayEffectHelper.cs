using UnityEngine;
using UnityEngine.UI;

namespace KiwiFramework.Runtime.UI
{
	/// <summary>
	/// 置灰效果辅助器
	/// </summary>
	public static partial class UIEffectHelper
	{
		#region Gray

		private static Material _grayMat;

		public static Material GrayMaterial
		{
			get
			{
				if (_grayMat == null)
				{
					var mat = Resources.Load<Material>("UIGray");
					_grayMat      = Object.Instantiate(mat);
					_grayMat.name = "Kiwi.UIGray";
				}

				return _grayMat;
			}
		}

		/// <summary>
		/// 设置置灰
		/// </summary>
		/// <param name="target">目标对象</param>
		/// <param name="value">是否置灰</param>
		public static void SetGray(Graphic target, bool value)
		{
			if (target != null)
				target.material = value ? GrayMaterial : null;
		}

		/// <summary>
		/// 设置置灰(递归,将子对象也置灰)
		/// </summary>
		/// <param name="go">目标对象</param>
		/// <param name="value">是否置灰</param>
		public static void SetGrayRecursion(GameObject go, bool value)
		{
			if (go == null)
				return;

			var targets = go.GetComponentsInChildren<IGray>();
			foreach (var target in targets)
			{
				target.SetGray(value);
			}
		}

		#endregion
	}
}