using System;
using OwlchemyVR;
using UnityEngine;
using UnityEngine.Events;

public class PullRopeController : MonoBehaviour
{
	[SerializeField]
	private GrabbableItem grabbableItem;

	[SerializeField]
	private Transform transformToTrack;

	[SerializeField]
	private float worldYToPullPast;

	private bool wasPastThreshold;

	private bool wasPulledSinceGrab;

	[SerializeField]
	private UnityEvent OnPulled;

	[SerializeField]
	private UnityEvent OnReleasedAfterPull;

	private void OnEnable()
	{
		GrabbableItem obj = grabbableItem;
		obj.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(obj.OnGrabbed, new Action<GrabbableItem>(Grabbed));
		GrabbableItem obj2 = grabbableItem;
		obj2.OnReleased = (Action<GrabbableItem>)Delegate.Combine(obj2.OnReleased, new Action<GrabbableItem>(Released));
	}

	private void OnDisable()
	{
		GrabbableItem obj = grabbableItem;
		obj.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(obj.OnGrabbed, new Action<GrabbableItem>(Grabbed));
		GrabbableItem obj2 = grabbableItem;
		obj2.OnReleased = (Action<GrabbableItem>)Delegate.Remove(obj2.OnReleased, new Action<GrabbableItem>(Released));
	}

	private void Update()
	{
		if (transformToTrack != null)
		{
			bool flag = transformToTrack.position.y <= worldYToPullPast;
			if (flag && !wasPastThreshold)
			{
				wasPastThreshold = true;
				wasPulledSinceGrab = true;
				OnPulled.Invoke();
			}
			else if (!flag && wasPastThreshold)
			{
				wasPastThreshold = false;
			}
		}
	}

	private void Grabbed(GrabbableItem item)
	{
		wasPulledSinceGrab = false;
	}

	private void Released(GrabbableItem item)
	{
		if (wasPulledSinceGrab)
		{
			wasPulledSinceGrab = false;
			OnReleasedAfterPull.Invoke();
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawSphere(new Vector3(base.transform.position.x, worldYToPullPast, base.transform.position.z), 0.01f);
	}
}
