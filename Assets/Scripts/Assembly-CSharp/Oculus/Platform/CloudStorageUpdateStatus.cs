using System.ComponentModel;

namespace Oculus.Platform
{
	public enum CloudStorageUpdateStatus : uint
	{
		[Description("UNKNOWN")]
		Unknown = 0u,
		[Description("OK")]
		Ok = 1u,
		[Description("BETTER_VERSION_STORED")]
		BetterVersionStored = 2u,
		[Description("MANUAL_MERGE_REQUIRED")]
		ManualMergeRequired = 3u
	}
}
