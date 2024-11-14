using System;
using System.Collections;
using OwlchemyVR;
using UnityEngine;

public class FlipBottle : MonoBehaviour
{
	public Transform centerOfMass;

	public PickupableItem intItem;

	private Rigidbody rb;

	private bool uprightSleepDetected;

	private bool canStartPollingForUprightSleep;

	private bool wasReleasedWithSufficientAngVelocity;

	private Quaternion prevRot;

	[SerializeField]
	private ParticleSystem particleToPlay;

	[SerializeField]
	private AudioClip[] soundClipToPlay;

	private int timeFlipped;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		rb.centerOfMass = centerOfMass.localPosition;
	}

	protected virtual void OnEnable()
	{
		PickupableItem pickupableItem = intItem;
		pickupableItem.OnReleasedWasNotSwappedBetweenHands = (Action<GrabbableItem>)Delegate.Combine(pickupableItem.OnReleasedWasNotSwappedBetweenHands, new Action<GrabbableItem>(Released));
		PickupableItem pickupableItem2 = intItem;
		pickupableItem2.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(pickupableItem2.OnGrabbed, new Action<GrabbableItem>(Grabbed));
	}

	protected virtual void OnDisable()
	{
		PickupableItem pickupableItem = intItem;
		pickupableItem.OnReleasedWasNotSwappedBetweenHands = (Action<GrabbableItem>)Delegate.Remove(pickupableItem.OnReleasedWasNotSwappedBetweenHands, new Action<GrabbableItem>(Released));
		PickupableItem pickupableItem2 = intItem;
		pickupableItem2.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(pickupableItem2.OnGrabbed, new Action<GrabbableItem>(Grabbed));
	}

	private void Grabbed(GrabbableItem item)
	{
		canStartPollingForUprightSleep = true;
		uprightSleepDetected = false;
	}

	private void Released(GrabbableItem item)
	{
		StartCoroutine(DetermineAngularVelocity());
	}

	private IEnumerator DetermineAngularVelocity()
	{
		yield return null;
		wasReleasedWithSufficientAngVelocity = IsAngularVelocityEnough();
	}

	private bool IsAngularVelocityEnough()
	{
		Quaternion rotation = base.transform.rotation;
		Quaternion quaternion = rotation * Quaternion.Inverse(prevRot);
		float num = 2f * Mathf.Acos(quaternion.w);
		float x = quaternion.x / Mathf.Sqrt(1f - quaternion.w * quaternion.w);
		float y = quaternion.y / Mathf.Sqrt(1f - quaternion.w * quaternion.w);
		float z = quaternion.z / Mathf.Sqrt(1f - quaternion.w * quaternion.w);
		return (new Vector3(x, y, z) * num * (1f / Time.deltaTime)).sqrMagnitude > 8f;
	}

	private void Update()
	{
		if (rb.IsSleeping() && canStartPollingForUprightSleep && !uprightSleepDetected && wasReleasedWithSufficientAngVelocity)
		{
			float num = Vector3.Dot(Vector3.up, base.transform.up);
			if (num > 0.98f && num < 1.02f)
			{
				uprightSleepDetected = true;
				DoReward();
			}
		}
	}

	private void LateUpdate()
	{
		prevRot = base.transform.rotation;
	}

	private void DoReward()
	{
		Debug.Log("AWARD! BOTTLE IS UPRIGHT!");
		particleToPlay.Play();
		AudioManager.Instance.Play(base.transform.position, soundClipToPlay[timeFlipped % soundClipToPlay.Length], 1f, 1f);
		timeFlipped++;
		canStartPollingForUprightSleep = false;
		uprightSleepDetected = false;
		wasReleasedWithSufficientAngVelocity = false;
	}
}
