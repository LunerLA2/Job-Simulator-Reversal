using Oculus.Platform.Models;

namespace Oculus.Platform
{
	public static class Application
	{
		public static Request<ApplicationVersion> GetVersion()
		{
			if (Core.IsInitialized())
			{
				return new Request<ApplicationVersion>(CAPI.ovr_Application_GetVersion());
			}
			return null;
		}
	}
}
