using OwlchemyVR;
using UnityEngine;

public class ControllerConfigurator : MonoBehaviour
{
	[SerializeField]
	private Transform handRotationTransform;

	[SerializeField]
	private Transform handModelContainerTransform;

	[SerializeField]
	private Transform mountOrientationContainer;

	[SerializeField]
	private Transform handRigidbodyTransform;

	[SerializeField]
	private HandController handController;

	[SerializeField]
	private SteamVR_IndividualController viveIndividualController;

	[SerializeField]
	private SteamVRCompatible_IndividualController otherIndividualController;

	private SteamVR_TrackedObject steamVR_TrackedObj;

	private Vector3 handLocalRotationEulerOriginal;

	private bool isLeftHanded = true;

	private Vector3 handModelContainerOriginalLocalPos;

	private Vector3 handRotationTransformOriginalLocalPos;

	private Vector3 handRigidbodyTransformOriginalLocalPos;

	[SerializeField]
	private GameObject customLeftModel;

	[SerializeField]
	private GameObject customRightModel;

	public HandController HandController
	{
		get
		{
			return handController;
		}
	}

	public SteamVR_TrackedObject SteamVR_TrackedObj
	{
		get
		{
			return steamVR_TrackedObj;
		}
	}

	public bool IsLeftHanded
	{
		get
		{
			return isLeftHanded;
		}
	}

	private void Awake()
	{
		handLocalRotationEulerOriginal = handRotationTransform.localEulerAngles;
		handModelContainerOriginalLocalPos = handModelContainerTransform.localPosition;
		handRotationTransformOriginalLocalPos = handRotationTransform.localPosition;
		handRigidbodyTransformOriginalLocalPos = handRigidbodyTransform.localPosition;
	}

	public void Init(int index)
	{
		steamVR_TrackedObj = base.gameObject.AddComponent<SteamVR_TrackedObject>();
		steamVR_TrackedObj.index = (SteamVR_TrackedObject.EIndex)index;
		base.gameObject.name = "HandUnknown";
		if (VRPlatform.GetCurrVRPlatformType() == VRPlatformTypes.SteamVR && VRPlatform.GetCurrVRPlatformHardwareType() != VRPlatformHardwareType.Vive)
		{
			otherIndividualController.Setup(steamVR_TrackedObj);
		}
		else if (viveIndividualController != null)
		{
			viveIndividualController.Setup(steamVR_TrackedObj);
		}
	}

	public void SetHandedness(bool leftHanded)
	{
		if (leftHanded)
		{
			handRigidbodyTransform.localPosition = handRigidbodyTransformOriginalLocalPos;
			handRotationTransform.localPosition = handRotationTransformOriginalLocalPos;
			handRotationTransform.localPosition = handRotationTransformOriginalLocalPos;
			handRotationTransform.localEulerAngles = handLocalRotationEulerOriginal;
			handModelContainerTransform.localScale = Vector3.one;
			handModelContainerTransform.localPosition = handModelContainerOriginalLocalPos;
			mountOrientationContainer.localScale = Vector3.one;
			isLeftHanded = true;
			base.gameObject.name = "HandLeft";
			if (customLeftModel != null)
			{
				customLeftModel.gameObject.SetActive(leftHanded);
			}
			if (customRightModel != null)
			{
				customRightModel.gameObject.SetActive(!leftHanded);
			}
			return;
		}
		Vector3 localPosition = handRigidbodyTransformOriginalLocalPos;
		localPosition.x = 0f - localPosition.x;
		handRigidbodyTransform.localPosition = localPosition;
		Vector3 localPosition2 = handRotationTransformOriginalLocalPos;
		localPosition2.x = 0f - localPosition2.x;
		handRotationTransform.localPosition = localPosition2;
		Vector3 localEulerAngles = handLocalRotationEulerOriginal;
		localEulerAngles.z = 180f - localEulerAngles.z;
		handRotationTransform.localEulerAngles = localEulerAngles;
		handModelContainerTransform.localScale = new Vector3(-1f, 1f, 1f);
		Vector3 localPosition3 = handModelContainerOriginalLocalPos;
		localPosition3.y = 0f - handModelContainerOriginalLocalPos.y;
		handModelContainerTransform.localPosition = localPosition3;
		mountOrientationContainer.localScale = new Vector3(-1f, 1f, 1f);
		isLeftHanded = false;
		base.gameObject.name = "HandRight";
		if (customLeftModel != null)
		{
			customLeftModel.gameObject.SetActive(leftHanded);
		}
		if (customRightModel != null)
		{
			customRightModel.gameObject.SetActive(!leftHanded);
		}
	}
}
