using Bright.Serialization;

using cfg;

using SimpleJSON;

using UnityEngine;

namespace KiwiFramework.Runtime
{
	/// <summary>
	/// 游戏配置管理器
	/// </summary>
	public static class GameConfig
	{
		private static Tables _tables;

		public static Tables tables
		{
			get
			{
				if (_tables == null)
					Initialize();

				return _tables;
			}
		}

		private const string CONST_CONFIG_FILE_PREFIX = "config_";

		/// <summary>
		/// 初始化
		/// </summary>
		private static void Initialize()
		{
			var tablesCtor       = typeof(Tables).GetConstructors()[0];
			var loaderReturnType = tablesCtor.GetParameters()[0].ParameterType.GetGenericArguments()[1];

			// 根据cfg.Tables的构造函数的Loader的返回值类型决定使用json还是ByteBuf Loader
			System.Delegate loader = loaderReturnType == typeof(ByteBuf)
				? new System.Func<string, ByteBuf>(LoadByteBuf)
				: new System.Func<string, JSONNode>(LoadJson);

			_tables = (Tables) tablesCtor.Invoke(new object[] {loader});
		}

		private static JSONNode LoadJson(string fileName)
		{
			string text = null;

			if (Application.isPlaying)
			{
				text = AssetLoader.assetHelper.LoadRawFileToText($"{CONST_CONFIG_FILE_PREFIX}{fileName}");
			}
			else
			{
#if UNITY_EDITOR
				text = AssetLoader.Editor.LoadRawFileToText($"{Application.dataPath}/GameAssets/Configs/{fileName}.json");
#endif
			}

			return JSON.Parse(text);
		}

		private static ByteBuf LoadByteBuf(string fileName)
		{
			byte[] bytes = null;
			if (Application.isPlaying)
			{
				bytes = AssetLoader.assetHelper.LoadRawFileToBytes($"{CONST_CONFIG_FILE_PREFIX}{fileName}");
			}
			else
			{
#if UNITY_EDITOR
				bytes = AssetLoader.Editor.LoadRawFileToBytes($"{Application.dataPath}/GameAssets/Configs/{fileName}.bytes");
#endif
			}

			return new ByteBuf(bytes);
		}
	}
}