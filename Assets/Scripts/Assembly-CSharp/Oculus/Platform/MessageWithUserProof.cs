using System;
using Oculus.Platform.Models;

namespace Oculus.Platform
{
	public class MessageWithUserProof : Message<UserProof>
	{
		public MessageWithUserProof(IntPtr c_message)
			: base(c_message)
		{
		}

		public override UserProof GetUserProof()
		{
			return base.Data;
		}

		protected override UserProof GetDataFromMessage(IntPtr c_message)
		{
			IntPtr obj = CAPI.ovr_Message_GetNativeMessage(c_message);
			IntPtr o = CAPI.ovr_Message_GetUserProof(obj);
			return new UserProof(o);
		}
	}
}
