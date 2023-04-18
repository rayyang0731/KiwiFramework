namespace KiwiFramework.Runtime
{
	/// <summary>
	/// FixedUpdate 接口(遵循 Mono 的 FixedUpdate)
	/// </summary>
	public interface IFixedTick : IBaseTick
	{
		/// <summary>
		/// 在 FixedTick 的执行队列中的排序
		/// </summary>
		int FixedTickOrder { get; }

		void OnFixedTick(float fixedDeltaTime);
	}
}