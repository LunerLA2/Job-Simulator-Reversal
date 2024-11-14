using System;

namespace Oculus.Platform.Models
{
	public class CloudStorageMetadata
	{
		public readonly string Bucket;

		public readonly long Counter;

		public readonly uint DataSize;

		public readonly string ExtraData;

		public readonly string Key;

		public readonly ulong SaveTime;

		public readonly CloudStorageDataStatus Status;

		public readonly string VersionHandle;

		public CloudStorageMetadata(IntPtr o)
		{
			Bucket = CAPI.ovr_CloudStorageMetadata_GetBucket(o);
			Counter = CAPI.ovr_CloudStorageMetadata_GetCounter(o);
			DataSize = CAPI.ovr_CloudStorageMetadata_GetDataSize(o);
			ExtraData = CAPI.ovr_CloudStorageMetadata_GetExtraData(o);
			Key = CAPI.ovr_CloudStorageMetadata_GetKey(o);
			SaveTime = CAPI.ovr_CloudStorageMetadata_GetSaveTime(o);
			Status = CAPI.ovr_CloudStorageMetadata_GetStatus(o);
			VersionHandle = CAPI.ovr_CloudStorageMetadata_GetVersionHandle(o);
		}
	}
}
