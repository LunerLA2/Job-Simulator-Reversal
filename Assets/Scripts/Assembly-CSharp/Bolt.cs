using System;
using System.Collections;
using UnityEngine;

public class Bolt : MonoBehaviour
{
	private enum Axis
	{
		X = 0,
		Y = 1,
		Z = 2
	}

	[SerializeField]
	private AttachableObject attachableObject;

	[SerializeField]
	private Transform twistTransform;

	[SerializeField]
	private Axis axisOfRotation;

	private Quaternion ogRotation;

	private void Start()
	{
		ogRotation = twistTransform.localRotation;
	}

	private void OnEnable()
	{
		AttachableObject obj = attachableObject;
		obj.OnAttach = (Action<AttachableObject, AttachablePoint>)Delegate.Combine(obj.OnAttach, new Action<AttachableObject, AttachablePoint>(Attached));
	}

	private void OnDisable()
	{
		AttachableObject obj = attachableObject;
		obj.OnAttach = (Action<AttachableObject, AttachablePoint>)Delegate.Remove(obj.OnAttach, new Action<AttachableObject, AttachablePoint>(Attached));
	}

	private void Attached(AttachableObject obj, AttachablePoint point)
	{
		Quaternion localRotation = twistTransform.localRotation;
		GoTweenConfig goTweenConfig = new GoTweenConfig().localPosition(Vector3.zero).setEaseType(GoEaseType.CircInOut);
		if (axisOfRotation == Axis.X)
		{
			twistTransform.localPosition = Vector3.right * 0.02f;
			goTweenConfig.localRotation(new Vector3(localRotation.eulerAngles.x + 180f, localRotation.eulerAngles.y, localRotation.eulerAngles.z));
		}
		else if (axisOfRotation == Axis.Y)
		{
			twistTransform.localPosition = Vector3.up * 0.02f;
			goTweenConfig.localRotation(new Vector3(localRotation.eulerAngles.x, localRotation.eulerAngles.y + 180f, localRotation.eulerAngles.z));
		}
		else if (axisOfRotation == Axis.Z)
		{
			twistTransform.localPosition = Vector3.back * 0.02f;
			goTweenConfig.localRotation(new Vector3(localRotation.eulerAngles.x, localRotation.eulerAngles.y, localRotation.eulerAngles.z - 180f), true);
		}
		Go.to(twistTransform, 1f, goTweenConfig);
		StartCoroutine(WaitThenResetRotation());
	}

	private IEnumerator WaitThenResetRotation()
	{
		yield return new WaitForSeconds(1f);
		twistTransform.localRotation = ogRotation;
	}
}
