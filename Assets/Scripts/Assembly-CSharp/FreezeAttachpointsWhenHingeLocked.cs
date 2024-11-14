using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeAttachpointsWhenHingeLocked : MonoBehaviour
{
	private enum FreezeTypes
	{
		DeactivateAttachpoint = 0,
		DisableInteraction = 1
	}

	[SerializeField]
	private GrabbableHinge hinge;

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

	[SerializeField]
	private bool refilAttachPointsOnLowerLock;

	private void Awake()
	{
		if (doDelayedFreezeOnAwake)
		{
			StartCoroutine(WaitAndFreeze(0.5f));
		}
	}

	public void RegisterAttachablePoints(AttachablePoint[] points, bool immediatelyWaitAndFreeze)
	{
		List<AttachablePoint> list = new List<AttachablePoint>();
		list.AddRange(pointsToFreeze);
		list.AddRange(points);
		pointsToFreeze = list.ToArray();
		if (immediatelyWaitAndFreeze)
		{
			StartCoroutine(WaitAndFreeze(0.1f));
		}
	}

	private IEnumerator WaitAndFreeze(float delay)
	{
		yield return new WaitForSeconds(delay);
		SetFreezeState(true);
	}

	private void OnEnable()
	{
		hinge.OnUpperLocked += UpperLocked;
		hinge.OnLowerLocked += LowerLocked;
		hinge.OnUpperUnlocked += Unlocked;
		hinge.OnLowerUnlocked += Unlocked;
	}

	private void OnDisable()
	{
		hinge.OnUpperLocked -= UpperLocked;
		hinge.OnLowerLocked -= LowerLocked;
		hinge.OnUpperUnlocked -= Unlocked;
		hinge.OnLowerUnlocked -= Unlocked;
	}

	private void UpperLocked(GrabbableHinge hinge, bool isInitial)
	{
		if (freezeOnUpperLock)
		{
			SetFreezeState(true);
		}
	}

	private void LowerLocked(GrabbableHinge hinge, bool isInitial)
	{
		if (freezeOnLowerLock)
		{
			SetFreezeState(true);
		}
	}

	private void Unlocked(GrabbableHinge hinge)
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
			if (s && attachablePoint.NumAttachedObjects == 0 && !attachablePoint.IsRefilling)
			{
				attachablePoint.RefillOneItem();
			}
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
