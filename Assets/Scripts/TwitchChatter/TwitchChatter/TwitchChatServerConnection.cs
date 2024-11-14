using System.Collections.Generic;
using UnityEngine;

namespace TwitchChatter
{
	internal class TwitchChatServerConnection
	{
		public delegate void OnReconnection();

		private enum ConnectionState
		{
			INVALID = -1,
			RequestingServerList = 0,
			Connecting = 1,
			Connected = 2,
			Authenticating = 3,
			Authenticated = 4,
			RequestingCapabilities = 5,
			CapabilitiesAcknowledged = 6,
			COUNT = 7
		}

		private static string C_ANONYMOUS_USER_NAME = "justinfan888111333";

		private static string C_TWITCH_CHAT_URL = "irc.chat.twitch.tv";

		private static int C_TWITCH_CHAT_PORT = 6667;

		private ConnectionState _state = ConnectionState.INVALID;

		private TwitchChatStream _stream;

		private string _userName;

		private string _password;

		private string _debugPrefix;

		private int _capsAcknowledged;

		private int _delay;

		private bool _wasConnected;

		private OnReconnection _reconnectionCallback;

		private bool _printDebugInfo;

		public bool printDebugInfo
		{
			set
			{
				_printDebugInfo = value;
			}
		}

		public TwitchChatStream stream
		{
			get
			{
				return _stream;
			}
		}

		public string userName
		{
			get
			{
				return _userName;
			}
		}

		public string password
		{
			get
			{
				return _password;
			}
		}

		public bool isCredentialsSpecified
		{
			get
			{
				if (!string.IsNullOrEmpty(_userName))
				{
					return !string.IsNullOrEmpty(_password);
				}
				return false;
			}
		}

		public bool isReadyToReceiveMessages
		{
			get
			{
				return _state >= ConnectionState.Connected;
			}
		}

		public bool isReadyToSendMessages
		{
			get
			{
				return _state == ConnectionState.CapabilitiesAcknowledged;
			}
		}

		public void setDebugInfo(string debugPrefix, bool printDebugInfo)
		{
			_debugPrefix = debugPrefix;
			_printDebugInfo = printDebugInfo;
		}

		public void initialize(string userName, string password, OnReconnection reconnectionCallback = null)
		{
			_userName = userName;
			_password = password;
			_reconnectionCallback = reconnectionCallback;
			log(" ==> RequestingServerList");
			_state = ConnectionState.RequestingServerList;
		}

		public void acknowledgeAuthentication()
		{
			if (_state == ConnectionState.Authenticating)
			{
				log(" ==> Authenticated");
				_state = ConnectionState.Authenticated;
			}
		}

		public void acknowledgeCapability()
		{
			if (_state == ConnectionState.RequestingCapabilities)
			{
				_capsAcknowledged++;
			}
		}

		public void process()
		{
			if (_state == ConnectionState.RequestingServerList)
			{
				_stream = new TwitchChatStream();
				_stream.start(C_TWITCH_CHAT_URL, C_TWITCH_CHAT_PORT);
				log(" ==> Connecting");
				_state = ConnectionState.Connecting;
			}
			else if (_state == ConnectionState.Connecting)
			{
				if (_stream.get_is_connected())
				{
					log(" ==> Connected");
					_state = ConnectionState.Connected;
					_delay = 0;
				}
			}
			else
			{
				if (_state < ConnectionState.Connected)
				{
					return;
				}
				if (!_stream.get_is_connected())
				{
					_stream.stop();
					_stream = null;
					log(" ==> RequestingServerList");
					_state = ConnectionState.RequestingServerList;
				}
				else if (_state == ConnectionState.Connected)
				{
					_delay += (int)(1000f * Time.deltaTime);
					if (_delay > 2000)
					{
						if (!string.IsNullOrEmpty(_userName) && !string.IsNullOrEmpty(_password))
						{
							List<string> list = new List<string>(2);
							list.Add("PASS " + _password);
							list.Add("NICK " + _userName);
							log("Connecting as " + _userName);
							_stream.submit_messages(list);
						}
						else
						{
							log("Connecting anonymously (User name and/or password unspecified)");
							_stream.submit_message("NICK " + C_ANONYMOUS_USER_NAME);
						}
						log(" ==> Authenticating");
						_state = ConnectionState.Authenticating;
					}
				}
				else if (_state == ConnectionState.Authenticated)
				{
					List<string> list2 = new List<string>(3);
					list2.Add("CAP REQ :twitch.tv/membership");
					list2.Add("CAP REQ :twitch.tv/tags");
					list2.Add("CAP REQ :twitch.tv/commands");
					_capsAcknowledged = 0;
					_stream.submit_messages(list2);
					log(" ==> RequestingCapabilities");
					_state = ConnectionState.RequestingCapabilities;
				}
				else if (_state == ConnectionState.RequestingCapabilities && _capsAcknowledged >= 3)
				{
					log(" ==> CapabilitiesAcknowledged");
					_state = ConnectionState.CapabilitiesAcknowledged;
					if (_wasConnected)
					{
						_reconnectionCallback();
					}
					_wasConnected = true;
				}
			}
		}

		public void stop()
		{
			if (_stream != null)
			{
				_stream.stop();
				_stream = null;
			}
			_state = ConnectionState.INVALID;
			_wasConnected = false;
		}

		private void log(string msg)
		{
			if (_printDebugInfo)
			{
				Debug.Log(_debugPrefix + msg);
			}
		}
	}
}
