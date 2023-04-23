using System.Collections;

namespace KiwiFramework.Runtime
{
	/// <summary>
	/// 多重指令
	/// </summary>
	/// <example>
	///	<code>
	///	public class XXXCommand : MacroCommand
	/// {
	/// 	protected override IEnumerator SubCommands()
	/// 	{
	/// 		yield return new Command1();
	/// 		yield return new Command2();
	/// 	}
	/// }
	/// </code>
	/// </example>
	public abstract class MacroCommand : Command
	{
		public override void Execute<TEvent>(TEvent msg)
		{
			var commands = SubCommands();

			while (commands.MoveNext())
			{
				if (commands.Current is ICommand current)
				{
					current.Execute(msg);
				}
			}
		}

		protected abstract IEnumerator SubCommands();
	}
}