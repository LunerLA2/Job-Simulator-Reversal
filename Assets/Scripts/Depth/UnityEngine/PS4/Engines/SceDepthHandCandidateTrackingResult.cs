using System;

namespace UnityEngine.PS4.Engines
{
	[Serializable]
	public struct SceDepthHandCandidateTrackingResult
	{
		public float x;

		public float y;

		public float distanceFromCamera;

		public int id;

		public SceDepthTrackingState state;
	}
}
