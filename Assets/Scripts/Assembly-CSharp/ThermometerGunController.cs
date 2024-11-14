using System;
using OwlchemyVR;
using TMPro;
using UnityEngine;

public class ThermometerGunController : MonoBehaviour
{
	[SerializeField]
	private GrabbableItem grabbable;

	[SerializeField]
	private Transform raycastPoint;

	[SerializeField]
	private LayerMask raycastLayerMask;

	[SerializeField]
	private float maxRaycastDistance;

	[SerializeField]
	private TextMeshPro temperatureText;

	[SerializeField]
	private TextMeshPro unitsText;

	[SerializeField]
	private LineRenderer laser;

	private bool isReading;

	private float targetTemperature;

	private float temperature;

	private int roundedTemperature;

	private Collider cachedClosestSolidCollider;

	private TemperatureStateItem cachedTargetItem;

	private void OnEnable()
	{
		GrabbableItem grabbableItem = grabbable;
		grabbableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(grabbableItem.OnGrabbed, new Action<GrabbableItem>(Grabbed));
		GrabbableItem grabbableItem2 = grabbable;
		grabbableItem2.OnReleased = (Action<GrabbableItem>)Delegate.Combine(grabbableItem2.OnReleased, new Action<GrabbableItem>(Released));
		if (grabbable.IsCurrInHand)
		{
			StartReading();
		}
		else
		{
			StopReading();
		}
	}

	private void OnDisable()
	{
		StopReading();
		GrabbableItem grabbableItem = grabbable;
		grabbableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(grabbableItem.OnGrabbed, new Action<GrabbableItem>(Grabbed));
		GrabbableItem grabbableItem2 = grabbable;
		grabbableItem2.OnReleased = (Action<GrabbableItem>)Delegate.Remove(grabbableItem2.OnReleased, new Action<GrabbableItem>(Released));
	}

	public void StartReading()
	{
		isReading = true;
		temperatureText.gameObject.SetActive(true);
		unitsText.gameObject.SetActive(true);
		laser.gameObject.SetActive(true);
		SetTemperature(0f, true);
		ClearRaycastCache();
	}

	public void StopReading()
	{
		isReading = false;
		temperatureText.gameObject.SetActive(false);
		unitsText.gameObject.SetActive(false);
		laser.gameObject.SetActive(false);
		ClearRaycastCache();
	}

	private void SetTemperature(float newTemperature, bool snap = false)
	{
		targetTemperature = newTemperature;
		if (snap)
		{
			temperature = targetTemperature;
		}
		int num = Mathf.RoundToInt(temperature);
		if (num != roundedTemperature)
		{
			roundedTemperature = num;
			RefreshTemperatureDisplay();
		}
	}

	private void RefreshTemperatureDisplay()
	{
		temperatureText.text = roundedTemperature.ToString();
	}

	private void ClearRaycastCache()
	{
		cachedClosestSolidCollider = null;
		cachedTargetItem = null;
	}

	private void Update()
	{
		if (!isReading)
		{
			return;
		}
		RaycastHit[] array = Physics.RaycastAll(raycastPoint.position, raycastPoint.forward, maxRaycastDistance, raycastLayerMask);
		Collider collider = null;
		Rigidbody rigidbody = null;
		TemperatureStateItem temperatureStateItem = null;
		float distance = maxRaycastDistance;
		for (int i = 0; i < array.Length; i++)
		{
			RaycastHit raycastHit = array[i];
			if (!raycastHit.collider.isTrigger && raycastHit.distance < distance)
			{
				distance = raycastHit.distance;
				collider = raycastHit.collider;
				rigidbody = raycastHit.rigidbody;
			}
		}
		if (cachedClosestSolidCollider != collider)
		{
			cachedClosestSolidCollider = collider;
			if (collider != null)
			{
				temperatureStateItem = collider.GetComponent<TemperatureStateItem>();
				if (temperatureStateItem == null && rigidbody != null)
				{
					temperatureStateItem = rigidbody.GetComponent<TemperatureStateItem>();
				}
			}
			cachedTargetItem = temperatureStateItem;
		}
		else
		{
			temperatureStateItem = cachedTargetItem;
		}
		if (temperatureStateItem != null)
		{
			SetTemperature(temperatureStateItem.TemperatureCelsius);
		}
		else
		{
			SetTemperature(21f);
		}
		laser.SetPosition(1, new Vector3(0f, 0f, distance));
		if (Mathf.Abs(temperature - targetTemperature) < 0.01f)
		{
			temperature = targetTemperature;
		}
		else
		{
			temperature = Mathf.Lerp(temperature, targetTemperature, Time.deltaTime * 20f);
		}
		RefreshTemperatureDisplay();
	}

	private void Grabbed(GrabbableItem grabbable)
	{
		StartReading();
	}

	private void Released(GrabbableItem grabbable)
	{
		StopReading();
	}
}
