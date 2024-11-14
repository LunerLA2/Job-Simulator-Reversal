using System;
using Oculus.Platform.Models;

namespace Oculus.Platform
{
	public class MessageWithCloudStorageData : Message<CloudStorageData>
	{
		public MessageWithCloudStorageData(IntPtr c_message)
			: base(c_message)
		{
		}

		public override CloudStorageData GetCloudStorageData()
		{
			return base.Data;
		}

		protected override CloudStorageData GetDataFromMessage(IntPtr c_message)
		{
			IntPtr obj = CAPI.ovr_Message_GetNativeMessage(c_message);
			IntPtr o = CAPI.ovr_Message_GetCloudStorageData(obj);
			return new CloudStorageData(o);
		}
	}
}
