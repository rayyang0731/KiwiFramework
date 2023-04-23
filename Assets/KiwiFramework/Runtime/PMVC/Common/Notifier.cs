namespace KiwiFramework.Runtime
{
	/// <summary>
	/// 消息通知器
	/// </summary>
	public abstract class Notifier : INotifier
	{
		public void SendNotify(IEventMessage msg) => EventSystem.SendMessage(msg);
	}
}