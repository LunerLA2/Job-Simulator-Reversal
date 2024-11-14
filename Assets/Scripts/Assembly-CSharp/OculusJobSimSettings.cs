using OwlchemyVR;
using UnityEngine;

public class OculusJobSimSettings : MonoBehaviour
{
	private void Start()
	{
		if (VRPlatform.GetCurrVRPlatformType() == VRPlatformTypes.Oculus && OVRManager.instance != null)
		{
			OVRManager.instance.queueAhead = true;
		}
	}
}
