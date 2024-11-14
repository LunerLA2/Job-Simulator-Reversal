namespace TwitchChatter
{
	public struct TwitchChatMessage
	{
		public enum TwitchChatMessageType
		{
			INVALID = -1,
			UserMessage = 0,
			Whisper = 1
		}

		public class EmoteData
		{
			public int id;

			public int index;
		}

		public TwitchChatMessageType type;

		public string userName;

		public string channelName;

		public string rawText;

		public string chatMessagePlainText;

		public string chatMessageMinusEmotes;

		public EmoteData[] emoteData;

		public static string[] RandomColors = new string[10] { "#AA64EA", "#00E700", "#3DB974", "#FF7F50", "#359BFF", "#FF5858", "#7D7dE1", "#FF69B4", "#E1762A", "#5F9EA0" };

		public string userNameColor;

		public bool isMod;

		public bool isSubscriber;

		public bool isTurbo;

		public void reset()
		{
			type = TwitchChatMessageType.INVALID;
			userName = null;
			channelName = null;
			rawText = null;
			chatMessageMinusEmotes = null;
			userNameColor = null;
			isMod = false;
			isSubscriber = false;
			isTurbo = false;
		}
	}
}
