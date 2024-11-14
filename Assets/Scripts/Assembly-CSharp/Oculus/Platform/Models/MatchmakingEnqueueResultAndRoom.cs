using System;

namespace Oculus.Platform.Models
{
	public class MatchmakingEnqueueResultAndRoom
	{
		public readonly MatchmakingEnqueueResult MatchmakingEnqueueResult;

		public readonly Room Room;

		public MatchmakingEnqueueResultAndRoom(IntPtr o)
		{
			MatchmakingEnqueueResult = new MatchmakingEnqueueResult(CAPI.ovr_MatchmakingEnqueueResultAndRoom_GetMatchmakingEnqueueResult(o));
			Room = new Room(CAPI.ovr_MatchmakingEnqueueResultAndRoom_GetRoom(o));
		}
	}
}
