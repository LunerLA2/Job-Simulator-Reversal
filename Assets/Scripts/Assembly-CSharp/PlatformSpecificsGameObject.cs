using OwlchemyVR;
using UnityEngine;

public class PlatformSpecificsGameObject : MonoBehaviour
{
	public VRPlatformTypes supportedPlatformTypes;

	public GameObject validPlatformGameObject;

	public GameObject invalidPlatformGameObject;

	private void Awake()
	{
		if (validPlatformGameObject == null && invalidPlatformGameObject == null)
		{
			validPlatformGameObject = base.gameObject;
		}
		bool flag = IsInMask();
		if (validPlatformGameObject != null)
		{
			validPlatformGameObject.gameObject.SetActive(flag);
		}
		if (invalidPlatformGameObject != null)
		{
			invalidPlatformGameObject.gameObject.SetActive(!flag);
		}
	}

	private bool IsInMask()
	{
		return (int)((uint)supportedPlatformTypes & (uint)(1 << (int)VRPlatform.GetCurrVRPlatformType())) > 0;
	}
}
