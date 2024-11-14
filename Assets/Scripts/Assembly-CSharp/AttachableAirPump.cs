using System;
using System.Collections;
using UnityEngine;

public class AttachableAirPump : MonoBehaviour
{
	[SerializeField]
	private AttachableObject pumpAttachableObject;

	[SerializeField]
	private GrabbableSlider grabbableSlider;

	[SerializeField]
	private bool autoInflateOnAttach;

	[SerializeField]
	private GameObject nonInteractableHandleMesh;

	private bool pumped;

	private void Start()
	{
		grabbableSlider.gameObject.SetActive(false);
		nonInteractableHandleMesh.SetActive(true);
	}

	private void OnEnable()
	{
		AttachableObject attachableObject = pumpAttachableObject;
		attachableObject.OnAttach = (Action<AttachableObject, AttachablePoint>)Delegate.Combine(attachableObject.OnAttach, new Action<AttachableObject, AttachablePoint>(PumpAttached));
		AttachableObject attachableObject2 = pumpAttachableObject;
		attachableObject2.OnDetach = (Action<AttachableObject, AttachablePoint>)Delegate.Combine(attachableObject2.OnDetach, new Action<AttachableObject, AttachablePoint>(PumpDetached));
	}

	private void OnDisable()
	{
		AttachableObject attachableObject = pumpAttachableObject;
		attachableObject.OnAttach = (Action<AttachableObject, AttachablePoint>)Delegate.Remove(attachableObject.OnAttach, new Action<AttachableObject, AttachablePoint>(PumpAttached));
		AttachableObject attachableObject2 = pumpAttachableObject;
		attachableObject2.OnDetach = (Action<AttachableObject, AttachablePoint>)Delegate.Remove(attachableObject2.OnDetach, new Action<AttachableObject, AttachablePoint>(PumpDetached));
	}

	private void PumpAttached(AttachableObject obj, AttachablePoint point)
	{
		if (autoInflateOnAttach)
		{
			Wheel component = point.transform.parent.GetComponent<Wheel>();
			if (component != null)
			{
				component.StartInflation(Wheel.InflationState.Inflate);
			}
		}
		grabbableSlider.gameObject.SetActive(true);
		nonInteractableHandleMesh.SetActive(false);
		grabbableSlider.LockLower();
		grabbableSlider.OnUpperLocked += PumpDepressed;
		grabbableSlider.OnLowerLocked += PumpReset;
	}

	private void PumpDetached(AttachableObject obj, AttachablePoint point)
	{
		if (autoInflateOnAttach)
		{
			Wheel component = point.transform.parent.GetComponent<Wheel>();
			if (component != null)
			{
				component.StopInflation();
			}
		}
		grabbableSlider.OnUpperLocked -= PumpDepressed;
		grabbableSlider.OnLowerLocked -= PumpReset;
		grabbableSlider.LockUpper();
		grabbableSlider.gameObject.SetActive(false);
		nonInteractableHandleMesh.SetActive(true);
	}

	public void PumpDepressed(GrabbableSlider grabbableSlider, bool isInitial)
	{
		Pump();
	}

	public void PumpReset(GrabbableSlider grabbableSlider, bool isInitial)
	{
		Reset();
	}

	public void Pump()
	{
		if (pumped)
		{
			return;
		}
		pumped = true;
		if (pumpAttachableObject.CurrentlyAttachedTo != null)
		{
			Wheel component = pumpAttachableObject.CurrentlyAttachedTo.transform.parent.GetComponent<Wheel>();
			if (component != null)
			{
				StartCoroutine(SinglePump(component));
			}
		}
	}

	private IEnumerator SinglePump(Wheel wheel)
	{
		wheel.StartInflation(Wheel.InflationState.Inflate);
		yield return new WaitForSeconds(0.2f);
		wheel.StopInflation();
	}

	public void Reset()
	{
		pumped = false;
	}
}
