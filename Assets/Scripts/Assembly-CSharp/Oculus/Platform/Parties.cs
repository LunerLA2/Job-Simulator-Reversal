using Oculus.Platform.Models;

namespace Oculus.Platform
{
	public static class Parties
	{
		public static Request<Party> GetCurrent()
		{
			if (Core.IsInitialized())
			{
				return new Request<Party>(CAPI.ovr_Party_GetCurrent());
			}
			return null;
		}
	}
}
