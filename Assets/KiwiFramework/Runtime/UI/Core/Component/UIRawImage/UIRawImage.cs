// using Cysharp.Threading.Tasks;
//
// using KiwiFramework.Runtime;
//
// using Sirenix.OdinInspector;
//
// using UnityEngine;
// using UnityEngine.UI;
//
// using UIEffectHelper = KiwiFramework.Runtime.UI.UIEffectHelper;
//
// namespace KiwiFramework.UI
// {
// 	[HideMonoScript]
// 	[RequireComponent(typeof(RawImage))]
// 	[AddComponentMenu("Kiwi/UI/UIRawImage")]
// 	public partial class UIRawImage : UIMultiStateElement<RawImageStateData>, IGray
// 	{
// 		#region RawImage 属性
//
// 		[SerializeField, HideInInspector]
// 		private RawImage _native;
//
// 		public RawImage Native
// 		{
// 			get
// 			{
// 				if (_native == null)
// 					_native = GetComponent<RawImage>();
// 				return _native;
// 			}
// 		}
//
// 		#endregion
//
// 		#region Texture 属性
//
// 		public Texture texture
// 		{
// 			get => Native == null ? null : Native.texture;
// 			set
// 			{
// 				if (Native == null) return;
//
// 				if (Native.texture != null && Application.isPlaying)
// 					SpriteManager.Instance.ReleaseSprite(Native.texture.name);
//
// 				Native.texture = value;
// 			}
// 		}
//
// 		#endregion
//
// 		#region Color 属性
//
// 		public Color color
// 		{
// 			get => Native == null ? Color.white : Native.color;
// 			set
// 			{
// 				if (Native == null) return;
//
// 				Native.color = value;
// 			}
// 		}
//
// 		#endregion
//
// 		/// <summary>
// 		/// 修改状态
// 		/// </summary>
// 		/// <param name="targetState">状态索引</param>
// 		/// <param name="force">是否强制设置状态索引,可缺省,默认为false</param>
// 		/// <returns>修改状态是否成功</returns>
// 		public override async UniTaskVoid SetState(int targetState, bool force = false)
// 		{
// 			if (!force && CurrentState == targetState)
// 				return;
//
// 			if (targetState < 0 || targetState >= stateDataStore.Count)
// 				return;
//
// 			CurrentState = targetState;
//
// 			var data = stateDataStore[targetState];
//
// 			if (data.overrideColor)
// 				color = data.color;
//
// 			if (!string.IsNullOrEmpty(data.textureName))
// 			{
// 				if (Application.isPlaying)
// 				{
// 					texture = await KiwiAssets.LoadAsync<Texture>(data.textureName);
// 				}
// 				else
// 				{
// #if UNITY_EDITOR
// 					texture = KiwiAssets.Editor.LoadByGUID<Texture>(data.textureGUID);
// 					UnityEditor.EditorUtility.SetDirty(gameObject);
// #endif
// 				}
// 			}
// 			else
// 			{
// 				texture = null;
// #if UNITY_EDITOR
// 				UnityEditor.EditorUtility.SetDirty(gameObject);
// #endif
// 			}
//
// 			if (data.autoUseNativeSize)
// 			{
// 				Native.SetNativeSize();
// 			}
// 			else if (data.overrideSize)
// 			{
// 				rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, data.size.x);
// 				rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, data.size.y);
// 			}
// 		}
//
// 		[SerializeField]
// 		[ShowInInspector, LabelText("是否可以置灰"), BoxGroup("状态属性", CenterLabel = true)]
// 		private bool _canGray = true;
//
// 		/// <summary>
// 		/// 是否可以置灰
// 		/// </summary>
// 		public bool CanGray
// 		{
// 			get => _canGray;
// 			set => _canGray = value;
// 		}
//
// 		/// <summary>
// 		/// 当前是否是置灰状态
// 		/// </summary>
// 		public bool isGrayState { get; private set; }
//
// 		/// <summary>
// 		/// 设置置灰状态
// 		/// </summary>
// 		/// <param name="isGray">是否置灰</param>
// 		public void SetGray(bool isGray)
// 		{
// 			if (!CanGray || isGray == isGrayState) return;
//
// 			UIEffectHelper.SetGray(Native, isGray);
// 			isGrayState = isGray;
// 		}
//
// 		protected override void OnDestroyed()
// 		{
// 			if (Application.isPlaying)
// 				KiwiAssets.Unload(texture);
//
// 			stateDataStore.Clear();
// 		}
// 	}
//
// #if UNITY_EDITOR
// 	public partial class UIRawImage
// 	{
// 		/// <summary>
// 		/// 当前全部状态的名字
// 		/// </summary>
// 		protected override string[] StateNames
// 		{
// 			get
// 			{
// 				var values = new string[stateDataStore.Count + 1];
// 				values[0] = "无";
// 				for (var i = 1; i < values.Length; i++)
// 					values[i] = $"{(i - 1).ToString()} : {stateDataStore[i - 1].name}";
//
// 				return values;
// 			}
// 		}
//
// 		protected override void OnStateDataAddElement()
// 		{
// 			if (stateDataStore.Count >= CONST_MAX_STATE_COUNT)
// 				return;
// 			var data = RawImageStateData.Default;
//
// 			stateDataStore.Add(data);
// 		}
//
// 		protected override void Reset()
// 		{
// 			base.Reset();
//
// 			stateDataStore.Clear();
// 		}
//
// 		[Button("置灰"),
// 		 BoxGroup("Debug", CenterLabel = true, Order = 15)]
// 		private void Gray() { SetGray(true); }
//
// 		[Button("取消置灰"),
// 		 BoxGroup("Debug")]
// 		private void UnGray() { SetGray(false); }
// 	}
// #endif
// }
