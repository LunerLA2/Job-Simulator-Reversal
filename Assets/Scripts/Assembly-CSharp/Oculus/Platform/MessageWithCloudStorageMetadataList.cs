using System;
using Oculus.Platform.Models;

namespace Oculus.Platform
{
	public class MessageWithCloudStorageMetadataList : Message<CloudStorageMetadataList>
	{
		public MessageWithCloudStorageMetadataList(IntPtr c_message)
			: base(c_message)
		{
		}

		public override CloudStorageMetadataList GetCloudStorageMetadataList()
		{
			return base.Data;
		}

		protected override CloudStorageMetadataList GetDataFromMessage(IntPtr c_message)
		{
			IntPtr obj = CAPI.ovr_Message_GetNativeMessage(c_message);
			IntPtr a = CAPI.ovr_Message_GetCloudStorageMetadataArray(obj);
			return new CloudStorageMetadataList(a);
		}
	}
}
