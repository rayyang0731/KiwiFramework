namespace KiwiFramework.Runtime
{
	public class Facade : IFacade
	{
		/// <summary>
		/// 启动
		/// </summary>
		public void Startup() { OnStartup(); }

		#region Proxy

		/// <summary>
		/// 注册代理
		/// </summary>
		/// <param name="proxy">代理对象</param>
		public void RegisterProxy<TProxy>(TProxy proxy) where TProxy : class, IProxy
			=> Model.Instance.RegisterProxy(proxy);

		/// <summary>
		/// 尝试获取代理
		/// </summary>
		/// <param name="proxy">代理对象</param>
		/// <typeparam name="TProxy">代理类型</typeparam>
		/// <returns>是否成功获取代理</returns>
		public bool TryGetProxy<TProxy>(out TProxy proxy) where TProxy : class, IProxy
			=> Model.Instance.TryGetProxy(out proxy);

		/// <summary>
		/// 获取代理
		/// </summary>
		/// <typeparam name="TProxy">代理类型</typeparam>
		/// <returns>代理对象</returns>
		public TProxy GetProxy<TProxy>() where TProxy : class, IProxy
			=> Model.Instance.GetProxy<TProxy>();

		/// <summary>
		/// 移除代理
		/// </summary>
		/// <typeparam name="TProxy">代理类型</typeparam>
		public void RemoveProxy<TProxy>() where TProxy : class, IProxy
			=> Model.Instance.RemoveProxy<TProxy>();

		#endregion

		#region Command

		/// <summary>
		/// 注册指令
		/// </summary>
		/// <typeparam name="TCommand">指令类型</typeparam>
		public void RegisterCommand<TCommand>() where TCommand : class, ICommand
			=> Controller.Instance.RegisterCommand<TCommand>();

		/// <summary>
		/// 执行指令
		/// </summary>
		/// <param name="data">数据</param>
		public void ExecuteCommand<TCommand>(TCommand data) where TCommand : class, ICommand
			=> Controller.Instance.ExecuteCommand(data);

		/// <summary>
		/// 移除指令
		/// </summary>
		public void RemoveCommand<TCommand>() where TCommand : class, ICommand
			=> Controller.Instance.RemoveCommand<TCommand>();

		#endregion

		#region Mediator

		/// <summary>
		/// 注册中介器
		/// </summary>
		/// <param name="mediator">中介器对象</param>
		public void RegisterMediator<TMediator>(TMediator mediator) where TMediator : class, IMediator
			=> View.Instance.RegisterMediator(mediator);

		/// <summary>
		/// 获取中介器
		/// </summary>
		/// <param name="mediatorTag">中介器标签</param>
		/// <returns></returns>
		public TMediator GetMediator<TMediator>(string mediatorTag) where TMediator : class, IMediator
			=> View.Instance.GetMediator(mediatorTag) as TMediator;

		/// <summary>
		/// 中介器是否已经被注册过
		/// </summary>
		/// <param name="mediatorTag">中介器标签</param>
		/// <returns>中介器对象是否已经被注册</returns>
		public bool IsExistMediator(string mediatorTag)
			=> View.Instance.IsExist(mediatorTag);

		/// <summary>
		/// 移除中介器
		/// </summary>
		/// <param name="mediatorTag">中介器标签</param>
		public void RemoveMediator(string mediatorTag)
			=> View.Instance.RemoveMediator(mediatorTag);

		#endregion

		#region Notice

		/// <summary>
		/// 发送通知
		/// </summary>
		public void SendNotify<TEvent>(TEvent msg) where TEvent : IEventMessage
			=> EventSystem.SendMessage(msg);

		#endregion

		/// <summary>
		/// 关闭
		/// </summary>
		public void ShutDown()
		{
			OnShutDown();

			Model.Instance.RemoveAllProxy();
			Controller.Instance.RemoveAllCommand();
			View.Instance.RemoveAllMediator();

			EventSystem.ClearAll();
		}

		protected virtual void OnStartup() { }

		protected virtual void OnShutDown() { }
	}
}