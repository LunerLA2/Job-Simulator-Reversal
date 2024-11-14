using UnityEngine;

public class AirFilterAttachPointController : MonoBehaviour
{
	[SerializeField]
	private AttachablePoint airFilterAttachPoint;

	[SerializeField]
	private AttachablePoint batteryAttachPoint;

	private bool spinFan;

	private bool airFilterAttached;

	private bool batteryAttached;

	private void OnEnable()
	{
		airFilterAttachPoint.OnObjectWasAttached += OnAirFilterAttached;
		batteryAttachPoint.OnObjectWasAttached += OnBatteryAttached;
		airFilterAttachPoint.OnObjectWasDetached += OnAirFilterDetached;
		batteryAttachPoint.OnObjectWasDetached += OnBatteryDetached;
	}

	private void OnDisable()
	{
		airFilterAttachPoint.OnObjectWasAttached -= OnAirFilterAttached;
		batteryAttachPoint.OnObjectWasAttached -= OnBatteryAttached;
		airFilterAttachPoint.OnObjectWasDetached -= OnAirFilterDetached;
		batteryAttachPoint.OnObjectWasDetached -= OnBatteryDetached;
	}

	private void OnAirFilterAttached(AttachablePoint point, AttachableObject obj)
	{
		airFilterAttached = true;
		CheckSpinConditions();
	}

	private void OnBatteryAttached(AttachablePoint point, AttachableObject obj)
	{
		batteryAttached = true;
		CheckSpinConditions();
	}

	private void OnAirFilterDetached(AttachablePoint point, AttachableObject obj)
	{
		airFilterAttached = false;
		CheckSpinConditions();
	}

	private void OnBatteryDetached(AttachablePoint point, AttachableObject obj)
	{
		batteryAttached = false;
		CheckSpinConditions();
	}

	private void CheckSpinConditions()
	{
		if (airFilterAttached && batteryAttached)
		{
			spinFan = true;
		}
		else
		{
			spinFan = false;
		}
		if (airFilterAttached)
		{
			airFilterAttachPoint.GetAttachedObject(0).GetComponent<VehicleAirFilter>().SetRotationStatus(spinFan);
		}
	}

	private void Start()
	{
		if (batteryAttachPoint.IsOccupied)
		{
			batteryAttached = true;
		}
		if (airFilterAttachPoint.IsOccupied)
		{
			airFilterAttached = true;
		}
		CheckSpinConditions();
	}

	private void Update()
	{
	}
}
