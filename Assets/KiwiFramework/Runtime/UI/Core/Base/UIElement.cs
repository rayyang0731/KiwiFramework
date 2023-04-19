using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.EventSystems;

using KiwiFramework.Runtime.UnityExtend;

namespace KiwiFramework.Runtime.UI
{
	/// <summary>
	/// UI元素基类
	/// </summary>
	[HideMonoScript]
	[RequireComponent(typeof(RectTransform))]
	public abstract partial class UIElement : UIBehaviour
	{
		#region GameObject 属性

		[SerializeField, HideInInspector] private GameObject _gameObject;

		[ShowInInspector, BoxGroup("base", LabelText = "基础属性", CenterLabel = true, Order = int.MinValue)]
		public new GameObject gameObject => _gameObject ? _gameObject : _gameObject = base.gameObject;

		#endregion

		#region RectTransform 属性

		[SerializeField, HideInInspector] private RectTransform _rectTransform;

		[ShowInInspector, BoxGroup("base")] public RectTransform rectTransform => _rectTransform ? _rectTransform : _rectTransform = this.rectTransform();

		#endregion

		/// <summary>
		/// 是否为代码中要调用的对象
		/// </summary>
		[SerializeField, HideInInspector]
		private bool _callInCode = false;

		/// <summary>
		/// 是否为代码中要调用的对象
		/// </summary>
		[ShowInInspector, LabelText("代码中调用"), BoxGroup("base"), DisableIf(nameof(ForceCallInCode))]
		public bool CallInCode
		{
			get
			{
				if (ForceCallInCode) _callInCode = true;
				return _callInCode;
			}
#if UNITY_EDITOR
			set => _callInCode = ForceCallInCode || value;
#endif
		}

		/// <summary>
		/// 是否强制为代码中要调用的对象
		/// </summary>
		public virtual bool ForceCallInCode => false;

		protected sealed override void Awake() { OnAwake(); }
		protected sealed override void Start() => OnStart();
		protected sealed override void OnDestroy() => OnDestroyed();

		protected virtual void OnAwake() { }
		protected virtual void OnStart() { }
		protected virtual void OnDestroyed() { }


		/// <summary>
		/// 添加子对象
		/// </summary>
		/// <param name="element"></param>
		public virtual void AddElement(UIElement element) { }

		/// <summary>
		/// 移除子对象
		/// </summary>
		/// <param name="element"></param>
		public virtual void RemoveElement(UIElement element) { }

		/// <summary>
		/// 获取子对象
		/// </summary>
		/// <param name="elementName"></param>
		/// <returns></returns>
		public virtual UIElement GetElement(string elementName) => null;
	}

#if UNITY_EDITOR
	public partial class UIElement
	{
		/// <summary>
		/// 在代码中使用的字段名
		/// </summary>
		[SerializeField, HideInInspector]
		private string fieldNameInCode = string.Empty;

		/// <summary>
		/// 在代码中使用的字段名
		/// </summary>
		[ShowInInspector, LabelText("字段名"), HorizontalGroup("base/fieldName", VisibleIf = nameof(CallInCode)),
		 DisableIf(nameof(hadFieldNameLocked)), Indent]
		protected internal string FieldNameInCode
		{
			get => string.IsNullOrEmpty(fieldNameInCode) ? gameObject.name : fieldNameInCode;
			set
			{
				if (!hadFieldNameLocked)
					fieldNameInCode = value;
			}
		}

		/// <summary>
		/// 是否锁定字段名
		/// </summary>
		[SerializeField, HideInInspector]
		private bool hadFieldNameLocked = false;

		/// <summary>
		/// 锁定字段名
		/// </summary>
		[HorizontalGroup("base/fieldName", Width = 20), ShowIf("@_lockFieldName == false"),
		 Button(SdfIconType.LockFill, Name = ""), GUIColor(1, 0, 0)]
		private void LockFieldName() { hadFieldNameLocked = true; }

		/// <summary>
		/// 解锁字段名
		/// </summary>
		[HorizontalGroup("base/fieldName", Width = 20), ShowIf("@_lockFieldName == true"),
		 Button(SdfIconType.UnlockFill, Name = ""), GUIColor(0, 1, 1)]
		private void UnlockFieldName() { hadFieldNameLocked = false; }

		/// <summary>
		/// 复制对象在代码中使用的字段名到剪贴板
		/// </summary>
		[HorizontalGroup("base/fieldName", Width = 50), Button(Name = "复制")]
		private void CopyFieldName()
		{
			// 复制到剪贴板
			GUIUtility.systemCopyBuffer = FieldNameInCode;
		}

		/// <summary>
		/// 检查有效性
		/// </summary>
		protected internal virtual bool CheckValidate(ref string error) { return true; }

		protected override void Reset()
		{
			_rectTransform = this.rectTransform();
			_gameObject    = gameObject;
		}

		/// <summary>
		/// 当类被加载或编辑器面板有值被修改调用
		/// </summary>
		protected override void OnValidate()
		{
			var error = string.Empty;
			if (!CheckValidate(ref error))
				Debug.LogError(error);
		}
	}
#endif
}