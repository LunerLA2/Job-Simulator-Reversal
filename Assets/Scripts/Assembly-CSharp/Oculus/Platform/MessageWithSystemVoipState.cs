using System;
using Oculus.Platform.Models;

namespace Oculus.Platform
{
	public class MessageWithSystemVoipState : Message<SystemVoipState>
	{
		public MessageWithSystemVoipState(IntPtr c_message)
			: base(c_message)
		{
		}

		public override SystemVoipState GetSystemVoipState()
		{
			return base.Data;
		}

		protected override SystemVoipState GetDataFromMessage(IntPtr c_message)
		{
			IntPtr obj = CAPI.ovr_Message_GetNativeMessage(c_message);
			IntPtr o = CAPI.ovr_Message_GetSystemVoipState(obj);
			return new SystemVoipState(o);
		}
	}
}
