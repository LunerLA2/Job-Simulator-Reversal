namespace TwitchChatter
{
	public struct TwitchServerMessage
	{
		public enum TwitchServerMessageType
		{
			INVALID = -1,
			TwitchChatter = 0,
			ConnectionError = 1,
			InvalidOAuthTokenError = 2,
			ImproperlyFormattedCredentialsError = 3,
			RoomState = 4,
			Unhandled = 5,
			AuthenticationAcknowledged = 500,
			CapabilitiesAcknowledged = 501,
			Ping = 502
		}

		public TwitchServerMessageType type;

		public string rawText;

		public void reset()
		{
			type = TwitchServerMessageType.INVALID;
			rawText = null;
		}
	}
}
