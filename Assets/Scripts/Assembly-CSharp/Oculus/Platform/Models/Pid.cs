using System;

namespace Oculus.Platform.Models
{
	public class Pid
	{
		public readonly string Id;

		public Pid(IntPtr o)
		{
			Id = CAPI.ovr_Pid_GetId(o);
		}
	}
}
