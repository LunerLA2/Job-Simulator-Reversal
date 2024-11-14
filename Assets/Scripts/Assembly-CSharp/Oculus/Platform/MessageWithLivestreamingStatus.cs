using System;
using Oculus.Platform.Models;

namespace Oculus.Platform
{
	public class MessageWithLivestreamingStatus : Message<LivestreamingStatus>
	{
		public MessageWithLivestreamingStatus(IntPtr c_message)
			: base(c_message)
		{
		}

		public override LivestreamingStatus GetLivestreamingStatus()
		{
			return base.Data;
		}

		protected override LivestreamingStatus GetDataFromMessage(IntPtr c_message)
		{
			IntPtr obj = CAPI.ovr_Message_GetNativeMessage(c_message);
			IntPtr o = CAPI.ovr_Message_GetLivestreamingStatus(obj);
			return new LivestreamingStatus(o);
		}
	}
}
