namespace KiwiFramework.Runtime
{
	/// <summary>
	/// LateTick 接口(遵循 Mono 的 LateUpdate)
	/// </summary>
	public interface ILateTick : IBaseTick
	{
		/// <summary>
		/// 在 LateTick 的执行队列中的排序
		/// </summary>
		int LateTickOrder { get; }

		void OnLateTick(float deltaTime);
	}
}