using System;
using OwlchemyVR;
using UnityEngine;

public class SegmentedCoverController : MonoBehaviour
{
	[SerializeField]
	private Transform TriggerListenerParent;

	private TriggerListener[] triggerListeners;

	[SerializeField]
	private WorldItemData[] validItemData;

	[SerializeField]
	private bool onlyFireOncePerObj = true;

	[SerializeField]
	private bool getRigidbodiesWorldItem = true;

	public Action<float> OnSegmentsPartiallyUncovered;

	public Action OnAllSegmentsUncovered;

	private void OnEnable()
	{
		triggerListeners = TriggerListenerParent.GetComponentsInChildren<TriggerListener>();
		for (int i = 0; i < triggerListeners.Length; i++)
		{
			TriggerListener obj = triggerListeners[i];
			obj.OnEnter = (Action<TriggerEventInfo>)Delegate.Combine(obj.OnEnter, new Action<TriggerEventInfo>(TriggerListenerEnter));
		}
	}

	private void OnDisable()
	{
		for (int i = 0; i < triggerListeners.Length; i++)
		{
			TriggerListener obj = triggerListeners[i];
			obj.OnEnter = (Action<TriggerEventInfo>)Delegate.Remove(obj.OnEnter, new Action<TriggerEventInfo>(TriggerListenerEnter));
		}
	}

	private void TriggerListenerEnter(TriggerEventInfo eventInfo)
	{
		if (onlyFireOncePerObj && eventInfo.listener.hasBeenActivated)
		{
			return;
		}
		WorldItem component;
		if (getRigidbodiesWorldItem)
		{
			if (eventInfo.other.attachedRigidbody == null)
			{
				return;
			}
			component = eventInfo.other.attachedRigidbody.GetComponent<WorldItem>();
		}
		else
		{
			component = eventInfo.other.GetComponent<WorldItem>();
		}
		if (!(component != null))
		{
			return;
		}
		WorldItemData data = component.Data;
		for (int i = 0; i < validItemData.Length; i++)
		{
			if (data == validItemData[i])
			{
				UncoverSegment(eventInfo.listener);
			}
		}
	}

	private void UncoverSegment(TriggerListener listener)
	{
		MeshRenderer component = listener.GetComponent<MeshRenderer>();
		if ((bool)component)
		{
			component.enabled = false;
		}
		listener.hasBeenActivated = true;
		int num = 0;
		bool flag = true;
		for (int i = 0; i < triggerListeners.Length; i++)
		{
			if (triggerListeners[i].hasBeenActivated)
			{
				num++;
			}
			else
			{
				flag = false;
			}
		}
		if (flag)
		{
			if (OnSegmentsPartiallyUncovered != null)
			{
				OnSegmentsPartiallyUncovered(1f);
			}
			if (OnAllSegmentsUncovered != null)
			{
				OnAllSegmentsUncovered();
			}
		}
		else if (OnSegmentsPartiallyUncovered != null)
		{
			OnSegmentsPartiallyUncovered((float)num / (float)triggerListeners.Length);
		}
	}
}
