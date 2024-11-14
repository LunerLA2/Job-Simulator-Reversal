using System;

namespace Oculus.Platform
{
	public class MessageWithLeaderboardDidUpdate : Message<bool>
	{
		public MessageWithLeaderboardDidUpdate(IntPtr c_message)
			: base(c_message)
		{
		}

		public override bool GetLeaderboardDidUpdate()
		{
			return base.Data;
		}

		protected override bool GetDataFromMessage(IntPtr c_message)
		{
			IntPtr obj = CAPI.ovr_Message_GetNativeMessage(c_message);
			IntPtr obj2 = CAPI.ovr_Message_GetLeaderboardUpdateStatus(obj);
			return CAPI.ovr_LeaderboardUpdateStatus_GetDidUpdate(obj2);
		}
	}
}
