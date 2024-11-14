using System.Collections.Generic;
using UnityEngine;

public class FilteredRigidbodyEnterExitTriggerEvents : RigidbodyEnterExitTriggerEvents
{
	private List<Rigidbody> rigidbodiesToIgnore = new List<Rigidbody>();

	[SerializeField]
	private bool ignoreKinematic;

	public void AddToListOfRigidbodiesToIgnore(List<Rigidbody> rbsToAdd)
	{
		rigidbodiesToIgnore.AddRange(rbsToAdd);
		for (int i = 0; i < rbsToAdd.Count; i++)
		{
			for (int j = 0; j < activeRigidbodiesTriggerInfo.Count; j++)
			{
				if (activeRigidbodiesTriggerInfo[j].Rigidbody == rbsToAdd[i])
				{
					RigidbodyTriggerInfo rigidbodyTriggerInfo = activeRigidbodiesTriggerInfo[j];
					activeRigidbodiesTriggerInfo.RemoveAt(j);
					if (OnRigidbodyExitTrigger != null)
					{
						OnRigidbodyExitTrigger(rigidbodyTriggerInfo.Rigidbody);
					}
					break;
				}
			}
		}
		doesContainAnyActiveRigidbodies = activeRigidbodiesTriggerInfo.Count > 0;
	}

	protected override void OnTriggerEnter(Collider other)
	{
		if (!(other.attachedRigidbody != null) || ((!ignoreKinematic || !other.attachedRigidbody.isKinematic) && !rigidbodiesToIgnore.Contains(other.attachedRigidbody)))
		{
			base.OnTriggerEnter(other);
		}
	}

	protected override void OnTriggerExit(Collider other)
	{
		if ((ignoreEventsWhenDisabled && !base.isActiveAndEnabled) || (ignoreTriggers && other.isTrigger) || !(other.attachedRigidbody != null))
		{
			return;
		}
		Rigidbody attachedRigidbody = other.attachedRigidbody;
		RigidbodyTriggerInfo rigidbodyTriggerInfo = FindRigidbodyTriggerInfo(attachedRigidbody);
		if (rigidbodyTriggerInfo != null)
		{
			if (rigidbodyTriggerInfo.CheckIsColliderCurrentlyInside(other))
			{
				rigidbodyTriggerInfo.RemoveCollider(other);
			}
			if (rigidbodyTriggerInfo.GetColliderInsideCount() == 0)
			{
				activeRigidbodiesTriggerInfo.Remove(rigidbodyTriggerInfo);
				if (OnRigidbodyExitTrigger != null)
				{
					OnRigidbodyExitTrigger(attachedRigidbody);
				}
				if (activeRigidbodiesTriggerInfo.Count == 0)
				{
					doesContainAnyActiveRigidbodies = false;
					ManagerRigidbodyEnterExitTriggerEvents.Instance.Remove(this);
				}
			}
		}
		else if (attachedRigidbody != null)
		{
			if ((!ignoreKinematic || !other.attachedRigidbody.isKinematic) && !rigidbodiesToIgnore.Contains(other.attachedRigidbody))
			{
				Debug.LogWarning("Error in rigidbodyExitTracker: Missing TriggerInfo:" + attachedRigidbody.name);
			}
		}
		else
		{
			Debug.LogWarning("Error in rigidbodyExitTracker: Missing TriggerInfo");
		}
	}
}
