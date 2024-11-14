using System;

namespace UnityEngine.PS4.Engines
{
	[Serializable]
	public struct SceDepthHeadCandidateTrackingResult
	{
		public float x;

		public float y;

		public float width;

		public float height;

		public float distanceFromCamera;

		public int id;

		public SceDepthTrackingState state;
	}
}
