using System;
using Oculus.Platform.Models;

namespace Oculus.Platform
{
	public class MessageWithMatchmakingBrowseResult : Message<MatchmakingBrowseResult>
	{
		public MessageWithMatchmakingBrowseResult(IntPtr c_message)
			: base(c_message)
		{
		}

		public override MatchmakingEnqueueResult GetMatchmakingEnqueueResult()
		{
			return base.Data.EnqueueResult;
		}

		public override RoomList GetRoomList()
		{
			return base.Data.Rooms;
		}

		protected override MatchmakingBrowseResult GetDataFromMessage(IntPtr c_message)
		{
			IntPtr obj = CAPI.ovr_Message_GetNativeMessage(c_message);
			IntPtr o = CAPI.ovr_Message_GetMatchmakingBrowseResult(obj);
			return new MatchmakingBrowseResult(o);
		}
	}
}
