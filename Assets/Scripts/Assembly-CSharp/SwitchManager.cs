using System;
using UnityEngine;
using UnityEngine.Events;

public class SwitchManager : MonoBehaviour
{
	[Serializable]
	public class SwitchEvent : UnityEvent<SwitchManager>
	{
	}

	public SwitchEvent OnSwitchOn;

	public SwitchEvent OnSwitchOff;

	[SerializeField]
	private Transform switchTip;

	[SerializeField]
	private Transform switchOffPosition;

	[SerializeField]
	private float distance = 0.05f;

	private bool isOff;

	private bool switchStateOneTime;

	private void Update()
	{
		isOff = Vector3.Distance(switchOffPosition.position, switchTip.position) > distance;
		if (isOff == switchStateOneTime)
		{
			if (isOff)
			{
				SwitchOn();
			}
			else
			{
				SwitchOff();
			}
			switchStateOneTime = !isOff;
		}
	}

	private void SwitchOn()
	{
		if (OnSwitchOn != null)
		{
			OnSwitchOn.Invoke(this);
		}
	}

	private void SwitchOff()
	{
		if (OnSwitchOn != null)
		{
			OnSwitchOff.Invoke(this);
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(switchOffPosition.position, distance);
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(switchTip.position, distance);
	}
}
