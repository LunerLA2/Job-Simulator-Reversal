using OwlchemyVR;
using UnityEngine;

public class TrackLocalPosition : MonoBehaviour
{
	[SerializeField]
	[Header("Events")]
	private LocalPositionChangedEvent localPositionChangedEvent;

	[Header("Optionally update only when grabbed")]
	[SerializeField]
	private bool onlyTrackWhileGrabbed;

	[SerializeField]
	private GrabbableItem grabbableItem;

	private Vector3 lastPos;

	private void Start()
	{
		lastPos = base.transform.localPosition;
		if (onlyTrackWhileGrabbed && grabbableItem == null)
		{
			Debug.LogError("TrackLocalPosition is missing grabbable slider, but wants to update when grabbed! Drag in the reference.", base.transform);
		}
	}

	private void LateUpdate()
	{
		if (localPositionChangedEvent.GetPersistentEventCount() == 0)
		{
			Debug.LogWarning("TrackLocalPosition doesn't have any listeners! Disabling.", base.transform);
			base.enabled = false;
		}
		else if ((!onlyTrackWhileGrabbed || grabbableItem.IsCurrInHand) && lastPos != base.transform.localPosition)
		{
			lastPos = base.transform.localPosition;
			localPositionChangedEvent.Invoke(lastPos);
		}
	}
}
