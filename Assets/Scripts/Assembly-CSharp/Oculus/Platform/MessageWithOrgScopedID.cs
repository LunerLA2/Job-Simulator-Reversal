using System;
using Oculus.Platform.Models;

namespace Oculus.Platform
{
	public class MessageWithOrgScopedID : Message<OrgScopedID>
	{
		public MessageWithOrgScopedID(IntPtr c_message)
			: base(c_message)
		{
		}

		public override OrgScopedID GetOrgScopedID()
		{
			return base.Data;
		}

		protected override OrgScopedID GetDataFromMessage(IntPtr c_message)
		{
			IntPtr obj = CAPI.ovr_Message_GetNativeMessage(c_message);
			IntPtr o = CAPI.ovr_Message_GetOrgScopedID(obj);
			return new OrgScopedID(o);
		}
	}
}
