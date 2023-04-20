using System;

using Sirenix.OdinInspector;

namespace KiwiFramework.Runtime
{
	public static class I18NSettings
	{
#if UNITY_EDITOR
		/// <summary>
		/// 获取指定多语言表中的全部 Key
		/// </summary>
		/// <returns></returns>
		public static ValueDropdownList<string> GetKey2Values()
		{
			var list = new ValueDropdownList<string>();

			foreach (var interfaceText in GameConfig.tables.InterfaceText.DataList)
			{
				list.Add(interfaceText.Key, interfaceText.Key);
				list.Add(interfaceText.Text, interfaceText.Key);
			}

			return list;
		}
#endif

		/// <summary>
		/// 切换语言
		/// </summary>
		/// <param name="translator">翻译程序</param>
		///	<example>
		///	<code>
		///	// 用于切换到英文
		/// string TextMapper_en(string key, string originText) 
		/// {
		/// 	return tables.I18N.GetOrDefault(key)?.En ?? originText;
		/// }
		/// 
		/// // 用于切换到中文
		/// string TextMapper_cn(string key, string originText) 
		/// {
		/// 	return tables.I18N.GetOrDefault(key)?.ZhCn ?? originText;
		/// }
		/// </code>
		/// </example>
		public static void SwitchLanguages(Func<string, string, string> translator) { GameConfig.tables.TranslateText(translator); }

		/// <summary>
		/// 获取多语言文本内容
		/// </summary>
		/// <param name="key">要获取多语言文本的 Key</param>
		/// <returns></returns>
		public static string GetValue(string key) { return GameConfig.tables.InterfaceText.GetOrDefault(key)?.Text; }
	}
}