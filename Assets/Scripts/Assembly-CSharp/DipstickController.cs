using OwlchemyVR;
using UnityEngine;

public class DipstickController : MonoBehaviour
{
	private const float fluidHackMin = 0.13f;

	private const float fluidHackMax = 0.03f;

	[SerializeField]
	private ParticleCollectionZone fluidTrackerCollectionZone;

	[SerializeField]
	private ParticleCollectionZone funnelParticleCollectionZone;

	[SerializeField]
	private float funnelDrainPerSecond = 100f;

	[SerializeField]
	private GrabbableSlider slider;

	[SerializeField]
	private ParticleSystem leakPFX;

	[SerializeField]
	private float maxVolume = 300f;

	[SerializeField]
	private float maxLeakPerSecond = 125f;

	[SerializeField]
	private UVScroll uvScroll;

	[SerializeField]
	private UVScroll secondaryUVScroll;

	[SerializeField]
	private WorldItem myWorldItem;

	[SerializeField]
	private float initialFluidAmount;

	[SerializeField]
	private WorldItemData initialFluidData;

	[SerializeField]
	private Transform[] tubeMeshes;

	private Transform enginePFXParent;

	private ParticleSystem[] engineParticles;

	private float normalizedDistanceToStartLeaking = 0.95f;

	private float leakMaxRate;

	private bool leaking;

	private float containedFluid;

	private Material uvMat;

	private Material secondaryUVMat;

	private FluidImpactParticleManager.FluidImpactDetails impactDetails;

	private void Awake()
	{
		impactDetails = new FluidImpactParticleManager.FluidImpactDetails();
		uvMat = uvScroll.GetComponent<MeshRenderer>().material;
		if (secondaryUVScroll != null)
		{
			secondaryUVMat = secondaryUVScroll.GetComponent<MeshRenderer>().material;
		}
		if (initialFluidAmount > 0f)
		{
			if (initialFluidData != null)
			{
				fluidTrackerCollectionZone.ApplyParticleQuantity(initialFluidData, initialFluidAmount, 21f);
			}
			else
			{
				Debug.LogWarning("Dipstick initial fluid data not set. Dipstick starting empty");
			}
		}
	}

	public void ChangeInitialFluid(WorldItemData oilData, float percentFull)
	{
		float num = Mathf.Clamp01(percentFull);
		fluidTrackerCollectionZone.Clear();
		fluidTrackerCollectionZone.ApplyParticleQuantity(oilData, num * maxVolume, 21f);
	}

	private void OnEnable()
	{
		slider.OnLowerUnlocked += LowerUnlocked;
		slider.OnLowerLocked += LowerLocked;
	}

	private void OnDisable()
	{
		slider.OnLowerUnlocked -= LowerUnlocked;
		slider.OnLowerLocked -= LowerLocked;
	}

	private void LowerUnlocked(GrabbableSlider gs)
	{
		SetTubeMeshVisible(true);
	}

	private void LowerLocked(GrabbableSlider slider, bool isInitial)
	{
		SetTubeMeshVisible(false);
	}

	private void SetTubeMeshVisible(bool enabled)
	{
		for (int i = 0; i < tubeMeshes.Length; i++)
		{
			tubeMeshes[i].gameObject.SetActive(enabled);
		}
	}

	private void Start()
	{
		leakMaxRate = GetEmissionRate();
		SetParticleEmisionRate(0f);
		SetTubeMeshVisible(false);
	}

	private void Update()
	{
		if (funnelParticleCollectionZone.GetTotalQuantity() > 0f && funnelParticleCollectionZone.MajorityFluidData != null)
		{
			fluidTrackerCollectionZone.ApplyParticleQuantity(funnelParticleCollectionZone.MajorityFluidData, funnelDrainPerSecond * Time.deltaTime, funnelParticleCollectionZone.TemperatureStateItem.TemperatureCelsius);
			funnelParticleCollectionZone.RemoveParticleQuantity(funnelDrainPerSecond * Time.deltaTime, ref impactDetails);
			SetEngineParticleColor();
		}
		containedFluid = fluidTrackerCollectionZone.GetTotalQuantity();
		uvMat.SetColor("_Color", fluidTrackerCollectionZone.CalculateCombinedFluidColor());
		uvScroll.manualY = Mathf.Lerp(0.13f, 0.03f, Mathf.InverseLerp(0f, maxVolume, containedFluid));
		if (secondaryUVMat != null)
		{
			secondaryUVMat.SetColor("_Color", fluidTrackerCollectionZone.CalculateCombinedFluidColor());
		}
		if (containedFluid > maxVolume)
		{
			fluidTrackerCollectionZone.RemoveParticleQuantity(containedFluid - maxVolume, ref impactDetails);
		}
		if (!slider.IsLowerLocked && !slider.IsUpperLocked)
		{
			GameEventsManager.Instance.ItemActionOccurredWithAmount(myWorldItem.Data, "USED_PARTIALLY", slider.NormalizedOffset);
		}
		if (slider.NormalizedOffset >= normalizedDistanceToStartLeaking && containedFluid > 0f)
		{
			if (!leaking)
			{
				GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "ACTIVATED");
				leaking = true;
			}
			float num = Mathf.InverseLerp(normalizedDistanceToStartLeaking, 1f, slider.NormalizedOffset);
			SetParticleEmisionRate(num * leakMaxRate);
			leakPFX.startColor = fluidTrackerCollectionZone.CalculateCombinedFluidColor();
			float num2 = num * Time.deltaTime * maxLeakPerSecond;
			if (num2 > containedFluid)
			{
				num2 = containedFluid;
			}
			fluidTrackerCollectionZone.RemoveParticleQuantity(num2, ref impactDetails);
		}
		else if (leaking)
		{
			leaking = false;
			GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "DEACTIVATED");
			SetParticleEmisionRate(0f);
		}
	}

	private void SetParticleEmisionRate(float emissionRate)
	{
		ParticleSystem.EmissionModule emission = leakPFX.emission;
		ParticleSystem.MinMaxCurve rate = emission.rate;
		rate.constantMax = emissionRate;
		emission.rate = rate;
	}

	private float GetEmissionRate()
	{
		return leakPFX.emission.rate.constantMax;
	}

	private void SetEngineParticleColor()
	{
		if (engineParticles != null)
		{
			for (int i = 0; i < engineParticles.Length; i++)
			{
				engineParticles[i].startColor = fluidTrackerCollectionZone.CalculateCombinedFluidColor();
			}
		}
		else if (enginePFXParent != null)
		{
			engineParticles = enginePFXParent.GetComponentsInChildren<ParticleSystem>();
		}
	}

	public void SetEnginePFXParent(Transform parent)
	{
		enginePFXParent = parent;
	}
}
