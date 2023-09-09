using System;
using System.Collections.Generic;

using UnityEngine;

namespace KiwiFramework.Runtime
{
	/// <summary>
	/// 状态机
	/// </summary>
	public class StateMachine<T> 
	{
		/// <summary>
		/// 状态库
		/// </summary>
		private readonly Dictionary<T, BaseState<T>> _stateMap = new(32);

		/// <summary>
		/// 默认状态节点
		/// </summary>
		private BaseState<T> _defaultState;

		/// <summary>
		/// 当前状态节点
		/// </summary>
		public BaseState<T> CurState { get; private set; }

		/// <summary>
		/// 之前状态节点
		/// </summary>
		public BaseState<T> PreState { get; private set; }

		/// <summary>
		/// 是否正在运行
		/// </summary>
		public bool IsRunning { get; private set; }

		/// <summary>
		/// 状态机持有者
		/// </summary>
		public object Owner { get; private set; }

		/// <summary>
		/// 当前运行的节点名称
		/// </summary>
		public T CurrentStateType => CurState != null ? CurState.StateType : default;

		/// <summary>
		/// 之前运行的节点名称
		/// </summary>
		public T PreStateName => PreState != null ? PreState.StateType : default;

		public StateMachine() { }

		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="owner">状态机持有者</param>
		public StateMachine(object owner) => Owner = owner;

		/// <summary>
		/// 启动状态机
		/// </summary>
		public void Startup()
		{
			if (IsRunning) return;

			if (_defaultState == null)
			{
				Debug.LogError("启动失败 : 未设置默认状态");
				return;
			}

			CurState ??= _defaultState;

			IsRunning = true;

			CurState.OnEnter();
		}

		/// <summary>
		/// 停止状态机
		/// </summary>
		public void Stop()
		{
			if (!IsRunning) return;

			IsRunning = false;

			CurState?.OnExit();
		}

		/// <summary>
		/// 更新状态机
		/// </summary>
		public void Update()
		{
			if (!IsRunning) return;

			CurState?.OnUpdate();
		}

		public void LateUpdate()
		{
			if (!IsRunning) return;

			CurState?.OnLateUpdate();
		}

		public void FixedUpdate()
		{
			if (!IsRunning) return;

			CurState?.OnFixedUpdate();
		}

		/// <summary>
		/// 当前是否是指定状态
		/// </summary>
		/// <param name="stateType">状态类型</param>
		/// <returns></returns>
		public bool IsCurrentState(T stateType) => CurState.StateType.Equals(stateType);

		/// <summary>
		/// 设置默认状态
		/// </summary>
		/// <param name="stateType">要设置的状态类型</param>
		public void SetDefault(T stateType)
		{
			if (_stateMap.TryGetValue(stateType, out var state))
				_defaultState = state;
			else
				Debug.LogError($"设置默认状态失败:{stateType}在状态机中无法找到.");
		}

		/// <summary>
		/// 加入状态节点
		/// </summary>
		/// <param name="stateNode">要添加的状态节点</param>
		/// <exception cref="ArgumentNullException"></exception>
		public void Add(BaseState<T> stateNode)
		{
			if (stateNode == null)
				throw new ArgumentNullException();

			stateNode.machine = this;

			var stateType = stateNode.StateType;

			if (!_stateMap.ContainsKey(stateType))
			{
				_stateMap.Add(stateType, stateNode);
				_defaultState ??= stateNode;
			}
			else
			{
				Debug.LogError($"状态节点被添加过 : {stateType}");
			}
		}

		/// <summary>
		/// 转换状态节点
		/// </summary>
		/// <param name="stateType">状态类型</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public bool ChangeState(T stateType)
		{
			var nextState = TryGetState(stateType);
			if (nextState == null)
			{
				Debug.LogError($"无法找到状态 : {Enum.GetName(typeof(T), stateType)}");
				return false;
			}

			if (CurState.Transition(stateType))
			{
				Debug.Log($"{Enum.GetName(typeof(T), CurState.StateType)} --> {Enum.GetName(typeof(T), stateType)}");

				CurState.OnExit();
				PreState = CurState;
				CurState = nextState;
				CurState.OnEnter();

				return true;
			}

			return false;
		}

		/// <summary>
		/// 尝试获得状态节点
		/// </summary>
		/// <param name="stateType">状态类型</param>
		/// <returns></returns>
		private BaseState<T> TryGetState(T stateType)
		{
			_stateMap.TryGetValue(stateType, out var result);
			return result;
		}
	}
}