// using System;
// using System.Linq;
//
// using Sirenix.OdinInspector;
//
// using UnityEngine;
//
// namespace KiwiFramework.Runtime
// {
//     /// <summary>
// 	/// UI音效类(点击音效控制)
// 	/// </summary>
// 	[Serializable]
// 	public class UISoundHelper
// 	{
// 		/// <summary>
// 		/// 音效描述
// 		/// </summary>
// 		[Serializable]
// 		private struct SoundDesc
// 		{
// 			/// <summary>
// 			/// 点击类型
// 			/// </summary>
// 			[LabelText("点击类型")]
// 			public POINTER_TYPE pointerType;
//
// 			/// <summary>
// 			/// 可用时音效资源名称
// 			/// </summary>
// 			[LabelText("可用音效")]
// 			public string enableSfxPath;
//
// 			/// <summary>
// 			/// 不可用时音效资源名称
// 			/// </summary>
// 			[LabelText("不可用音效")]
// 			public string disableSfxPath;
//
// 			/// <summary>
// 			/// 获得音效路径
// 			/// </summary>
// 			/// <param name="enable">当前可用状态</param>
// 			/// <returns></returns>
// 			public string GetSoundPath(bool enable) { return enable ? enableSfxPath : disableSfxPath; }
//
// 			public SoundDesc(POINTER_TYPE pointerType, string enableSfxPath, string disableSfxPath)
// 			{
// 				this.pointerType    = pointerType;
// 				this.enableSfxPath  = enableSfxPath;
// 				this.disableSfxPath = disableSfxPath;
// 			}
// 		}
//
// 		/// <summary>
// 		/// 是否静音
// 		/// </summary>
// 		[ShowInInspector, ReadOnly, DisplayAsString, LabelText("是否静音"), SuffixLabel("static")]
// 		public static bool Mute;
//
// #if UNITY_EDITOR
// 		[ShowInInspector, LabelText("点击类型"), FoldoutGroup("Add", GroupName = "添加音效数据")]
// 		private POINTER_TYPE _pointerType = POINTER_TYPE.DOWN;
//
// 		[ShowInInspector, LabelText("可用音效"), FoldoutGroup("Add")]
// 		private string _enableSoundPath = string.Empty;
//
// 		[ShowInInspector, LabelText("不可用音效"), FoldoutGroup("Add")]
// 		private string _disableSoundPath = string.Empty;
//
// 		[Button(Name = "@dataExist?\"该类型已存在\":\"添加\""), FoldoutGroup("Add"), DisableIf("dataExist", true)]
// 		private void AddSound()
// 		{
// 			if (dataExist)
// 				return;
//
// 			var newData = new SoundDesc[sounds.Length + 1];
// 			sounds.CopyTo(newData, 0);
// 			newData[sounds.Length] = new SoundDesc(_pointerType, _enableSoundPath, _disableSoundPath);
// 			sounds                 = newData;
// 		}
//
// 		private bool dataExist => sounds.Any(data => data.pointerType == _pointerType);
// #endif
//
// 		/// <summary>
// 		/// 各点击类型对应音效资源名称
// 		/// </summary>
// 		[SerializeField]
// 		[LabelText("音效数据描述"), ListDrawerSettings(HideAddButton = true)]
// 		private SoundDesc[] sounds = { };
//
// 		private bool TryGet(POINTER_TYPE type, out string name)
// 		{
// 			name = string.Empty;
//
// 			if (sounds.Length <= 0) return false;
//
// 			foreach (var data in sounds)
// 			{
// 				if (data.pointerType != type) continue;
// 				name = data.enableSfxPath;
// 				return true;
// 			}
//
// 			return false;
// 		}
//
// 		/// <summary>
// 		/// 播放声音
// 		/// </summary>
// 		public void Play(POINTER_TYPE type)
// 		{
// 			if (Mute)
// 				return;
//
// 			if (TryGet(type, out var path))
// 			{
// 				Debug.Log($"Play Sound : {type} | {path}");
// 			}
// 		}
// 	}
// }