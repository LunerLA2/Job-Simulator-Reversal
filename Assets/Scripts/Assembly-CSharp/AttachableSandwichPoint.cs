using System;
using System.Collections;
using OwlchemyVR;
using UnityEngine;

public class AttachableSandwichPoint : AttachablePoint
{
	[SerializeField]
	private WorldItemData reserveExtraSpaceForSpecialItem;

	[SerializeField]
	private float extraSpaceToReserve = 0.01f;

	[SerializeField]
	private Transform rodScaler;

	[SerializeField]
	private Transform highlightTransform;

	[SerializeField]
	private SphereCollider attachTrigger;

	[SerializeField]
	private float maxSandwichHeight = 0.29f;

	[SerializeField]
	private float defaultHeightOfRod = 0.29f;

	[SerializeField]
	private float minimumAvailableRodSpace = 0.1f;

	[SerializeField]
	private float itemSlideSpeed = 1f;

	[SerializeField]
	private float itemCompression = 0.002f;

	private float sandwichHeight;

	private float currentRodHeight;

	private int numSlidingItems;

	private Vector3 defaultHighlightPos;

	private Vector3 defaultAttachTriggerOffset;

	public override bool IsOccupied
	{
		get
		{
			return sandwichHeight >= maxSandwichHeight;
		}
	}

	public AttachableObject TopmostAttachedObject
	{
		get
		{
			return GetAttachedObject(base.NumAttachedObjects - 1);
		}
	}

	public float SandwichHeight
	{
		get
		{
			return sandwichHeight;
		}
	}

	public override bool IsBusy
	{
		get
		{
			return base.IsBusy || numSlidingItems > 0;
		}
	}

	public event Action<AttachableObject> OnItemFinishedSliding;

	public override bool CanAcceptItem(WorldItemData item)
	{
		if (item == reserveExtraSpaceForSpecialItem)
		{
			return !IsOccupied;
		}
		return sandwichHeight < maxSandwichHeight - extraSpaceToReserve;
	}

	protected override void Awake()
	{
		currentRodHeight = defaultHeightOfRod;
		defaultHighlightPos = highlightTransform.localPosition;
		defaultAttachTriggerOffset = attachTrigger.center;
		base.Awake();
	}

	private void UpdateSandwichHeight()
	{
		Go.killAllTweensWithTarget(rodScaler);
		Go.to(rodScaler, 0.5f, new GoTweenConfig().scale(new Vector3(1f, currentRodHeight / defaultHeightOfRod, 1f)).setEaseType(GoEaseType.QuadInOut));
		highlightTransform.localPosition = defaultHighlightPos + Vector3.back * (currentRodHeight - defaultHeightOfRod);
		attachTrigger.center = defaultAttachTriggerOffset + Vector3.back * (currentRodHeight - defaultHeightOfRod);
	}

	public override void Attach(AttachableObject o, int index = -1, bool suppressEvents = false, bool suppressEffects = false)
	{
		if (index == -1)
		{
			index = attachedObjects.Count;
		}
		base.Attach(o, index, suppressEvents, suppressEffects);
		if (index > 0)
		{
			SetPickupEnabled(attachedObjects[index - 1], false);
		}
		StartCoroutine(SlideItemAsync(o, index, sandwichHeight + (o.Thickness - itemCompression) / 2f, false));
		sandwichHeight += o.Thickness - itemCompression;
		currentRodHeight = Mathf.Max(sandwichHeight + minimumAvailableRodSpace, defaultHeightOfRod);
		UpdateSandwichHeight();
		detachDistance = currentRodHeight - sandwichHeight + o.Thickness / 2f;
		if (o.PickupableItem != null)
		{
			PickupableItem obj = o.PickupableItem;
			obj.OnReleased = (Action<GrabbableItem>)Delegate.Combine(obj.OnReleased, new Action<GrabbableItem>(AttachedObjectReleased));
		}
	}

	public override void Detach(AttachableObject o, bool suppressEvents = false, bool suppressEffects = false)
	{
		base.Detach(o);
		sandwichHeight -= o.Thickness - itemCompression;
		currentRodHeight = Mathf.Max(defaultHeightOfRod, sandwichHeight + minimumAvailableRodSpace);
		if (currentRodHeight < defaultHeightOfRod)
		{
			currentRodHeight = defaultHeightOfRod;
		}
		UpdateSandwichHeight();
		if (attachedObjects.Count > 0)
		{
			SetPickupEnabled(attachedObjects[attachedObjects.Count - 1], true);
		}
		if (o.PickupableItem != null)
		{
			PickupableItem obj = o.PickupableItem;
			obj.OnReleased = (Action<GrabbableItem>)Delegate.Remove(obj.OnReleased, new Action<GrabbableItem>(AttachedObjectReleased));
		}
	}

	private IEnumerator SlideItemAsync(AttachableObject o, int index, float targetHeight, bool isInstant)
	{
		numSlidingItems++;
		SetPickupEnabled(o, false);
		o.ForceRealign(true);
		Vector3 localPos = o.transform.localPosition;
		if (!isInstant)
		{
			for (float height = 0f - localPos.z; height > targetHeight; height = Mathf.Max(targetHeight, height - Time.deltaTime * itemSlideSpeed))
			{
				localPos.z = 0f - height;
				o.transform.localPosition = localPos;
				yield return null;
			}
		}
		localPos.z = 0f - targetHeight;
		o.transform.localPosition = localPos;
		if (index == attachedObjects.Count - 1)
		{
			SetPickupEnabled(o, true);
		}
		numSlidingItems--;
		if (this.OnItemFinishedSliding != null)
		{
			this.OnItemFinishedSliding(o);
		}
	}

	private void AttachedObjectReleased(GrabbableItem grabbable)
	{
		AttachableObject component = grabbable.GetComponent<AttachableObject>();
		if (component != null)
		{
			StartCoroutine(SlideItemAsync(component, attachedObjects.Count - 1, sandwichHeight - (component.Thickness - itemCompression) / 2f, false));
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireCube(base.transform.TransformPoint(new Vector3(0f, 0f, 0f - maxSandwichHeight)), new Vector3(0.0175f, 0.001f, 0.0175f));
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(base.transform.TransformPoint(new Vector3(0f, 0f, 0f - defaultHeightOfRod)), new Vector3(0.0175f, 0.001f, 0.0175f));
		Gizmos.DrawWireCube(base.transform.TransformPoint(new Vector3(0f, 0f, 0f - defaultHeightOfRod + minimumAvailableRodSpace)), new Vector3(0.0175f, 0.001f, 0.0175f));
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(base.transform.TransformPoint(new Vector3(0f, 0f, 0f - currentRodHeight)), new Vector3(0.0175f, 0.001f, 0.0175f));
	}
}
