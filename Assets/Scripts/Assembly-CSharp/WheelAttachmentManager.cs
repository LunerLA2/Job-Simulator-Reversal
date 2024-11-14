using UnityEngine;

public class WheelAttachmentManager : VehicleHardware
{
	[SerializeField]
	private Transform spinTransform;

	[SerializeField]
	private AttachablePoint wheelAttachPoint;

	private Wheel wheelReference;

	[SerializeField]
	private ParticleSystem sparksPFX;

	[SerializeField]
	private AudioSourceHelper screechSource;

	public Transform SpinTransform
	{
		get
		{
			return spinTransform;
		}
	}

	public AttachablePoint WheelAttachPoint
	{
		get
		{
			return wheelAttachPoint;
		}
	}

	private void OnEnable()
	{
		wheelAttachPoint.OnObjectWasAttached += WheelAttached;
		wheelAttachPoint.OnObjectWasDetached += WheelDetached;
	}

	private void OnDisable()
	{
		wheelAttachPoint.OnObjectWasAttached -= WheelAttached;
		wheelAttachPoint.OnObjectWasDetached -= WheelDetached;
	}

	public void SetFlatFX(bool s)
	{
		if (!(this == null))
		{
			ParticleSystem.EmissionModule emission = sparksPFX.emission;
			emission.enabled = s;
			if (s)
			{
				screechSource.Play();
			}
			else
			{
				screechSource.Stop();
			}
		}
	}

	public void SpawnWheel(GameObject wheelPrefab)
	{
		GameObject gameObject = Object.Instantiate(wheelPrefab);
		BasePrefabSpawner component = gameObject.GetComponent<BasePrefabSpawner>();
		if (component != null)
		{
			gameObject = component.LastSpawnedPrefabGO;
		}
		AttachableObject component2 = gameObject.GetComponent<AttachableObject>();
		component2.AttachTo(wheelAttachPoint, -1, true, true);
		Wheel component3 = component2.GetComponent<Wheel>();
		if (component3 != null)
		{
			wheelReference = component3;
			component3.SetInflationPercentage(1f);
		}
	}

	public void SetWheelInflation(float perc)
	{
		if (wheelAttachPoint.NumAttachedObjects > 0)
		{
			Wheel component = wheelAttachPoint.AttachedObjects[0].GetComponent<Wheel>();
			if (component != null)
			{
				component.SetInflationPercentage(perc);
			}
		}
	}

	public bool GetWheelIsFlat()
	{
		if (wheelReference != null)
		{
			return wheelReference.CurrentInflationPercentage <= 0.4f;
		}
		return true;
	}

	private void WheelAttached(AttachablePoint point, AttachableObject obj)
	{
		Wheel component = obj.GetComponent<Wheel>();
		wheelReference = component;
	}

	private void WheelDetached(AttachablePoint point, AttachableObject obj)
	{
		wheelReference = null;
	}
}
