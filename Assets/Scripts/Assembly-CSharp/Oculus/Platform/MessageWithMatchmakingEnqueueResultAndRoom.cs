using System;
using Oculus.Platform.Models;

namespace Oculus.Platform
{
	public class MessageWithMatchmakingEnqueueResultAndRoom : Message<MatchmakingEnqueueResultAndRoom>
	{
		public MessageWithMatchmakingEnqueueResultAndRoom(IntPtr c_message)
			: base(c_message)
		{
		}

		public override MatchmakingEnqueueResultAndRoom GetMatchmakingEnqueueResultAndRoom()
		{
			return base.Data;
		}

		protected override MatchmakingEnqueueResultAndRoom GetDataFromMessage(IntPtr c_message)
		{
			IntPtr obj = CAPI.ovr_Message_GetNativeMessage(c_message);
			IntPtr o = CAPI.ovr_Message_GetMatchmakingEnqueueResultAndRoom(obj);
			return new MatchmakingEnqueueResultAndRoom(o);
		}
	}
}
