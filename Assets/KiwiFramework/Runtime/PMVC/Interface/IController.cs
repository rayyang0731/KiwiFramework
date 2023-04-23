namespace KiwiFramework.Runtime
{
	/// <summary>
	/// 控制器接口
	/// </summary>
	public interface IController
	{
		/// <summary>
		/// 注册指令
		/// </summary>
		/// <typeparam name="T">指令类型</typeparam>
		void RegisterCommand<T>() where T : class, ICommand;

		/// <summary>
		/// 执行指令
		/// </summary>
		/// <param name="data">数据</param>
		/// <typeparam name="T">指令类型</typeparam>
		void ExecuteCommand<T>(T data) where T : class, ICommand;

		/// <summary>
		/// 移除指令
		/// </summary>
		/// <typeparam name="T">指令类型</typeparam>
		void RemoveCommand<T>() where T : class, ICommand;

		/// <summary>
		/// 移除全部指令
		/// </summary>
		void RemoveAllCommand();
	}
}