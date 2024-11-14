using System;
using UnityEngine;

[RequireComponent(typeof(AttachableObject))]
public class OliveController : MonoBehaviour
{
	[SerializeField]
	private GameObject toothpick;

	private AttachableObject attachable;

	private void Awake()
	{
		attachable = GetComponent<AttachableObject>();
	}

	private void OnEnable()
	{
		AttachableObject attachableObject = attachable;
		attachableObject.OnAttach = (Action<AttachableObject, AttachablePoint>)Delegate.Combine(attachableObject.OnAttach, new Action<AttachableObject, AttachablePoint>(Attached));
		AttachableObject attachableObject2 = attachable;
		attachableObject2.OnDetach = (Action<AttachableObject, AttachablePoint>)Delegate.Combine(attachableObject2.OnDetach, new Action<AttachableObject, AttachablePoint>(Detached));
	}

	private void OnDisable()
	{
		AttachableObject attachableObject = attachable;
		attachableObject.OnAttach = (Action<AttachableObject, AttachablePoint>)Delegate.Remove(attachableObject.OnAttach, new Action<AttachableObject, AttachablePoint>(Attached));
		AttachableObject attachableObject2 = attachable;
		attachableObject2.OnDetach = (Action<AttachableObject, AttachablePoint>)Delegate.Remove(attachableObject2.OnDetach, new Action<AttachableObject, AttachablePoint>(Detached));
	}

	private void Attached(AttachableObject o, AttachablePoint point)
	{
		toothpick.SetActive(true);
	}

	private void Detached(AttachableObject o, AttachablePoint point)
	{
		toothpick.SetActive(false);
	}
}
