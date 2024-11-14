using System;

namespace UnityEngine.PS4.IODevices
{
	public class SceCameraFrameData
	{
		public uint sizeThis;

		public uint readMode;

		public SceCameraFramePosition[,] framePosition;

		public IntPtr[,] pFramePointerList;

		public uint[,] frameSize;

		public uint[] status;

		public SceCameraMeta meta;

		public SceCameraFrameData()
		{
			sizeThis = 488u;
			framePosition = new SceCameraFramePosition[2, 4];
			pFramePointerList = new IntPtr[2, 4];
			frameSize = new uint[2, 4];
			status = new uint[2];
			meta = new SceCameraMeta();
		}
	}
}
