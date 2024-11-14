using System.Collections.Generic;
using UnityEngine;

namespace TwitchChatter
{
	public class TwitchChatClientInternal
	{
		private TwitchChatServerConnection _serverConnection;

		private List<string> _outgoingMessageQueue;

		private List<string> _channelList;

		private List<ChatMessageNotificationDelegate> _chatMessageListeners;

		private List<ServerMessageNotificationDelegate> _serverMessageListeners;

		private List<WhisperMessageNotificationDelegate> _whisperMessageListeners;

		private List<ChannelJoinedNotificationDelegate> _channelJoinedListeners;

		private List<WhispersEnabledNotificationDelegate> _whispersEnabledListeners;

		private bool _whispersRequested;

		private bool _shouldPrintAllStreamMessages;

		private static TwitchChatClientInternal _singleton;

		private bool _isInitialized;

		private bool _printVerboseDebugInfo;

		private static string C_ROOMSTATE_MSG = ":tmi.twitch.tv ROOMSTATE #";

		public static TwitchChatClientInternal singleton
		{
			get
			{
				if (_singleton == null)
				{
					_singleton = new TwitchChatClientInternal();
				}
				return _singleton;
			}
		}

		public bool isInitialized
		{
			get
			{
				return _isInitialized;
			}
		}

		public bool isConnected
		{
			get
			{
				if (_serverConnection != null)
				{
					return _serverConnection.isReadyToSendMessages;
				}
				return false;
			}
		}

		public bool isWhispersEnabled
		{
			get
			{
				if (isConnected)
				{
					return _whispersRequested;
				}
				return false;
			}
		}

		public bool printVerboseDebugInfo
		{
			get
			{
				return _printVerboseDebugInfo;
			}
			set
			{
				if (_serverConnection != null)
				{
					_serverConnection.printDebugInfo = value;
				}
				_printVerboseDebugInfo = value;
			}
		}

		public void AddChatListener(ChatMessageNotificationDelegate func)
		{
			_chatMessageListeners.Add(func);
		}

		public void RemoveChatListener(ChatMessageNotificationDelegate func)
		{
			_chatMessageListeners.Remove(func);
		}

		public void AddServerListener(ServerMessageNotificationDelegate func)
		{
			_serverMessageListeners.Add(func);
		}

		public void RemoveServerListener(ServerMessageNotificationDelegate func)
		{
			_serverMessageListeners.Remove(func);
		}

		public void AddWhisperListener(WhisperMessageNotificationDelegate func)
		{
			_whisperMessageListeners.Add(func);
		}

		public void RemoveWhisperListener(WhisperMessageNotificationDelegate func)
		{
			_whisperMessageListeners.Remove(func);
		}

		public void AddChannelJoinedListener(ChannelJoinedNotificationDelegate func)
		{
			_channelJoinedListeners.Add(func);
		}

		public void RemoveChannelJoinedListener(ChannelJoinedNotificationDelegate func)
		{
			_channelJoinedListeners.Remove(func);
		}

		public void AddWhispersEnabledListener(WhispersEnabledNotificationDelegate func)
		{
			_whispersEnabledListeners.Add(func);
		}

		public void RemoveWhispersEnabledListener(WhispersEnabledNotificationDelegate func)
		{
			_whispersEnabledListeners.Remove(func);
		}

		public void JoinChannel(string userName, string oAuthPassword, string channelName)
		{
			if (_serverConnection == null)
			{
				_serverConnection = new TwitchChatServerConnection();
				_serverConnection.setDebugInfo("[TwitchChatter] ", printVerboseDebugInfo);
				_serverConnection.initialize(userName, oAuthPassword, OnChatServerReconnection);
			}
			if (!_channelList.Contains(channelName))
			{
				_channelList.Add(channelName);
			}
			QueueOutgoingMessage("JOIN #" + channelName);
		}

		public void LeaveChannel(string channelName)
		{
			if (_channelList.Contains(channelName))
			{
				QueueOutgoingMessage("PART #" + channelName);
			}
			_channelList.Remove(channelName);
		}

		public void SendMessage(string channelName, string msg)
		{
			if (_channelList.Contains(channelName))
			{
				QueueOutgoingMessage("PRIVMSG #" + channelName + " :" + msg);
			}
		}

		public void EnableWhispers(string userName, string oAuthPassword)
		{
			if (_serverConnection == null)
			{
				_serverConnection = new TwitchChatServerConnection();
				_serverConnection.setDebugInfo("[TwitchChatter] ", printVerboseDebugInfo);
				_serverConnection.initialize(userName, oAuthPassword, OnChatServerReconnection);
			}
			else if (!_serverConnection.isCredentialsSpecified)
			{
				Debug.LogWarning("Connection already established without credentials. To enable whispers, first disconnect then retry with credentials.");
				return;
			}
			_whispersRequested = true;
		}

		public void SendWhisper(string userName, string msg)
		{
			if (_whispersRequested)
			{
				QueueOutgoingMessage("PRIVMSG #jtv :.w " + userName + " " + msg);
			}
			else
			{
				Debug.LogWarning("You must call EnableWhispers to send whispers.");
			}
		}

		public void Awake()
		{
			_outgoingMessageQueue = new List<string>();
			_channelList = new List<string>();
			_chatMessageListeners = new List<ChatMessageNotificationDelegate>();
			_serverMessageListeners = new List<ServerMessageNotificationDelegate>();
			_whisperMessageListeners = new List<WhisperMessageNotificationDelegate>();
			_channelJoinedListeners = new List<ChannelJoinedNotificationDelegate>();
			_whispersEnabledListeners = new List<WhispersEnabledNotificationDelegate>();
			_isInitialized = true;
		}

		public void Update()
		{
			ProcessStream();
		}

		public void Disconnect()
		{
			if (_serverConnection != null)
			{
				_serverConnection.stop();
				_serverConnection = null;
			}
			_channelList.Clear();
			_whispersRequested = false;
		}

		public void OnDestroy()
		{
			_isInitialized = false;
			if (_chatMessageListeners != null)
			{
				_chatMessageListeners.Clear();
				_chatMessageListeners = null;
			}
			if (_serverMessageListeners != null)
			{
				_serverMessageListeners.Clear();
				_serverMessageListeners = null;
			}
			if (_whisperMessageListeners != null)
			{
				_whisperMessageListeners.Clear();
				_whisperMessageListeners = null;
			}
			if (_channelJoinedListeners != null)
			{
				_channelJoinedListeners.Clear();
				_channelJoinedListeners = null;
			}
			if (_whispersEnabledListeners != null)
			{
				_whispersEnabledListeners.Clear();
				_whispersEnabledListeners = null;
			}
			Disconnect();
			_singleton = null;
		}

		private void OnChatServerReconnection()
		{
			foreach (string channel in _channelList)
			{
				QueueOutgoingMessage("JOIN #" + channel);
			}
		}

		private void ProcessStream()
		{
			TwitchChatServerConnection serverConnection = _serverConnection;
			List<string> outgoingMessageQueue = _outgoingMessageQueue;
			if (serverConnection == null)
			{
				return;
			}
			bool isReadyToSendMessages = serverConnection.isReadyToSendMessages;
			serverConnection.process();
			if (!isReadyToSendMessages && serverConnection.isReadyToSendMessages && serverConnection.isCredentialsSpecified && _whispersRequested)
			{
				foreach (WhispersEnabledNotificationDelegate whispersEnabledListener in _whispersEnabledListeners)
				{
					whispersEnabledListener();
				}
			}
			if (!serverConnection.isReadyToReceiveMessages)
			{
				return;
			}
			if (serverConnection.isReadyToSendMessages && outgoingMessageQueue.Count > 0)
			{
				serverConnection.stream.submit_messages(outgoingMessageQueue);
				outgoingMessageQueue.Clear();
			}
			foreach (string new_twitch_message in serverConnection.stream.get_new_twitch_messages())
			{
				if (_shouldPrintAllStreamMessages)
				{
					Debug.Log(new_twitch_message);
				}
				TwitchChatMessage chat_message = default(TwitchChatMessage);
				TwitchServerMessage server_message = default(TwitchServerMessage);
				TwitchChatMessageTokenizer.tokenize(new_twitch_message, ref chat_message, ref server_message);
				if (server_message.type != TwitchServerMessage.TwitchServerMessageType.INVALID)
				{
					bool flag = true;
					if (server_message.type == TwitchServerMessage.TwitchServerMessageType.AuthenticationAcknowledged)
					{
						serverConnection.acknowledgeAuthentication();
					}
					else if (server_message.type == TwitchServerMessage.TwitchServerMessageType.InvalidOAuthTokenError || server_message.type == TwitchServerMessage.TwitchServerMessageType.ImproperlyFormattedCredentialsError || server_message.type == TwitchServerMessage.TwitchServerMessageType.ConnectionError)
					{
						flag = true;
						string text = "Error logging in to Twitch servers.";
						if (server_message.type == TwitchServerMessage.TwitchServerMessageType.InvalidOAuthTokenError)
						{
							text += " Your OAuth token is invalid or you may have mistyped your user name. Visit https://twitchapps.com/tmi/ to get a valid OAuth token.";
						}
						else if (server_message.type == TwitchServerMessage.TwitchServerMessageType.ImproperlyFormattedCredentialsError)
						{
							bool num = !string.IsNullOrEmpty(serverConnection.userName);
							bool flag2 = !string.IsNullOrEmpty(serverConnection.password);
							bool flag3 = flag2 && serverConnection.password.ToLower().IndexOf("oauth:") != -1;
							if (!num)
							{
								text += " You must specify a user name to login.";
							}
							else if (!flag2)
							{
								text += " You must specify an OAuth token to login.";
							}
							else if (!flag3)
							{
								text += " Your OAuth token should begin with the prefix \"oauth:\"";
							}
						}
						Debug.LogError("TwitchChatter: " + text);
					}
					else if (server_message.type == TwitchServerMessage.TwitchServerMessageType.CapabilitiesAcknowledged)
					{
						serverConnection.acknowledgeCapability();
					}
					else if (server_message.rawText.IndexOf(C_ROOMSTATE_MSG) != -1)
					{
						foreach (ChannelJoinedNotificationDelegate channelJoinedListener in _channelJoinedListeners)
						{
							channelJoinedListener(server_message.rawText.Substring(server_message.rawText.IndexOf(C_ROOMSTATE_MSG) + C_ROOMSTATE_MSG.Length));
						}
					}
					else if (server_message.type == TwitchServerMessage.TwitchServerMessageType.Ping)
					{
						QueueOutgoingMessage("PONG :tmi.twitch.tv");
					}
					else if (server_message.type == TwitchServerMessage.TwitchServerMessageType.TwitchChatter)
					{
						flag = true;
						if (printVerboseDebugInfo)
						{
							Debug.Log(new_twitch_message);
						}
					}
					if (!flag)
					{
						continue;
					}
					foreach (ServerMessageNotificationDelegate serverMessageListener in _serverMessageListeners)
					{
						serverMessageListener(ref server_message);
					}
				}
				else
				{
					if (chat_message.type == TwitchChatMessage.TwitchChatMessageType.INVALID)
					{
						continue;
					}
					if (chat_message.type == TwitchChatMessage.TwitchChatMessageType.UserMessage)
					{
						foreach (ChatMessageNotificationDelegate chatMessageListener in _chatMessageListeners)
						{
							chatMessageListener(ref chat_message);
						}
					}
					else
					{
						if (chat_message.type != TwitchChatMessage.TwitchChatMessageType.Whisper || !_whispersRequested)
						{
							continue;
						}
						foreach (WhisperMessageNotificationDelegate whisperMessageListener in _whisperMessageListeners)
						{
							whisperMessageListener(ref chat_message);
						}
					}
				}
			}
		}

		private void QueueOutgoingMessage(string msg)
		{
			_outgoingMessageQueue.Add(msg);
		}

		private TwitchChatClientInternal()
		{
		}
	}
}
