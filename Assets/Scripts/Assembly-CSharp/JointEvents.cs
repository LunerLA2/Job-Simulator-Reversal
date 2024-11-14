using System;
using UnityEngine;

public class JointEvents : MonoBehaviour
{
	public Action<float> OnJointBreakEvent;

	private void OnJointBreak(float breakForce)
	{
		if (OnJointBreakEvent != null)
		{
			OnJointBreakEvent(breakForce);
		}
	}
}
