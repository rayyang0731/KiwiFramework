// using System.Text;
//
// using Sirenix.OdinInspector;
//
// using TMPro;
//
// using UnityEditor;
//
// using UnityEngine;
//
// namespace KiwiFramework.Runtime
// {
// 	/// <summary>
// 	/// 可多状态图片显示组件
// 	/// </summary>
// 	[AddComponentMenu("KiwiUI/Graphic/Label")]
// 	[RequireComponent(typeof(TextMeshProUGUI))]
// 	[ExecuteInEditMode, HideMonoScript]
// 	public partial class UILabel : UIMultiStateComponentBase<LabelStateData>, IGrayable
// 	{
// 		#region TextMeshPro 属性
//
// 		[SerializeField, HideInInspector] private TextMeshProUGUI _textMeshPro;
//
// 		/// <summary>
// 		/// TMP 组件对象
// 		/// </summary>
// 		public TextMeshProUGUI textMeshPro
// 		{
// 			get
// 			{
// 				if (_textMeshPro == null)
// 					_textMeshPro = GetComponent<TextMeshProUGUI>();
// 				return _textMeshPro;
// 			}
// 		}
//
// 		#endregion
//
// 		#region Text 属性
//
// 		private string _originText;
//
// 		/// <summary>
// 		/// TMP 组件对象
// 		/// </summary>
// 		public string text
// 		{
// 			get => _originText;
// 			set
// 			{
// 				if (textMeshPro == null)
// 					return;
//
// 				_originText      = value;
// 				textMeshPro.text = value;
// 			}
// 		}
//
// 		#endregion
//
// 		#region SpriteText 属性
//
// 		public string spriteText
// 		{
// 			get => _originText;
// 			set
// 			{
// 				_originText = value;
//
// 				if (textMeshPro == null)
// 					return;
// #if USE_ZSTRING
// 				using (zstring.Block())
// 				{
// 					zstring originVal = value;
// 					var     len = originVal.Length;
// 					zstring val = "";
//
// 					for (var i = 0; i < len; i++)
// 						val += zstring.Format("<sprite name=\"{0}\">", originVal.Substring(i, 1));
//
// 					textMeshPro.text = val.Intern();
// 				}
// #else
// 				var sb = new StringBuilder();
// 				for (var i = 0; i < value.Length; i++)
// 					sb.AppendFormat("<sprite name=\"{0}\">", value.Substring(i, 1));
//
// 				textMeshPro.text = sb.ToString();
// #endif
// 			}
// 		}
//
// 		#endregion
//
// 		#region Color 属性
//
// 		public Color color
// 		{
// 			get => textMeshPro == null ? Color.white : textMeshPro.color;
// 			set
// 			{
// 				if (textMeshPro == null) return;
//
// 				textMeshPro.color = value;
// 			}
// 		}
//
// 		#endregion
//
// 		#region 置灰属性
//
// 		/// <summary>
// 		/// 置灰之前的颜色
// 		/// </summary>
// 		private Color _lastColor;
//
// 		/// <summary>
// 		/// 置灰时的颜色
// 		/// </summary>
// 		private Color _grayColor;
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
// 		#endregion
//
// 		/// <summary>
// 		/// 修改状态
// 		/// </summary>
// 		/// <param name="targetState">状态索引</param>
// 		/// <param name="force">是否强制设置状态索引,可缺省,默认为false</param>
// 		/// <returns>修改状态是否成功</returns>
// 		public override bool SetState(int targetState, bool force = false)
// 		{
// 			if (!base.SetState(targetState, force))
// 				return false;
//
// 			CurrentState = targetState;
//
// 			var data = stateDataStore[targetState];
//
// 			if (data.use_i18n)
// 			{
// 				if (Application.isPlaying)
// 				{
// 					RefreshI18NText();
// 				}
// #if UNITY_EDITOR
// 				else
// 				{
// 					text = data.GetConfigValue(data.tableID);
// 					EditorUtility.SetDirty(gameObject);
// 				}
// #endif
// 			}
// 			else if (data.use_customText)
// 			{
// 				text = data.customText;
// #if UNITY_EDITOR
// 				if (!Application.isPlaying)
// 					EditorUtility.SetDirty(gameObject);
// #endif
// 			}
//
// 			if (data.overrideFont)
// 				textMeshPro.font = data.font;
//
// 			if (data.overrideColor)
// 				color = data.color;
//
// 			return true;
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
// 			if (!CanGray) return;
//
// 			if (isGray)
// 			{
// 				if (isGrayState) return;
//
// 				_lastColor   = color;
// 				_grayColor.r = _grayColor.g = _grayColor.b = color.grayscale;
// 				_grayColor.a = color.a;
// 				color        = _grayColor;
//
// 				isGrayState = true;
// 			}
// 			else
// 			{
// 				if (!isGrayState) return;
//
// 				color = _lastColor;
//
// 				isGrayState = false;
// 			}
// 		}
//
// 		/// <summary>
// 		/// 刷新本地化文字显示
// 		/// </summary>
// 		private void RefreshI18NText()
// 		{
// 			var curStateData = stateDataStore[CurrentState];
//
// 			if (I18NSettings.GetValue(curStateData.tableName, curStateData.tableID, out var val))
// 				text = val;
// 			else
// 				Debug.LogError($"获取本地化文字失败, 配置表名称 : {curStateData.tableName}, 配置表 Key : {curStateData.tableID}");
// 		}
//
// 		protected override void Awake()
// 		{
// 			base.Awake();
//
// 			_originText = text;
// 			_lastColor  = color;
// 		}
// 	}
//
// #if UNITY_EDITOR
//
// 	public partial class UILabel
// 	{
// 		/// <summary>
// 		/// 清理组件,删除 text 中的内容,并去除 SubMesh
// 		/// </summary>
// 		private void ClearTMP()
// 		{
// 			text = string.Empty;
//
// 			if (textMeshPro.canvasRenderer)
// 				textMeshPro.ClearMesh();
// 		}
//
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
// 			var data = new LabelStateData
// 			           {
// #if UNITY_EDITOR
// 				           name = $"State {stateDataStore.Count.ToString()}",
// #endif
// 				           use_i18n      = false,
// 				           tableName     = string.Empty,
// 				           tableID       = string.Empty,
// 				           overrideFont  = false,
// 				           font          = null,
// 				           overrideColor = false,
// 				           color         = Color.black
// 			           };
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
// 		[Button("置灰"), BoxGroup("Debug", CenterLabel = true, Order = 15)]
// 		private void Gray() { SetGray(true); }
//
// 		[Button("取消置灰"), BoxGroup("Debug")] private void UnGray() { SetGray(false); }
// 	}
//
// #endif
// }