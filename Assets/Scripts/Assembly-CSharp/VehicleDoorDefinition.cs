using System;
using System.Collections;
using OwlchemyVR;
using UnityEngine;

[Serializable]
public class VehicleDoorDefinition
{
	[SerializeField]
	private GrabbableHinge doorHinge;

	[SerializeField]
	private VehicleDoorTypes doorType = VehicleDoorTypes.All;

	[SerializeField]
	private WorldItem doorWorldItem;

	public VehicleDoorTypes DoorType
	{
		get
		{
			return doorType;
		}
	}

	public GrabbableHinge DoorHinge
	{
		get
		{
			return doorHinge;
		}
	}

	public WorldItem DoorWorldItem
	{
		get
		{
			return doorWorldItem;
		}
	}

	public void InitDoorWorldItem()
	{
		if (doorWorldItem == null)
		{
			doorWorldItem = doorHinge.gameObject.GetComponent<WorldItem>();
		}
	}

	public IEnumerator ForceDoorClosed(bool instant = false)
	{
		VehicleDoorController doorController = doorHinge.GetComponent<VehicleDoorController>();
		if (doorController != null)
		{
			doorController.enabled = false;
		}
		BreakFromHand();
		yield return null;
		if (!doorHinge.IsLowerLocked)
		{
			doorHinge.Unlock(false);
		}
		doorHinge.Grabbable.enabled = false;
		if (!instant)
		{
			bool previousKin = doorHinge.Grabbable.Rigidbody.isKinematic;
			doorHinge.Grabbable.Rigidbody.isKinematic = true;
			Go.to(doorHinge.transform, (doorType != VehicleDoorTypes.Trunk) ? 0.2f : 0.1f, new GoTweenConfig().localRotation(Quaternion.identity).setEaseType(GoEaseType.QuadInOut));
			yield return new WaitForSeconds((doorType != VehicleDoorTypes.Trunk) ? 0.2f : 0.1f);
			doorHinge.Grabbable.Rigidbody.isKinematic = previousKin;
		}
		WorldItemData queuedDoorEventData = null;
		bool isEventQueued = false;
		if (!doorHinge.IsLowerLocked)
		{
			doorHinge.LockLower(true);
			queuedDoorEventData = doorWorldItem.Data;
			isEventQueued = true;
			if (doorController != null)
			{
				doorController.PlayDoorClosedAudio();
			}
		}
		if (doorController != null && !doorController.enabled)
		{
			doorController.enabled = true;
		}
		if (isEventQueued)
		{
			for (int i = 0; i < 75; i++)
			{
				yield return null;
			}
			GameEventsManager.Instance.ItemActionOccurred(queuedDoorEventData, "CLOSED");
		}
	}

	public IEnumerator ForceDoorOpen()
	{
		BreakFromHand();
		doorHinge.Grabbable.enabled = true;
		bool previousKin = doorHinge.Grabbable.Rigidbody.isKinematic;
		doorHinge.Grabbable.Rigidbody.isKinematic = true;
		doorHinge.Unlock(false);
		Quaternion desiredRot = Quaternion.identity;
		if (doorHinge.HingeAxis == GrabbableHinge.Axis.X)
		{
			desiredRot = Quaternion.Euler(Vector3.right * doorHinge.UpperLimit);
		}
		else if (doorHinge.HingeAxis == GrabbableHinge.Axis.Y)
		{
			desiredRot = Quaternion.Euler(Vector3.up * doorHinge.UpperLimit);
		}
		else if (doorHinge.HingeAxis == GrabbableHinge.Axis.Z)
		{
			desiredRot = Quaternion.Euler(Vector3.forward * doorHinge.UpperLimit);
		}
		Go.to(doorHinge.transform, 0.2f, new GoTweenConfig().localRotation(desiredRot).setEaseType(GoEaseType.QuadInOut));
		VehicleDoorController doorController = doorHinge.GetComponent<VehicleDoorController>();
		doorController.PlayDoorOpenAudio();
		yield return new WaitForSeconds(0.2f);
		doorHinge.Grabbable.Rigidbody.isKinematic = previousKin;
	}

	private void BreakFromHand()
	{
		if (doorHinge.Grabbable.IsCurrInHand && doorHinge.Grabbable.CurrInteractableHand != null)
		{
			doorHinge.Grabbable.CurrInteractableHand.ManuallyReleaseJoint();
		}
	}

	public void AllowOpening()
	{
		doorHinge.Grabbable.enabled = true;
	}
}
