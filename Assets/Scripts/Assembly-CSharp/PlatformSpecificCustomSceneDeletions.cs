using OwlchemyVR;
using UnityEngine;

public class PlatformSpecificCustomSceneDeletions : MonoBehaviour
{
	[SerializeField]
	private VRPlatformTypes platformToDeleteOn = VRPlatformTypes.PSVR;

	public bool IsDeletionPlatform(VRPlatformTypes vrPlatform)
	{
		return platformToDeleteOn == vrPlatform;
	}
}
