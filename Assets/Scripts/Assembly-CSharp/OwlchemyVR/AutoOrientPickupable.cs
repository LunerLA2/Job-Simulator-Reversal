using System;
using UnityEngine;

namespace OwlchemyVR
{
	public class AutoOrientPickupable : MonoBehaviour
	{
		[SerializeField]
		private bool isOrientToPosition = true;

		[SerializeField]
		private bool isOrientToRotation = true;

		[SerializeField]
		private Transform orientToPoint;

		private PickupableItem pickupableItem;

		private Vector3 rotationOffsetForSteamVRTouch = new Vector3(-40f, 0f, 0f);

		private Vector3 positionOffsetForSteamVRTouch = new Vector3(0f, -0.04f, 0.03f);

		private Vector3 rotationOffsetForSteamWindows = new Vector3(-40f, 0f, 0f);

		private Vector3 positionOffsetForSteamWindows = new Vector3(0f, 0f, 0f);

		private void Awake()
		{
			pickupableItem = GetComponent<PickupableItem>();
		}

		private void OnEnable()
		{
			PickupableItem obj = pickupableItem;
			obj.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(obj.OnGrabbed, new Action<GrabbableItem>(Grabbed));
		}

		private void OnDisable()
		{
			PickupableItem obj = pickupableItem;
			obj.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(obj.OnGrabbed, new Action<GrabbableItem>(Grabbed));
		}

		private void Grabbed(GrabbableItem grabbedItem)
		{
			if (isOrientToPosition)
			{
				Vector3 posOffsetFromMountPoint = base.transform.InverseTransformPoint(orientToPoint.transform.position);
				if (VRPlatform.GetCurrVRPlatformType() == VRPlatformTypes.SteamVR)
				{
					if (VRPlatform.GetCurrVRPlatformHardwareType() == VRPlatformHardwareType.OculusRift)
					{
						posOffsetFromMountPoint += positionOffsetForSteamVRTouch;
					}
					else if (VRPlatform.GetCurrVRPlatformHardwareType() == VRPlatformHardwareType.WindowsMR)
					{
						posOffsetFromMountPoint += positionOffsetForSteamWindows;
					}
				}
				pickupableItem.CurrInteractableHand.SnapToPosition(posOffsetFromMountPoint);
			}
			if (!isOrientToRotation)
			{
				return;
			}
			Quaternion rotOffsetFromMountPoint = Quaternion.Inverse(base.transform.rotation) * orientToPoint.transform.rotation;
			if (VRPlatform.GetCurrVRPlatformType() == VRPlatformTypes.SteamVR)
			{
				if (VRPlatform.GetCurrVRPlatformHardwareType() == VRPlatformHardwareType.OculusRift)
				{
					rotOffsetFromMountPoint.eulerAngles += rotationOffsetForSteamVRTouch;
				}
				else if (VRPlatform.GetCurrVRPlatformHardwareType() == VRPlatformHardwareType.WindowsMR)
				{
					rotOffsetFromMountPoint.eulerAngles += rotationOffsetForSteamWindows;
				}
			}
			pickupableItem.CurrInteractableHand.SnapToRotation(rotOffsetFromMountPoint);
		}
	}
}
