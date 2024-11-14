using System;
using OwlchemyVR;
using UnityEngine;

public class TriggerItemActivation : MonoBehaviour
{
	[SerializeField]
	private TriggerListener triggerListener;

	[SerializeField]
	private WorldItemData validItemData;

	[SerializeField]
	private bool deactivationEventOnExit;

	private void OnEnable()
	{
		TriggerListener obj = triggerListener;
		obj.OnEnter = (Action<TriggerEventInfo>)Delegate.Combine(obj.OnEnter, new Action<TriggerEventInfo>(Enter));
		TriggerListener obj2 = triggerListener;
		obj2.OnExit = (Action<TriggerEventInfo>)Delegate.Combine(obj2.OnExit, new Action<TriggerEventInfo>(Exit));
	}

	private void OnDisable()
	{
		TriggerListener obj = triggerListener;
		obj.OnEnter = (Action<TriggerEventInfo>)Delegate.Remove(obj.OnEnter, new Action<TriggerEventInfo>(Enter));
		TriggerListener obj2 = triggerListener;
		obj2.OnExit = (Action<TriggerEventInfo>)Delegate.Remove(obj2.OnExit, new Action<TriggerEventInfo>(Exit));
	}

	private void Enter(TriggerEventInfo info)
	{
		if (!(info.other.attachedRigidbody == null))
		{
			WorldItem component = info.other.attachedRigidbody.GetComponent<WorldItem>();
			if (!(component == null) && (!(validItemData != null) || !(component.Data != validItemData)))
			{
				info.listener.hasBeenActivated = true;
				GameEventsManager.Instance.ItemActionOccurred(GetComponent<WorldItem>().Data, "ACTIVATED");
			}
		}
	}

	private void Exit(TriggerEventInfo info)
	{
		if (deactivationEventOnExit && !(info.other.attachedRigidbody == null))
		{
			WorldItem component = info.other.attachedRigidbody.GetComponent<WorldItem>();
			if (!(component == null) && (!(validItemData != null) || !(component.Data != validItemData)))
			{
				info.listener.hasBeenActivated = false;
				GameEventsManager.Instance.ItemActionOccurred(GetComponent<WorldItem>().Data, "DEACTIVATED");
			}
		}
	}
}
