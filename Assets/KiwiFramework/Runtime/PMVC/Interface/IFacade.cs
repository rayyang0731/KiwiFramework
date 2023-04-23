namespace KiwiFramework.Runtime
{
	/// <summary>
	/// 外观接口
	/// </summary>
	public interface IFacade
	{
		#region Proxy 代理

		/// <summary>
		/// 注册代理
		/// </summary>
		/// <param name="proxy">代理对象</param>
		void RegisterProxy<T>(T proxy) where T : class, IProxy;

		/// <summary>
		/// 尝试获取代理
		/// </summary>
		/// <param name="proxy">代理对象</param>
		/// <typeparam name="T">代理对象类型</typeparam>
		/// <returns>是否获取成功</returns>
		bool TryGetProxy<T>(out T proxy) where T : class, IProxy;

		/// <summary>
		/// 获取代理
		/// </summary>
		/// <typeparam name="T">代理类型</typeparam>
		/// <returns>获取到的代理对象</returns>
		T GetProxy<T>() where T : class, IProxy;

		/// <summary>
		/// 移除代理
		/// </summary>
		/// <typeparam name="T">代理类型</typeparam>
		void RemoveProxy<T>() where T : class, IProxy;

		#endregion

		#region Command 指令

		/// <summary>
		/// 注册指令
		/// </summary>
		/// <typeparam name="T">指令类型</typeparam>
		void RegisterCommand<T>() where T : class, ICommand;

		/// <summary>
		/// 执行指令
		/// </summary>
		/// <param name="data">数据</param>
		void ExecuteCommand<T>(T data) where T : class, ICommand;

		/// <summary>
		/// 移除指令
		/// </summary>
		void RemoveCommand<T>() where T : class, ICommand;

		#endregion

		#region Mediator 中介器

		/// <summary>
		/// 注册中介器
		/// </summary>
		/// <param name="mediator">中介器对象</param>
		void RegisterMediator<T>(T mediator) where T : class, IMediator;

		/// <summary>
		/// 获取中介器
		/// </summary>
		/// <param name="mediatorTag">中介器标签</param>
		/// <typeparam name="T">中介器类型</typeparam>
		/// <returns>中介器对象</returns>
		T GetMediator<T>(string mediatorTag) where T : class, IMediator;

		/// <summary>
		/// 移除中介器
		/// </summary>
		/// <param name="mediatorTag">中介器标签</param>
		void RemoveMediator(string mediatorTag);

		#endregion

		#region Notice 消息通知

		/// <summary>
		/// 发送消息
		/// </summary>
		/// <param name="msg">数据</param>
		void SendNotify<TEvent>(TEvent msg) where TEvent : IEventMessage;

		#endregion

		/// <summary>
		/// 清理释放
		/// </summary>
		void ShutDown();
	}
}