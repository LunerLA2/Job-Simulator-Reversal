using System.Runtime.InteropServices;

namespace UnityEngine.PS4.Engines
{
	public class HandDetection
	{
		[DllImport("HandDetectionPlugin")]
		private static extern int PrxHandDetectionInitialise();

		[DllImport("HandDetectionPlugin")]
		private static extern int PrxHandDetectionShutdown();

		public static int Init()
		{
			return PrxHandDetectionInitialise();
		}

		public static int Term()
		{
			return PrxHandDetectionShutdown();
		}
	}
}
