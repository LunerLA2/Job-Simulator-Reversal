using System;
using System.Collections.Generic;

namespace TwitchChatter
{
	internal static class TwitchChatMessageTokenizer
	{
		private struct Emote_Entry : IComparable<Emote_Entry>
		{
			public int id;

			public int start_index;

			public int end_index;

			public int CompareTo(Emote_Entry other)
			{
				return start_index - other.start_index;
			}
		}

		public static string C_TWITCH_CHATTER_STRING = "[TwitchChatter]";

		private static string C_TWITCH_SYSTEM_STRING = ":tmi.twitch.tv";

		private static string C_JTV_STRING = ":jtv";

		private static string C_PING_STRING = "PING :tmi.twitch.tv";

		private static string C_PRIV_MSG_STRING = "PRIVMSG";

		private static string C_JOIN_STRING = "JOIN";

		private static string C_PART_STRING = "PART";

		private static string C_WHISPER_STRING = "WHISPER";

		public static void tokenize(string message, ref TwitchChatMessage chat_message, ref TwitchServerMessage server_message)
		{
			string text = string.Copy(message);
			chat_message.reset();
			server_message.reset();
			chat_message.rawText = text;
			server_message.rawText = text;
			List<Emote_Entry> list = null;
			if (text[0] == '@')
			{
				text = text.Substring(1);
				while (text.Length > 0 && text[0] != ':')
				{
					int num = text.IndexOf(';');
					int num2 = text.IndexOf(' ');
					int num3 = ((num == -1) ? num2 : ((num < num2) ? num : num2));
					string text2 = text.Substring(0, num3);
					int num4 = text2.IndexOf('=');
					string text3 = text2.Substring(0, num4);
					string text4 = "";
					if (num4 < text2.Length)
					{
						text4 = text2.Substring(num4 + 1, text2.Length - num4 - 1);
					}
					if (text4 != "")
					{
						if (text3[0] == 'c')
						{
							if (text3.Equals("color"))
							{
								chat_message.userNameColor = text4;
							}
						}
						else if (text3[0] == 'd')
						{
							if (text3.Equals("display-name"))
							{
								chat_message.userName = text4;
							}
						}
						else if (text3[0] == 'e')
						{
							if (text3.Equals("emotes"))
							{
								string[] array = text4.Split('/');
								list = new List<Emote_Entry>();
								for (int i = 0; i < array.Length; i++)
								{
									int num5 = array[i].IndexOf(':');
									int id = Convert.ToInt32(array[i].Substring(0, num5));
									string[] array2 = array[i].Substring(num5 + 1).Split(',');
									for (int j = 0; j < array2.Length; j++)
									{
										string[] array3 = array2[j].Split('-');
										Emote_Entry item = default(Emote_Entry);
										item.id = id;
										item.start_index = Convert.ToInt32(array3[0]);
										item.end_index = Convert.ToInt32(array3[1]);
										list.Add(item);
									}
								}
								list.Sort();
							}
						}
						else if (text3[0] == 'm')
						{
							if (text3.Equals("mod"))
							{
								chat_message.isMod = text4[0] == '1';
							}
						}
						else if (text3[0] == 's')
						{
							if (text3.Equals("subscriber"))
							{
								chat_message.isSubscriber = text4[0] == '1';
							}
						}
						else if (text3[0] == 't')
						{
							if (text3.Equals("turbo"))
							{
								chat_message.isTurbo = text4[0] == '1';
							}
						}
						else
						{
							char c = text3[0];
							int num9 = 117;
						}
					}
					text = text.Substring(num3 + 1);
				}
			}
			if (text.IndexOf(C_PING_STRING) != -1)
			{
				server_message.type = TwitchServerMessage.TwitchServerMessageType.Ping;
			}
			else
			{
				if (text.IndexOf(C_JTV_STRING) != -1)
				{
					return;
				}
				if (text.IndexOf(C_TWITCH_SYSTEM_STRING) != -1)
				{
					if (text.IndexOf("001") != -1)
					{
						server_message.type = TwitchServerMessage.TwitchServerMessageType.AuthenticationAcknowledged;
					}
					else if (text.IndexOf("NOTICE * :") != -1)
					{
						if (text.IndexOf("Login unsuccessful") != -1 || text.IndexOf("Login authentication failed") != -1)
						{
							server_message.type = TwitchServerMessage.TwitchServerMessageType.InvalidOAuthTokenError;
						}
						else if (text.IndexOf("Error logging in") != -1 || text.IndexOf("Improperly formatted auth") != -1)
						{
							server_message.type = TwitchServerMessage.TwitchServerMessageType.ImproperlyFormattedCredentialsError;
						}
						else
						{
							server_message.type = TwitchServerMessage.TwitchServerMessageType.ConnectionError;
						}
					}
					else if (text.IndexOf("CAP * ACK") != -1)
					{
						server_message.type = TwitchServerMessage.TwitchServerMessageType.CapabilitiesAcknowledged;
					}
					else
					{
						server_message.type = TwitchServerMessage.TwitchServerMessageType.Unhandled;
					}
				}
				else if (text.IndexOf(C_TWITCH_CHATTER_STRING) != -1)
				{
					server_message.type = TwitchServerMessage.TwitchServerMessageType.TwitchChatter;
				}
				else
				{
					if (text.IndexOf('!') == -1)
					{
						return;
					}
					if (chat_message.userName == null)
					{
						string text5 = text.Substring(1, text.IndexOf('!') - 1);
						chat_message.userName = text5.Substring(0, 1).ToUpper() + text5.Substring(1);
					}
					text = text.Substring(text.IndexOf(' ') + 1);
					string text6 = text.Substring(0, text.IndexOf(' '));
					if (text6.Equals(C_PRIV_MSG_STRING))
					{
						chat_message.type = TwitchChatMessage.TwitchChatMessageType.UserMessage;
					}
					else if (!text6.Equals(C_JOIN_STRING) && !text6.Equals(C_PART_STRING) && text6.Equals(C_WHISPER_STRING))
					{
						chat_message.type = TwitchChatMessage.TwitchChatMessageType.Whisper;
					}
					text = text.Substring(text.IndexOf(' ') + 1);
					if (text.Length > 0 && text[0] == '#')
					{
						chat_message.channelName = text.Substring(1);
					}
					if (chat_message.type == TwitchChatMessage.TwitchChatMessageType.UserMessage)
					{
						chat_message.channelName = text.Substring(1, text.IndexOf(' ') - 1);
					}
					if (chat_message.type != 0 && chat_message.type != TwitchChatMessage.TwitchChatMessageType.Whisper)
					{
						return;
					}
					chat_message.chatMessagePlainText = text.Substring(text.IndexOf(':') + 1);
					chat_message.chatMessageMinusEmotes = string.Copy(chat_message.chatMessagePlainText);
					if (list != null)
					{
						chat_message.emoteData = new TwitchChatMessage.EmoteData[list.Count];
						int num6 = 0;
						for (int k = 0; k < list.Count; k++)
						{
							int num7 = list[k].start_index - num6;
							int num8 = list[k].end_index - list[k].start_index + 1;
							chat_message.chatMessageMinusEmotes = chat_message.chatMessageMinusEmotes.Remove(num7, num8);
							chat_message.emoteData[k] = new TwitchChatMessage.EmoteData();
							chat_message.emoteData[k].id = list[k].id;
							chat_message.emoteData[k].index = num7;
							num6 += num8;
						}
					}
				}
			}
		}
	}
}
