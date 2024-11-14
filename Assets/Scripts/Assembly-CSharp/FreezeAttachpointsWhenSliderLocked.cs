using System.Collections;
using OwlchemyVR2;
using UnityEngine;

public class FreezeAttachpointsWhenSliderLocked : MonoBehaviour
{
	private enum FreezeTypes
	{
		DeactivateAttachpoint = 0,
		DisableInteraction = 1
	}

	[SerializeField]
	private GrabbableSlider slider;

	[SerializeField]
	private OwlchemyVR2.GrabbableSlider owlVR2Slider;

	[SerializeField]
	private AttachablePoint[] pointsToFreeze;

	[SerializeField]
	private FreezeTypes freezeType;

	[SerializeField]
	private bool freezeOnUpperLock;

	[SerializeField]
	private bool freezeOnLowerLock;

	[SerializeField]
	private bool doDelayedFreezeOnAwake;

	private void Awake()
	{
		if (doDelayedFreezeOnAwake)
		{
			StartCoroutine(WaitAndFreeze());
		}
	}

	private IEnumerator WaitAndFreeze()
	{
		yield return new WaitForSeconds(0.5f);
		SetFreezeState(true);
	}

	private void OnEnable()
	{
		if (slider != null)
		{
			slider.OnUpperLocked += UpperLocked;
			slider.OnLowerLocked += LowerLocked;
			slider.OnUpperUnlocked += Unlocked;
			slider.OnLowerUnlocked += Unlocked;
		}
		if (owlVR2Slider != null)
		{
			owlVR2Slider.OnUpperLocked += UpperLockedV2;
			owlVR2Slider.OnLowerLocked += LowerLockedV2;
			owlVR2Slider.OnUpperUnlocked += UnlockedV2;
			owlVR2Slider.OnLowerUnlocked += UnlockedV2;
		}
	}

	private void OnDisable()
	{
		if (slider != null)
		{
			slider.OnUpperLocked -= UpperLocked;
			slider.OnLowerLocked -= LowerLocked;
			slider.OnUpperUnlocked -= Unlocked;
			slider.OnLowerUnlocked -= Unlocked;
		}
		if (owlVR2Slider != null)
		{
			owlVR2Slider.OnUpperLocked -= UpperLockedV2;
			owlVR2Slider.OnLowerLocked -= LowerLockedV2;
			owlVR2Slider.OnUpperUnlocked -= UnlockedV2;
			owlVR2Slider.OnLowerUnlocked -= UnlockedV2;
		}
	}

	private void UpperLocked(GrabbableSlider slider, bool isInitial)
	{
		if (freezeOnUpperLock)
		{
			SetFreezeState(true);
		}
	}

	private void LowerLocked(GrabbableSlider slider, bool isInitial)
	{
		if (freezeOnLowerLock)
		{
			SetFreezeState(true);
		}
	}

	private void Unlocked(GrabbableSlider slider)
	{
		SetFreezeState(false);
	}

	private void UpperLockedV2(OwlchemyVR2.GrabbableSlider slider, bool isInitial)
	{
		if (freezeOnUpperLock)
		{
			SetFreezeState(true);
		}
	}

	private void LowerLockedV2(OwlchemyVR2.GrabbableSlider slider, bool isInitial)
	{
		if (freezeOnLowerLock)
		{
			SetFreezeState(true);
		}
	}

	private void UnlockedV2(OwlchemyVR2.GrabbableSlider slider)
	{
		SetFreezeState(false);
	}

	private void SetFreezeState(bool s)
	{
		AttachableObject attachableObject = null;
		AttachablePoint attachablePoint = null;
		for (int i = 0; i < pointsToFreeze.Length; i++)
		{
			attachablePoint = pointsToFreeze[i];
			if (freezeType == FreezeTypes.DeactivateAttachpoint)
			{
				attachablePoint.gameObject.SetActive(!s);
			}
			else
			{
				if (freezeType != FreezeTypes.DisableInteraction)
				{
					continue;
				}
				for (int j = 0; j < attachablePoint.AttachedObjects.Count; j++)
				{
					attachableObject = attachablePoint.AttachedObjects[j];
					if (attachableObject != null)
					{
						attachableObject.PickupableItem.enabled = !s;
					}
				}
			}
		}
	}
}
