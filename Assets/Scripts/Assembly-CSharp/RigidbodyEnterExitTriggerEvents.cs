using System;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyEnterExitTriggerEvents : MonoBehaviour
{
	[SerializeField]
	protected bool ignoreTriggers;

	[SerializeField]
	protected bool ignoreEventsWhenDisabled;

	public Action<Rigidbody> OnRigidbodyEnterTrigger;

	public Action<Rigidbody> OnRigidbodyExitTrigger;

	public Action OnUnknownRigidbodyDestroyedInsideOfTrigger;

	protected List<RigidbodyTriggerInfo> activeRigidbodiesTriggerInfo = new List<RigidbodyTriggerInfo>();

	protected bool doesContainAnyActiveRigidbodies;

	public List<RigidbodyTriggerInfo> ActiveRigidbodiesTriggerInfo
	{
		get
		{
			return activeRigidbodiesTriggerInfo;
		}
	}

	public bool DoesContainAnyActiveRigidbodies
	{
		get
		{
			return doesContainAnyActiveRigidbodies;
		}
	}

	public void SetIgnoreTriggers(bool ignore)
	{
		ignoreTriggers = ignore;
	}

	private void OnEnable()
	{
		if (activeRigidbodiesTriggerInfo.Count > 0)
		{
			doesContainAnyActiveRigidbodies = true;
			ManagerRigidbodyEnterExitTriggerEvents.Instance.Add(this);
		}
	}

	private void OnDisable()
	{
		if (activeRigidbodiesTriggerInfo.Count > 0)
		{
			doesContainAnyActiveRigidbodies = false;
			if (ManagerRigidbodyEnterExitTriggerEvents._noCreateInstance != null)
			{
				ManagerRigidbodyEnterExitTriggerEvents._noCreateInstance.Remove(this);
			}
		}
	}

	protected virtual void OnTriggerEnter(Collider other)
	{
		if ((!ignoreEventsWhenDisabled || base.isActiveAndEnabled) && (!ignoreTriggers || !other.isTrigger) && other.attachedRigidbody != null)
		{
			Rigidbody attachedRigidbody = other.attachedRigidbody;
			RigidbodyTriggerInfo rigidbodyTriggerInfo = FindRigidbodyTriggerInfo(attachedRigidbody);
			bool flag = false;
			if (rigidbodyTriggerInfo == null)
			{
				rigidbodyTriggerInfo = new RigidbodyTriggerInfo(attachedRigidbody);
				activeRigidbodiesTriggerInfo.Add(rigidbodyTriggerInfo);
				doesContainAnyActiveRigidbodies = true;
				ManagerRigidbodyEnterExitTriggerEvents.Instance.Add(this);
				rigidbodyTriggerInfo.AddCollider(other);
				flag = true;
			}
			else if (!rigidbodyTriggerInfo.CheckIsColliderCurrentlyInside(other))
			{
				rigidbodyTriggerInfo.AddCollider(other);
				flag = true;
			}
			if (flag && rigidbodyTriggerInfo.GetColliderInsideCount() == 1 && OnRigidbodyEnterTrigger != null)
			{
				OnRigidbodyEnterTrigger(attachedRigidbody);
			}
		}
	}

	protected virtual void OnTriggerExit(Collider other)
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
			Debug.LogWarning("Error in rigidbodyExitTracker: Missing TriggerInfo:" + attachedRigidbody.name);
		}
		else
		{
			Debug.LogWarning("Error in rigidbodyExitTracker: Missing TriggerInfo");
		}
	}

	public bool RevalidateRigidbodiesInTrigger()
	{
		if (activeRigidbodiesTriggerInfo.Count > 0)
		{
			bool result = false;
			for (int i = 0; i < activeRigidbodiesTriggerInfo.Count; i++)
			{
				Rigidbody rigidbody = activeRigidbodiesTriggerInfo[i].Rigidbody;
				activeRigidbodiesTriggerInfo[i].RevalidateColliders();
				if (!(rigidbody == null) && rigidbody.gameObject.activeInHierarchy && activeRigidbodiesTriggerInfo[i].GetColliderInsideCount() != 0)
				{
					continue;
				}
				activeRigidbodiesTriggerInfo.RemoveAt(i);
				if (activeRigidbodiesTriggerInfo.Count == 0)
				{
					doesContainAnyActiveRigidbodies = false;
					ManagerRigidbodyEnterExitTriggerEvents.Instance.Remove(this);
					result = true;
				}
				i--;
				if (rigidbody != null)
				{
					if (OnRigidbodyExitTrigger != null)
					{
						OnRigidbodyExitTrigger(rigidbody);
					}
				}
				else if (OnUnknownRigidbodyDestroyedInsideOfTrigger != null)
				{
					OnUnknownRigidbodyDestroyedInsideOfTrigger();
				}
			}
			return result;
		}
		return false;
	}

	public bool AdvancedRevalidateRigidbodiesInTrigger(int startingActiveRigidbodyIndex, int endingActiveRigidbodyIndex)
	{
		if (activeRigidbodiesTriggerInfo.Count > 0)
		{
			bool result = false;
			endingActiveRigidbodyIndex = Mathf.Min(endingActiveRigidbodyIndex, activeRigidbodiesTriggerInfo.Count);
			for (int i = startingActiveRigidbodyIndex; i < endingActiveRigidbodyIndex; i++)
			{
				Rigidbody rigidbody = activeRigidbodiesTriggerInfo[i].Rigidbody;
				activeRigidbodiesTriggerInfo[i].RevalidateColliders();
				if (!(rigidbody == null) && rigidbody.gameObject.activeInHierarchy && activeRigidbodiesTriggerInfo[i].GetColliderInsideCount() != 0)
				{
					continue;
				}
				activeRigidbodiesTriggerInfo.RemoveAt(i);
				if (activeRigidbodiesTriggerInfo.Count == 0)
				{
					doesContainAnyActiveRigidbodies = false;
					ManagerRigidbodyEnterExitTriggerEvents.Instance.Remove(this);
					result = true;
				}
				i--;
				endingActiveRigidbodyIndex = Mathf.Min(endingActiveRigidbodyIndex, activeRigidbodiesTriggerInfo.Count);
				if (rigidbody != null)
				{
					if (OnRigidbodyExitTrigger != null)
					{
						OnRigidbodyExitTrigger(rigidbody);
					}
				}
				else if (OnUnknownRigidbodyDestroyedInsideOfTrigger != null)
				{
					OnUnknownRigidbodyDestroyedInsideOfTrigger();
				}
			}
			return result;
		}
		return false;
	}

	public bool RevalidateRigidbodyRigidbodyTriggerInfo(RigidbodyTriggerInfo rigidbodyTriggerInfo)
	{
		bool result = false;
		Rigidbody rigidbody = rigidbodyTriggerInfo.Rigidbody;
		rigidbodyTriggerInfo.RevalidateColliders();
		if (rigidbody == null || !rigidbody.gameObject.activeInHierarchy || rigidbodyTriggerInfo.GetColliderInsideCount() == 0)
		{
			activeRigidbodiesTriggerInfo.Remove(rigidbodyTriggerInfo);
			if (activeRigidbodiesTriggerInfo.Count == 0)
			{
				doesContainAnyActiveRigidbodies = false;
				ManagerRigidbodyEnterExitTriggerEvents.Instance.Remove(this);
				result = true;
			}
			if (rigidbody != null)
			{
				if (OnRigidbodyExitTrigger != null)
				{
					OnRigidbodyExitTrigger(rigidbody);
				}
			}
			else if (OnUnknownRigidbodyDestroyedInsideOfTrigger != null)
			{
				OnUnknownRigidbodyDestroyedInsideOfTrigger();
			}
		}
		return result;
	}

	protected RigidbodyTriggerInfo FindRigidbodyTriggerInfo(Rigidbody r)
	{
		for (int i = 0; i < activeRigidbodiesTriggerInfo.Count; i++)
		{
			if (activeRigidbodiesTriggerInfo[i].Rigidbody == r)
			{
				return activeRigidbodiesTriggerInfo[i];
			}
		}
		return null;
	}

	public void Clear()
	{
		ActiveRigidbodiesTriggerInfo.Clear();
	}
}
