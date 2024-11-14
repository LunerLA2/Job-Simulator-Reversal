using System;
using Oculus.Platform.Models;

namespace Oculus.Platform
{
	public class MessageWithCloudStorageConflictMetadata : Message<CloudStorageConflictMetadata>
	{
		public MessageWithCloudStorageConflictMetadata(IntPtr c_message)
			: base(c_message)
		{
		}

		public override CloudStorageConflictMetadata GetCloudStorageConflictMetadata()
		{
			return base.Data;
		}

		protected override CloudStorageConflictMetadata GetDataFromMessage(IntPtr c_message)
		{
			IntPtr obj = CAPI.ovr_Message_GetNativeMessage(c_message);
			IntPtr o = CAPI.ovr_Message_GetCloudStorageConflictMetadata(obj);
			return new CloudStorageConflictMetadata(o);
		}
	}
}
