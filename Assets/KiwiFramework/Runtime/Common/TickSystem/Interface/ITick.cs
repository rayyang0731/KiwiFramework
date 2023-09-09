namespace KiwiFramework.Runtime
{
	/// <summary>
	/// Tick 接口(遵循 Mono 的 Update)
	/// </summary>
	public interface ITick : IBaseTick
	{
		/// <summary>
		/// 在 Update 的执行队列中的排序
		/// </summary>
		int TickOrder { get; }

		void OnTick(float deltaTime);
	}
}