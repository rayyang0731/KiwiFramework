namespace KiwiFramework.Runtime.Utility
{
	/// <summary>
	/// 码表工具
	/// </summary>
	public class Stopwatch
	{
		private readonly System.Diagnostics.Stopwatch _stopwatch;

		/// <summary>
		/// 用时(单位:毫秒)
		/// </summary>
		public long ElapseMilliseconds => _stopwatch.ElapsedMilliseconds;

		public Stopwatch() { _stopwatch = System.Diagnostics.Stopwatch.StartNew(); }

		/// <summary>
		/// 刷新
		/// </summary>
		public void Refresh() { _stopwatch?.Restart(); }

		/// <summary>
		/// 停止
		/// </summary>
		public void Stop() { _stopwatch.Stop(); }
	}
}