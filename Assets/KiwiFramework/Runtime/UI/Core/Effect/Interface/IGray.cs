namespace KiwiFramework.Runtime.UI
{
    /// <summary>
    /// 可灰态接口
    /// </summary>
    public interface IGray
    {
        bool isGrayState { get; }
        void SetGray(bool isGray);
    }
}