using System;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class HandPushHelperArea : MonoBehaviour
{
	[SerializeField]
	private PlayerPartDetector playerPartDetector;

	[SerializeField]
	private Rigidbody rbToPush;

	[SerializeField]
	private Transform forwardDirectionReference;

	[SerializeField]
	private float angleThreshold = 70f;

	[SerializeField]
	private float velocityThreshold = 0.1f;

	[SerializeField]
	private float velocityMultiplier = 1f;

	private List<InteractionHandController> pushingHands = new List<InteractionHandController>();

	private void OnEnable()
	{
		PlayerPartDetector obj = playerPartDetector;
		obj.OnHandEntered = (Action<PlayerPartDetector, InteractionHandController>)Delegate.Combine(obj.OnHandEntered, new Action<PlayerPartDetector, InteractionHandController>(HandEntered));
		PlayerPartDetector obj2 = playerPartDetector;
		obj2.OnHandExited = (Action<PlayerPartDetector, InteractionHandController>)Delegate.Combine(obj2.OnHandExited, new Action<PlayerPartDetector, InteractionHandController>(HandExited));
	}

	private void OnDisable()
	{
		PlayerPartDetector obj = playerPartDetector;
		obj.OnHandEntered = (Action<PlayerPartDetector, InteractionHandController>)Delegate.Remove(obj.OnHandEntered, new Action<PlayerPartDetector, InteractionHandController>(HandEntered));
		PlayerPartDetector obj2 = playerPartDetector;
		obj2.OnHandExited = (Action<PlayerPartDetector, InteractionHandController>)Delegate.Remove(obj2.OnHandExited, new Action<PlayerPartDetector, InteractionHandController>(HandExited));
	}

	private void HandEntered(PlayerPartDetector ppd, InteractionHandController hand)
	{
		if (!pushingHands.Contains(hand))
		{
			pushingHands.Add(hand);
		}
	}

	private void HandExited(PlayerPartDetector ppd, InteractionHandController hand)
	{
		if (pushingHands.Contains(hand))
		{
			pushingHands.Remove(hand);
		}
	}

	private void Update()
	{
		if (pushingHands.Count <= 0)
		{
			return;
		}
		Vector3 zero = Vector3.zero;
		for (int i = 0; i < pushingHands.Count; i++)
		{
			zero += pushingHands[i].GetRawHandVelocity();
		}
		if (zero.magnitude >= velocityThreshold)
		{
			zero.Normalize();
			float num = Vector3.Angle(zero, forwardDirectionReference.forward);
			if (num <= angleThreshold)
			{
				rbToPush.velocity = zero * velocityMultiplier;
			}
		}
	}
}
