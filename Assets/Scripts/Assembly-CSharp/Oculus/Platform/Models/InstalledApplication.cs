using System;

namespace Oculus.Platform.Models
{
	public class InstalledApplication
	{
		public readonly string ApplicationId;

		public readonly string PackageName;

		public readonly int VersionCode;

		public readonly string VersionName;

		public InstalledApplication(IntPtr o)
		{
			ApplicationId = CAPI.ovr_InstalledApplication_GetApplicationId(o);
			PackageName = CAPI.ovr_InstalledApplication_GetPackageName(o);
			VersionCode = CAPI.ovr_InstalledApplication_GetVersionCode(o);
			VersionName = CAPI.ovr_InstalledApplication_GetVersionName(o);
		}
	}
}
