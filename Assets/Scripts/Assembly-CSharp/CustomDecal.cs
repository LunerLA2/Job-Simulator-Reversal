using System;
using OwlchemyVR;
using UnityEngine;

public class CustomDecal : MonoBehaviour
{
	[SerializeField]
	private TriggerListener decalTrigger;

	[SerializeField]
	private SpriteRenderer spriteRenderer;

	[SerializeField]
	private WorldItem wItem;

	[SerializeField]
	private bool clearOnAwake;

	private void Awake()
	{
		if (clearOnAwake)
		{
			spriteRenderer.sprite = null;
		}
	}

	private void OnEnable()
	{
		TriggerListener triggerListener = decalTrigger;
		triggerListener.OnEnter = (Action<TriggerEventInfo>)Delegate.Combine(triggerListener.OnEnter, new Action<TriggerEventInfo>(Enter));
	}

	private void OnDisable()
	{
		TriggerListener triggerListener = decalTrigger;
		triggerListener.OnEnter = (Action<TriggerEventInfo>)Delegate.Remove(triggerListener.OnEnter, new Action<TriggerEventInfo>(Enter));
	}

	private void Enter(TriggerEventInfo info)
	{
		Rigidbody attachedRigidbody = info.other.attachedRigidbody;
		if (!(attachedRigidbody == null))
		{
			DecalGunController component = attachedRigidbody.GetComponent<DecalGunController>();
			if (!(component == null))
			{
				spriteRenderer.sprite = component.CurrentDecal;
				GameEventsManager.Instance.ItemActionOccurred(wItem.Data, "ACTIVATED");
			}
		}
	}
}
