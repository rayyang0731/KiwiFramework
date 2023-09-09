namespace KiwiFramework.Runtime
{
	/// <summary>
	/// 指令接口
	/// </summary>
	public interface ICommand
	{
		/// <summary>
		/// 执行指令
		/// </summary>
		void Execute<T>(T data);
	}
}