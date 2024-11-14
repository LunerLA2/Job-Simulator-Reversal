using System;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class PlayerPartDetector : MonoBehaviour
{
	[SerializeField]
	private RigidbodyEnterExitTriggerEvents rigidbodyTriggerEvents;

	[SerializeField]
	private bool detectHandTriggers;

	public Action<PlayerPartDetector, InteractionHandController> OnHandEntered;

	public Action<PlayerPartDetector, InteractionHandController> OnHandExited;

	public Action<PlayerPartDetector, HeadController> OnHeadEntered;

	public Action<PlayerPartDetector, HeadController> OnHeadExited;

	public Action<PlayerPartDetector> OnAnyPartEntered;

	public Action<PlayerPartDetector> OnAnyPartExited;

	public Action<PlayerPartDetector> OnFirstPartEntered;

	public Action<PlayerPartDetector> OnLastPartExited;

	private List<InteractionHandController> detectedHands = new List<InteractionHandController>();

	private HeadController detectedHead;

	private bool isAnythingInside;

	public List<InteractionHandController> DetectedHands
	{
		get
		{
			return detectedHands;
		}
	}

	public HeadController DetectedHead
	{
		get
		{
			return detectedHead;
		}
	}

	public bool IsAnythingInside
	{
		get
		{
			return isAnythingInside;
		}
	}

	private void OnEnable()
	{
		rigidbodyTriggerEvents.SetIgnoreTriggers(!detectHandTriggers);
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = rigidbodyTriggerEvents;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(RigidbodyEnteredTrigger));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents2 = rigidbodyTriggerEvents;
		rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger, new Action<Rigidbody>(RigidbodyExitedTrigger));
	}

	private void OnDisable()
	{
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = rigidbodyTriggerEvents;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(RigidbodyEnteredTrigger));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents2 = rigidbodyTriggerEvents;
		rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger, new Action<Rigidbody>(RigidbodyExitedTrigger));
	}

	private void RigidbodyEnteredTrigger(Rigidbody r)
	{
		InteractionHandController componentInParent = r.GetComponentInParent<InteractionHandController>();
		if (componentInParent != null)
		{
			if (!detectedHands.Contains(componentInParent))
			{
				detectedHands.Add(componentInParent);
				isAnythingInside = true;
				if (OnHandEntered != null)
				{
					OnHandEntered(this, componentInParent);
				}
				if (OnAnyPartEntered != null)
				{
					OnAnyPartEntered(this);
				}
				if (detectedHands.Count == 1 && detectedHead == null && OnFirstPartEntered != null)
				{
					OnFirstPartEntered(this);
				}
			}
			return;
		}
		HeadController componentInParent2 = r.GetComponentInParent<HeadController>();
		if (componentInParent2 != null && detectedHead != componentInParent2)
		{
			detectedHead = componentInParent2;
			isAnythingInside = true;
			if (OnHeadEntered != null)
			{
				OnHeadEntered(this, componentInParent2);
			}
			if (OnAnyPartEntered != null)
			{
				OnAnyPartEntered(this);
			}
			if (detectedHands.Count == 0 && OnFirstPartEntered != null)
			{
				OnFirstPartEntered(this);
			}
		}
	}

	private void RigidbodyExitedTrigger(Rigidbody r)
	{
		InteractionHandController componentInParent = r.GetComponentInParent<InteractionHandController>();
		if (componentInParent != null)
		{
			if (detectedHands.Contains(componentInParent))
			{
				detectedHands.Remove(componentInParent);
				if (detectedHead == null && detectedHands.Count == 0)
				{
					isAnythingInside = false;
				}
				if (OnHandExited != null)
				{
					OnHandExited(this, componentInParent);
				}
				if (OnAnyPartExited != null)
				{
					OnAnyPartExited(this);
				}
				if (detectedHands.Count == 0 && detectedHead == null && OnLastPartExited != null)
				{
					OnLastPartExited(this);
				}
			}
			return;
		}
		HeadController componentInParent2 = r.GetComponentInParent<HeadController>();
		if (componentInParent2 != null && detectedHead == componentInParent2)
		{
			detectedHead = null;
			if (detectedHead == null && detectedHands.Count == 0)
			{
				isAnythingInside = false;
			}
			if (OnHeadExited != null)
			{
				OnHeadExited(this, componentInParent2);
			}
			if (OnAnyPartExited != null)
			{
				OnAnyPartExited(this);
			}
			if (detectedHands.Count == 0 && OnLastPartExited != null)
			{
				OnLastPartExited(this);
			}
		}
	}
}
