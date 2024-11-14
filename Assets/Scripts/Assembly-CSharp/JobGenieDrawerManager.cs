using System;
using UnityEngine;

public class JobGenieDrawerManager : MonoBehaviour
{
	[SerializeField]
	private RigidbodyEnterExitTriggerEvents rigidBodyEvents;

	[SerializeField]
	private AttachablePoint[] cartGenieAttachPoints;

	[SerializeField]
	private AttachablePoint[] cartGenieAttachPointsPSVR;

	[SerializeField]
	private GrabbableSlider drawerSlider;

	[SerializeField]
	private GrabbableSlider drawerSliderPSVR;

	private float lastDrawerOffset = float.MaxValue;

	private void OnEnable()
	{
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = rigidBodyEvents;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(Enter));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents2 = rigidBodyEvents;
		rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger, new Action<Rigidbody>(Exit));
		for (int i = 0; i < cartGenieAttachPoints.Length; i++)
		{
			if (cartGenieAttachPoints[i] != null)
			{
				cartGenieAttachPoints[i].OnObjectWasRefilled += OnCartAttachPointRefilled;
				cartGenieAttachPoints[i].OnObjectWasAttached += OnGenieAttachedToCart;
			}
		}
	}

	private void OnDisable()
	{
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = rigidBodyEvents;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(Enter));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents2 = rigidBodyEvents;
		rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger, new Action<Rigidbody>(Exit));
	}

	private void LateUpdate()
	{
		bool flag = false;
		float num = float.MinValue;
		AttachablePoint[] array = cartGenieAttachPoints;
		GrabbableSlider grabbableSlider = drawerSlider;
		if (lastDrawerOffset != grabbableSlider.Offset)
		{
			lastDrawerOffset = grabbableSlider.Offset;
			foreach (AttachablePoint attachablePoint in array)
			{
				if (attachablePoint.NumAttachedObjects > 0)
				{
					bool flag2 = ShouldAttachPointOnAttachedGenieBeEnabled(attachablePoint);
					JobGenieCartridge component = attachablePoint.GetAttachedObject(0).GetComponent<JobGenieCartridge>();
					if (!flag2 && (component == null || component.AttachablePoint.NumAttachedObjects > 0))
					{
						flag = true;
						num = Mathf.Max(num, attachablePoint.transform.localPosition.z + attachablePoint.transform.parent.localPosition.z + 0.04f);
						flag2 = true;
					}
					if (component != null)
					{
						component.AttachablePoint.enabled = flag2;
					}
				}
			}
		}
		if (flag)
		{
			grabbableSlider.SetNormalizedAngle(Mathf.Abs(grabbableSlider.UpperLimit + num) / grabbableSlider.Range);
			lastDrawerOffset = grabbableSlider.Offset;
		}
	}

	private void Enter(Rigidbody rb)
	{
		if ((bool)rb.GetComponentInChildren<JobCartridge>())
		{
			rb.GetComponentInChildren<AttachableObject>().enabled = false;
		}
		if ((bool)rb.GetComponentInChildren<JobGenieCartridge>())
		{
			rb.GetComponentInChildren<JobGenieCartridge>().AttachablePoint.enabled = false;
		}
	}

	private void Exit(Rigidbody rb)
	{
		if ((bool)rb.GetComponentInChildren<JobCartridge>())
		{
			rb.GetComponentInChildren<AttachableObject>().enabled = true;
		}
		if ((bool)rb.GetComponentInChildren<JobGenieCartridge>())
		{
			rb.GetComponentInChildren<JobGenieCartridge>().AttachablePoint.enabled = true;
		}
	}

	private void OnCartAttachPointRefilled(AttachablePoint point, AttachableObject obj)
	{
		JobGenieCartridge component = obj.GetComponent<JobGenieCartridge>();
		if (component != null)
		{
			component.AttachablePoint.enabled = ShouldAttachPointOnAttachedGenieBeEnabled(point);
		}
	}

	private void OnGenieAttachedToCart(AttachablePoint point, AttachableObject obj)
	{
		JobGenieCartridge component = obj.GetComponent<JobGenieCartridge>();
		if (component != null)
		{
			component.AttachablePoint.enabled = ShouldAttachPointOnAttachedGenieBeEnabled(point);
		}
	}

	private bool ShouldAttachPointOnAttachedGenieBeEnabled(AttachablePoint point)
	{
		float num = point.transform.localPosition.z + point.transform.parent.localPosition.z + 0.04f;
		GrabbableSlider grabbableSlider = drawerSlider;
		if (grabbableSlider.LowerLimit - num < grabbableSlider.Offset)
		{
			return false;
		}
		return true;
	}
}
