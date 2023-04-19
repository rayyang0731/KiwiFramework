using System;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.U2D;

namespace KiwiFramework.Runtime.UI
{
	/// <summary>
	/// Sprite 管理器
	/// </summary>
	public class SpriteManager : Singleton<SpriteManager>
	{
		/// <summary>
		/// 被加载的Sprite
		/// </summary>
		private readonly Dictionary<string, Sprite> _sprites = new();

		/// <summary>
		/// 被加载的Atlas
		/// </summary>
		private readonly Dictionary<string, SpriteAtlas> _atlases = new();

		/// <summary>
		/// 引用计数
		/// </summary>
		private readonly Dictionary<string, int> _refCounts = new();

		protected override void OnSingletonInit() { SpriteAtlasManager.atlasRequested += RequestedAtlas; }

		private static void RequestedAtlas(string spriteAtlasName, Action<SpriteAtlas> callback)
		{
			AssetLoader.LoadAsync<SpriteAtlas>(
				spriteAtlasName,
				(_, asset, _, _) => callback.Invoke(asset),
				(name, message, _) => Debug.LogError($"加载 SpriteAtlas [{name}] 加载失败 : {message}")
			);
		}

		/// <summary>
		/// 获取 Sprite
		/// </summary>
		/// <param name="key">Sprite 名称(AtlasName_SpriteName or SpriteName)</param>
		/// <returns></returns>
		public async UniTask<Sprite> GetSprite(string key)
		{
			if (!_sprites.ContainsKey(key))
				await LoadSprite(key);

			if (!_refCounts.ContainsKey(key))
			{
				Debug.LogException(new Exception($"Sprite [{key}] 未被加载过."));
				return null;
			}

			_refCounts[key]++;

			if (key.Contains("_"))
			{
				var split     = key.Split('_');
				var atlasName = split[0];
				_refCounts[atlasName]++;
			}

			return _sprites[key];
		}

		/// <summary>
		/// 释放 Sprite
		/// </summary>
		/// <param name="key">Sprite 名称(AtlasName_SpriteName or SpriteName)</param>
		public void ReleaseSprite(string key)
		{
			if (!_sprites.ContainsKey(key)) return;

			_refCounts[key]--;
			if (_refCounts[key] <= 0)
				UnloadSprite(key);

			if (key.Contains("_"))
			{
				var split     = key.Split('_');
				var atlasName = split[0];
				_refCounts[atlasName]--;
				if (_refCounts[atlasName] <= 0)
					UnloadSpriteAtlas(atlasName);
			}
		}

		/// <summary>
		/// 释放 Sprite
		/// </summary>
		/// <param name="sprite">要释放的 Sprite 对象</param>
		public void ReleaseSprite(Sprite sprite)
		{
			using var enumerator = _sprites.GetEnumerator();
			while (enumerator.MoveNext())
			{
				var (name, spr) = enumerator.Current;
				if (spr != sprite) continue;

				ReleaseSprite(name);
				break;
			}
		}

		/// <summary>
		/// 加载 Sprite
		/// </summary>
		/// <param name="key">Sprite 名称(AtlasName_SpriteName or SpriteName)</param>
		private async UniTask LoadSprite(string key)
		{
			if (_refCounts.ContainsKey(key)) return;

			string atlasName = null;

			if (key.Contains("_"))
			{
				var split = key.Split('_');
				atlasName = split[0];
			}

			Sprite sprite;

			if (string.IsNullOrEmpty(atlasName))
			{
				sprite = await AssetLoader.LoadAsync<Sprite>(key);
				if (sprite == null)
				{
					Debug.LogError($"sprite 加载失败 : {key}");
					return;
				}
			}
			else
			{
				if (!_atlases.TryGetValue(atlasName, out var atlas))
				{
					atlas = await AssetLoader.LoadAsync<SpriteAtlas>(atlasName);

					if (atlas == null)
					{
						Debug.LogError($"Atlas [{atlasName}] 加载失败");
						return;
					}

					_atlases.Add(atlasName, atlas);
					_refCounts.Add(atlasName, 0);
				}

				sprite = atlas.GetSprite(key);
				if (sprite == null)
				{
					Debug.LogError($"从 Atlas [{atlasName}] 中加载 Sprite [{key}] 失败");
					return;
				}

				_refCounts[atlasName]++;
			}

			_sprites.Add(key, sprite);
			_refCounts.Add(key, 0);
		}

		/// <summary>
		/// 卸载 Sprite
		/// </summary>
		/// <param name="key">Sprite 名称(AtlasName_SpriteName or SpriteName)</param>
		private void UnloadSprite(string key)
		{
			_sprites.Remove(key, out var sprite);
			KiwiAssets.Unload(sprite);
			_refCounts.Remove(key);

			string atlasName = null;

			if (key.Contains("_"))
			{
				var split = key.Split('_');
				atlasName = split[0];
			}

			if (!string.IsNullOrEmpty(atlasName))
			{
				if (_atlases.ContainsKey(atlasName))
				{
					_refCounts[atlasName]--;
					if (_refCounts[atlasName] <= 0)
					{
						UnloadSpriteAtlas(atlasName);
					}
				}
			}
		}

		/// <summary>
		/// 卸载 SpriteAtlas
		/// </summary>
		/// <param name="atlasName">要卸载的 Atlas 的名字</param>
		private void UnloadSpriteAtlas(string atlasName)
		{
			if (!_atlases.TryGetValue(atlasName, out var atlas))
				return;

			_atlases.Remove(atlasName);
			KiwiAssets.Unload(atlas);
			_refCounts.Remove(atlasName);
		}
	}
}