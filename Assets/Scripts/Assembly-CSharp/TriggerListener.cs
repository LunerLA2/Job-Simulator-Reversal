using System;
using UnityEngine;

public class TriggerListener : MonoBehaviour
{
	public Action<TriggerEventInfo> OnEnter;

	public Action<TriggerEventInfo> OnExit;

	public Action<TriggerEventInfo> OnStay;

	[SerializeField]
	private bool fireStayEvent;

	[HideInInspector]
	public bool hasBeenActivated;

	private TriggerEventInfo eventInfo = new TriggerEventInfo();

	private void OnTriggerEnter(Collider col)
	{
		if (OnEnter != null)
		{
			eventInfo.Set(this, col);
			OnEnter(eventInfo);
		}
	}

	private void OnTriggerStay(Collider col)
	{
		if (fireStayEvent && OnStay != null)
		{
			eventInfo.Set(this, col);
			OnStay(eventInfo);
		}
	}

	private void OnTriggerExit(Collider col)
	{
		if (OnExit != null)
		{
			eventInfo.Set(this, col);
			OnExit(eventInfo);
		}
	}
}
