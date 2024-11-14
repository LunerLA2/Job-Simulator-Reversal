using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevitationZone : MonoBehaviour
{
	[SerializeField]
	private RigidbodyEnterExitTriggerEvents colEvents;

	private List<Rigidbody> TrackedObjects = new List<Rigidbody>();

	private List<float> Timers = new List<float>();

	private List<bool> IsRoutineRunning = new List<bool>();

	[SerializeField]
	private Transform centerOfTracking;

	[SerializeField]
	private float pullRadius = 0.25f;

	private float timeOnGroundTillLevitation = 2f;

	private void OnEnable()
	{
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = colEvents;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(Enter));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents2 = colEvents;
		rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger, new Action<Rigidbody>(Exit));
	}

	private void OnDisable()
	{
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = colEvents;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(Enter));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents2 = colEvents;
		rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger, new Action<Rigidbody>(Exit));
	}

	private void Enter(Rigidbody rb)
	{
		TrackedObjects.Add(rb);
		Timers.Add(0f);
		IsRoutineRunning.Add(false);
	}

	private void Exit(Rigidbody rb)
	{
		for (int i = 0; i < TrackedObjects.Count; i++)
		{
			if (rb == TrackedObjects[i])
			{
				TrackedObjects[i].isKinematic = false;
				TrackedObjects.Remove(TrackedObjects[i]);
				Timers.RemoveAt(i);
				IsRoutineRunning.RemoveAt(i);
			}
		}
	}

	private void Update()
	{
		for (int i = 0; i < Timers.Count; i++)
		{
			List<float> timers;
			List<float> list = (timers = Timers);
			int index;
			int index2 = (index = i);
			float num = timers[index];
			list[index2] = num + Time.deltaTime;
			if (Timers[i] > timeOnGroundTillLevitation && !IsRoutineRunning[i])
			{
				IsRoutineRunning[i] = true;
				StartCoroutine(MoveToLevitatePosition(TrackedObjects[i]));
			}
		}
	}

	private void OnGrabbed(Rigidbody rb)
	{
	}

	private IEnumerator MoveToLevitatePosition(Rigidbody rb)
	{
		rb.isKinematic = true;
		float t = 0f - Time.deltaTime;
		while (t < 1f)
		{
			t = Mathf.Min(t + Time.deltaTime / 10f, 1f);
			Vector3 cachedPos = rb.transform.position;
			cachedPos.y = centerOfTracking.position.y;
			rb.transform.position = Vector3.Lerp(rb.transform.position, cachedPos, t);
			yield return null;
		}
		yield return null;
	}

	private void OnDrawGizmos()
	{
		if (!(centerOfTracking == null))
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(centerOfTracking.position, pullRadius);
		}
	}
}
