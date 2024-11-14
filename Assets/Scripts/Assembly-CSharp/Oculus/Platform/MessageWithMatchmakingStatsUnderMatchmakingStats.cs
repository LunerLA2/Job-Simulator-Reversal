using System;
using Oculus.Platform.Models;

namespace Oculus.Platform
{
	public class MessageWithMatchmakingStatsUnderMatchmakingStats : Message<MatchmakingStats>
	{
		public MessageWithMatchmakingStatsUnderMatchmakingStats(IntPtr c_message)
			: base(c_message)
		{
		}

		public override MatchmakingStats GetMatchmakingStats()
		{
			return base.Data;
		}

		protected override MatchmakingStats GetDataFromMessage(IntPtr c_message)
		{
			IntPtr obj = CAPI.ovr_Message_GetNativeMessage(c_message);
			IntPtr o = CAPI.ovr_Message_GetMatchmakingStats(obj);
			return new MatchmakingStats(o);
		}
	}
}
