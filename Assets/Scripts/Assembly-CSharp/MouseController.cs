using System;
using OwlchemyVR;
using UnityEngine;

public class MouseController : MonoBehaviour
{
	private const int HAPTICS_CLICK_PULSE_RATE_MCIRO_SEC = 650;

	private const float HAPTICS_CLICK_LENGTH_SECONDS = 0.02f;

	[SerializeField]
	private GrabbableItem grabbableItem;

	[SerializeField]
	private Animation clickAnimation;

	[SerializeField]
	private AnimationClip clickDownClip;

	[SerializeField]
	private AnimationClip clickUpClip;

	[SerializeField]
	private AudioClip clickSound;

	[SerializeField]
	private MeshRenderer clickHighlight;

	private bool isClicked;

	private bool heldForOneFrame;

	private HapticInfoObject hapticObject;

	public Action OnClicked;

	public Action OnClickUp;

	private void OnEnable()
	{
		GrabbableItem obj = grabbableItem;
		obj.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(obj.OnGrabbed, new Action<GrabbableItem>(Grabbed));
		GrabbableItem obj2 = grabbableItem;
		obj2.OnReleased = (Action<GrabbableItem>)Delegate.Combine(obj2.OnReleased, new Action<GrabbableItem>(Released));
	}

	private void OnDisable()
	{
		GrabbableItem obj = grabbableItem;
		obj.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(obj.OnGrabbed, new Action<GrabbableItem>(Grabbed));
		GrabbableItem obj2 = grabbableItem;
		obj2.OnReleased = (Action<GrabbableItem>)Delegate.Remove(obj2.OnReleased, new Action<GrabbableItem>(Released));
	}

	private void Grabbed(GrabbableItem item)
	{
	}

	private void Released(GrabbableItem item)
	{
		if (hapticObject.IsRunning && item.CurrInteractableHand != null)
		{
			item.CurrInteractableHand.HapticsController.RemoveHaptic(hapticObject);
		}
	}

	private void Awake()
	{
		float pulseRateMicroSec = 650f;
		hapticObject = new HapticInfoObject(pulseRateMicroSec, 0.02f);
		hapticObject.DeactiveHaptic();
	}

	private void Update()
	{
		clickHighlight.enabled = !isClicked && grabbableItem.IsCurrInHand;
		bool flag = false;
		if (grabbableItem.IsCurrInHand)
		{
			if (heldForOneFrame)
			{
				if (VRPlatform.GetCurrVRPlatformHardwareType() == VRPlatformHardwareType.OculusRift)
				{
					if (grabbableItem.CurrInteractableHand.IsSqueezedButtonDown() || flag)
					{
						ClickDown();
					}
					else if (grabbableItem.CurrInteractableHand.IsSqueezedButtonUp() || flag)
					{
						ClickUp();
					}
				}
				else if (grabbableItem.CurrInteractableHand.IsGrabInputButtonDown() || flag)
				{
					ClickDown();
				}
				else if (grabbableItem.CurrInteractableHand.IsGrabInputButtonUp() || flag)
				{
					ClickUp();
				}
			}
			else
			{
				heldForOneFrame = true;
			}
		}
		else
		{
			heldForOneFrame = false;
		}
	}

	private void ClickDown()
	{
		hapticObject.Restart();
		if (!grabbableItem.CurrInteractableHand.HapticsController.ContainHaptic(hapticObject))
		{
			grabbableItem.CurrInteractableHand.HapticsController.AddNewHaptic(hapticObject);
		}
		clickAnimation.Stop();
		clickAnimation.clip = clickDownClip;
		clickAnimation.Play();
		AudioManager.Instance.Play(base.transform.position, clickSound, 1f, 1f);
		if (OnClicked != null)
		{
			OnClicked();
		}
		isClicked = true;
	}

	private void ClickUp()
	{
		clickAnimation.Stop();
		clickAnimation.clip = clickUpClip;
		clickAnimation.Play();
		if (OnClickUp != null)
		{
			OnClickUp();
		}
		isClicked = false;
	}
}
