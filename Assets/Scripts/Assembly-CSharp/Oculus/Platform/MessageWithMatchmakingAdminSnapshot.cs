using System;
using Oculus.Platform.Models;

namespace Oculus.Platform
{
	public class MessageWithMatchmakingAdminSnapshot : Message<MatchmakingAdminSnapshot>
	{
		public MessageWithMatchmakingAdminSnapshot(IntPtr c_message)
			: base(c_message)
		{
		}

		public override MatchmakingAdminSnapshot GetMatchmakingAdminSnapshot()
		{
			return base.Data;
		}

		protected override MatchmakingAdminSnapshot GetDataFromMessage(IntPtr c_message)
		{
			IntPtr obj = CAPI.ovr_Message_GetNativeMessage(c_message);
			IntPtr o = CAPI.ovr_Message_GetMatchmakingAdminSnapshot(obj);
			return new MatchmakingAdminSnapshot(o);
		}
	}
}
