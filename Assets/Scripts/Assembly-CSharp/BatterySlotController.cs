using UnityEngine;

public class BatterySlotController : MonoBehaviour
{
	[SerializeField]
	private GrabbableHinge needleHinge;

	[SerializeField]
	private AttachablePoint attachPoint;

	private GoTween lastTween;

	private void Update()
	{
	}

	private void OnEnable()
	{
		attachPoint.OnObjectWasAttached += OnBatteryAttached;
		attachPoint.OnObjectWasDetached += OnBatteryDetached;
	}

	private void OnDisable()
	{
		attachPoint.OnObjectWasAttached -= OnBatteryAttached;
		attachPoint.OnObjectWasDetached -= OnBatteryAttached;
	}

	private void OnBatteryAttached(AttachablePoint point, AttachableObject obj)
	{
		if (lastTween != null)
		{
			lastTween.pause();
		}
		lastTween = needleHinge.transform.localRotationTo(1f, new Vector3(0f, 0f, needleHinge.upperLimit));
	}

	private void OnBatteryDetached(AttachablePoint point, AttachableObject obj)
	{
		if (lastTween != null)
		{
			lastTween.pause();
		}
		lastTween = needleHinge.transform.localRotationTo(1f, new Vector3(0f, 0f, needleHinge.lowerLimit));
	}
}
