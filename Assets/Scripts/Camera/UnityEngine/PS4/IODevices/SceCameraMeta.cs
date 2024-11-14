using System.Runtime.InteropServices;

namespace UnityEngine.PS4.IODevices
{
	[StructLayout(LayoutKind.Sequential)]
	public class SceCameraMeta
	{
		public uint metaMode;

		public uint[,] format;

		public ulong[] frame;

		public ulong[] timestamp;

		public uint[] deviceTimestamp;

		public SceCameraExposureGain[] exposureGain;

		public SceCameraWhiteBalance[] whiteBalance;

		public SceCameraGamma[] gamma;

		public uint[] luminance;

		public SceFVector3 acceleration;

		public uint[] reserved;

		public SceCameraMeta()
		{
			metaMode = 0u;
			format = new uint[2, 4];
			frame = new ulong[2];
			timestamp = new ulong[2];
			deviceTimestamp = new uint[2];
			exposureGain = new SceCameraExposureGain[2];
			whiteBalance = new SceCameraWhiteBalance[2];
			gamma = new SceCameraGamma[2];
			luminance = new uint[2];
			acceleration = new SceFVector3(0f, 0f, 0f);
			reserved = new uint[16];
		}
	}
}
