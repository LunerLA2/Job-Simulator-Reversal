using System;

namespace UnityEngine.PS4.Engines
{
	[Serializable]
	public struct SceDepthTrackingResultValidationInformation
	{
		public int id;

		public SceDepthTrackingResultValidationState validationState;
	}
}
