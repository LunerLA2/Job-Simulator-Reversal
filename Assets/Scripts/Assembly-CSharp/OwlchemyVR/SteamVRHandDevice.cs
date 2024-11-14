using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Valve.VR;

namespace OwlchemyVR
{
	public class SteamVRHandDevice : MonoBehaviour, IHandDevice
	{
		private const float TRIGGER_ENGAGED_PERCENTAGE = 0.3f;

		private const int TRIGGER_R_AXIS = 1;

		private PlayerController playerController;

		private List<SteamVR_TrackedObject> trackedObjs;

		private SteamVR_TrackedObject rhTrackedObj;

		private SteamVR_TrackedObject lhTrackedObj;

		private void Awake()
		{
			trackedObjs = new List<SteamVR_TrackedObject>();
			SteamVR_Utils.Event.Listen("device_connected", OnDeviceConnected);
		}

		private void OnDestroy()
		{
			SteamVR_Utils.Event.Remove("device_connected", OnDeviceConnected);
		}

		private void OnEnable()
		{
			playerController = Object.FindObjectOfType<PlayerController>();
		}

		private void OnDisable()
		{
		}

		private SteamVR_TrackedObject GetTrackedObject(Handedness handedness)
		{
			return (handedness != Handedness.Right) ? lhTrackedObj : rhTrackedObj;
		}

		public HandState GetSuggestedHandState(Hand hand)
		{
			HandState handState = new HandState();
			SteamVR_TrackedObject trackedObject = GetTrackedObject(hand.Handedness);
			if (trackedObject != null)
			{
				if (trackedObject.isValid)
				{
					handState.Position = trackedObject.transform.position;
					handState.EulerAngles = trackedObject.transform.eulerAngles;
				}
				else
				{
					handState.Position = hand.transform.position;
					handState.EulerAngles = hand.transform.eulerAngles;
				}
				VRControllerState_t pControllerState = default(VRControllerState_t);
				SteamVR.instance.hmd.GetControllerState((uint)trackedObject.index, ref pControllerState);
				handState.CommonButtons[0] = (handState.CustomButtons[0] = pControllerState.rAxis1.x >= 0.3f);
				handState.CommonButtons[1] = (handState.CustomButtons[1] = (pControllerState.ulButtonPressed & 4) != 0);
				handState.CustomButtons[2] = (pControllerState.ulButtonPressed & 0x100000000L) != 0;
				handState.CustomButtons[3] = (pControllerState.ulButtonTouched & 0x100000000L) != 0;
				handState.CustomButtons[4] = (pControllerState.ulButtonTouched & 2) != 0;
				handState.CommonAxes[0] = (handState.CustomAxes[0] = pControllerState.rAxis0.x);
				handState.CommonAxes[1] = (handState.CustomAxes[0] = pControllerState.rAxis0.y);
			}
			else
			{
				handState.Position = hand.transform.position;
				handState.EulerAngles = hand.transform.eulerAngles;
			}
			return handState;
		}

		private void OnDeviceConnected(params object[] args)
		{
			int deviceIndex = (int)args[0];
			if (SteamVR.instance.hmd.GetTrackedDeviceClass((uint)deviceIndex) != ETrackedDeviceClass.Controller)
			{
				return;
			}
			if ((bool)args[1])
			{
				GameObject gameObject = new GameObject("SteamVR_TrackedDevice_" + deviceIndex);
				gameObject.transform.SetParent(base.transform, false);
				SteamVR_TrackedObject steamVR_TrackedObject = gameObject.AddComponent<SteamVR_TrackedObject>();
				steamVR_TrackedObject.index = (SteamVR_TrackedObject.EIndex)deviceIndex;
				steamVR_TrackedObject.origin = base.transform;
				while (steamVR_TrackedObject.origin.parent != null)
				{
					steamVR_TrackedObject.origin = steamVR_TrackedObject.origin.parent;
				}
				trackedObjs.Add(steamVR_TrackedObject);
			}
			else
			{
				SteamVR_TrackedObject steamVR_TrackedObject2 = trackedObjs.FirstOrDefault((SteamVR_TrackedObject o) => o.index == (SteamVR_TrackedObject.EIndex)deviceIndex);
				if (steamVR_TrackedObject2 != null)
				{
					trackedObjs.Remove(steamVR_TrackedObject2);
					if (rhTrackedObj == steamVR_TrackedObject2)
					{
						rhTrackedObj = null;
					}
					else if (lhTrackedObj == steamVR_TrackedObject2)
					{
						lhTrackedObj = null;
					}
					Object.Destroy(steamVR_TrackedObject2.gameObject);
				}
			}
			DetermineHandedness();
		}

		public void DetermineHandedness()
		{
			SteamVR_TrackedObject[] array = trackedObjs.Where((SteamVR_TrackedObject o) => o.isValid).ToArray();
			if (array.Length >= 2)
			{
				int leftMostDeviceIndex = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost);
				lhTrackedObj = array.FirstOrDefault((SteamVR_TrackedObject o) => o.index == (SteamVR_TrackedObject.EIndex)leftMostDeviceIndex);
				rhTrackedObj = array.FirstOrDefault((SteamVR_TrackedObject o) => o != lhTrackedObj);
			}
			else if (array.Length == 1)
			{
				if (playerController.Hmd.transform.InverseTransformPoint(trackedObjs[0].transform.position).x > 0f)
				{
					rhTrackedObj = trackedObjs[0];
					lhTrackedObj = null;
				}
				else
				{
					rhTrackedObj = null;
					lhTrackedObj = trackedObjs[0];
				}
			}
			else
			{
				Debug.Log("No controllers detected. Handedness calibration failed.");
			}
		}

		public void TriggerHapticPulse(Hand hand, float pulseRate)
		{
			SteamVR_TrackedObject trackedObject = GetTrackedObject(hand.Handedness);
			if (trackedObject != null)
			{
				SteamVR.instance.hmd.TriggerHapticPulse((uint)trackedObject.index, 0u, (char)pulseRate);
			}
		}

		private void Update()
		{
		}
	}
}
