using System.Collections;
using UnityEngine;

public class AttachableStackPoint : AttachablePoint
{
	public enum StackRotationAlignmentType
	{
		Full = 360,
		ClosestHalfTurn = 180,
		ClosestQuarterTurn = 90
	}

	public enum StackItemInteractionMode
	{
		TopmostOnly = 0,
		All = 1,
		None = 2
	}

	public enum StackDirectionType
	{
		X = 0,
		Y = 1,
		Z = 2
	}

	[SerializeField]
	public int maxItems = 5;

	[SerializeField]
	public float itemSpacing = 0.05f;

	[SerializeField]
	public StackRotationAlignmentType rotationAlignmentType = StackRotationAlignmentType.Full;

	[SerializeField]
	public StackDirectionType stackingDirection = StackDirectionType.Z;

	[SerializeField]
	public float alignPositionJitter;

	[SerializeField]
	public float alignRotationJitter;

	[SerializeField]
	public StackItemInteractionMode itemInteractionMode;

	[SerializeField]
	private bool disableEdibleItems;

	private Vector3 targetLocalPosition;

	public override bool IsOccupied
	{
		get
		{
			return attachedObjects.Count >= maxItems;
		}
	}

	public AttachableObject TopmostAttachedObject
	{
		get
		{
			return GetAttachedObject(base.NumAttachedObjects - 1);
		}
	}

	public override void Attach(AttachableObject o, int index = -1, bool suppressEvents = false, bool suppressEffects = false)
	{
		if (isRefilling && o != refillObj)
		{
			return;
		}
		base.Attach(o, index, suppressEvents, suppressEffects);
		if (!isRefilling)
		{
			if (stackingDirection == StackDirectionType.X)
			{
				base.transform.Translate(0f - itemSpacing, 0f, 0f);
			}
			else if (stackingDirection == StackDirectionType.Y)
			{
				base.transform.Translate(0f, 0f - itemSpacing, 0f);
			}
			else if (stackingDirection == StackDirectionType.Z)
			{
				base.transform.Translate(0f, 0f, 0f - itemSpacing);
			}
		}
		JitterObject(o);
		AdjustItemSpacing();
		SetItemInteractions();
		if ((bool)o.GetComponent<EdibleItem>() && disableEdibleItems)
		{
			o.GetComponent<EdibleItem>().SetNumberOfBitesTaken(0);
			o.GetComponent<EdibleItem>().enabled = false;
		}
	}

	private void JitterObject(AttachableObject o)
	{
		float z = Quaternion.FromToRotation(Vector3.up, base.transform.InverseTransformDirection(o.AttachLocation.up)).eulerAngles.z;
		float num = Mathf.Round(z / (float)rotationAlignmentType) * (float)rotationAlignmentType;
		o.transform.Rotate(base.transform.forward, num - z + Random.Range(0f - alignRotationJitter, alignRotationJitter), Space.World);
		if (stackingDirection == StackDirectionType.X)
		{
			o.transform.localPosition += new Vector3(0f, Random.Range(0f - alignPositionJitter, alignPositionJitter), Random.Range(0f - alignPositionJitter, alignPositionJitter));
		}
		else if (stackingDirection == StackDirectionType.Y)
		{
			o.transform.localPosition += new Vector3(Random.Range(0f - alignPositionJitter, alignPositionJitter), 0f, Random.Range(0f - alignPositionJitter, alignPositionJitter));
		}
		else if (stackingDirection == StackDirectionType.Z)
		{
			o.transform.localPosition += new Vector3(Random.Range(0f - alignPositionJitter, alignPositionJitter), Random.Range(0f - alignPositionJitter, alignPositionJitter), 0f);
		}
	}

	public override void Detach(AttachableObject o, bool suppressEvents = false, bool suppressEffects = false)
	{
		if (!isRefilling)
		{
			if (stackingDirection == StackDirectionType.X)
			{
				base.transform.Translate(itemSpacing, 0f, 0f);
			}
			else if (stackingDirection == StackDirectionType.Y)
			{
				base.transform.Translate(0f, itemSpacing, 0f);
			}
			else if (stackingDirection == StackDirectionType.Z)
			{
				base.transform.Translate(0f, 0f, itemSpacing);
			}
			base.Detach(o);
			if (!isRefilling)
			{
				AdjustItemSpacing();
				SetItemInteractions();
			}
		}
	}

	private void AdjustItemSpacing()
	{
		float num = 0f;
		for (int num2 = attachedObjects.Count - 1; num2 >= 0; num2--)
		{
			AttachableObject attachableObject = attachedObjects[num2];
			float magnitude = Vector3.Project(attachableObject.AttachLocation.position - attachableObject.transform.position, attachableObject.AttachLocation.forward).magnitude;
			num += itemSpacing;
			Vector3 localPosition = attachableObject.transform.localPosition;
			if (stackingDirection == StackDirectionType.X)
			{
				localPosition.x = num - magnitude;
			}
			else if (stackingDirection == StackDirectionType.Y)
			{
				localPosition.y = num - magnitude;
			}
			else if (stackingDirection == StackDirectionType.Z)
			{
				localPosition.z = num - magnitude;
			}
			attachableObject.transform.localPosition = localPosition;
		}
	}

	protected override void SetItemInteractions(CachedInteractionState[] oldItemInteractions)
	{
		for (int num = oldItemInteractions.Length - 1; num >= 0; num--)
		{
			for (int num2 = attachedObjects.Count - 1; num2 >= 0; num2--)
			{
				if (attachedObjects[num2] == oldItemInteractions[num].AttachableObject)
				{
					AttachableObject o = attachedObjects[num];
					if (itemInteractionMode == StackItemInteractionMode.TopmostOnly)
					{
						bool enablePickup = num2 == attachedObjects.Count - 1;
						SetPickupEnabled(o, enablePickup);
					}
					else if (itemInteractionMode == StackItemInteractionMode.All)
					{
						SetPickupEnabled(o, oldItemInteractions[num].IsInteractable);
					}
					else if (itemInteractionMode == StackItemInteractionMode.None)
					{
						SetPickupEnabled(o, false);
					}
				}
			}
		}
	}

	protected override IEnumerator RefillObjectAsync(bool immediate)
	{
		Vector3 startingLocalPos = base.transform.localPosition;
		Vector3 targetLocalPos = startingLocalPos;
		if (base.transform.parent != null)
		{
			if (stackingDirection == StackDirectionType.X)
			{
				targetLocalPos = base.transform.parent.InverseTransformPoint(base.transform.position - base.transform.right * itemSpacing);
			}
			else if (stackingDirection == StackDirectionType.Y)
			{
				targetLocalPos = base.transform.parent.InverseTransformPoint(base.transform.position - base.transform.up * itemSpacing);
			}
			else if (stackingDirection == StackDirectionType.Z)
			{
				targetLocalPos = base.transform.parent.InverseTransformPoint(base.transform.position - base.transform.forward * itemSpacing);
			}
		}
		else if (stackingDirection == StackDirectionType.X)
		{
			targetLocalPos = base.transform.position - base.transform.right * itemSpacing;
		}
		else if (stackingDirection == StackDirectionType.Y)
		{
			targetLocalPos = base.transform.position - base.transform.up * itemSpacing;
		}
		else if (stackingDirection == StackDirectionType.Z)
		{
			targetLocalPos = base.transform.position - base.transform.forward * itemSpacing;
		}
		targetLocalPosition = targetLocalPos;
		if (!immediate)
		{
			float refillProgress = 0f;
			while (refillProgress < 1f)
			{
				if (growRefillingObject)
				{
					refillObj.transform.localScale = Vector3.one * refillProgress;
					refillObj.ForceRealign();
				}
				base.transform.localPosition = Vector3.Lerp(startingLocalPos, targetLocalPos, refillProgress);
				refillProgress = Mathf.Min(refillProgress + Time.deltaTime / refillDuration, 1f);
				yield return null;
			}
		}
		refillObj.transform.localScale = Vector3.one;
		base.transform.localPosition = targetLocalPos;
	}

	protected override void FinishRefill()
	{
		base.FinishRefill();
		refillObj.transform.localScale = Vector3.one;
		base.transform.localPosition = targetLocalPosition;
	}
}
