using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwitchChatter
{
	public class TwitchEmoteCache : MonoBehaviour
	{
		public delegate void OnLoadCallback(Sprite sprite);

		[SerializeField]
		private Sprite _loadingIcon;

		[SerializeField]
		private Sprite _invalidIcon;

		private const string C_TWITCH_EMOTE_URL = "http://static-cdn.jtvnw.net/emoticons/v1/:emote_id/:size";

		private const string C_ID_REPLACE_PHRASE = ":emote_id";

		private const string C_SIZE_REPLACE_PHRASE = ":size";

		private static TwitchEmoteCache _singleton;

		private Dictionary<int, Sprite>[] _idSpriteMaps;

		public static Sprite GetSpriteForEmoteID(int emoteID, OnLoadCallback callback = null)
		{
			return GetSpriteForEmoteID(emoteID, EmoteSize.Standard, callback);
		}

		public static Sprite GetSpriteForEmoteID(int emoteID, EmoteSize size, OnLoadCallback callback = null)
		{
			if (_singleton != null)
			{
				if (_singleton._idSpriteMaps[(int)size].ContainsKey(emoteID))
				{
					return _singleton._idSpriteMaps[(int)size][emoteID];
				}
				_singleton.StartCoroutine(_singleton.LoadEmote(emoteID, size, callback));
				return _singleton._loadingIcon;
			}
			return null;
		}

		public static void Clear()
		{
			if (!(_singleton != null) || _singleton._idSpriteMaps == null)
			{
				return;
			}
			Dictionary<int, Sprite>[] idSpriteMaps = _singleton._idSpriteMaps;
			foreach (Dictionary<int, Sprite> dictionary in idSpriteMaps)
			{
				if (dictionary == null)
				{
					continue;
				}
				int[] array = new int[dictionary.Keys.Count];
				dictionary.Keys.CopyTo(array, 0);
				int[] array2 = array;
				foreach (int key in array2)
				{
					Sprite sprite = dictionary[key];
					if (sprite != _singleton._invalidIcon)
					{
						Object.Destroy(sprite.texture);
					}
					dictionary.Remove(key);
				}
				dictionary.Clear();
			}
		}

		private void Awake()
		{
			_singleton = this;
			_idSpriteMaps = new Dictionary<int, Sprite>[3];
			for (int i = 0; i < 3; i++)
			{
				_idSpriteMaps[i] = new Dictionary<int, Sprite>();
			}
		}

		private void OnDestroy()
		{
			Clear();
			_idSpriteMaps = null;
			_singleton = null;
		}

		private IEnumerator LoadEmote(int emoteID, EmoteSize size, OnLoadCallback callback = null)
		{
			string url = "http://static-cdn.jtvnw.net/emoticons/v1/:emote_id/:size".Replace(":emote_id", string.Concat(emoteID)).Replace(":size", GetURLParameterForResolution(size));
			WWW www = new WWW(url);
			yield return www;
			Sprite sprite;
			if (string.IsNullOrEmpty(www.error))
			{
				Texture2D texture2D = new Texture2D(32, 32, TextureFormat.ARGB32, false);
				www.LoadImageIntoTexture(texture2D);
				sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
			}
			else
			{
				sprite = _invalidIcon;
			}
			_idSpriteMaps[(int)size][emoteID] = sprite;
			if (callback != null)
			{
				callback(sprite);
			}
		}

		private string GetURLParameterForResolution(EmoteSize size)
		{
			switch (size)
			{
			case EmoteSize.Standard:
				return "1.0";
			case EmoteSize.Medium:
				return "2.0";
			case EmoteSize.Large:
				return "3.0";
			default:
				return "1.0";
			}
		}
	}
}
