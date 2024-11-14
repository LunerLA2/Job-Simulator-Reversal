using System;
using Oculus.Platform.Models;

namespace Oculus.Platform
{
	public class MessageWithCloudStorageUpdateResponse : Message<CloudStorageUpdateResponse>
	{
		public MessageWithCloudStorageUpdateResponse(IntPtr c_message)
			: base(c_message)
		{
		}

		public override CloudStorageUpdateResponse GetCloudStorageUpdateResponse()
		{
			return base.Data;
		}

		protected override CloudStorageUpdateResponse GetDataFromMessage(IntPtr c_message)
		{
			IntPtr obj = CAPI.ovr_Message_GetNativeMessage(c_message);
			IntPtr o = CAPI.ovr_Message_GetCloudStorageUpdateResponse(obj);
			return new CloudStorageUpdateResponse(o);
		}
	}
}
