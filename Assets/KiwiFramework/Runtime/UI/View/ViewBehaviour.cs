// using System.Collections.Generic;
//
// using Sirenix.OdinInspector;
//
// using UnityEngine;
// using UnityEngine.EventSystems;
//
// namespace KiwiFramework.Runtime.UI
// {
// 	[RequireComponent(typeof(UIView)), DisallowMultipleComponent]
// 	public abstract partial class ViewBehaviour : UIBehaviour, ITick, ILateTick, IFixedTick
// 	{
// 		#region 内部字段
//
// 		[ShowInInspector, BoxGroup("内部字段", CenterLabel = true), ReadOnly, HideIf("@parentView == null")]
// 		[LabelText("父界面")]
// 		internal ViewBehaviour parentView = null;
//
// 		[ShowInInspector, BoxGroup("内部字段"), ReadOnly, HideIf("@ChildViews.Count <= 0")]
// 		[LabelText("子界面")]
// 		protected internal readonly Dictionary<string, ViewBehaviour> ChildViews = new();
//
// 		/// <summary>
// 		/// 界面容器组件
// 		/// </summary>
// 		internal UIView uiview = null;
//
// 		/// <summary>
// 		/// 状态机
// 		/// </summary>
// 		private StateMachine<UIStateType> _stateMachine;
//
// 		/// <summary>
// 		/// 界面名称
// 		/// </summary>
// 		internal string ViewName;
//
// 		#endregion
//
// 		#region Update LateUpdate FixedUpdate
//
// 		public virtual bool runInBackground { get; }
//
// 		public bool ExecuteUpdate
// 		{
// 			get => uiview.ExecuteUpdate;
// 			set
// 			{
// 				uiview.ExecuteUpdate = value;
// 				if (value)
// 					UIManager.Instance.AddUpdate(this);
// 				else
// 					UIManager.Instance.RemoveUpdate(this);
// 			}
// 		}
//
// 		public virtual int TickOrder
// 		{
// 			get => uiview.UpdateOrder;
// 			set => uiview.UpdateOrder = value;
// 		}
//
// 		public bool ExecuteLateUpdate
// 		{
// 			get => uiview.ExecuteLateUpdate;
// 			set
// 			{
// 				uiview.ExecuteLateUpdate = value;
// 				if (value)
// 					UIManager.Instance.AddLateUpdate(this);
// 				else
// 					UIManager.Instance.RemoveLateUpdate(this);
// 			}
// 		}
//
// 		public virtual int LateTickOrder
// 		{
// 			get => uiview.LateUpdateOrder;
// 			set => uiview.LateUpdateOrder = value;
// 		}
//
// 		public bool ExecuteFixedUpdate
// 		{
// 			get => uiview.ExecuteFixedUpdate;
// 			set
// 			{
// 				uiview.ExecuteFixedUpdate = value;
// 				if (value)
// 					UIManager.Instance.AddFixedUpdate(this);
// 				else
// 					UIManager.Instance.AddFixedUpdate(this);
// 			}
// 		}
//
// 		public virtual int FixedTickOrder
// 		{
// 			get => uiview.FixedUpdateOrder;
// 			set => uiview.FixedUpdateOrder = value;
// 		}
//
// 		#endregion
//
// 		#region View Public Methods
//
// 		/// <summary>
// 		/// 打开其他界面
// 		/// <para>用于打开其他界面,把此界面压入回退栈中</para>
// 		/// </summary>
// 		/// <param name="viewName">要打开的界面名称</param>
// 		public void OpenOtherView(string viewName) { UIManager.Instance.OpenView(viewName); }
//
// 		/// <summary>
// 		/// 打开子界面
// 		/// <para>打开一个指定名称的界面作为此界面的子界面</para>
// 		/// <para>当调用此界面的 OnViewResume,OnViewShow,OnViewHide 方法时,同时调用子界面的这些方法</para>
// 		/// <para>在关闭此界面时,会先关闭子界面,再关闭自己</para>
// 		/// </summary>
// 		/// <param name="viewName">要打开的界面名称</param>
// 		public void OpenChildView(string viewName)
// 		{
// 			var childView = UIManager.Instance.OpenView(viewName);
// 			ChildViews.Add(viewName, childView);
// 		}
//
// 		/// <summary>
// 		/// 剥离子界面
// 		/// </summary>
// 		/// <param name="viewName">要剥离的界面名称</param>
// 		public void DetachChildView(string viewName)
// 		{
// 			if (ChildViews.ContainsKey(viewName))
// 				ChildViews.Remove(viewName);
// 		}
//
// 		/// <summary>
// 		/// 显示界面(拉起界面)
// 		/// </summary>
// 		internal void Show() { _stateMachine.ChangeState(UIStateType.Resume); }
//
// 		/// <summary>
// 		/// 隐藏界面(挂起界面)
// 		/// </summary>
// 		internal void Hide() { _stateMachine.ChangeState(UIStateType.Pause); }
//
// 		/// <summary>
// 		/// 关闭界面
// 		/// </summary>
// 		internal void Close() { _stateMachine.ChangeState(UIStateType.Close); }
//
// 		#endregion
//
// 		#region View Lifecycle Methods
//
// 		void ITick.OnTick(float deltaTime) { _stateMachine.Update(); }
//
// 		void ILateTick.OnLateTick(float deltaTime) { _stateMachine.LateUpdate(); }
//
// 		void IFixedTick.OnFixedTick(float fixedDeltaTime) { _stateMachine.FixedUpdate(); }
//
// 		#endregion
//
// 		#region Unity Methods
//
// 		protected override void Awake()
// 		{
// 			// uiview = GetComponent<ViewDefine>();
//
// 			_stateMachine = new StateMachine<UIStateType>(this);
// 			_stateMachine.Add(new UIStateCreate(this));
// 			_stateMachine.Add(new UIStateOpen(this));
// 			_stateMachine.Add(new UIStateResume(this));
// 			_stateMachine.Add(new UIStateRunning(this));
//
// 			if (ExecuteUpdate)
// 				UIManager.Instance.AddUpdate(this);
// 			if (ExecuteLateUpdate)
// 				UIManager.Instance.AddLateUpdate(this);
// 			if (ExecuteFixedUpdate)
// 				UIManager.Instance.AddFixedUpdate(this);
// 		}
//
// 		protected override void OnDestroy()
// 		{
// 			// if (ModuleSystem.Contains<UIManager>())
// 			{
// 				if (ExecuteUpdate)
// 					UIManager.Instance.RemoveUpdate(this);
// 				if (ExecuteLateUpdate)
// 					UIManager.Instance.RemoveLateUpdate(this);
// 				if (ExecuteFixedUpdate)
// 					UIManager.Instance.RemoveFixedUpdate(this);
// 			}
//
// 			OnViewDestroyed();
// 		}
//
// 		#endregion
//
// 		#region Command
//
// 		/// <summary>
// 		/// 注册指令监听
// 		/// </summary>
// 		internal virtual void RegisterCommands() { }
//
// 		/// <summary>
// 		/// 注销指令监听
// 		/// </summary>
// 		internal virtual void UnregisterCommands() { }
//
// 		#endregion
//
// 		#region Elements
//
// 		private readonly List<string> _elements = new();
//
// 		// /// <summary>
// 		// /// 创建 Element
// 		// /// </summary>
// 		// /// <param name="elementTag">Element 标签</param>
// 		// /// <typeparam name="TElement">Element 类型</typeparam>
// 		// /// <returns>被创建的 Element 对象</returns>
// 		// internal TElement CreateElement<TElement>(string elementTag) where TElement : Element, new()
// 		// {
// 		// 	var element = new TElement();
// 		// 	element.SetName(elementTag);
// 		//
// 		// 	return element;
// 		// }
// 		//
// 		// /// <summary>
// 		// /// 添加 Element
// 		// /// </summary>
// 		// internal void AddElement<TElement>(TElement element) where TElement : Element, new()
// 		// {
// 		// 	// if (AppFacade.Instance.IsExistMediator(element))
// 		// 	{
// 		// 		Debug.Log($"[{element.Name}] Element 已经存在,返回已经存在的 Element.");
// 		// 		return;
// 		// 	}
// 		//
// 		// 	// AppFacade.Instance.RegisterMediator(element);
// 		// 	// _elements.Add(element.Name);
// 		// }
// 		//
// 		// /// <summary>
// 		// /// 移除全部 Element
// 		// /// </summary>
// 		// internal void RemoveAllElements()
// 		// {
// 		// 	foreach (var element in _elements)
// 		// 	{
// 		// 		// AppFacade.Instance.RemoveMediator(element);
// 		// 	}
// 		//
// 		// 	_elements.Clear();
// 		// }
//
// 		/// <summary>
// 		/// 注册 Elements
// 		/// </summary>
// 		protected internal virtual void RegisterElements() { }
//
// 		#endregion
//
// 		#region View Lifecycle Methods
//
// 		/// <summary>
// 		/// 当界面被创建
// 		/// <para>相当于MonoBehaviour.Awake</para>
// 		/// <para>只操作数据,不操作界面中的对象</para>
// 		/// </summary>
// 		internal virtual void OnViewCreated() { }
//
// 		/// <summary>
// 		/// 当界面实例化完成
// 		/// <para>相当于 MonoBehaviour.Start</para>
// 		/// </summary>
// 		protected internal virtual void OnViewOpened() { }
//
// 		/// <summary>
// 		/// 界面Update
// 		/// <para>相当于 MonoBehaviour.Update</para>
// 		/// </summary>
// 		protected internal virtual void OnViewUpdate(float deltaTime) { }
//
// 		/// <summary>
// 		/// 界面 LateUpdate
// 		/// <para>相当于 MonoBehaviour.LateUpdate</para>
// 		/// </summary>
// 		protected internal virtual void OnViewLateUpdate(float deltaTime) { }
//
// 		/// <summary>
// 		/// 界面 FixedUpdate
// 		/// <para>相当于 MonoBehaviour.FixedUpdate</para>
// 		/// </summary>
// 		protected internal virtual void OnViewFixedUpdate(float fixedDeltaTime) { }
//
// 		/// <summary>
// 		/// 当界面关闭
// 		/// </summary>
// 		protected internal virtual void OnViewClosed() { }
//
// 		/// <summary>
// 		/// 当界面被删除
// 		/// 相当于 MonoBehaviour.OnDestroy
// 		/// </summary>
// 		protected virtual void OnViewDestroyed() { }
//
// 		/// <summary>
// 		/// 当界面被拉起
// 		/// <para>当从其他界面回到本界面或再次打开本界面时调用</para>
// 		/// </summary>
// 		protected internal virtual void OnViewResume() { }
//
// 		/// <summary>
// 		/// 当界面被挂起
// 		/// </summary>
// 		protected internal virtual void OnViewPause() { }
//
// 		/// <summary>
// 		/// 当打开动画播放完成
// 		/// </summary>
// 		public void OnOpenAnimationFinished() { Show(); }
//
// 		/// <summary>
// 		/// 当关闭动画播放完成
// 		/// </summary>
// 		public void OnCloseAnimationFinished() { UIManager.Instance.InternalCloseView(ViewName); }
//
// 		#endregion
// 	}
//
// #if UNITY_EDITOR
// 	public partial class ViewBehaviour
// 	{
// 		// protected override void Reset() { uiview = GetComponent<ViewDefine>(); }
// 	}
// #endif
// }