namespace KiwiFramework.Runtime
{
	/// <summary>
	/// Tick 基础接口
	/// </summary>
	public interface IBaseTick
	{
		/// <summary>
		/// 是否后台运行
		/// </summary>
		bool runInBackground { get; }
	}
}