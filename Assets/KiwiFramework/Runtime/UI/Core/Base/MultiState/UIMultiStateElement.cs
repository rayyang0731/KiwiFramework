// using System;
// using System.Collections.Generic;
//
// using Cysharp.Threading.Tasks;
//
// using KiwiFramework.Runtime;
// using KiwiFramework.Runtime.UI;
//
// using Sirenix.OdinInspector;
//
// #if UNITY_EDITOR
// using Sirenix.Utilities.Editor;
//
// using UnityEditor;
// #endif
//
// using UnityEngine;
//
// namespace KiwiFramework.UI
// {
// 	/// <summary>
// 	/// 拥有多状态的 UIComponent 基类
// 	/// </summary>
// 	public abstract partial class UIMultiStateElement<T> : UIElement, IMultiState where T : class
// 	{
// 		#region Private Variables
//
// 		/// <summary>
// 		/// 默认状态
// 		/// </summary>
// 		[SerializeField, HideInInspector]
// 		protected int defaultState = -1;
//
// 		/// <summary>
// 		/// 当前状态
// 		/// </summary>
// 		private int _currentState;
//
// 		/// <summary>
// 		/// 全部状态数据
// 		/// </summary>
// 		[SerializeField]
// 		[LabelText("状态数据"), PropertySpace]
// 		[ListDrawerSettings(
// 			 ShowIndexLabels = true,
// 			 ListElementLabelName = "name",
// 			 OnTitleBarGUI = nameof(OnStateDataTitleBarGUI),
// 			 OnBeginListElementGUI = nameof(OnStateDataBeginListElementGUI),
// 			 OnEndListElementGUI = nameof(OnStateDataEndListElementGUI),
// 			 CustomRemoveElementFunction = nameof(OnStateDataRemoveElement),
// 			 CustomAddFunction = nameof(OnStateDataAddElement)), PropertyOrder(3)]
// 		protected List<T> stateDataStore = new();
//
// 		#endregion
//
// 		#region Public Properties
//
// 		/// <summary>
// 		/// 默认状态,如果为-1,则为无默认状态
// 		/// </summary>
// 		public int DefaultState => defaultState;
//
// 		[SerializeField]
// 		[ShowInInspector, LabelText("是否受到父物体状态影响"), BoxGroup("状态属性", CenterLabel = true), PropertyOrder(4)]
// 		private bool affectedByParent = true;
//
// 		/// <summary>
// 		/// 本地状态是否受到父物体状态影响
// 		/// </summary>
// 		public bool AffectedByParent
// 		{
// 			get => affectedByParent;
// 			set => affectedByParent = value;
// 		}
//
// 		/// <summary>
// 		/// 当前状态
// 		/// </summary>
// 		[ShowInInspector]
// 		[HideInEditorMode, LabelText("当前状态"), PropertyOrder(9), DisableInPlayMode, DisplayAsString]
// 		public int CurrentState
// 		{
// 			get => _currentState;
// 			protected set => _currentState = Mathf.Clamp(value, -1, stateDataStore.Count);
// 		}
//
// 		/// <summary>
// 		/// 状态数量
// 		/// </summary>
// 		public int StateCount => stateDataStore?.Count ?? 0;
//
// 		#endregion
//
// 		#region Public Methods
//
// 		/// <summary>
// 		/// 添加状态
// 		/// </summary>
// 		/// <param name="data">要添加/插入的状态数据</param>
// 		/// <param name="insetIndex">新状态要插入的索引,可缺省,默认为-1,状态添加到末尾</param>
// 		public virtual void AddState(object data, int insetIndex = -1)
// 		{
// 			if (data is T stateData)
// 			{
// 				if (insetIndex < 0 || insetIndex >= stateDataStore.Count)
// 					stateDataStore.Add(stateData);
// 				else
// 					stateDataStore.Insert(insetIndex, stateData);
// 				return;
// 			}
//
// 			Debug.LogException(new ArgumentException($"参数类型错误,必须为{typeof(T).Name}类型", nameof(data)));
// 		}
//
// 		/// <summary>
// 		/// 移除状态
// 		/// </summary>
// 		/// <param name="index">状态索引，可缺省，缺省时移除最后一个状态</param>
// 		/// <returns>移除状态是否成功</returns>
// 		public virtual bool RemoveState(int index = -1)
// 		{
// 			if (index > stateDataStore.Count - 1)
// 				return false;
//
// 			if (index < 0)
// 				index = stateDataStore.Count - 1;
//
// 			stateDataStore.RemoveAt(index);
// 			return true;
// 		}
//
// 		/// <summary>
// 		/// 修改状态
// 		/// </summary>
// 		/// <param name="targetState">状态索引</param>
// 		/// <param name="force">是否强制设置状态索引,可缺省,默认为false</param>
// 		/// <returns>修改状态是否成功</returns>
// 		public abstract UniTaskVoid SetState(int targetState, bool force = false);
//
// 		/// <summary>
// 		/// 获得状态
// 		/// </summary>
// 		/// <param name="index">目标状态索引值</param>
// 		/// <param name="data">返回状态数据</param>
// 		/// <returns>如果获取成功返回 true,否则返回 false</returns>
// 		public virtual bool GetState(int index, out T data)
// 		{
// 			if (stateDataStore.Count == 0)
// 			{
// 				data = null;
// 				return false;
// 			}
//
// 			if (index < 0 || index >= stateDataStore.Count)
// 			{
// 				data = null;
// 				Debug.LogException(new ArgumentOutOfRangeException(nameof(index), "获取状态时,索引越界"));
// 				return false;
// 			}
//
// 			data = stateDataStore[index];
// 			return true;
// 		}
//
// 		/// <summary>
// 		/// 修改状态数据
// 		/// </summary>
// 		/// <param name="index">目标状态索引值</param>
// 		/// <param name="data">要修改的状态数据</param>
// 		/// <returns>修改数据状态是否成功</returns>
// 		public virtual void ChangeStateData(int index, object data)
// 		{
// 			if (data is T stateData)
// 			{
// 				if (stateDataStore.Count == 0 || index < 0 || index >= stateDataStore.Count)
// 				{
// 					Debug.LogException(new ArgumentOutOfRangeException(nameof(index), "修改状态数据时,索引越界"));
// 					return;
// 				}
//
// 				stateDataStore[index] = stateData;
// 				return;
// 			}
//
// 			return;
// 		}
//
// 		/// <summary>
// 		/// 清除全部状态
// 		/// </summary>
// 		/// <returns>清除状态是否成功</returns>
// 		public virtual void ClearState()
// 		{
// 			if (stateDataStore == null)
// 			{
// 				Debug.LogError("状态数据列表为 null.");
// 				return;
// 			}
//
// 			if (stateDataStore.Count == 0)
// 				return;
//
// 			stateDataStore.Clear();
//
// 			return;
// 		}
//
// 		/// <summary>
// 		/// 重置为默认状态
// 		/// </summary>
// 		/// <returns>重置状态是否成功</returns>
// 		public virtual void ResetState()
// 		{
// 			if (defaultState < 0 || stateDataStore == null || stateDataStore.Count == 0)
// 			{
// 				CurrentState = defaultState;
// 				return;
// 			}
//
// 			SetState(defaultState, true);
// 		}
//
// 		#endregion
//
// 		#region Override Methods
//
// 		protected override void OnAwake()
// 		{
// 			base.OnAwake();
//
// 			ResetState();
// 		}
//
// 		#endregion
// 	}
//
// #if UNITY_EDITOR
// 	public abstract partial class UIMultiStateElement<T>
// 	{
// 		/// <summary>
// 		/// 最小状态数量
// 		/// </summary>
// 		protected const int CONST_MIN_STATE_COUNT = 0;
//
// 		/// <summary>
// 		/// 最大状态数量
// 		/// </summary>
// 		protected const int CONST_MAX_STATE_COUNT = 10;
//
// 		/// <summary>
// 		/// 所有状态的名称
// 		/// </summary>
// 		protected virtual string[] StateNames
// 		{
// 			get
// 			{
// 				return new[]
// 				       {
// 					       "无"
// 				       };
// 			}
// 		}
//
// 		protected virtual void OnStateDataTitleBarGUI()
// 		{
// 			if (defaultState >= StateNames.Length)
// 				defaultState = -1;
//
// 			EditorGUILayout.BeginHorizontal();
// 			{
// 				SirenixEditorFields.SegmentedProgressBarField(stateDataStore.Count, CONST_MIN_STATE_COUNT, CONST_MAX_STATE_COUNT);
//
// 				EditorGUILayout.LabelField("默认状态", GUILayout.Width(50));
//
// 				var targetState = EditorGUILayout.Popup(defaultState + 1, StateNames, GUILayout.Width(80)) - 1;
//
// 				if (defaultState != targetState)
// 				{
// 					defaultState = targetState;
// 					EditorUtility.SetDirty(this);
// 				}
//
// 				if (defaultState < 0)
// 					CurrentState = -1;
// 			}
// 			EditorGUILayout.EndHorizontal();
// 		}
//
// 		protected virtual void OnStateDataBeginListElementGUI(int index)
// 		{
// 			if (Application.isPlaying && CurrentState == index)
// 				GUIHelper.PushColor(Color.cyan);
// 			else
// 				GUIHelper.PushColor(Color.white);
//
// 			var rect = GUIHelper.GetCurrentLayoutRect();
// 			rect.x    =  rect.x + rect.width - 40;
// 			rect.y    += 6;
// 			rect.size =  Vector2.one * 16;
// 			if (SirenixEditorGUI.IconButton(rect, EditorIcons.MagnifyingGlass, "预览状态效果"))
// 				SetState(index, true);
//
// 			SirenixEditorGUI.BeginIndentedVertical(SirenixGUIStyles.BoxContainer);
// 		}
//
// 		protected virtual void OnStateDataEndListElementGUI(int index)
// 		{
// 			SirenixEditorGUI.EndIndentedVertical();
//
// 			GUIHelper.PopColor();
// 		}
//
// 		protected virtual void OnStateDataRemoveElement(T data)
// 		{
// 			if (stateDataStore.Count <= CONST_MIN_STATE_COUNT)
// 				return;
// 			stateDataStore.Remove(data);
// 		}
//
// 		protected virtual void OnStateDataAddElement() { }
//
// 		protected internal override bool CheckValidate(ref string msg)
// 		{
// 			if (DefaultState < -1 || DefaultState >= stateDataStore.Count || StateNames.Length <= DefaultState)
// 			{
// 				msg = $"{nameof(DefaultState)} 对于状态库的数量来说越界了 : {DefaultState}";
// 				return false;
// 			}
//
// 			return true;
// 		}
//
// 		protected override void Reset()
// 		{
// 			base.Reset();
//
// 			ResetState();
// 		}
// 	}
// #endif
// }
