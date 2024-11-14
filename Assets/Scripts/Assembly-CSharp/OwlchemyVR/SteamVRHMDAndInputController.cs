using UnityEngine;
using Valve.VR;

namespace OwlchemyVR
{
	public class SteamVRHMDAndInputController : MasterHMDAndInputController
	{
		[SerializeField]
		protected GameObject controllerPrefabOld;

		[SerializeField]
		protected GameObject controllerPrefabOculus;

		[SerializeField]
		protected GameObject controllerPrefabWindows;

		[SerializeField]
		protected GameObject controllerPrefabSteamVRCompatible;

		private bool firstHandOk;

		private bool secondHandOk;

		public override MonoBehaviour[] TrackingScripts
		{
			get
			{
				return GetComponentsInChildren<SteamVR_TrackedObject>();
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			SteamVR_Utils.Event.Listen("device_connected", OnDeviceConnected);
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			SteamVR_Utils.Event.Remove("device_connected", OnDeviceConnected);
		}

		public override void Awake()
		{
			SteamVR instance = SteamVR.instance;
			bool eventDataValue = false;
			if (instance != null)
			{
				eventDataValue = true;
				if (controllerPrefabOld != null && instance.hmd_ModelNumber == "Vive Developer Edition")
				{
					controllerPrefab = controllerPrefabOld;
				}
				else if (VRPlatform.GetCurrVRPlatformHardwareType() == VRPlatformHardwareType.WindowsMR)
				{
					controllerPrefab = controllerPrefabWindows;
				}
				else if (VRPlatform.GetCurrVRPlatformHardwareType() == VRPlatformHardwareType.OculusRift)
				{
					controllerPrefab = controllerPrefabOculus;
				}
				else if (VRPlatform.GetCurrVRPlatformHardwareType() != VRPlatformHardwareType.Vive)
				{
					controllerPrefab = controllerPrefabSteamVRCompatible;
				}
				SteamVR_Render.instance.lockPhysicsUpdateRateToRenderFrequency = false;
				Object.DontDestroyOnLoad(SteamVR_Render.instance.gameObject);
			}
			AnalyticsManager.CustomEvent("Headset Connected", "Connected", eventDataValue);
			base.Awake();
		}

		public override void Start()
		{
			trackedHmdTransform = Camera.main.transform;
		}

		private void OnDeviceConnected(params object[] args)
		{
			int num = (int)args[0];
			SteamVR instance = SteamVR.instance;
			if (instance.hmd.GetTrackedDeviceClass((uint)num) == ETrackedDeviceClass.Controller && !DoesControllerIndexExist(num) && (bool)args[1])
			{
				SetupController(num);
			}
		}

		protected override void SetupCorrectHandedness()
		{
			if (controllersConfigs.Count == 2)
			{
				bool isValid = controllersConfigs[0].SteamVR_TrackedObj.isValid;
				bool isValid2 = controllersConfigs[1].SteamVR_TrackedObj.isValid;
				if (isValid && isValid2)
				{
					SetupHandedness();
				}
				SetupRightAndLeftHandPointers();
				isHMDAndInputReady = true;
			}
		}

		private void SetUpHandednessVive()
		{
			int deviceIndex = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost);
			if (deviceIndex >= 0)
			{
				if (controllersConfigs[0].SteamVR_TrackedObj.index == (SteamVR_TrackedObject.EIndex)deviceIndex)
				{
					controllersConfigs[0].SetHandedness(true);
					controllersConfigs[1].SetHandedness(false);
					leftHandController = controllersConfigs[0].GetComponent<InteractionHandController>();
					rightHandController = controllersConfigs[1].GetComponent<InteractionHandController>();
				}
				else if (controllersConfigs[1].SteamVR_TrackedObj.index == (SteamVR_TrackedObject.EIndex)deviceIndex)
				{
					controllersConfigs[0].SetHandedness(false);
					controllersConfigs[1].SetHandedness(true);
					rightHandController = controllersConfigs[0].GetComponent<InteractionHandController>();
					leftHandController = controllersConfigs[1].GetComponent<InteractionHandController>();
				}
				else
				{
					Debug.LogWarning("Left Most Controller index did not match up with actual controllers");
					controllersConfigs[0].SetHandedness(false);
					controllersConfigs[1].SetHandedness(true);
					rightHandController = controllersConfigs[0].GetComponent<InteractionHandController>();
					leftHandController = controllersConfigs[1].GetComponent<InteractionHandController>();
				}
				if (leftHandController != null)
				{
					trackedLeftHandTransform = leftHandController.GetComponentInParent<SteamVR_TrackedObject>().transform;
				}
				if (rightHandController != null)
				{
					trackedRightHandTransform = rightHandController.GetComponentInParent<SteamVR_TrackedObject>().transform;
				}
			}
		}

		private void SetupHandedness()
		{
			if (!firstHandOk && controllersConfigs[0].SteamVR_TrackedObj.index != SteamVR_TrackedObject.EIndex.None)
			{
				uint index = (uint)controllersConfigs[0].SteamVR_TrackedObj.index;
				switch (OpenVR.System.GetControllerRoleForTrackedDeviceIndex(index))
				{
				case ETrackedControllerRole.LeftHand:
					controllersConfigs[0].SetHandedness(true);
					firstHandOk = true;
					leftHandController = controllersConfigs[0].GetComponent<InteractionHandController>();
					break;
				default:
					controllersConfigs[0].SetHandedness(false);
					firstHandOk = true;
					rightHandController = controllersConfigs[0].GetComponent<InteractionHandController>();
					break;
				case ETrackedControllerRole.Invalid:
					firstHandOk = false;
					break;
				}
			}
			if (!secondHandOk && controllersConfigs[1].SteamVR_TrackedObj.index != SteamVR_TrackedObject.EIndex.None)
			{
				uint index2 = (uint)controllersConfigs[1].SteamVR_TrackedObj.index;
				switch (OpenVR.System.GetControllerRoleForTrackedDeviceIndex(index2))
				{
				case ETrackedControllerRole.LeftHand:
					controllersConfigs[1].SetHandedness(true);
					secondHandOk = true;
					leftHandController = controllersConfigs[1].GetComponent<InteractionHandController>();
					break;
				default:
					controllersConfigs[1].SetHandedness(false);
					secondHandOk = true;
					rightHandController = controllersConfigs[1].GetComponent<InteractionHandController>();
					break;
				case ETrackedControllerRole.Invalid:
					secondHandOk = false;
					break;
				}
			}
			if (firstHandOk && secondHandOk)
			{
				hasHandednessBeenSet = true;
			}
			else
			{
				hasHandednessBeenSet = false;
			}
		}
	}
}
