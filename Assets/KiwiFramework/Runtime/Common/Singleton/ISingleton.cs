using System;

namespace KiwiFramework.Runtime
{
	/// <summary>
	/// 单例接口
	/// </summary>
	public interface ISingleton : IDisposable
	{
		/// <summary>
		/// 单例是否可以被重置
		/// </summary>
		bool canResetSingleton { get; }

		/// <summary>
		/// 单例执行初始化之前
		/// </summary>
		void SingletonPreInit();

		/// <summary>
		/// 单例初始化
		/// </summary>
		void SingletonInit();

		/// <summary>
		/// 单例重置
		/// </summary>
		void SingletonReset();

		/// <summary>
		/// 单例释放
		/// </summary>
		void SingletonRelease();
	}
}