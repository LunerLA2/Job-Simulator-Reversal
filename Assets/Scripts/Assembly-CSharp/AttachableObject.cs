using System;
using System.Collections;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class AttachableObject : MonoBehaviour
{
	[SerializeField]
	private Transform attachLocation;

	[SerializeField]
	private AttachLocationOverride[] attachLocationOverrides;

	[SerializeField]
	private AttachablePointData[] validAttachablePointDatas;

	[SerializeField]
	private AttachablePoint startAttachedTo;

	[SerializeField]
	private bool retainRollWhenAttached;

	[SerializeField]
	private bool reversible;

	[SerializeField]
	private float thickness;

	[SerializeField]
	private bool mustBeHeldToAttach = true;

	[SerializeField]
	private bool retainDistanceWhenAttached;

	[SerializeField]
	private RigidbodyRemover rigidbodyRemover;

	[SerializeField]
	private Collider[] collidersToToggleOnAttach;

	[SerializeField]
	private GameObject[] objectsToIgnoreHandOnAttach;

	private AttachablePoint currentlyAttachedTo;

	private List<AttachablePoint> attachPointsCurrentlyInRange = new List<AttachablePoint>();

	private int hapticsRateMicroSec = 650;

	private float hapticsLengthSeconds = 0.02f;

	private HapticInfoObject hapticObject;

	private float hapticDistanceThreshold = 0.005f;

	[SerializeField]
	private float hapticDistanceThresholdMultiplier = 1f;

	private float lastHapticDistance;

	private AttachablePoint cachedLastClosestAttachpoint;

	private PickupableItem pickupableItem;

	private RigidbodyConstraints cachedContraints;

	private bool cachedIsKinematic;

	private float mass;

	private bool isAttached;

	private bool isInHand;

	private Vector3 localPositionOnGrab;

	private Quaternion localRotationOnGrab;

	private Vector3 attachOffsetOnGrab;

	public Action<AttachableObject, AttachablePoint> OnAttach;

	public Action<AttachableObject, AttachablePoint> OnDetach;

	public float Thickness
	{
		get
		{
			return thickness;
		}
	}

	public float Mass
	{
		get
		{
			return mass;
		}
	}

	public AttachLocationOverride[] AttachLocationOverrides
	{
		get
		{
			return attachLocationOverrides;
		}
	}

	public AttachablePointData[] ValidAttachablePointDatas
	{
		get
		{
			return validAttachablePointDatas;
		}
	}

	public AttachablePoint CachedLastClosestAttachpoint
	{
		get
		{
			return cachedLastClosestAttachpoint;
		}
	}

	public Transform AttachLocation
	{
		get
		{
			if (currentlyAttachedTo == null)
			{
				return attachLocation;
			}
			return GetAttachLocation(currentlyAttachedTo.Data);
		}
	}

	public AttachablePoint CurrentlyAttachedTo
	{
		get
		{
			return currentlyAttachedTo;
		}
	}

	public PickupableItem PickupableItem
	{
		get
		{
			return pickupableItem;
		}
	}

	public void EditorSetAttachLocation(Transform t)
	{
		attachLocation = t;
	}

	private void Awake()
	{
		if (base.transform.localScale != Vector3.one)
		{
			Debug.LogWarning("Attachable Object is not scaled to one" + base.gameObject.name, base.gameObject);
		}
		hapticObject = new HapticInfoObject(hapticsRateMicroSec, hapticsLengthSeconds);
		hapticObject.DeactiveHaptic();
		pickupableItem = GetComponent<PickupableItem>();
		pickupableItem.UpdateRigidbody();
		if (pickupableItem.Rigidbody != null)
		{
			mass = pickupableItem.Rigidbody.mass;
		}
	}

	private void Start()
	{
		if (isAttached)
		{
			return;
		}
		if (startAttachedTo != null)
		{
			AttachTo(startAttachedTo);
			return;
		}
		AttachablePoint closestPoint = GetClosestPoint();
		if (closestPoint != null)
		{
			AttachTo(closestPoint);
		}
	}

	private void OnEnable()
	{
		PickupableItem obj = pickupableItem;
		obj.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(obj.OnGrabbed, new Action<GrabbableItem>(ItemGrabbed));
		PickupableItem obj2 = pickupableItem;
		obj2.OnReleasedWasNotSwappedBetweenHands = (Action<GrabbableItem>)Delegate.Combine(obj2.OnReleasedWasNotSwappedBetweenHands, new Action<GrabbableItem>(ItemReleased));
		if (pickupableItem.IsCurrInHand && !isInHand)
		{
			ItemGrabbed(pickupableItem);
		}
	}

	private void OnDisable()
	{
		PickupableItem obj = pickupableItem;
		obj.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(obj.OnGrabbed, new Action<GrabbableItem>(ItemGrabbed));
		PickupableItem obj2 = pickupableItem;
		obj2.OnReleasedWasNotSwappedBetweenHands = (Action<GrabbableItem>)Delegate.Remove(obj2.OnReleasedWasNotSwappedBetweenHands, new Action<GrabbableItem>(ItemReleased));
	}

	public void SetThickness(float thickness)
	{
		this.thickness = thickness;
	}

	public Transform GetAttachLocation(AttachablePointData pointData)
	{
		if (attachLocationOverrides.Length == 0)
		{
			return attachLocation;
		}
		for (int i = 0; i < attachLocationOverrides.Length; i++)
		{
			if (attachLocationOverrides[i].AttachablePointData == pointData)
			{
				return attachLocationOverrides[i].Location;
			}
		}
		return attachLocation;
	}

	private void LateUpdate()
	{
		if (isInHand)
		{
			if (isAttached)
			{
				Vector3 attachOffset = GetAttachOffset(currentlyAttachedTo);
				Vector3 vector = (attachOffset - attachOffsetOnGrab) * currentlyAttachedTo.detachMovementRatio;
				if (vector.sqrMagnitude > currentlyAttachedTo.SqrDetachDistance)
				{
					Detach();
				}
				else
				{
					AlignTo(currentlyAttachedTo, attachOffsetOnGrab + vector);
				}
			}
			else if (attachPointsCurrentlyInRange.Count > 0)
			{
				GetClosestPoint();
			}
		}
		else if (!isAttached && !mustBeHeldToAttach && attachPointsCurrentlyInRange.Count > 0)
		{
			AttachablePoint closestPoint = GetClosestPoint();
			if (closestPoint != null)
			{
				AttachTo(closestPoint);
			}
		}
	}

	public void GotInRangeOfAttachablePoint(AttachablePoint point)
	{
		if (!attachPointsCurrentlyInRange.Contains(point))
		{
			attachPointsCurrentlyInRange.Add(point);
		}
	}

	public void LeftRangeOfAttachablePoint(AttachablePoint point)
	{
		if (attachPointsCurrentlyInRange.Contains(point))
		{
			attachPointsCurrentlyInRange.Remove(point);
		}
	}

	public void GotInRangeOfInstantSnapAttachablePoint(AttachablePoint point)
	{
		if (pickupableItem != null && pickupableItem.IsCurrInHand)
		{
			StartCoroutine(WaitUntilEndOfFrameThenTryRelaseOfHandForAttachPointInstantSnapping());
		}
	}

	private IEnumerator WaitUntilEndOfFrameThenTryRelaseOfHandForAttachPointInstantSnapping()
	{
		yield return new WaitForEndOfFrame();
		if (pickupableItem != null && pickupableItem.IsCurrInHand)
		{
			pickupableItem.CurrInteractableHand.TryRelease();
		}
	}

	private void ItemGrabbed(GrabbableItem item)
	{
		isInHand = true;
		if (isAttached)
		{
			if (rigidbodyRemover != null && rigidbodyRemover.HasBeenRemoved)
			{
				rigidbodyRemover.RestoreRigidbodies();
				pickupableItem.UpdateRigidbody();
			}
			if (currentlyAttachedTo.detachDistance > 0f)
			{
				localPositionOnGrab = base.transform.localPosition;
				localRotationOnGrab = base.transform.localRotation;
				attachOffsetOnGrab = GetAttachOffset(currentlyAttachedTo);
				lastHapticDistance = float.NegativeInfinity;
			}
			else
			{
				Detach();
			}
		}
	}

	private void ItemReleased(GrabbableItem item)
	{
		isInHand = false;
		if (hapticObject.IsRunning && item.CurrInteractableHand != null)
		{
			item.CurrInteractableHand.HapticsController.RemoveHaptic(hapticObject);
		}
		if (isAttached && currentlyAttachedTo != null)
		{
			PhysicallyAttachTo(currentlyAttachedTo, false);
			if (currentlyAttachedTo.retainDistanceOnAttach && !currentlyAttachedTo.IsRefilling)
			{
				AlignTo(currentlyAttachedTo, GetAttachOffset(currentlyAttachedTo));
				return;
			}
			base.transform.localPosition = localPositionOnGrab;
			base.transform.localRotation = localRotationOnGrab;
		}
		else if (!isAttached)
		{
			AttachablePoint closestPoint = GetClosestPoint();
			if (closestPoint != null)
			{
				AttachTo(closestPoint);
			}
		}
	}

	private Vector3 GetAttachOffset(AttachablePoint point)
	{
		return point.transform.InverseTransformPoint(GetAttachLocation(point.Data).position);
	}

	public void Detach(bool suppressEvents = false, bool suppressEffects = false)
	{
		if (!isAttached || currentlyAttachedTo == null)
		{
			return;
		}
		if (rigidbodyRemover != null && rigidbodyRemover.HasBeenRemoved)
		{
			rigidbodyRemover.RestoreRigidbodies();
			pickupableItem.UpdateRigidbody();
		}
		if (pickupableItem.Rigidbody != null)
		{
			pickupableItem.Rigidbody.Sleep();
			pickupableItem.Rigidbody.constraints = cachedContraints;
			pickupableItem.Rigidbody.isKinematic = cachedIsKinematic;
		}
		base.transform.SetParent(GlobalStorage.Instance.ContentRoot, true);
		ToggleColliders();
		IgnoreHands(false);
		if (currentlyAttachedTo.OnlyAllowDetachWhenCurrInHand)
		{
			pickupableItem.enabled = true;
		}
		AttachablePoint attachablePoint = currentlyAttachedTo;
		isAttached = false;
		currentlyAttachedTo = null;
		if (!suppressEvents)
		{
			if (OnDetach != null)
			{
				OnDetach(this, attachablePoint);
			}
			GameEventsManager.Instance.ItemAppliedToItemActionOccurred(pickupableItem.InteractableItem.WorldItemData, attachablePoint.Data, "DEATTACHED_FROM");
		}
		attachablePoint.Detach(this, suppressEvents, suppressEffects);
	}

	public void ManuallyClearInRanges()
	{
		AttachablePoint[] array = attachPointsCurrentlyInRange.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].ObjectExitedRange(this);
		}
	}

	private AttachablePoint GetClosestPoint()
	{
		AttachablePoint attachablePoint = null;
		AttachablePoint result = null;
		float num = float.PositiveInfinity;
		for (int i = 0; i < attachPointsCurrentlyInRange.Count; i++)
		{
			if (attachPointsCurrentlyInRange[i] != null)
			{
				attachablePoint = attachPointsCurrentlyInRange[i];
				if ((!(attachablePoint.PickupableItem == null) && !(attachablePoint.PickupableItem.Rigidbody != pickupableItem.Rigidbody)) || !attachablePoint.isActiveAndEnabled || attachablePoint.IsOccupied || attachablePoint.IsBusy || attachablePoint.IsRefilling || !attachablePoint.CanAcceptItem(pickupableItem.InteractableItem.WorldItemData))
				{
					continue;
				}
				bool flag = false;
				for (int j = 0; j < validAttachablePointDatas.Length; j++)
				{
					if (validAttachablePointDatas[j] == attachablePoint.Data)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					float num2 = Vector3.Distance(GetAttachLocation(attachablePoint.Data).position, attachablePoint.GetPoint());
					if (num2 < num)
					{
						num = num2;
						result = attachablePoint;
					}
				}
			}
			else
			{
				attachPointsCurrentlyInRange.RemoveAt(i);
				i--;
			}
		}
		cachedLastClosestAttachpoint = result;
		return result;
	}

	public void AttachTo(AttachablePoint attachablePoint, int index = -1, bool suppressEvents = false, bool suppressEffects = false)
	{
		if (attachablePoint.PickupableItem == PickupableItem)
		{
			return;
		}
		if (Array.IndexOf(validAttachablePointDatas, attachablePoint.Data) == -1)
		{
			Debug.LogWarning("AttachablePoint data '" + attachablePoint.Data.name.ToString() + "' is not valid for this AttachableObject: " + base.gameObject.name);
			return;
		}
		if (pickupableItem.Rigidbody != null)
		{
			cachedContraints = pickupableItem.Rigidbody.constraints;
			cachedIsKinematic = pickupableItem.Rigidbody.isKinematic;
		}
		if (attachablePoint.PickupableItem != null)
		{
			AttachableObject attachableObject = attachablePoint.PickupableItem.GetComponent<AttachableObject>();
			bool flag = false;
			do
			{
				if (attachableObject != null)
				{
					if (attachableObject.CurrentlyAttachedTo != null)
					{
						if (attachableObject.CurrentlyAttachedTo.PickupableItem != null)
						{
							AttachableObject attachableObject2 = attachableObject;
							attachableObject = attachableObject.CurrentlyAttachedTo.PickupableItem.GetComponent<AttachableObject>();
							if (attachableObject == null)
							{
								attachableObject = attachableObject2;
							}
						}
						else
						{
							flag = true;
						}
					}
					else
					{
						flag = true;
					}
				}
				else
				{
					flag = true;
				}
			}
			while (!flag);
			if (attachableObject != null)
			{
				AttachablePoint[] componentsInChildren = base.transform.GetComponentsInChildren<AttachablePoint>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].ObjectExitedRange(attachableObject);
				}
			}
		}
		PhysicallyAttachTo(attachablePoint, true);
		ToggleColliders();
		IgnoreHands(true);
		if (!suppressEvents)
		{
			if (OnAttach != null)
			{
				OnAttach(this, attachablePoint);
			}
			GameEventsManager.Instance.ItemAppliedToItemActionOccurred(pickupableItem.InteractableItem.WorldItemData, attachablePoint.Data, "ATTACHED_TO");
		}
		isAttached = true;
		currentlyAttachedTo = attachablePoint;
		if (attachablePoint.OnlyAllowDetachWhenCurrInHand && attachablePoint.PickupableItem != null && !attachablePoint.PickupableItem.IsCurrInHand)
		{
			pickupableItem.enabled = false;
			SelectedChangeOutlineController[] componentsInChildren2 = pickupableItem.GetComponentsInChildren<SelectedChangeOutlineController>();
			if (componentsInChildren2 != null)
			{
				AttachablePoint attachablePoint2 = attachablePoint;
				bool flag2 = true;
				do
				{
					if (attachablePoint2.PickupableItem != null)
					{
						AttachableObject component = attachablePoint2.PickupableItem.GetComponent<AttachableObject>();
						if (component != null && component != this)
						{
							if (component.isAttached)
							{
								if (attachablePoint2 != component.CurrentlyAttachedTo && component.CurrentlyAttachedTo != null && component.CurrentlyAttachedTo.PickupableItem != null)
								{
									attachablePoint2 = component.CurrentlyAttachedTo;
								}
								else
								{
									flag2 = false;
								}
							}
							else
							{
								flag2 = false;
							}
						}
						else
						{
							flag2 = false;
						}
					}
					else
					{
						flag2 = false;
					}
				}
				while (flag2);
				for (int j = 0; j < componentsInChildren2.Length; j++)
				{
					componentsInChildren2[j].ForceConnectionToInteractableItem(attachablePoint2.PickupableItem.InteractableItem);
				}
			}
		}
		attachablePoint.Attach(this, index, suppressEvents, suppressEffects);
	}

	private void PhysicallyAttachTo(AttachablePoint tr, bool align)
	{
		if (tr.Data.RequiresRigidbodyRemover)
		{
			if (rigidbodyRemover != null)
			{
				rigidbodyRemover.RemoveRigidbodies();
			}
			else
			{
				Debug.Log("AttachablePoint requires rigidbody remover, but AttachableObject does not have one.");
			}
		}
		else if (pickupableItem.Rigidbody != null)
		{
			pickupableItem.Rigidbody.Sleep();
			pickupableItem.Rigidbody.isKinematic = true;
		}
		base.transform.parent = tr.transform;
		if (align)
		{
			if (retainDistanceWhenAttached && !tr.IsRefilling)
			{
				AlignToRetainDistance(tr);
			}
			else
			{
				AlignTo(tr, Vector3.zero);
			}
		}
	}

	public void ForceRealign(bool retainDistance = false)
	{
		if (currentlyAttachedTo != null)
		{
			if (retainDistance)
			{
				AlignToRetainDistance(currentlyAttachedTo);
			}
			else
			{
				AlignTo(currentlyAttachedTo, Vector3.zero);
			}
		}
	}

	private void AlignToRetainDistance(AttachablePoint point)
	{
		AlignTo(point, GetAttachOffset(point));
	}

	private void AlignTo(AttachablePoint tr, Vector3 offset)
	{
		if (!tr.omniDirectionDetach)
		{
			offset.x = 0f;
			offset.y = 0f;
		}
		if (pickupableItem.IsCurrInHand)
		{
			float magnitude = offset.magnitude;
			if (Mathf.Abs(magnitude - lastHapticDistance) >= hapticDistanceThreshold * hapticDistanceThresholdMultiplier)
			{
				lastHapticDistance = magnitude;
				hapticObject.Restart();
				if (!pickupableItem.CurrInteractableHand.HapticsController.ContainHaptic(hapticObject))
				{
					pickupableItem.CurrInteractableHand.HapticsController.AddNewHaptic(hapticObject);
				}
			}
		}
		base.transform.position = tr.GetPoint(base.transform.position);
		Transform transform = GetAttachLocation(tr.Data);
		bool flag = reversible && Vector3.Dot(transform.forward, tr.transform.forward) < 0f;
		if (retainRollWhenAttached && !tr.IsRefilling)
		{
			if (flag)
			{
				base.transform.rotation = Quaternion.FromToRotation(transform.forward, -tr.transform.forward) * base.transform.rotation;
			}
			else
			{
				base.transform.rotation = Quaternion.FromToRotation(transform.forward, tr.transform.forward) * base.transform.rotation;
			}
		}
		else
		{
			if (flag)
			{
				base.transform.localRotation = Quaternion.identity;
			}
			else
			{
				base.transform.localRotation = Quaternion.Euler(180f, 0f, 0f);
			}
			base.transform.localRotation = Quaternion.Inverse(transform.rotation) * base.transform.rotation;
		}
		Vector3 localScale = base.transform.localScale;
		Vector3 translation = -base.transform.InverseTransformPoint(transform.position - tr.transform.TransformVector(offset));
		translation.x *= localScale.x;
		translation.y *= localScale.y;
		translation.z *= localScale.z;
		base.transform.Translate(translation);
		if (pickupableItem.Rigidbody != null)
		{
			pickupableItem.Rigidbody.MovePosition(base.transform.position);
			pickupableItem.Rigidbody.MoveRotation(base.transform.rotation);
		}
	}

	private void ToggleColliders()
	{
		for (int i = 0; i < collidersToToggleOnAttach.Length; i++)
		{
			collidersToToggleOnAttach[i].enabled = !collidersToToggleOnAttach[i].enabled;
		}
	}

	private void IgnoreHands(bool ignore)
	{
		for (int i = 0; i < objectsToIgnoreHandOnAttach.Length; i++)
		{
			GameObject gameObject = objectsToIgnoreHandOnAttach[i];
			if (ignore)
			{
				if (gameObject.layer == 0)
				{
					gameObject.layer = 13;
				}
				else if (gameObject.layer == 8)
				{
					gameObject.layer = 9;
				}
			}
			else if (gameObject.layer == 13)
			{
				gameObject.layer = 0;
			}
			else if (gameObject.layer == 9)
			{
				gameObject.layer = 8;
			}
		}
	}

	public void AttachPointGrabbed(AttachablePoint tr)
	{
		if (tr == currentlyAttachedTo && currentlyAttachedTo.OnlyAllowDetachWhenCurrInHand)
		{
			SelectedChangeOutlineController[] componentsInChildren = pickupableItem.GetComponentsInChildren<SelectedChangeOutlineController>();
			pickupableItem.enabled = true;
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].ForceConnectionToInteractableItem(pickupableItem.InteractableItem);
				componentsInChildren[i].ForceUpdateItemSelected();
			}
		}
	}

	public void AttachPointReleased(AttachablePoint tr)
	{
		if (tr == currentlyAttachedTo && currentlyAttachedTo.OnlyAllowDetachWhenCurrInHand)
		{
			if (pickupableItem.IsCurrInHand && pickupableItem.CurrInteractableHand != null)
			{
				pickupableItem.CurrInteractableHand.TryRelease();
			}
			pickupableItem.enabled = false;
			SelectedChangeOutlineController[] componentsInChildren = pickupableItem.GetComponentsInChildren<SelectedChangeOutlineController>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].ForceConnectionToInteractableItem(tr.PickupableItem.InteractableItem);
			}
		}
	}
}
