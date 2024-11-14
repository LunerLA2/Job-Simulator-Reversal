using System.Collections.Generic;
using UnityEngine;

public class ManagerRigidbodyEnterExitTriggerEvents : MonoBehaviour
{
	private const int NUM_OF_FRAMES_TO_DIVIDE_ACROSS = 7;

	private List<RigidbodyEnterExitTriggerEvents> rigidbodyEnterExitTriggerEventsContainingRigidbodies = new List<RigidbodyEnterExitTriggerEvents>();

	private int lastIndex;

	private int numOfTriggerEventsAtStartOfLoop;

	private int triggerZonesToCheckPerFrame;

	private int currFrame;

	private bool containsRigidbodiesToCheck;

	private bool isAdvancedCheck = true;

	private int numOfRigidbodiesAtStartOfLoop;

	private int rigidbodiesToCheckPerFrame;

	private int lastTriggerZoneIndex;

	private int lastRigidbodyIndex;

	private int rigidbodiesCheckedThisLoop;

	public static ManagerRigidbodyEnterExitTriggerEvents Instance
	{
		get
		{
			if (_noCreateInstance == null)
			{
				_noCreateInstance = Object.FindObjectOfType(typeof(ManagerRigidbodyEnterExitTriggerEvents)) as ManagerRigidbodyEnterExitTriggerEvents;
				if (_noCreateInstance == null)
				{
					_noCreateInstance = new GameObject("_RigidbodyEnterExitTriggerEventManager").AddComponent<ManagerRigidbodyEnterExitTriggerEvents>();
				}
			}
			return _noCreateInstance;
		}
	}

	public static ManagerRigidbodyEnterExitTriggerEvents _noCreateInstance { get; private set; }

	private void Awake()
	{
		if (_noCreateInstance == null)
		{
			_noCreateInstance = this;
			Object.DontDestroyOnLoad(base.gameObject);
		}
		else if (_noCreateInstance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	public void Add(RigidbodyEnterExitTriggerEvents triggerEvent)
	{
		if (!rigidbodyEnterExitTriggerEventsContainingRigidbodies.Contains(triggerEvent))
		{
			rigidbodyEnterExitTriggerEventsContainingRigidbodies.Add(triggerEvent);
			containsRigidbodiesToCheck = true;
		}
	}

	public void Remove(RigidbodyEnterExitTriggerEvents triggerEvent)
	{
		rigidbodyEnterExitTriggerEventsContainingRigidbodies.Remove(triggerEvent);
		if (rigidbodyEnterExitTriggerEventsContainingRigidbodies.Count == 0)
		{
			containsRigidbodiesToCheck = false;
		}
	}

	private void Update()
	{
		if (isAdvancedCheck)
		{
			AdvancedRevalidateRigidbodiesInTriggers();
		}
		else
		{
			RevalidateRigidbodiesInTriggers();
		}
	}

	private void AdvancedRevalidateRigidbodiesInTriggers()
	{
		if (!containsRigidbodiesToCheck)
		{
			return;
		}
		if (currFrame == 0)
		{
			numOfRigidbodiesAtStartOfLoop = 0;
			for (int i = 0; i < rigidbodyEnterExitTriggerEventsContainingRigidbodies.Count; i++)
			{
				numOfRigidbodiesAtStartOfLoop += rigidbodyEnterExitTriggerEventsContainingRigidbodies[i].ActiveRigidbodiesTriggerInfo.Count;
			}
			rigidbodiesToCheckPerFrame = Mathf.CeilToInt((float)numOfRigidbodiesAtStartOfLoop / 7f);
			lastTriggerZoneIndex = 0;
			lastRigidbodyIndex = 0;
			currFrame++;
			rigidbodiesCheckedThisLoop = 0;
			return;
		}
		int num = 0;
		int num2 = lastTriggerZoneIndex;
		int num3 = lastRigidbodyIndex;
		bool flag = false;
		bool flag2 = false;
		if (currFrame >= 8)
		{
			flag2 = true;
		}
		int count = rigidbodyEnterExitTriggerEventsContainingRigidbodies.Count;
		while (lastTriggerZoneIndex < count && (num < rigidbodiesToCheckPerFrame || flag2))
		{
			RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = rigidbodyEnterExitTriggerEventsContainingRigidbodies[lastTriggerZoneIndex];
			if (rigidbodyEnterExitTriggerEvents != null)
			{
				if (rigidbodyEnterExitTriggerEvents.DoesContainAnyActiveRigidbodies)
				{
					lastRigidbodyIndex = rigidbodyEnterExitTriggerEvents.ActiveRigidbodiesTriggerInfo.Count;
					if (num3 < lastRigidbodyIndex)
					{
						if (!flag2 && lastRigidbodyIndex - num3 > rigidbodiesToCheckPerFrame - num)
						{
							lastRigidbodyIndex = num3 + (rigidbodiesToCheckPerFrame - num);
							flag = true;
						}
						num += lastRigidbodyIndex - num3;
						rigidbodiesCheckedThisLoop += lastRigidbodyIndex - num3;
						if (rigidbodyEnterExitTriggerEvents.AdvancedRevalidateRigidbodiesInTrigger(num3, lastRigidbodyIndex))
						{
							count = rigidbodyEnterExitTriggerEventsContainingRigidbodies.Count;
						}
						if (flag)
						{
							break;
						}
					}
				}
			}
			else
			{
				rigidbodyEnterExitTriggerEventsContainingRigidbodies.RemoveAt(lastTriggerZoneIndex);
				lastTriggerZoneIndex--;
				count = rigidbodyEnterExitTriggerEventsContainingRigidbodies.Count;
			}
			lastTriggerZoneIndex++;
			num3 = 0;
			lastRigidbodyIndex = 0;
		}
		if (flag2)
		{
			currFrame = 0;
		}
		else
		{
			currFrame++;
		}
	}

	private void RevalidateRigidbodiesInTriggers()
	{
		if (rigidbodyEnterExitTriggerEventsContainingRigidbodies.Count <= 0)
		{
			return;
		}
		if (currFrame == 0)
		{
			numOfTriggerEventsAtStartOfLoop = rigidbodyEnterExitTriggerEventsContainingRigidbodies.Count;
			triggerZonesToCheckPerFrame = Mathf.CeilToInt((float)numOfTriggerEventsAtStartOfLoop / 7f);
			lastIndex = 0;
		}
		int num = lastIndex;
		lastIndex = Mathf.Min(lastIndex + triggerZonesToCheckPerFrame, rigidbodyEnterExitTriggerEventsContainingRigidbodies.Count);
		for (int i = num; i < lastIndex; i++)
		{
			RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = rigidbodyEnterExitTriggerEventsContainingRigidbodies[i];
			if (rigidbodyEnterExitTriggerEvents != null)
			{
				if (rigidbodyEnterExitTriggerEvents.DoesContainAnyActiveRigidbodies && rigidbodyEnterExitTriggerEvents.RevalidateRigidbodiesInTrigger())
				{
					lastIndex = Mathf.Min(lastIndex, rigidbodyEnterExitTriggerEventsContainingRigidbodies.Count);
				}
			}
			else
			{
				rigidbodyEnterExitTriggerEventsContainingRigidbodies.RemoveAt(i);
				i--;
				lastIndex = Mathf.Min(lastIndex, rigidbodyEnterExitTriggerEventsContainingRigidbodies.Count);
			}
		}
		currFrame++;
		if (currFrame >= 7)
		{
			currFrame = 0;
		}
	}
}
