using System;
using Oculus.Platform.Models;

namespace Oculus.Platform
{
	public class MessageWithPingResult : Message<PingResult>
	{
		public MessageWithPingResult(IntPtr c_message)
			: base(c_message)
		{
		}

		public override PingResult GetPingResult()
		{
			return base.Data;
		}

		protected override PingResult GetDataFromMessage(IntPtr c_message)
		{
			IntPtr obj = CAPI.ovr_Message_GetPingResult(c_message);
			bool flag = CAPI.ovr_PingResult_IsTimeout(obj);
			return new PingResult(CAPI.ovr_PingResult_GetID(obj), (!flag) ? new ulong?(CAPI.ovr_PingResult_GetPingTimeUsec(obj)) : null);
		}
	}
}
