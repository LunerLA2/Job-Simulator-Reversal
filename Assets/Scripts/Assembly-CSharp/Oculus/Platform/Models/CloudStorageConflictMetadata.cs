using System;

namespace Oculus.Platform.Models
{
	public class CloudStorageConflictMetadata
	{
		public readonly CloudStorageMetadata Local;

		public readonly CloudStorageMetadata Remote;

		public CloudStorageConflictMetadata(IntPtr o)
		{
			Local = new CloudStorageMetadata(CAPI.ovr_CloudStorageConflictMetadata_GetLocal(o));
			Remote = new CloudStorageMetadata(CAPI.ovr_CloudStorageConflictMetadata_GetRemote(o));
		}
	}
}
