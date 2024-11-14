using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TwitchChatter
{
	public class TwitchText : Text
	{
		public enum ListenMode
		{
			Chat = 0,
			Whisper = 1,
			Custom = 2
		}

		public enum TextMode
		{
			PlainText = 0,
			WithEmotes = 1
		}

		public delegate void TwitchEmoteImageInitializationCallback(TwitchEmoteImage image);

		private class Emote
		{
			public int characterIndex;

			public TwitchEmoteImage image;

			public Vector3 position;

			public Emote(int index, TwitchEmoteImage image)
			{
				characterIndex = index;
				this.image = image;
				position = Vector3.zero;
			}
		}

		[Tooltip("Print chat room messages or whispers.")]
		[SerializeField]
		private ListenMode _listenMode;

		[Tooltip("(Chat-only) Name of the Twitch channel chat to render. If blank, chat messages from all channels will be rendered.")]
		[SerializeField]
		private string _channelName;

		[Tooltip("(Whisper-only) Name of the Twitch user's whispers to render. If blank, whispers from all users will be rendered.")]
		[SerializeField]
		private string _userName;

		[Tooltip("Render plain text or text with emoticons.")]
		[SerializeField]
		private TextMode _textMode;

		[Tooltip("Custom scale factor for Twitch emoticons.")]
		[SerializeField]
		private float _emoticonScaleFactor = 1f;

		[Tooltip("Maximum number of characters to render. Older messages will be removed. Popular or spam-filled channels may require a lower number.")]
		[SerializeField]
		private int _maxCharacterCount = 1000;

		[Tooltip("Precede each message with the sender's user name.")]
		[SerializeField]
		private bool _renderUserNames = true;

		private List<Emote> _activeEmotes;

		private List<TwitchEmoteImage> _inactiveImageList;

		private List<TwitchEmoteImageInitializationCallback> _emoteImageInitializationListeners;

		private bool _shouldUpdateImagePositions;

		public ListenMode listenMode
		{
			get
			{
				return _listenMode;
			}
			set
			{
				if (_listenMode != value)
				{
					_listenMode = value;
					OnListenModeModified();
				}
			}
		}

		public string channelName
		{
			get
			{
				return _channelName;
			}
			set
			{
				_channelName = value;
			}
		}

		public TextMode textMode
		{
			get
			{
				return _textMode;
			}
			set
			{
				_textMode = value;
			}
		}

		public float emoticonScaleFactor
		{
			get
			{
				return _emoticonScaleFactor;
			}
			set
			{
				if (_emoticonScaleFactor != value)
				{
					_emoticonScaleFactor = value;
					OnEmoticonScaleFactorModified();
				}
			}
		}

		public int maxCharacterCount
		{
			get
			{
				return _maxCharacterCount;
			}
			set
			{
				if (_maxCharacterCount != value)
				{
					_maxCharacterCount = value;
					OnMaxCharacterCountModified();
				}
			}
		}

		public void OnCustomMessage(ref TwitchChatMessage msg)
		{
			if (_listenMode == ListenMode.Custom)
			{
				RenderMessage(ref msg);
			}
		}

		public void Clear()
		{
			text = "";
			if (_activeEmotes == null)
			{
				return;
			}
			foreach (Emote activeEmote in _activeEmotes)
			{
				activeEmote.image.sprite = null;
				activeEmote.image.enabled = false;
				Object.Destroy(activeEmote.image.gameObject);
				activeEmote.image = null;
			}
			_activeEmotes.Clear();
		}

		public void AddEmoteImageInitializationListener(TwitchEmoteImageInitializationCallback callback)
		{
			if (_emoteImageInitializationListeners == null)
			{
				_emoteImageInitializationListeners = new List<TwitchEmoteImageInitializationCallback>();
			}
			_emoteImageInitializationListeners.Add(callback);
		}

		public void RemoveEmoteImageInitializationListener(TwitchEmoteImageInitializationCallback callback)
		{
			if (_emoteImageInitializationListeners != null)
			{
				_emoteImageInitializationListeners.Remove(callback);
			}
		}

		public void OnListenModeModified()
		{
			UnregisterListeners();
			RegisterListener();
		}

		public void OnEmoticonScaleFactorModified()
		{
			foreach (Emote activeEmote in _activeEmotes)
			{
				activeEmote.image.scaleFactor = _emoticonScaleFactor;
			}
		}

		public void OnMaxCharacterCountModified()
		{
			PerformTextClipping();
		}

		protected override void Start()
		{
			base.Start();
			_activeEmotes = new List<Emote>();
			_inactiveImageList = new List<TwitchEmoteImage>();
			UnregisterListeners();
			RegisterListener();
		}

		protected override void OnPopulateMesh(VertexHelper vertexHelper)
		{
			base.OnPopulateMesh(vertexHelper);
			if (_activeEmotes == null)
			{
				return;
			}
			List<UIVertex> list = new List<UIVertex>();
			vertexHelper.GetUIVertexStream(list);
			foreach (Emote activeEmote in _activeEmotes)
			{
				if (activeEmote.characterIndex * 6 + 5 <= list.Count)
				{
					Vector3 zero = Vector3.zero;
					for (int i = 0; i < 6; i++)
					{
						zero += list[activeEmote.characterIndex * 6 + i].position;
					}
					activeEmote.position = zero / 6f;
				}
			}
			_shouldUpdateImagePositions = true;
		}

		private void Update()
		{
			if (!_shouldUpdateImagePositions || _activeEmotes == null)
			{
				return;
			}
			foreach (Emote activeEmote in _activeEmotes)
			{
				activeEmote.image.rectTransform.anchoredPosition = activeEmote.position;
			}
			_shouldUpdateImagePositions = false;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			UnregisterListeners();
			Clear();
		}

		private void OnChatMessage(ref TwitchChatMessage msg)
		{
			if (_listenMode == ListenMode.Chat && (string.IsNullOrEmpty(_channelName) || msg.channelName.Equals(_channelName.ToLower())))
			{
				RenderMessage(ref msg);
			}
		}

		private void OnWhisper(ref TwitchChatMessage msg)
		{
			if (_listenMode == ListenMode.Whisper && (string.IsNullOrEmpty(_userName) || msg.userName.ToLower().Equals(_userName.ToLower())))
			{
				RenderMessage(ref msg);
			}
		}

		private void RegisterListener()
		{
			TwitchChatClientInternal singleton = TwitchChatClientInternal.singleton;
			if (singleton != null && singleton.isInitialized)
			{
				if (_listenMode == ListenMode.Chat)
				{
					singleton.AddChatListener(OnChatMessage);
				}
				else if (_listenMode == ListenMode.Whisper)
				{
					singleton.AddWhisperListener(OnWhisper);
				}
			}
		}

		private void UnregisterListeners()
		{
			TwitchChatClientInternal singleton = TwitchChatClientInternal.singleton;
			if (singleton != null && singleton.isInitialized)
			{
				singleton.RemoveChatListener(OnChatMessage);
				singleton.RemoveWhisperListener(OnWhisper);
			}
		}

		private void RenderMessage(ref TwitchChatMessage msg)
		{
			string text = "\r\n";
			if (_renderUserNames)
			{
				string text2 = msg.userNameColor;
				if (string.IsNullOrEmpty(msg.userNameColor))
				{
					text2 = TwitchChatMessage.RandomColors[Random.Range(0, TwitchChatMessage.RandomColors.Length)];
				}
				text = text + "<color=" + text2 + ">";
				text += msg.userName;
				text += "</color>";
				text += ": ";
			}
			string text3 = null;
			if (_textMode == TextMode.PlainText)
			{
				text3 = msg.chatMessagePlainText;
			}
			else if (_textMode == TextMode.WithEmotes)
			{
				text3 = msg.chatMessageMinusEmotes;
				if (msg.emoteData != null && msg.emoteData.Length != 0)
				{
					int num = 0;
					string text4 = "   |   ";
					int num2 = text4.Length / 2;
					TwitchChatMessage.EmoteData[] emoteData = msg.emoteData;
					foreach (TwitchChatMessage.EmoteData emoteData2 in emoteData)
					{
						TwitchEmoteImage twitchEmoteInstance = GetTwitchEmoteInstance(emoteData2.id);
						int num3 = emoteData2.index + num * text4.Length;
						text3 = text3.Insert(num3, text4);
						_activeEmotes.Add(new Emote(this.text.Length + text.Length + num3 + num2, twitchEmoteInstance));
						num++;
					}
				}
			}
			text += text3;
			this.text += text;
			PerformTextClipping();
		}

		private TwitchEmoteImage GetTwitchEmoteInstance(int emoteID)
		{
			TwitchEmoteImage twitchEmoteImage = null;
			if (_inactiveImageList.Count > 0)
			{
				twitchEmoteImage = _inactiveImageList[_inactiveImageList.Count - 1];
				_inactiveImageList.RemoveAt(_inactiveImageList.Count - 1);
				twitchEmoteImage.enabled = true;
			}
			else
			{
				GameObject obj = new GameObject();
				obj.transform.SetParent(base.transform, false);
				twitchEmoteImage = obj.AddComponent<TwitchEmoteImage>();
				twitchEmoteImage.scaleFactor = _emoticonScaleFactor;
			}
			twitchEmoteImage.emoteID = emoteID;
			if (_emoteImageInitializationListeners != null)
			{
				foreach (TwitchEmoteImageInitializationCallback emoteImageInitializationListener in _emoteImageInitializationListeners)
				{
					emoteImageInitializationListener(twitchEmoteImage);
				}
			}
			return twitchEmoteImage;
		}

		private void PerformTextClipping()
		{
			if (text.Length <= _maxCharacterCount)
			{
				return;
			}
			int startIndex = text.Length - _maxCharacterCount;
			startIndex = text.LastIndexOf("<color=#", startIndex);
			if (startIndex < 0)
			{
				return;
			}
			int num = 0;
			foreach (Emote activeEmote in _activeEmotes)
			{
				if (activeEmote.characterIndex < startIndex)
				{
					num++;
					activeEmote.image.sprite = null;
					activeEmote.image.enabled = false;
					_inactiveImageList.Add(activeEmote.image);
					activeEmote.image = null;
				}
				else
				{
					activeEmote.characterIndex -= startIndex;
				}
			}
			_activeEmotes.RemoveRange(0, num);
			text = text.Substring(startIndex);
		}
	}
}
