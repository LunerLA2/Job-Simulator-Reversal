using System;
using System.Collections;
using OwlchemyVR;
using UnityEngine;

public class BotInventoryFlyoutItem : MonoBehaviour
{
	private const float FALLBACK_CHECK_FOR_ITEM_TIME = 1f;

	private float timeToNextCheck;

	private GrabbableItem itemToCheckFor;

	private bool didAssignItem;

	private bool isDestroying;

	private GrabbableItem grabbable;

	private Transform ejectToLocation;

	public Action<BotInventoryFlyoutItem> OnItemWasPickedUp;

	public GrabbableItem Grabbable
	{
		get
		{
			return grabbable;
		}
	}

	private void Update()
	{
		if (!didAssignItem || isDestroying)
		{
			return;
		}
		if (timeToNextCheck > 0f)
		{
			timeToNextCheck -= Time.deltaTime;
			return;
		}
		if (itemToCheckFor == null)
		{
			ReleaseAndDestroySelf(itemToCheckFor);
		}
		else if (itemToCheckFor.transform.parent != base.transform)
		{
			ReleaseAndDestroySelf(itemToCheckFor);
		}
		timeToNextCheck = 1f;
	}

	public void SetupGrabbableEvent(GrabbableItem _grabbableItem)
	{
		grabbable = _grabbableItem;
		grabbable.enabled = true;
		GrabbableItem grabbableItem = grabbable;
		grabbableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(grabbableItem.OnGrabbed, new Action<GrabbableItem>(ItemWasGrabbed));
	}

	public void AssignItem()
	{
		grabbable.transform.SetParent(base.transform);
		itemToCheckFor = grabbable;
		didAssignItem = true;
	}

	public void DoFlyout(GrabbableItem _grabbableItem, Transform _ejectTo)
	{
		timeToNextCheck = 1f;
		if (_grabbableItem == null)
		{
			Debug.LogWarning("FlyoutItem was spawned with no grabbable, so you'll never be able to remove it. Probably not intended.");
			return;
		}
		grabbable = _grabbableItem;
		ejectToLocation = _ejectTo;
		StartCoroutine(InternalDoFlyout());
	}

	private IEnumerator InternalDoFlyout()
	{
		grabbable.enabled = false;
		grabbable.transform.SetParent(base.transform);
		grabbable.transform.localPosition = Vector3.zero;
		grabbable.transform.localRotation = Quaternion.identity;
		itemToCheckFor = grabbable;
		didAssignItem = true;
		Go.to(base.transform, Vector3.Distance(base.transform.position, ejectToLocation.position), new GoTweenConfig().position(ejectToLocation.position).rotation(ejectToLocation.rotation).setEaseType(GoEaseType.QuadInOut));
		yield return new WaitForSeconds(1f);
		SetupGrabbableEvent(grabbable);
	}

	private void ItemWasGrabbed(GrabbableItem item)
	{
		item.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(item.OnGrabbed, new Action<GrabbableItem>(ItemWasGrabbed));
		if (OnItemWasPickedUp != null)
		{
			OnItemWasPickedUp(this);
		}
		ReleaseAndDestroySelf(item);
	}

	private void ReleaseAndDestroySelf(GrabbableItem item)
	{
		if (!isDestroying)
		{
			isDestroying = true;
			if (item != null)
			{
				item.transform.SetParent(GlobalStorage.Instance.ContentRoot, true);
			}
			UnityEngine.Object.Destroy(base.gameObject, 0.1f);
		}
	}

	public void Cancel()
	{
		GrabbableItem grabbableItem = grabbable;
		grabbableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(grabbableItem.OnGrabbed, new Action<GrabbableItem>(ItemWasGrabbed));
	}
}
