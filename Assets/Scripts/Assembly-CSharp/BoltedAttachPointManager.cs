using System;
using System.Collections;
using OwlchemyVR;
using UnityEngine;

public class BoltedAttachPointManager : MonoBehaviour
{
	public Action OnBolted;

	public Action OnUnbolted;

	public Action OnStartBolted;

	[SerializeField]
	private AttachablePoint attachPointToBeBoltedIn;

	[SerializeField]
	private AttachablePoint[] boltAttachPoints;

	[SerializeField]
	[Tooltip("How long it takes before this objects detaches")]
	private float timeToDetach;

	private bool isFallingOff;

	private float attachTime;

	private AttachableObject boltableObjectCurrentlyInRange;

	private bool bolted;

	private void OnEnable()
	{
		attachPointToBeBoltedIn.OnObjectEnteredRange += BoltableObjectEnteredRange;
		attachPointToBeBoltedIn.OnObjectExitedRange += BoltableObjectExitedRange;
		attachPointToBeBoltedIn.OnObjectWasAttached += BoltableObjectAttached;
		attachPointToBeBoltedIn.OnObjectWasDetached += BoltableObjectDetached;
		for (int i = 0; i < boltAttachPoints.Length; i++)
		{
			boltAttachPoints[i].OnObjectWasAttached += BoltAttached;
			boltAttachPoints[i].OnObjectWasDetached += BoltDetached;
		}
	}

	private void OnDisable()
	{
		attachPointToBeBoltedIn.OnObjectEnteredRange -= BoltableObjectEnteredRange;
		attachPointToBeBoltedIn.OnObjectExitedRange -= BoltableObjectExitedRange;
		attachPointToBeBoltedIn.OnObjectWasAttached -= BoltableObjectAttached;
		attachPointToBeBoltedIn.OnObjectWasDetached -= BoltableObjectDetached;
		for (int i = 0; i < boltAttachPoints.Length; i++)
		{
			boltAttachPoints[i].OnObjectWasAttached -= BoltAttached;
			boltAttachPoints[i].OnObjectWasDetached -= BoltDetached;
		}
	}

	private void Start()
	{
		StartCoroutine(WaitAndSetInitialState());
	}

	private IEnumerator WaitAndSetInitialState()
	{
		yield return new WaitForSeconds(0.1f);
		if (attachPointToBeBoltedIn.NumAttachedObjects > 0)
		{
			if (IsAnyBoltAttached())
			{
				attachPointToBeBoltedIn.AttachedObjects[0].PickupableItem.enabled = false;
			}
			SetCanAttachBolts(true);
		}
		else
		{
			SetCanAttachBolts(false);
		}
	}

	private void BoltableObjectEnteredRange(AttachableObject obj)
	{
		boltableObjectCurrentlyInRange = obj;
		SetCanAttachBolts(true);
	}

	private void BoltableObjectExitedRange(AttachableObject obj)
	{
		if (boltableObjectCurrentlyInRange == obj)
		{
			boltableObjectCurrentlyInRange = null;
			SetCanAttachBolts(false);
		}
	}

	private void BoltableObjectAttached(AttachablePoint point, AttachableObject obj)
	{
		SetCanAttachBolts(true);
		if (!IsAnyBoltAttached())
		{
			BeginFallingOff();
		}
	}

	private void BoltableObjectDetached(AttachablePoint point, AttachableObject obj)
	{
		CancelFallingOff();
		SetCanAttachBolts(false);
	}

	private void BoltAttached(AttachablePoint point, AttachableObject obj)
	{
		if (attachPointToBeBoltedIn.NumAttachedObjects > 0)
		{
			attachPointToBeBoltedIn.AttachedObjects[0].PickupableItem.enabled = false;
		}
		else if (boltableObjectCurrentlyInRange != null)
		{
			GrabbableItem component = boltableObjectCurrentlyInRange.GetComponent<GrabbableItem>();
			if (component != null && component.IsCurrInHand && component.CurrInteractableHand != null)
			{
				component.CurrInteractableHand.ManuallyReleaseJoint();
			}
		}
		CancelFallingOff();
	}

	private void BoltDetached(AttachablePoint point, AttachableObject obj)
	{
		if (!IsAnyBoltAttached() && attachPointToBeBoltedIn.NumAttachedObjects > 0)
		{
			attachPointToBeBoltedIn.AttachedObjects[0].PickupableItem.enabled = true;
			BeginFallingOff();
		}
	}

	private void SetCanAttachBolts(bool canAttachBolts)
	{
		for (int i = 0; i < boltAttachPoints.Length; i++)
		{
			boltAttachPoints[i].gameObject.SetActive(canAttachBolts);
		}
	}

	public bool IsAnyBoltAttached()
	{
		for (int i = 0; i < boltAttachPoints.Length; i++)
		{
			if (boltAttachPoints[i].NumAttachedObjects > 0)
			{
				return true;
			}
		}
		return false;
	}

	private void BeginFallingOff()
	{
		if (!isFallingOff && timeToDetach > 0f)
		{
			isFallingOff = true;
			attachTime = Time.time;
		}
	}

	private void CancelFallingOff()
	{
		if (isFallingOff)
		{
			isFallingOff = false;
		}
	}

	private void Update()
	{
		if (isFallingOff && Time.time - attachTime >= timeToDetach)
		{
			if (attachPointToBeBoltedIn.NumAttachedObjects > 0)
			{
				attachPointToBeBoltedIn.AttachedObjects[0].Detach();
			}
			isFallingOff = false;
		}
	}
}
