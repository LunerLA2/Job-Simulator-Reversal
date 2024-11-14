using System;
using Oculus.Platform.Models;

namespace Oculus.Platform
{
	public class MessageWithCloudStorageMetadataUnderLocal : Message<CloudStorageMetadata>
	{
		public MessageWithCloudStorageMetadataUnderLocal(IntPtr c_message)
			: base(c_message)
		{
		}

		public override CloudStorageMetadata GetCloudStorageMetadata()
		{
			return base.Data;
		}

		protected override CloudStorageMetadata GetDataFromMessage(IntPtr c_message)
		{
			IntPtr obj = CAPI.ovr_Message_GetNativeMessage(c_message);
			IntPtr o = CAPI.ovr_Message_GetCloudStorageMetadata(obj);
			return new CloudStorageMetadata(o);
		}
	}
}
