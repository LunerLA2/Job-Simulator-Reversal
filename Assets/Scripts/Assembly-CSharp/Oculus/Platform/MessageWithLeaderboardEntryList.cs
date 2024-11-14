using System;
using Oculus.Platform.Models;

namespace Oculus.Platform
{
	public class MessageWithLeaderboardEntryList : Message<LeaderboardEntryList>
	{
		public MessageWithLeaderboardEntryList(IntPtr c_message)
			: base(c_message)
		{
		}

		public override LeaderboardEntryList GetLeaderboardEntryList()
		{
			return base.Data;
		}

		protected override LeaderboardEntryList GetDataFromMessage(IntPtr c_message)
		{
			IntPtr obj = CAPI.ovr_Message_GetNativeMessage(c_message);
			IntPtr a = CAPI.ovr_Message_GetLeaderboardEntryArray(obj);
			return new LeaderboardEntryList(a);
		}
	}
}
