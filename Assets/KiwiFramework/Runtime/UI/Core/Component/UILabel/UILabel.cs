using Sirenix.OdinInspector;

using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace KiwiFramework.Runtime.UI
{
	/// <summary>
	/// 可多状态图片显示组件
	/// </summary>
	[AddComponentMenu("KiwiUI/UILabel")]
	[RequireComponent(typeof(TextMeshProUGUI))]
	[HideMonoScript]
	public partial class UILabel : UIMultiStateElement<LabelStateData>, IGray
	{
		#region TextMeshPro 属性

		[SerializeField, HideInInspector] private TextMeshProUGUI native;

		/// <summary>
		/// TMP 组件对象
		/// </summary>
		public TextMeshProUGUI Native
		{
			get
			{
				if (native == null)
					native = GetComponent<TextMeshProUGUI>();
				return native;
			}
		}

		#endregion

		#region Text 属性

		private string _originText;

		/// <summary>
		/// TMP 组件对象
		/// </summary>
		public string text
		{
			get => _originText;
			set
			{
				if (Native == null)
					return;

				_originText = value;
				Native.text = value;
			}
		}

		#endregion

		#region SpriteText 属性

		/// <summary>
		/// 是否只是短时使用(例如HUD的飘字,存在时间短)
		/// </summary>
		[SerializeField, LabelText("优化图片字"), BoxGroup("图片字", CenterLabel = true, Order = 1),
		 PropertyTooltip("借助 zstring 优化文本, 降低 GC, 但是开启优化后, 借助缓存, 文本文字只在短时间使用, 如果要持续显示文字,请不要勾选优化.")]
		public bool spriteTextFlash = true;

		[ShowInInspector, ReadOnly, LabelText("图片字内容"), BoxGroup("图片字")]
		public string spriteText
		{
			get => _originText;
			set
			{
				_originText = value;

				if (Native == null)
					return;

				using (zstring.Block())
				{
					zstring originVal = value;
					var     len       = originVal.Length;
					zstring val       = "";

					for (var i = 0; i < len; i++)
						val += zstring.Format("<sprite name=\"{0}\">", originVal.Substring(i, 1));

					Native.text = spriteTextFlash ? val : val.Intern();
				}
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

		#region 置灰属性

		/// <summary>
		/// 置灰之前的颜色
		/// </summary>
		private Color _lastColor;

		/// <summary>
		/// 置灰时的颜色
		/// </summary>
		private Color _grayColor;

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

		#endregion

		/// <summary>
		/// 修改状态
		/// </summary>
		/// <param name="targetState">状态索引</param>
		/// <param name="force">是否强制设置状态索引,可缺省,默认为false</param>
		/// <returns>修改状态是否成功</returns>
		public override void SetState(int targetState, bool force = false)
		{
			if (!force && CurrentState == targetState)
				return;

			if (targetState < 0 || targetState >= stateDataStore.Count)
				return;

			CurrentState = targetState;

			var data = stateDataStore[targetState];

			if (data.use_i18n)
			{
				if (Application.isPlaying)
				{
					RefreshI18NText();
				}

				else
				{
#if UNITY_EDITOR
					text = I18NSettings.GetValue(data.tableID);
					EditorUtility.SetDirty(gameObject);
#endif
				}
			}
			else if (data.use_customText)
			{
				text = data.customText;
#if UNITY_EDITOR
				if (!Application.isPlaying)
					EditorUtility.SetDirty(gameObject);
#endif
			}

			if (data.overrideFont)
				Native.font = data.font;

			if (data.overrideColor)
				color = data.color;
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
			if (!CanGray) return;

			if (isGray)
			{
				if (isGrayState) return;

				_lastColor   = color;
				_grayColor.r = _grayColor.g = _grayColor.b = color.grayscale;
				_grayColor.a = color.a;
				color        = _grayColor;

				isGrayState = true;
			}
			else
			{
				if (!isGrayState) return;

				color = _lastColor;

				isGrayState = false;
			}
		}

		/// <summary>
		/// 刷新本地化文字显示
		/// </summary>
		private void RefreshI18NText()
		{
			var curStateData = stateDataStore[CurrentState];

			if (curStateData.use_i18n)
				text = I18NSettings.GetValue(curStateData.tableID);
		}

		/// <summary>
		/// 重置为默认状态
		/// </summary>
		/// <returns>重置状态是否成功</returns>
		public override void ResetState()
		{
			base.ResetState();

			_originText = text;
			_lastColor  = color;
		}
	}

#if UNITY_EDITOR

	public partial class UILabel
	{
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
			var data = LabelStateData.Default;
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