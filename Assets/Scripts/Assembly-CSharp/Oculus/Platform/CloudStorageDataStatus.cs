using System.ComponentModel;

namespace Oculus.Platform
{
	public enum CloudStorageDataStatus : uint
	{
		[Description("UNKNOWN")]
		Unknown = 0u,
		[Description("IN_SYNC")]
		InSync = 1u,
		[Description("NEEDS_DOWNLOAD")]
		NeedsDownload = 2u,
		[Description("REMOTE_DOWNLOADING")]
		RemoteDownloading = 3u,
		[Description("NEEDS_UPLOAD")]
		NeedsUpload = 4u,
		[Description("LOCAL_UPLOADING")]
		LocalUploading = 5u,
		[Description("IN_CONFLICT")]
		InConflict = 6u
	}
}
