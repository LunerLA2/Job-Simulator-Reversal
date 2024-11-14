using System;
using Oculus.Platform.Models;

namespace Oculus.Platform
{
	public class MessageWithMatchmakingEnqueueResult : Message<MatchmakingEnqueueResult>
	{
		public MessageWithMatchmakingEnqueueResult(IntPtr c_message)
			: base(c_message)
		{
		}

		public override MatchmakingEnqueueResult GetMatchmakingEnqueueResult()
		{
			return base.Data;
		}

		protected override MatchmakingEnqueueResult GetDataFromMessage(IntPtr c_message)
		{
			IntPtr obj = CAPI.ovr_Message_GetNativeMessage(c_message);
			IntPtr o = CAPI.ovr_Message_GetMatchmakingEnqueueResult(obj);
			return new MatchmakingEnqueueResult(o);
		}
	}
}
