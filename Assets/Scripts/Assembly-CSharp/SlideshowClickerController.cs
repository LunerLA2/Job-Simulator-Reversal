using OwlchemyVR;
using UnityEngine;

public class SlideshowClickerController : MonoBehaviour
{
	[SerializeField]
	private WorldItem worldItem;

	[SerializeField]
	private PickupableItem pickupableItem;

	[SerializeField]
	private GrabbableSlider buttonSlider;

	private int clicked;

	public bool Clicked
	{
		get
		{
			return clicked > 0;
		}
	}

	private void OnEnable()
	{
		if (OfficeManager.Instance != null && OfficeManager.Instance.SlideshowPresentation != null)
		{
			OfficeManager.Instance.SlideshowPresentation.ConnectClicker(this);
		}
		else
		{
			Debug.LogError("Failed to connect clicker: Cannot find slideshow presentation.");
		}
		clicked = 0;
	}

	private void Update()
	{
		if (VRPlatform.GetCurrVRPlatformHardwareType() != VRPlatformHardwareType.OculusRift)
		{
			return;
		}
		bool flag = false;
		if (pickupableItem.IsCurrInHand)
		{
			if (VRPlatform.GetCurrVRPlatformType() == VRPlatformTypes.Oculus)
			{
				OculusTouch_IndividualController oculusTouchController = pickupableItem.CurrInteractableHand.HandController.OculusTouchController;
				if (oculusTouchController.ControllerType == OVRInput.Controller.LTouch)
				{
					if (OVRInput.Get(OVRInput.Button.Three | OVRInput.Button.PrimaryIndexTrigger | OVRInput.Button.PrimaryThumbstick))
					{
						flag = true;
					}
				}
				else if (OVRInput.Get(OVRInput.Button.One | OVRInput.Button.SecondaryIndexTrigger | OVRInput.Button.SecondaryThumbstick))
				{
					flag = true;
				}
			}
			else if (VRPlatform.GetCurrVRPlatformType() == VRPlatformTypes.SteamVR)
			{
				SteamVRCompatible_IndividualController steamVRCompatibleController = pickupableItem.CurrInteractableHand.HandController.SteamVRCompatibleController;
				if (steamVRCompatibleController.IsThumbPressed() || steamVRCompatibleController.IsTriggerPressed())
				{
					flag = true;
				}
			}
		}
		else if (buttonSlider.IsLowerLocked)
		{
			buttonSlider.Unlock();
		}
		if (flag)
		{
			Click();
			buttonSlider.SetLowerLockType(GrabbableSlider.LockType.Permanent);
			buttonSlider.LockLower();
		}
		else
		{
			buttonSlider.Unlock();
		}
	}

	private void LateUpdate()
	{
		if (clicked > 0)
		{
			clicked--;
		}
	}

	public void Click()
	{
		if (pickupableItem.IsCurrInHand)
		{
			GameEventsManager.Instance.ItemActionOccurred(worldItem.Data, "ACTIVATED");
			clicked = 2;
		}
	}
}
