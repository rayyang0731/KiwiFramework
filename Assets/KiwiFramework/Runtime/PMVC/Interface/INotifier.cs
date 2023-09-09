namespace KiwiFramework.Runtime
{
    /// <summary>
    /// 消息发送器接口
    /// </summary>
    public interface INotifier
    {
        /// <summary>
        /// 发送消息
        /// </summary>
        void SendNotify(IEventMessage msg);
    }
}