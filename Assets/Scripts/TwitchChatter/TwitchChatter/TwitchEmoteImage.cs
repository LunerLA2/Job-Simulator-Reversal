using UnityEngine;
using UnityEngine.UI;

namespace TwitchChatter
{
	public class TwitchEmoteImage : Image
	{
		private int _emoteID = -1;

		private EmoteSize _size;

		private float _scaleFactor = 1f;

		public int emoteID
		{
			get
			{
				return _emoteID;
			}
			set
			{
				_emoteID = value;
				RefreshImage();
			}
		}

		public EmoteSize size
		{
			get
			{
				return _size;
			}
			set
			{
				_size = value;
				RefreshImage();
			}
		}

		public float scaleFactor
		{
			get
			{
				return _scaleFactor;
			}
			set
			{
				if (_scaleFactor != value)
				{
					_scaleFactor = value;
					RefreshImage();
				}
			}
		}

		private void onLoadCallback(Sprite sprite)
		{
			RefreshImage();
		}

		private void RefreshImage()
		{
			if (_emoteID > 0)
			{
				base.sprite = TwitchEmoteCache.GetSpriteForEmoteID(_emoteID, _size, onLoadCallback);
				base.rectTransform.sizeDelta = _scaleFactor * new Vector2(base.sprite.rect.width, base.sprite.rect.height);
			}
		}
	}
}
