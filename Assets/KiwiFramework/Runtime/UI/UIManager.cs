// using System.Collections.Generic;
// using System.Linq;
//
// using Cysharp.Threading.Tasks;
//
// using KiwiFramework.Runtime.UI.Layer;
//
// using UnityEngine;
//
// using Object = UnityEngine.Object;
//
// namespace KiwiFramework.Runtime.UI
// {
// 	public sealed class UIManager : Singleton<UIManager>
// 	{
// 		protected override bool useDriver => true;
//
// 		/// <summary>
// 		/// UI 主 Canvas
// 		/// </summary>
// 		public RootCanvas rootCanvas;
//
// 		private readonly List<ITick> _updateStore = new();
//
// 		private readonly List<ILateTick> _lateUpdateStore = new();
//
// 		private readonly List<IFixedTick> _fixedUpdateStore = new();
//
// 		/// <summary>
// 		/// 全部界面(key:界面名称 value:界面对象)
// 		/// </summary>
// 		private readonly Dictionary<string, ViewBehaviour> _allViewMap = new();
//
// 		/// <summary>
// 		/// 界面回退栈
// 		/// </summary>
// 		private readonly Stack<string> _stackRollback = new();
//
// 		/// <summary>
// 		/// 底层 Canvas
// 		/// </summary>
// 		private Canvas _normalCanvas;
//
// 		private CanvasGroup _normalCanvasGroup;
//
// 		/// <summary>
// 		/// 上层 Canvas
// 		/// </summary>
// 		private Canvas _forwardCanvas;
//
// 		private CanvasGroup _forwardCanvasGroup;
//
// 		/// <summary>
// 		/// 顶层 Canvas
// 		/// </summary>
// 		private Canvas _topCanvas;
//
// 		private CanvasGroup _topCanvasGroup;
//
// 		/// <summary>
// 		/// Max Canvas
// 		/// </summary>
// 		private Canvas _maxCanvas;
//
// 		/// <summary>
// 		/// Max CanvasGroup
// 		/// </summary>
// 		private CanvasGroup _maxCanvasGroup;
//
// 		/// <summary>
// 		/// 驱动器实体
// 		/// </summary>
// 		private GameObject _driver;
//
// 		protected override async void OnSingletonInit()
// 		{
// 			await InitRootCanvasAndLayer();
//
// 			Debug.Log($"{nameof(UIManager)} 初始化完成!");
// 		}
//
// 		protected override void OnSingletonReset() { ClearAll(); }
//
// 		protected override void OnSingletonRelease() { ClearAll(); }
//
// 		/// <summary>
// 		/// 初始化主 Canvas
// 		/// </summary>
// 		private async UniTask InitRootCanvasAndLayer()
// 		{
// 			if (rootCanvas != null && !rootCanvas.Equals(null)) return;
//
// 			var mainCanvasGo = await AssetLoader.assetHelper.InstantiateAsync<GameObject>("RootCanvas");
// 			if (mainCanvasGo != null)
// 			{
// 				Object.DontDestroyOnLoad(mainCanvasGo);
// 				rootCanvas = mainCanvasGo.GetComponent<RootCanvas>();
// 			}
// 		}
//
// 		/// <summary>
// 		/// 尝试获得 UI 子 Canvas
// 		/// </summary>
// 		private void TryToGetSubCanvas()
// 		{
// 			if (rootCanvas == null || rootCanvas.Equals(null))
// 				return;
//
// 			if (_normalCanvas == null || _normalCanvas.Equals(null))
// 			{
// 				_normalCanvas              = rootCanvas.transform.Find("Normal").GetComponent<Canvas>();
// 				_normalCanvasGroup         = _normalCanvas.GetComponent<CanvasGroup>();
// 				_normalCanvas.sortingOrder = NormalCanvasSortOrder;
// 			}
//
// 			if (_forwardCanvas == null || _forwardCanvas.Equals(null))
// 			{
// 				_forwardCanvas              = rootCanvas.transform.Find("Forward").GetComponent<Canvas>();
// 				_forwardCanvasGroup         = _forwardCanvas.GetComponent<CanvasGroup>();
// 				_forwardCanvas.sortingOrder = ForwardCanvasSortOrder;
// 			}
//
// 			if (_topCanvas == null || _topCanvas.Equals(null))
// 			{
// 				_topCanvas              = rootCanvas.transform.Find("Top").GetComponent<Canvas>();
// 				_topCanvasGroup         = _topCanvas.GetComponent<CanvasGroup>();
// 				_topCanvas.sortingOrder = TopCanvasSortOrder;
// 			}
//
// 			if (_maxCanvas == null || _maxCanvas.Equals(null))
// 			{
// 				_maxCanvas              = rootCanvas.transform.Find("Max").GetComponent<Canvas>();
// 				_maxCanvasGroup         = _maxCanvas.GetComponent<CanvasGroup>();
// 				_maxCanvas.sortingOrder = MaxCanvasSortOrder;
// 			}
// 		}
//
// 		/// <summary>
// 		/// 尝试压入回退栈
// 		/// </summary>
// 		/// <param name="viewName">界面名称</param>
// 		private void TryToPushInRollbackStack(string viewName)
// 		{
// 			var lastViewName = string.Empty;
// 			if (_stackRollback.Count > 0)
// 			{
// 				lastViewName = _stackRollback.Peek();
// 				if (lastViewName == viewName) return;
// 			}
//
// 			if (!string.IsNullOrEmpty(lastViewName))
// 				CloseView(lastViewName);
//
// 			_stackRollback.Push(viewName);
// 		}
//
// 		/// <summary>
// 		/// 尝试回退到栈顶的界面
// 		/// </summary>
// 		/// <param name="viewName">当前被关闭的界面名称</param>
// 		private void TryToPopUpRollbackStack(string viewName)
// 		{
// 			var view          = _allViewMap[viewName];
// 			var stackViewName = string.Empty;
// 			if (_stackRollback.Count > 0)
// 			{
// 				stackViewName = _stackRollback.Peek();
// 				if (stackViewName != viewName)
// 				{
// 					var wannaRollBackViewName = _stackRollback.Pop();
// 					if (wannaRollBackViewName != null)
// 						OpenView(wannaRollBackViewName);
// 				}
// 				else
// 				{
// 					view.Show();
// 				}
// 			}
//
// 			if (stackViewName != viewName)
// 				InternalCloseView(viewName);
// 		}
//
// 		/// <summary>
// 		/// 关闭界面
// 		/// </summary>
// 		/// <param name="viewName">要关闭的界面名称</param>
// 		internal void InternalCloseView(string viewName)
// 		{
// 			var view = _allViewMap[viewName];
// 			_allViewMap.Remove(viewName);
// 			Object.Destroy(view.gameObject);
// 		}
//
// 		/// <summary>
// 		/// 控制子 Canvas 的显隐,如果 Canvas 下没有界面,则关闭子 Canvas 的渲染和响应,反之则打开
// 		/// </summary>
// 		private void ControlSubCanvasDisplay()
// 		{
// 			ControlSubCanvasDisplay(_normalCanvasGroup, _normalCanvas.transform.childCount > 0);
// 			ControlSubCanvasDisplay(_forwardCanvasGroup, _forwardCanvas.transform.childCount > 0);
// 			ControlSubCanvasDisplay(_topCanvasGroup, _topCanvas.transform.childCount > 0);
// 		}
//
// 		private void ControlSubCanvasDisplay(CanvasGroup canvasGroup, bool display)
// 		{
// 			var alpha = display ? 1 : 0;
// 			if (System.Math.Abs(alpha - canvasGroup.alpha) > 0.00001f)
// 				canvasGroup.alpha = alpha;
//
// 			if (display != canvasGroup.interactable)
// 				canvasGroup.interactable = display;
//
// 			if (display != canvasGroup.blocksRaycasts)
// 				canvasGroup.blocksRaycasts = display;
// 		}
//
// 		internal void AddUpdate(ITick update)
// 		{
// 			if (_updateStore.Contains(update)) return;
// 			_updateStore.Add(update);
// 			if (update.TickOrder > 0)
// 				_updateStore.Sort((x, y) => x.TickOrder.CompareTo(y.TickOrder));
// 		}
//
// 		internal void RemoveUpdate(ITick update)
// 		{
// 			if (_updateStore.Contains(update))
// 				_updateStore.Remove(update);
// 		}
//
// 		internal void AddLateUpdate(ILateTick lateUpdate)
// 		{
// 			if (_lateUpdateStore.Contains(lateUpdate)) return;
// 			_lateUpdateStore.Add(lateUpdate);
// 			if (lateUpdate.LateTickOrder > 0)
// 				_lateUpdateStore.Sort((x, y) => x.LateTickOrder.CompareTo(y.LateTickOrder));
// 		}
//
// 		internal void RemoveLateUpdate(ILateTick lateUpdate)
// 		{
// 			if (_lateUpdateStore.Contains(lateUpdate))
// 				_lateUpdateStore.Remove(lateUpdate);
// 		}
//
// 		internal void AddFixedUpdate(IFixedTick fixedTick)
// 		{
// 			if (_fixedUpdateStore.Contains(fixedTick)) return;
// 			_fixedUpdateStore.Add(fixedTick);
// 			if (fixedTick.FixedTickOrder > 0)
// 				_fixedUpdateStore.Sort((x, y) => x.FixedTickOrder.CompareTo(y.FixedTickOrder));
// 		}
//
// 		internal void RemoveFixedUpdate(IFixedTick fixedTick)
// 		{
// 			if (_fixedUpdateStore.Contains(fixedTick))
// 				_fixedUpdateStore.Remove(fixedTick);
// 		}
//
// 		/// <summary>
// 		/// 根据层级排序值获得层级 Canvas
// 		/// </summary>
// 		/// <param name="sortLayer">界面层级排序值</param>
// 		/// <returns></returns>
// 		public Canvas GetLayerCanvas(int sortLayer)
// 		{
// 			return sortLayer switch
// 			       {
// 				       >= MaxCanvasSortOrder     => _maxCanvas,
// 				       >= TopCanvasSortOrder     => _topCanvas,
// 				       >= ForwardCanvasSortOrder => _forwardCanvas,
// 				       _                         => _normalCanvas
// 			       };
// 		}
//
// 		/// <summary>
// 		/// 是否存在此实例
// 		/// </summary>
// 		/// <param name="obj">要判断的更新对象</param>
// 		/// <returns></returns>
// 		public bool ExistTick<T>(T obj)
// 		{
// 			switch (obj)
// 			{
// 				case ITick update:
// 					return _updateStore.Contains(update);
// 				case ILateTick lateUpdate:
// 					return _lateUpdateStore.Contains(lateUpdate);
// 				case IFixedTick fixedUpdate:
// 					return _fixedUpdateStore.Contains(fixedUpdate);
// 				default:
// 					return false;
// 			}
// 		}
//
// 		/// <summary>
// 		/// 打开界面
// 		/// </summary>
// 		/// <param name="viewAlias">界面别名</param>
// 		/// <typeparam name="T"></typeparam>
// 		/// <returns></returns>
// 		public T OpenView<T>(string viewAlias = null) where T : ViewBehaviour { return OpenView(typeof(T).Name, viewAlias) as T; }
//
// 		/// <summary>
// 		/// 打开界面
// 		/// </summary>
// 		/// <param name="viewName">界面名称</param>
// 		/// <param name="viewAlias">界面别名</param>
// 		/// <returns>被打开的界面对象</returns>
// 		public ViewBehaviour OpenView(string viewName, string viewAlias = null)
// 		{
// 			if (viewAlias == null)
// 				viewAlias = viewName;
//
// 			if (_allViewMap.TryGetValue(viewAlias, out var view))
// 			{
// 				view.Show();
// 				return view;
// 			}
//
// 			var viewGo = AssetManager.Instance.Load(GameAssetType.UI, viewAlias) as GameObject;
//
// 			if (viewGo == null)
// 			{
// 				Debug.LogError($"加载界面失败 : {viewAlias}");
// 				return null;
// 			}
//
// 			var viewDefine = viewGo.GetComponent<ViewDefine>();
// 			var viewRT     = viewGo.transform;
//
// 			viewRT.SetParent(GetLayerCanvas(viewDefine.SortLayer).rectTransform());
// 			viewRT.SetLocalPosition(0, 0, 0);
// 			viewRT.SetLocalScale(1, 1, 1);
// 			viewRT.SetAsLastSibling();
//
// 			viewGo.name   = viewAlias;
// 			view          = viewGo.ForceGetComponent(viewName) as ViewBehaviour;
// 			view.ViewName = viewAlias;
//
// 			_allViewMap.Add(viewAlias, view);
//
// 			if (view.uiview.ViewLogicType != VIEW_LOGIC_TYPE.ROLLBACK) return view;
//
// 			TryToPushInRollbackStack(viewAlias);
//
// 			ControlSubCanvasDisplay();
//
// 			return view;
// 		}
//
// 		/// <summary>
// 		/// 获取界面
// 		/// </summary>
// 		/// <param name="viewName">界面名称或者界面别名</param>
// 		/// <returns>如果获取不到,则返回 null</returns>
// 		public ViewBehaviour GetView(string viewName) { return _allViewMap.TryGetValue(viewName, out var view) ? view : null; }
//
// 		/// <summary>
// 		/// 获取界面
// 		/// </summary>
// 		/// <param name="viewAlias">界面别名</param>
// 		/// <returns>如果获取不到,则返回 null</returns>
// 		public T GetView<T>(string viewAlias = null) where T : ViewBehaviour { return GetView(string.IsNullOrEmpty(viewAlias) ? typeof(T).Name : viewAlias) as T; }
//
// 		/// <summary>
// 		/// 关闭界面
// 		/// </summary>
// 		/// <param name="viewName">界面名称或界面别名</param>
// 		public void CloseView(string viewName)
// 		{
// 			if (!_allViewMap.ContainsKey(viewName))
// 			{
// 				Debug.LogError($"[{viewName}]界面并没有被打开过,无法执行关闭操作");
// 				return;
// 			}
//
// 			var view          = _allViewMap[viewName];
// 			var viewLogicType = view.uiview.ViewLogicType;
//
// 			if (viewLogicType == VIEW_LOGIC_TYPE.ROLLBACK)
// 				TryToPopUpRollbackStack(viewName);
// 			else
// 				view.Close();
//
// 			ControlSubCanvasDisplay();
// 		}
//
// 		/// <summary>
// 		/// 关闭界面
// 		/// </summary>
// 		/// <param name="viewAlias">界面别名</param>
// 		/// <typeparam name="T"></typeparam>
// 		public void CloseView<T>(string viewAlias = null) where T : ViewBehaviour { CloseView(string.IsNullOrEmpty(viewAlias) ? typeof(T).Name : viewAlias); }
//
// 		private void Update()
// 		{
// 			if (_updateStore.Count == 0) return;
//
// 			foreach (var obj in _updateStore.Where(obj => obj != null))
// 			{
// 				var view = obj as ViewBehaviour;
// 				if (view != null && !view.IsDestroyed())
// 				{
// 					if (obj.runInBackground || view.isActiveAndEnabled)
// 					{
// 						obj.OnTick(Time.deltaTime);
// 					}
// 				}
// 			}
// 		}
//
// 		private void LateUpdate()
// 		{
// 			if (_lateUpdateStore.Count == 0) return;
//
// 			foreach (var obj in _lateUpdateStore.Where(obj => obj != null))
// 			{
// 				var view = obj as ViewBehaviour;
// 				if (view != null && !view.IsDestroyed())
// 				{
// 					if (obj.runInBackground || view.isActiveAndEnabled)
// 					{
// 						obj.OnLateTick(Time.deltaTime);
// 					}
// 				}
// 			}
// 		}
//
// 		private void FixedUpdate()
// 		{
// 			if (_fixedUpdateStore.Count == 0) return;
//
// 			foreach (var obj in _fixedUpdateStore.Where(obj => obj != null))
// 			{
// 				var view = obj as ViewBehaviour;
// 				if (view != null && !view.IsDestroyed())
// 				{
// 					if (obj.runInBackground || view.isActiveAndEnabled)
// 					{
// 						obj.OnFixedTick(Time.fixedDeltaTime);
// 					}
// 				}
// 			}
// 		}
//
// 		private void ClearAll()
// 		{
// 			_updateStore.Clear();
// 			_lateUpdateStore.Clear();
// 			_fixedUpdateStore.Clear();
//
// 			_allViewMap.Clear();
//
// 			_stackRollback.Clear();
//
// 			Object.Destroy(rootCanvas.gameObject);
//
// 			rootCanvas = null;
//
// 			_normalCanvas  = null;
// 			_forwardCanvas = null;
// 			_topCanvas     = null;
//
// 			_normalCanvasGroup  = null;
// 			_forwardCanvasGroup = null;
// 			_topCanvasGroup     = null;
// 		}
// 	}
// }