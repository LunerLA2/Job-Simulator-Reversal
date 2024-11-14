using System;
using OwlchemyVR;
using UnityEngine;

[Serializable]
public class HapticTransformInfo
{
	private const float HAPTICS_DURATION = 0.02f;

	[SerializeField]
	private GrabbableItem grabbableItem;

	[SerializeField]
	private Transform trackedTransform;

	[SerializeField]
	private int hapticsRateMicroSec = 650;

	[SerializeField]
	private bool hapticBasedOnPosition;

	[SerializeField]
	private float distanceToHaptic;

	[SerializeField]
	private bool hapticBasedOnAngle;

	[SerializeField]
	private float angleToHaptic;

	[SerializeField]
	private AudioClip tickAudioClip;

	[SerializeField]
	private int hapticFiresPerAudioTick;

	private Vector3 lastPosition;

	private Quaternion lastRotation;

	private HapticInfoObject hapticObject;

	private bool wasHeld;

	private int audioTickBuildup;

	public void ManualAwake()
	{
		hapticObject = new HapticInfoObject(hapticsRateMicroSec, 0.02f);
		hapticObject.DeactiveHaptic();
		if (hapticBasedOnPosition || hapticBasedOnAngle || tickAudioClip != null)
		{
			ResetThresholds();
			if (trackedTransform == null)
			{
				Debug.LogError("HapticTransformInfo not set up right");
			}
		}
		if ((hapticBasedOnPosition || hapticBasedOnAngle) && grabbableItem == null)
		{
			Debug.LogError("HapticTransformInfo not set up right");
		}
	}

	public void ManualUpdate()
	{
		if (!(grabbableItem != null))
		{
			return;
		}
		if (grabbableItem.IsCurrInHand)
		{
			if (hapticBasedOnPosition && Vector3.Distance(trackedTransform.position, lastPosition) >= distanceToHaptic)
			{
				lastPosition = trackedTransform.position;
				DoHaptic();
			}
			if (hapticBasedOnAngle && Quaternion.Angle(trackedTransform.rotation, lastRotation) >= angleToHaptic)
			{
				lastRotation = trackedTransform.rotation;
				DoHaptic();
			}
			wasHeld = true;
		}
		else if (wasHeld)
		{
			wasHeld = false;
			RemoveHaptic();
		}
	}

	private void DoHaptic()
	{
		hapticObject.Restart();
		if (!grabbableItem.CurrInteractableHand.HapticsController.ContainHaptic(hapticObject))
		{
			grabbableItem.CurrInteractableHand.HapticsController.AddNewHaptic(hapticObject);
		}
		if (tickAudioClip != null)
		{
			audioTickBuildup++;
			if (audioTickBuildup >= hapticFiresPerAudioTick)
			{
				audioTickBuildup -= hapticFiresPerAudioTick;
				AudioManager.Instance.Play(trackedTransform, tickAudioClip, 1f, 1f);
			}
		}
	}

	private void RemoveHaptic()
	{
		if (hapticObject.IsRunning && grabbableItem.CurrInteractableHand != null)
		{
			grabbableItem.CurrInteractableHand.HapticsController.RemoveHaptic(hapticObject);
		}
		hapticObject.DeactiveHaptic();
		ResetThresholds();
	}

	private void ResetThresholds()
	{
		if (trackedTransform != null)
		{
			lastPosition = trackedTransform.position;
			lastRotation = trackedTransform.rotation;
		}
	}
}
