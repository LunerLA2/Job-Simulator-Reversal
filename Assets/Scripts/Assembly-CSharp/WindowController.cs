using System;
using OwlchemyVR;
using UnityEngine;

public class WindowController : MonoBehaviour
{
	[Tooltip("The mesh that is moved and scaled")]
	[SerializeField]
	private Transform windowGlass;

	[SerializeField]
	private Transform handle;

	[SerializeField]
	private GrabbableItem grabbableItem;

	[SerializeField]
	private ConfigurableJoint joint;

	[SerializeField]
	private Transform centerOfRotation;

	[SerializeField]
	private float downYPosition;

	[SerializeField]
	private float downYScale;

	[SerializeField]
	private float upYPosition;

	[SerializeField]
	private float upYScale;

	[Range(0f, 1f)]
	public float progress = 1f;

	private Vector3 localWindowPosition;

	private Vector3 localWindowScale;

	private Vector3 lastHandlePosition;

	private Vector3 handlePosition;

	private void Awake()
	{
		lastHandlePosition = handlePosition;
	}

	private void OnEnable()
	{
		GrabbableItem obj = grabbableItem;
		obj.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(obj.OnGrabbed, new Action<GrabbableItem>(Grabbed));
	}

	private void OnDisable()
	{
		GrabbableItem obj = grabbableItem;
		obj.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(obj.OnGrabbed, new Action<GrabbableItem>(Grabbed));
	}

	private void Grabbed(GrabbableItem item)
	{
	}

	private void LateUpdate()
	{
		if (!grabbableItem.IsCurrInHand)
		{
			return;
		}
		handlePosition = handle.position;
		Vector2 center = new Vector2(centerOfRotation.position.y, centerOfRotation.position.z);
		Vector2 vector = new Vector2(lastHandlePosition.y, lastHandlePosition.z);
		Vector2 vector2 = new Vector2(handlePosition.y, handlePosition.z);
		float clockWiseDelta = GetClockWiseDelta(center, vector, vector2);
		if (vector != vector2)
		{
			if (clockWiseDelta > 0f)
			{
				progress += clockWiseDelta * 10f;
			}
			else
			{
				progress -= (0f - clockWiseDelta) * 10f;
			}
		}
		progress = Mathf.Clamp01(progress);
		localWindowPosition = windowGlass.localPosition;
		localWindowScale = windowGlass.localScale;
		localWindowPosition.y = Mathf.Lerp(downYPosition, upYPosition, progress);
		localWindowScale.y = Mathf.Lerp(downYScale, upYScale, progress);
		windowGlass.localPosition = localWindowPosition;
		windowGlass.localScale = localWindowScale;
		lastHandlePosition = handle.position;
	}

	private float GetClockWiseDelta(Vector2 center, Vector2 lastFrame, Vector2 currentFrame)
	{
		return (lastFrame.x - center.x) * (currentFrame.y - center.y) - (lastFrame.y - center.y) * (currentFrame.x - center.x);
	}
}
