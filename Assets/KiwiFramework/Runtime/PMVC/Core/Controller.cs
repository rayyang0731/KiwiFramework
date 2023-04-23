using System;
using System.Collections.Generic;

using UnityEngine;

namespace KiwiFramework.Runtime
{
	/// <summary>
	/// 控制层管理器
	/// </summary>
	public class Controller : Singleton<Controller>, IController
	{
		/// <summary>
		/// 指令字典
		/// </summary>
		private readonly Dictionary<Type, bool> _commandMap = new();

		public void RegisterCommand<T>() where T : class, ICommand
		{
			var type = typeof(T);
			if (!_commandMap.ContainsKey(type))
			{
				_commandMap.Add(type, true);
				Debug.LogFormat($"Register command: {type}");
			}
		}

		public void ExecuteCommand<T>(T data) where T : class, ICommand
		{
			var type = typeof(T);
			if (!_commandMap.ContainsKey(type))
				return;

			if (Activator.CreateInstance(type) is ICommand command)
			{
				command.Execute(data);
				Debug.LogFormat($"Execute command: {type}");
			}
		}

		public void RemoveCommand<T>() where T : class, ICommand
		{
			var type = typeof(T);
			if (_commandMap.ContainsKey(type))
			{
				_commandMap.Remove(type);
				Debug.LogFormat($"Remove command: {type}");
			}
		}

		public void RemoveAllCommand() => _commandMap.Clear();
	}
}