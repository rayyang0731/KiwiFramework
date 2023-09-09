namespace KiwiFramework.Runtime.UnityExtend
{
	/// <summary>
	/// Unity Class 方法扩展类
	/// </summary>
	public static partial class Extend
	{
		/// <summary>
		/// 判断是否为 null
		/// </summary>
		/// <param name="obj">被扩展的对象</param>
		/// <returns></returns>
		public static bool IsNull(this UnityEngine.Object obj) => obj == null || obj.Equals(null);
	}
}