namespace KiwiFramework.Runtime
{
	/// <summary>
	/// 状态节点基类
	/// </summary>
	public abstract class BaseState<T>
	{
		/// <summary>
		/// 所属状态机
		/// </summary>
		protected internal StateMachine<T> machine { get; internal set; }

		/// <summary>
		/// 状态类型
		/// </summary>
		public T StateType { get; }

		public BaseState(T stateType) { StateType = stateType; }

		/// <summary>
		/// 进入状态
		/// </summary>
		public virtual void OnEnter() { }

		/// <summary>
		/// 状态更新
		/// </summary>
		public virtual void OnUpdate() { }

		/// <summary>
		/// 当状态执行 LateUpdate
		/// </summary>
		public virtual void OnLateUpdate() { }

		/// <summary>
		/// 当状态执行 FixedUpdate
		/// </summary>
		public virtual void OnFixedUpdate() { }

		/// <summary>
		/// 退出状态
		/// </summary>
		public virtual void OnExit() { }

		/// <summary>
		/// 跳转条件
		/// </summary>
		/// <param name="stateType">状态类型</param>
		/// <returns>是否允许跳转状态</returns>
		public virtual bool Transition(T stateType) => true;
	}
}