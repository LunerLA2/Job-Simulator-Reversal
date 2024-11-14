using System;
using UnityEngine;

public class AirPumpAttachment : MonoBehaviour
{
	[SerializeField]
	private AttachableObject attachableObject;

	[SerializeField]
	private Collider airPumpZone;

	private bool airPumpIsActive;

	[SerializeField]
	private float raycastDistance = 0.5f;

	[SerializeField]
	private ParticleSystem visualEffects;

	private void Start()
	{
		airPumpIsActive = false;
		airPumpZone.enabled = false;
	}

	private void OnEnable()
	{
		AttachableObject obj = attachableObject;
		obj.OnAttach = (Action<AttachableObject, AttachablePoint>)Delegate.Combine(obj.OnAttach, new Action<AttachableObject, AttachablePoint>(AttachedToHandle));
		AttachableObject obj2 = attachableObject;
		obj2.OnDetach = (Action<AttachableObject, AttachablePoint>)Delegate.Combine(obj2.OnDetach, new Action<AttachableObject, AttachablePoint>(DetachedToHandle));
	}

	private void OnDisable()
	{
		AttachableObject obj = attachableObject;
		obj.OnAttach = (Action<AttachableObject, AttachablePoint>)Delegate.Remove(obj.OnAttach, new Action<AttachableObject, AttachablePoint>(AttachedToHandle));
		AttachableObject obj2 = attachableObject;
		obj2.OnDetach = (Action<AttachableObject, AttachablePoint>)Delegate.Remove(obj2.OnDetach, new Action<AttachableObject, AttachablePoint>(DetachedToHandle));
	}

	private void AttachedToHandle(AttachableObject obj, AttachablePoint cd)
	{
		airPumpIsActive = true;
		airPumpZone.enabled = true;
		visualEffects.Play();
	}

	private void DetachedToHandle(AttachableObject obj, AttachablePoint cd)
	{
		airPumpIsActive = false;
		airPumpZone.enabled = false;
		visualEffects.Stop();
	}

	private void FixedUpdate()
	{
		if (!airPumpIsActive)
		{
			return;
		}
		RaycastHit[] array = Physics.RaycastAll(base.transform.position, base.transform.forward, raycastDistance, 256);
		if (array.Length == 0)
		{
			return;
		}
		for (int i = 0; i < array.Length; i++)
		{
			if ((bool)array[i].rigidbody)
			{
				float num = 10f;
				array[i].rigidbody.AddForce((array[i].rigidbody.position - base.transform.position) * num);
			}
		}
	}
}
