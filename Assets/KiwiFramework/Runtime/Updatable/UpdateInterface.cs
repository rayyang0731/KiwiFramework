namespace KiwiFramework.Runtime
{
	public interface IFixedUpdate
	{
		int FixedUpdateOrder { get; }
		void OnFixedUpdate(float fixedDeltaTime);
	}

	public interface IUpdate
	{
		int UpdateOrder { get; }
		void OnUpdate(float deltaTime);
	}

	public interface ILateUpdate
	{
		int LateUpdateOrder { get; }
		void OnLateUpdate(float deltaTime);
	}
}
