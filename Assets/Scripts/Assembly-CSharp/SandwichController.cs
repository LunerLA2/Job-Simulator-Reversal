using System;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

[RequireComponent(typeof(GrabbableItem))]
public class SandwichController : MonoBehaviour
{
	[SerializeField]
	private float disassembleForceFactor = 10f;

	[SerializeField]
	private AttachablePoint topItemAttachPoint;

	[SerializeField]
	private AudioClip disassembleSound;

	private List<AttachableObject> items = new List<AttachableObject>();

	private GrabbableItem grabbable;

	private AttachableObject topItem;

	private WorldItem worldItem;

	private void Awake()
	{
		worldItem = GetComponent<WorldItem>();
		grabbable = GetComponent<GrabbableItem>();
	}

	public void Finalize(WorldItemData worldItemData)
	{
		if (worldItemData != null)
		{
			worldItem.ManualSetData(worldItemData);
		}
		GameEventsManager.Instance.ItemActionOccurred(worldItem.Data, "CREATED");
	}

	public void AddItem(AttachableObject item, bool isTop = false)
	{
		if (isTop)
		{
			topItem = item;
			item.OnDetach = (Action<AttachableObject, AttachablePoint>)Delegate.Combine(item.OnDetach, new Action<AttachableObject, AttachablePoint>(TopItemDetached));
			item.AttachTo(topItemAttachPoint, 0, false, true);
		}
		else
		{
			item.transform.SetParent(base.transform, true);
			item.transform.localScale = Vector3.one;
			if (item.PickupableItem != null)
			{
				item.PickupableItem.enabled = false;
			}
			ColliderGrabbableItemPointer[] componentsInChildren = item.GetComponentsInChildren<ColliderGrabbableItemPointer>();
			foreach (ColliderGrabbableItemPointer colliderGrabbableItemPointer in componentsInChildren)
			{
				colliderGrabbableItemPointer.grabbableItem = grabbable;
			}
			item.GetComponent<SelectedChangeOutlineController>().ForceConnectionToInteractableItem(grabbable.InteractableItem);
		}
		items.Add(item);
	}

	public void RemoveItem(AttachableObject item)
	{
		item.transform.SetParent(GlobalStorage.Instance.ContentRoot, true);
		item.transform.localScale = Vector3.one;
		item.PickupableItem.enabled = true;
		ColliderGrabbableItemPointer[] componentsInChildren = item.GetComponentsInChildren<ColliderGrabbableItemPointer>();
		foreach (ColliderGrabbableItemPointer colliderGrabbableItemPointer in componentsInChildren)
		{
			colliderGrabbableItemPointer.grabbableItem = item.PickupableItem;
		}
		item.GetComponent<RigidbodyRemover>().RestoreRigidbodies();
		item.PickupableItem.UpdateRigidbody();
		item.PickupableItem.Rigidbody.AddForce((item.transform.position - base.transform.position) * disassembleForceFactor, ForceMode.VelocityChange);
		item.GetComponent<SelectedChangeOutlineController>().ForceConnectionToInteractableItem(item.GetComponent<InteractableItem>());
		items.Remove(item);
	}

	private void TopItemDetached(AttachableObject o, AttachablePoint point)
	{
		Disassemble();
	}

	public void Disassemble()
	{
		if (grabbable.IsCurrInHand)
		{
			grabbable.CurrInteractableHand.TryRelease();
		}
		AudioManager.Instance.Play(base.transform.position, disassembleSound, 1f, 1f);
		if (topItem != null)
		{
			AttachableObject attachableObject = topItem;
			attachableObject.OnDetach = (Action<AttachableObject, AttachablePoint>)Delegate.Remove(attachableObject.OnDetach, new Action<AttachableObject, AttachablePoint>(TopItemDetached));
			if (topItem.PickupableItem.IsCurrInHand)
			{
				topItem.PickupableItem.CurrInteractableHand.TryRelease();
			}
		}
		while (items.Count > 0)
		{
			RemoveItem(items[0]);
		}
		GameEventsManager.Instance.ItemActionOccurred(worldItem.Data, "DESTROYED");
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.I))
		{
			Disassemble();
		}
	}
}
