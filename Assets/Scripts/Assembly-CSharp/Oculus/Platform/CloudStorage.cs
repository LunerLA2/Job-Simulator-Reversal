using Oculus.Platform.Models;
using UnityEngine;

namespace Oculus.Platform
{
	public static class CloudStorage
	{
		public static Request<CloudStorageUpdateResponse> Delete(string bucket, string key)
		{
			if (Core.IsInitialized())
			{
				return new Request<CloudStorageUpdateResponse>(CAPI.ovr_CloudStorage_Delete(bucket, key));
			}
			return null;
		}

		public static Request<CloudStorageData> Load(string bucket, string key)
		{
			if (Core.IsInitialized())
			{
				return new Request<CloudStorageData>(CAPI.ovr_CloudStorage_Load(bucket, key));
			}
			return null;
		}

		public static Request<CloudStorageMetadataList> LoadBucketMetadata(string bucket)
		{
			if (Core.IsInitialized())
			{
				return new Request<CloudStorageMetadataList>(CAPI.ovr_CloudStorage_LoadBucketMetadata(bucket));
			}
			return null;
		}

		public static Request<CloudStorageConflictMetadata> LoadConflictMetadata(string bucket, string key)
		{
			if (Core.IsInitialized())
			{
				return new Request<CloudStorageConflictMetadata>(CAPI.ovr_CloudStorage_LoadConflictMetadata(bucket, key));
			}
			return null;
		}

		public static Request<CloudStorageData> LoadHandle(string handle)
		{
			if (Core.IsInitialized())
			{
				return new Request<CloudStorageData>(CAPI.ovr_CloudStorage_LoadHandle(handle));
			}
			return null;
		}

		public static Request<CloudStorageMetadata> LoadMetadata(string bucket, string key)
		{
			if (Core.IsInitialized())
			{
				return new Request<CloudStorageMetadata>(CAPI.ovr_CloudStorage_LoadMetadata(bucket, key));
			}
			return null;
		}

		public static Request<CloudStorageUpdateResponse> ResolveKeepLocal(string bucket, string key, string remoteHandle)
		{
			if (Core.IsInitialized())
			{
				return new Request<CloudStorageUpdateResponse>(CAPI.ovr_CloudStorage_ResolveKeepLocal(bucket, key, remoteHandle));
			}
			return null;
		}

		public static Request<CloudStorageUpdateResponse> ResolveKeepRemote(string bucket, string key, string remoteHandle)
		{
			if (Core.IsInitialized())
			{
				return new Request<CloudStorageUpdateResponse>(CAPI.ovr_CloudStorage_ResolveKeepRemote(bucket, key, remoteHandle));
			}
			return null;
		}

		public static Request<CloudStorageUpdateResponse> Save(string bucket, string key, byte[] data, long counter, string extraData)
		{
			if (Core.IsInitialized())
			{
				return new Request<CloudStorageUpdateResponse>(CAPI.ovr_CloudStorage_Save(bucket, key, data, (uint)data.Length, counter, extraData));
			}
			return null;
		}

		public static Request<CloudStorageMetadataList> GetNextCloudStorageMetadataListPage(CloudStorageMetadataList list)
		{
			if (!list.HasNextPage)
			{
				Debug.LogWarning("Oculus.Platform.GetNextCloudStorageMetadataListPage: List has no next page");
				return null;
			}
			if (Core.IsInitialized())
			{
				return new Request<CloudStorageMetadataList>(CAPI.ovr_HTTP_GetWithMessageType(list.NextUrl, 1544004335));
			}
			return null;
		}
	}
}
