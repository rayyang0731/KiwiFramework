using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.UI;

namespace KiwiFramework.Runtime.UI
{
	/// <summary>
	/// 可多状态图片显示组件
	/// </summary>
	[AddComponentMenu("KiwiUI/UIImage"), RequireComponent(typeof(RichImage)), HideMonoScript]
	public partial class UIImage : UIMultiStateElement<ImageStateData>, IGray
	{
		#region Image 属性

		[SerializeField, HideInInspector] private RichImage native;

		public RichImage Native
		{
			get
			{
				if (native == null)
					native = GetComponent<RichImage>();
				return native;
			}
		}

		#endregion

		#region Sprite 属性

		public Sprite sprite
		{
			get => Native == null ? null : Native.sprite;
			set
			{
				if (Native == null) return;

				if (Native.sprite != null && Application.isPlaying)
					AssetLoader.Unload(Native.sprite);

				Native.sprite = value;
			}
		}

		#endregion

		#region Color 属性

		public Color color
		{
			get => Native == null ? Color.white : Native.color;
			set
			{
				if (Native == null) return;

				Native.color = value;
			}
		}

		#endregion

		/// <summary>
		/// 修改状态
		/// </summary>
		/// <param name="targetState">状态索引</param>
		/// <param name="force">是否强制设置状态索引,可缺省,默认为false</param>
		/// <returns>修改状态是否成功</returns>
		public override async void SetState(int targetState, bool force = false)
		{
			if (!force && CurrentState == targetState)
				return;

			if (targetState < 0 || targetState >= stateDataStore.Count)
				return;

			CurrentState = targetState;

			var data = stateDataStore[targetState];

			if (data.overrideColor)
				color = data.color;

			if (!string.IsNullOrEmpty(data.spriteName))
			{
				if (Application.isPlaying)
				{
					sprite = await AssetLoader.LoadAsync<Sprite>(data.spriteName);
				}
				else
				{
#if UNITY_EDITOR
					sprite = AssetLoader.Editor.LoadByGUID<Sprite>(data.spriteGUID);
					UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
				}
			}
			else
			{
				sprite = null;
#if UNITY_EDITOR
				UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
			}

			if (data.autoUseNativeSize && Native.type is Image.Type.Simple or Image.Type.Filled)
			{
				Native.SetNativeSize();
			}
			else if (data.overrideSize)
			{
				rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, data.size.x);
				rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, data.size.y);
			}
		}

		[SerializeField]
		[ShowInInspector, LabelText("是否可以置灰"), BoxGroup("状态属性", CenterLabel = true)]
		private bool _canGray = true;

		/// <summary>
		/// 是否可以置灰
		/// </summary>
		public bool CanGray
		{
			get => _canGray;
			set => _canGray = value;
		}

		/// <summary>
		/// 当前是否是置灰状态
		/// </summary>
		public bool isGrayState { get; private set; }

		/// <summary>
		/// 设置置灰状态
		/// </summary>
		/// <param name="isGray">是否置灰</param>
		public void SetGray(bool isGray)
		{
			if (!CanGray || isGray == isGrayState) return;

			UIEffectHelper.SetGray(Native, isGray);
			isGrayState = isGray;
		}

		/// <summary>
		/// 设置图片类型
		/// </summary>
		/// <param name="imageType">要设置的图片类型</param>
		public void SetImageType(ImageType imageType)
		{
			if (Native.ImageType != imageType)
				Native.ImageType = imageType;
		}

		/// <summary>
		/// 设置翻转类型
		/// </summary>
		/// <param name="flipType">要设置的翻转类型</param>
		public void SetFlip(FlipType flipType)
		{
			if (Native.ImageType != ImageType.FLIP)
				return;

			Native.FlipType = flipType;
		}

		/// <summary>
		/// 设置镜像类型
		/// </summary>
		/// <param name="mirrorType">要设置的镜像类型</param>
		public void SetMirror(MirrorType mirrorType)
		{
			if (Native.ImageType != ImageType.MIRROR)
				return;

			Native.MirrorType = mirrorType;
		}

		protected override void OnDestroyed()
		{
			if (Application.isPlaying)
			{
				AssetLoader.Unload(sprite);
			}

			stateDataStore.Clear();
		}
	}

#if UNITY_EDITOR
	public partial class UIImage
	{
		/// <summary>
		/// 当前全部状态的名字
		/// </summary>
		protected override string[] StateNames
		{
			get
			{
				var values = new string[stateDataStore.Count + 1];
				values[0] = "无";
				for (var i = 1; i < values.Length; i++)
					values[i] = $"{(i - 1).ToString()} : {stateDataStore[i - 1].name}";

				return values;
			}
		}

		protected override void OnStateDataAddElement()
		{
			if (stateDataStore.Count >= CONST_MAX_STATE_COUNT)
				return;
			var data = ImageStateData.Default;

			stateDataStore.Add(data);
		}

		protected override void Reset()
		{
			base.Reset();

			stateDataStore.Clear();
		}

		[Button("置灰"), BoxGroup("Debug", CenterLabel = true, Order = 15)]
		private void Gray() { SetGray(true); }

		[Button("取消置灰"), BoxGroup("Debug")] private void UnGray() { SetGray(false); }
	}
#endif
}