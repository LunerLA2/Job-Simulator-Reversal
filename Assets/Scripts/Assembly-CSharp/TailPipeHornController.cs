using System;
using UnityEngine;

public class TailPipeHornController : MonoBehaviour
{
	[SerializeField]
	private AttachableObject attachableObject;

	[SerializeField]
	private AudioClip hornAttachClip;

	private void OnEnable()
	{
		AttachableObject obj = attachableObject;
		obj.OnAttach = (Action<AttachableObject, AttachablePoint>)Delegate.Combine(obj.OnAttach, new Action<AttachableObject, AttachablePoint>(OnAttach));
	}

	private void OnDisable()
	{
		AttachableObject obj = attachableObject;
		obj.OnAttach = (Action<AttachableObject, AttachablePoint>)Delegate.Remove(obj.OnAttach, new Action<AttachableObject, AttachablePoint>(OnAttach));
	}

	private void OnAttach(AttachableObject attachableObject, AttachablePoint attachablePoint)
	{
		AudioManager.Instance.Play(base.transform.position, hornAttachClip, 1f, 1f);
	}
}
