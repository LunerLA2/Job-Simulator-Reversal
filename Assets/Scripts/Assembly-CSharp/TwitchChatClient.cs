using TwitchChatter;
using UnityEngine;

public class TwitchChatClient : MonoBehaviour
{
	[Tooltip("User name to use when connecting to Twitch chat and sending messages.")]
	[SerializeField]
	private string _userName;

	[Tooltip("OAuth password to use when connecting to Twitch chat. Visit https://twitchapps.com/tmi/ to get yours.")]
	[SerializeField]
	private string _oAuthPassword;

	[Tooltip("These channels will be joined when this component's Awake function is called.")]
	[SerializeField]
	private string[] _autoJoinChannels;

	[SerializeField]
	[Tooltip("Enable whispers immediately upon Awake. You must specify a user name and OAuth password for whispers to function.")]
	private bool _autoEnableWhispers;

	private static TwitchChatClient _singleton;

	private TwitchChatClientInternal _internalClient;

	public string userName
	{
		get
		{
			return _userName;
		}
		set
		{
			_userName = value;
		}
	}

	public string oAuthPassword
	{
		get
		{
			return _oAuthPassword;
		}
		set
		{
			_oAuthPassword = value;
		}
	}

	public string[] autoJoinChannels
	{
		get
		{
			return _autoJoinChannels;
		}
		set
		{
			_autoJoinChannels = value;
		}
	}

	public bool autoEnableWhispers
	{
		get
		{
			return _autoEnableWhispers;
		}
		set
		{
			_autoEnableWhispers = value;
		}
	}

	public bool isConnected
	{
		get
		{
			return _internalClient != null && _internalClient.isConnected;
		}
	}

	public bool isWhispersEnabled
	{
		get
		{
			return _internalClient != null && _internalClient.isWhispersEnabled;
		}
	}

	public static TwitchChatClient singleton
	{
		get
		{
			return _singleton;
		}
	}

	public void AddChatListener(ChatMessageNotificationDelegate func)
	{
		_internalClient.AddChatListener(func);
	}

	public void RemoveChatListener(ChatMessageNotificationDelegate func)
	{
		_internalClient.RemoveChatListener(func);
	}

	public void AddServerListener(ServerMessageNotificationDelegate func)
	{
		_internalClient.AddServerListener(func);
	}

	public void RemoveServerListener(ServerMessageNotificationDelegate func)
	{
		_internalClient.RemoveServerListener(func);
	}

	public void AddChannelJoinedListener(ChannelJoinedNotificationDelegate func)
	{
		_internalClient.AddChannelJoinedListener(func);
	}

	public void RemoveChannelJoinedListener(ChannelJoinedNotificationDelegate func)
	{
		_internalClient.RemoveChannelJoinedListener(func);
	}

	public void JoinChannel(string channelName)
	{
		_internalClient.JoinChannel(_userName, _oAuthPassword, channelName);
	}

	public void LeaveChannel(string channelName)
	{
		_internalClient.LeaveChannel(channelName);
	}

	public void SendMessage(string channelName, string msg)
	{
		_internalClient.SendMessage(channelName, msg);
	}

	public void EnableWhispers()
	{
		if (string.IsNullOrEmpty(_userName) || string.IsNullOrEmpty(_oAuthPassword))
		{
			Debug.LogWarning("A valid user name and OAuth password must be provided to receive whispers. Ensure the _userName and _oAuthPassword members in TwitchChatClient are initialized.", this);
		}
		else
		{
			_internalClient.EnableWhispers(_userName, _oAuthPassword);
		}
	}

	public void AddWhisperListener(WhisperMessageNotificationDelegate func)
	{
		_internalClient.AddWhisperListener(func);
	}

	public void RemoveWhisperListener(WhisperMessageNotificationDelegate func)
	{
		_internalClient.RemoveWhisperListener(func);
	}

	public void AddWhispersEnabledListener(WhispersEnabledNotificationDelegate func)
	{
		_internalClient.AddWhispersEnabledListener(func);
	}

	public void RemoveWhispersEnabledListener(WhispersEnabledNotificationDelegate func)
	{
		_internalClient.RemoveWhispersEnabledListener(func);
	}

	public void SendWhisper(string userName, string msg)
	{
		_internalClient.SendWhisper(userName, msg);
	}

	public void Disconnect()
	{
		_internalClient.Disconnect();
	}

	private void Awake()
	{
		_singleton = this;
		_internalClient = TwitchChatClientInternal.singleton;
		_internalClient.Awake();
		if (_autoJoinChannels != null && _autoJoinChannels.Length > 0)
		{
			string[] array = _autoJoinChannels;
			foreach (string channelName in array)
			{
				JoinChannel(channelName);
			}
		}
		if (_autoEnableWhispers && !string.IsNullOrEmpty(_userName) && !string.IsNullOrEmpty(_oAuthPassword))
		{
			EnableWhispers();
		}
	}

	private void Update()
	{
		if (_internalClient != null)
		{
			_internalClient.Update();
		}
	}

	private void OnDestroy()
	{
		if (_internalClient != null)
		{
			_internalClient.OnDestroy();
			_internalClient = null;
		}
		_singleton = null;
	}
}
