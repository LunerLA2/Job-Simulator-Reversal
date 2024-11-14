using System;
using OwlchemyVR;
using UnityEngine;

[RequireComponent(typeof(FluidParticleRaycastEmitter))]
public class ContainerFluidSystem : MonoBehaviour
{
	private const float CUBIC_METERS_TO_ML = 1000000f;

	private const float CC_TO_KG_WATER = 0.001f;

	public const float SPEED_FOR_CONTAINER_TO_RETURN_TO_ROOM_TEMP = 0.05f;

	[HideInInspector]
	[SerializeField]
	private float fluidFullPercent;

	[SerializeField]
	[HideInInspector]
	private float fluidOverFillPercent;

	[SerializeField]
	private int numFluidSides = 8;

	[SerializeField]
	private bool showFluidSides;

	[SerializeField]
	private bool showFluidBottom;

	[SerializeField]
	private CapsuleCollider measurementCapsule;

	[SerializeField]
	private Transform fluidParent;

	[SerializeField]
	private FluidInContainerController fluidInContainerControllerPrefab;

	[SerializeField]
	private FluidOverflowParticle fluidOverflowParticlePrefab;

	[SerializeField]
	private ParticleCollectionZone particleCollectionZone;

	[SerializeField]
	[HideInInspector]
	private FluidInContainerController fluid;

	private FluidOverflowParticle fluidOverflowParticle;

	[SerializeField]
	private Transform additionaLowPoint;

	private float containerRadius;

	private float containerHeight;

	private Transform middleOfTopOfContainerTranform;

	private Transform lowPointOfRimTransform;

	private int lastFrameOfManualPour;

	private bool isCurrentlyPouring;

	[SerializeField]
	private WorldItemData startingFluidData;

	[Range(0f, 1f)]
	[SerializeField]
	private float startingPercentageOfCupFilled;

	[SerializeField]
	private bool isPouringEnabled = true;

	private FluidParticleRaycastEmitter fluidParticleRaycastEmitter;

	private float totalContainerVolume;

	private float containerMass;

	private float centerOfMassOffsetFromBottom = 0.45f;

	private Vector3 liquidCenterOfMass;

	private Vector3 emptyCenterOfMass;

	private FluidImpactParticleManager fluidImpactParticleEffectManagerCached;

	private WorldItemData currMajorityFluidData;

	[SerializeField]
	private RigidbodyRemover rigidbodyRemover;

	public Action<ContainerFluidSystem> OnFluidEmptied;

	public Action<ContainerFluidSystem> OnFluidPartiallyFilled;

	private FluidParticleEmitCollisionInfo fluidParticleEmitCollisionInfo = new FluidParticleEmitCollisionInfo();

	private Color prevFluidColor = Color.clear;

	private Rigidbody rb;

	private bool isParticleCollectionZoneDirty = true;

	private Vector3 fluidSurfaceLaggedPos;

	private Vector3 fluidSurfaceVelocity;

	public float FluidFullPercent
	{
		get
		{
			return fluidFullPercent;
		}
	}

	public float ContainerRadius
	{
		get
		{
			return containerRadius;
		}
	}

	public float ContainerHeight
	{
		get
		{
			return containerHeight;
		}
	}

	public WorldItemData CurrMajorityFluidData
	{
		get
		{
			return currMajorityFluidData;
		}
	}

	private void Awake()
	{
		fluidParticleRaycastEmitter = GetComponent<FluidParticleRaycastEmitter>();
		rb = GetComponent<Rigidbody>();
		containerHeight = measurementCapsule.height;
		containerRadius = measurementCapsule.radius;
		containerMass = rb.mass;
		totalContainerVolume = VolumeInMLFromRadiusAndHeight(containerRadius, containerHeight);
		if (base.transform.localScale != Vector3.one)
		{
			totalContainerVolume = VolumeInMLFromRadiusAndHeight(containerRadius * base.transform.localScale.x, containerHeight * base.transform.localScale.y);
			Debug.LogWarning("Container Fluid System not scaled correctly:" + base.gameObject.name, base.gameObject);
		}
		if (measurementCapsule.enabled)
		{
			Debug.LogError("Fluid Measurement Capsule Should not be enabled");
		}
		if (fluid == null)
		{
			fluid = UnityEngine.Object.Instantiate(fluidInContainerControllerPrefab, Vector3.zero, Quaternion.identity) as FluidInContainerController;
			fluid.transform.SetParent(fluidParent, false);
		}
		particleCollectionZone.SetSurfaceLocationTransform(fluid.SurfaceTransform);
		middleOfTopOfContainerTranform = new GameObject().transform;
		middleOfTopOfContainerTranform.SetParent(base.transform, false);
		middleOfTopOfContainerTranform.gameObject.name = "MiddleOfTopOfContainer";
		middleOfTopOfContainerTranform.position = fluidParent.transform.position;
		Vector3 localPosition = middleOfTopOfContainerTranform.localPosition;
		localPosition.y += containerHeight;
		middleOfTopOfContainerTranform.localPosition = localPosition;
		lowPointOfRimTransform = new GameObject().transform;
		lowPointOfRimTransform.gameObject.name = "LowPointOfRim";
		lowPointOfRimTransform.SetParent(base.transform, false);
		fluidOverflowParticle = UnityEngine.Object.Instantiate(fluidOverflowParticlePrefab, Vector3.zero, Quaternion.identity) as FluidOverflowParticle;
		fluidOverflowParticle.transform.SetParent(lowPointOfRimTransform, false);
		fluidOverflowParticle.SetEmissionMultiplier(0f);
		if (startingPercentageOfCupFilled > 0f && startingFluidData != null)
		{
			fluid.SurfaceTransform.localPosition = new Vector3(0f, startingPercentageOfCupFilled * containerHeight, 0f);
			particleCollectionZone.ApplyParticleQuantity(startingFluidData, startingPercentageOfCupFilled * totalContainerVolume, 21f);
		}
		UpdateStateFromParticleCollectionZone(false);
		UpdateLowestPositionOnCup();
		CalculateCenterOfMass();
	}

	private void Start()
	{
		fluid.SetParticleEffectsBasedOnTemperature(particleCollectionZone.AvgTemperatureCelsius);
		fluidSurfaceLaggedPos = fluid.SurfaceTransform.position;
	}

	private void OnEnable()
	{
		isParticleCollectionZoneDirty = true;
		ParticleCollectionZone obj = particleCollectionZone;
		obj.OnParticleIsCollecting = (Action<ParticleCollectionZone, WorldItemData, float>)Delegate.Combine(obj.OnParticleIsCollecting, new Action<ParticleCollectionZone, WorldItemData, float>(ParticleZoneIsCollecting));
		ParticleCollectionZone obj2 = particleCollectionZone;
		obj2.OnParticleQuantityUnitAdded = (Action<ParticleCollectionZone, WorldItemData>)Delegate.Combine(obj2.OnParticleQuantityUnitAdded, new Action<ParticleCollectionZone, WorldItemData>(ParticleQuantityUnitAdded));
		ParticleCollectionZone obj3 = particleCollectionZone;
		obj3.OnParticlesBeingRemoved = (Action<ParticleCollectionZone>)Delegate.Combine(obj3.OnParticlesBeingRemoved, new Action<ParticleCollectionZone>(ParticlesBeingRemoved));
		if (particleCollectionZone.TemperatureStateItem != null)
		{
			TemperatureStateItem temperatureStateItem = particleCollectionZone.TemperatureStateItem;
			temperatureStateItem.OnTemperatureChangeWholeUnit = (Action<TemperatureStateItem>)Delegate.Combine(temperatureStateItem.OnTemperatureChangeWholeUnit, new Action<TemperatureStateItem>(TemperatureChangeWholeUnit));
		}
		if (rigidbodyRemover != null)
		{
			RigidbodyRemover obj4 = rigidbodyRemover;
			obj4.OnRigidbodyAdded = (Action<RigidbodyRemover>)Delegate.Combine(obj4.OnRigidbodyAdded, new Action<RigidbodyRemover>(RecacheRigidbody));
		}
		fluid.SetParent(this);
		fluid.BuildMesh(numFluidSides, showFluidSides, showFluidBottom);
	}

	private void OnDisable()
	{
		ParticleCollectionZone obj = particleCollectionZone;
		obj.OnParticleIsCollecting = (Action<ParticleCollectionZone, WorldItemData, float>)Delegate.Remove(obj.OnParticleIsCollecting, new Action<ParticleCollectionZone, WorldItemData, float>(ParticleZoneIsCollecting));
		ParticleCollectionZone obj2 = particleCollectionZone;
		obj2.OnParticleQuantityUnitAdded = (Action<ParticleCollectionZone, WorldItemData>)Delegate.Remove(obj2.OnParticleQuantityUnitAdded, new Action<ParticleCollectionZone, WorldItemData>(ParticleQuantityUnitAdded));
		ParticleCollectionZone obj3 = particleCollectionZone;
		obj3.OnParticlesBeingRemoved = (Action<ParticleCollectionZone>)Delegate.Remove(obj3.OnParticlesBeingRemoved, new Action<ParticleCollectionZone>(ParticlesBeingRemoved));
		if (particleCollectionZone.TemperatureStateItem != null)
		{
			TemperatureStateItem temperatureStateItem = particleCollectionZone.TemperatureStateItem;
			temperatureStateItem.OnTemperatureChangeWholeUnit = (Action<TemperatureStateItem>)Delegate.Remove(temperatureStateItem.OnTemperatureChangeWholeUnit, new Action<TemperatureStateItem>(TemperatureChangeWholeUnit));
		}
		if (rigidbodyRemover != null)
		{
			RigidbodyRemover obj4 = rigidbodyRemover;
			obj4.OnRigidbodyAdded = (Action<RigidbodyRemover>)Delegate.Remove(obj4.OnRigidbodyAdded, new Action<RigidbodyRemover>(RecacheRigidbody));
		}
	}

	private void RecacheRigidbody(RigidbodyRemover remover)
	{
		rb = rigidbodyRemover.PrimaryRigidbody;
	}

	private void ParticleZoneIsCollecting(ParticleCollectionZone zone, WorldItemData fluid, float amt)
	{
		isParticleCollectionZoneDirty = true;
		GameEventsManager.Instance.ItemActionOccurredWithAmount(zone.CollectionZoneWorldItem.Data, "FILLED_TO_AMOUNT", fluidFullPercent);
	}

	private void ParticlesBeingRemoved(ParticleCollectionZone zone)
	{
		isParticleCollectionZoneDirty = true;
	}

	private void ParticleQuantityUnitAdded(ParticleCollectionZone zone, WorldItemData fluid)
	{
		GameEventsManager.Instance.ItemActionOccurredWithAmount(zone.CollectionZoneWorldItem.Data, "FILLED_TO_AMOUNT", fluidFullPercent);
	}

	private void TemperatureChangeWholeUnit(TemperatureStateItem stateItem)
	{
		fluid.SetParticleEffectsBasedOnTemperature(particleCollectionZone.AvgTemperatureCelsius);
		GameEventsManager.Instance.ItemActionOccurredWithAmount(particleCollectionZone.CollectionZoneWorldItem.Data, "HEATED_TO_DEGREES", stateItem.TemperatureCelsius);
		GameEventsManager.Instance.ItemActionOccurredWithAmount(particleCollectionZone.CollectionZoneWorldItem.Data, "HEATED_TO_DEGREES_MINUS_ROOM_TEMP", stateItem.TemperatureCelsius - 21f);
	}

	private void Update()
	{
		if (isParticleCollectionZoneDirty)
		{
			UpdateStateFromParticleCollectionZone(true);
		}
		if (fluidFullPercent > 0f)
		{
			UpdateFluidMovement();
			if (rb != null && !rb.IsSleeping())
			{
				UpdateLowestPositionOnCup();
			}
			if ((rb != null && !rb.IsSleeping()) || fluidFullPercent >= 1f || fluidOverFillPercent > 0f || isParticleCollectionZoneDirty)
			{
				isParticleCollectionZoneDirty = false;
				if (isPouringEnabled)
				{
					PourLiquid();
				}
			}
			else
			{
				isParticleCollectionZoneDirty = false;
			}
			if (!fluid.gameObject.activeSelf)
			{
				if (OnFluidPartiallyFilled != null)
				{
					OnFluidPartiallyFilled(this);
				}
				fluid.gameObject.SetActive(true);
			}
		}
		else
		{
			if (fluid.gameObject.activeSelf)
			{
				if (OnFluidEmptied != null)
				{
					OnFluidEmptied(this);
				}
				fluidOverflowParticle.SetEmissionMultiplier(0f);
				fluid.gameObject.SetActive(false);
				isCurrentlyPouring = false;
			}
			isParticleCollectionZoneDirty = false;
		}
		if (!ShouldPour() && isCurrentlyPouring && Time.frameCount > lastFrameOfManualPour)
		{
			isCurrentlyPouring = false;
			fluidOverflowParticle.SetEmissionMultiplier(0f);
		}
	}

	private void UpdateLowestPositionOnCup()
	{
		Vector3 zero = Vector3.zero;
		zero = Vector3.ProjectOnPlane(base.transform.InverseTransformDirection(Vector3.down), fluid.SurfaceTransform.up).normalized;
		Quaternion localRotation = Quaternion.FromToRotation(Vector3.forward, zero);
		lowPointOfRimTransform.localRotation = localRotation;
		lowPointOfRimTransform.localPosition = middleOfTopOfContainerTranform.localPosition + zero * containerRadius;
		if (additionaLowPoint != null && additionaLowPoint.position.y < lowPointOfRimTransform.position.y)
		{
			lowPointOfRimTransform.position = additionaLowPoint.position;
			lowPointOfRimTransform.rotation = Quaternion.identity;
		}
	}

	private float VolumeInMLFromRadiusAndHeight(float radiusMeters, float heightMeters)
	{
		return (float)Math.PI * Mathf.Pow(radiusMeters, 2f) * heightMeters * 1000000f;
	}

	public void SetIsPouringEnabled(bool en)
	{
		isPouringEnabled = en;
	}

	public void ManualPourLiquid(float amountInML, Vector3 position)
	{
		if (amountInML < 0f)
		{
			Debug.LogWarning("Can't pour negative amount");
			return;
		}
		float heightMeters = fluidFullPercent * containerHeight;
		float num = VolumeInMLFromRadiusAndHeight(containerRadius, heightMeters);
		float num2 = Mathf.Clamp(num - amountInML, 0f, num);
		if (fluidImpactParticleEffectManagerCached != null || (fluidImpactParticleEffectManagerCached = FluidImpactParticleManager.Instance) != null)
		{
			float num3 = num - num2;
			if (num2 < 0f)
			{
				num3 = num;
			}
			if (num3 > 0f)
			{
				if (!isCurrentlyPouring)
				{
					isCurrentlyPouring = true;
					fluidOverflowParticle.SetEmissionMultiplier(1f, false);
				}
				FluidImpactParticleManager.FluidImpactDetails fluidImpactDetails = fluidImpactParticleEffectManagerCached.FetchPooledFluidImpactDetails();
				if (fluidImpactDetails != null)
				{
					RemoveParticleQuantity(num3, ref fluidImpactDetails);
					ApplyParticleQuanity(fluidImpactDetails, position);
				}
				else
				{
					Debug.LogWarning("FluidImpactDetails was returned as null.");
				}
			}
			else if (isCurrentlyPouring)
			{
				isCurrentlyPouring = false;
				fluidOverflowParticle.SetEmissionMultiplier(0f, false);
			}
		}
		else
		{
			fluidImpactParticleEffectManagerCached = FluidImpactParticleManager.Instance;
		}
		lastFrameOfManualPour = Time.frameCount + 2;
		GameEventsManager.Instance.ItemActionOccurredWithAmount(particleCollectionZone.CollectionZoneWorldItem.Data, "FILLED_TO_AMOUNT", fluidFullPercent);
	}

	private void PourLiquid()
	{
		if (!ShouldPour())
		{
			return;
		}
		if (!isCurrentlyPouring)
		{
			isCurrentlyPouring = true;
			fluidOverflowParticle.SetEmissionMultiplier(1f);
		}
		float heightMeters = fluidFullPercent * containerHeight;
		float num = VolumeInMLFromRadiusAndHeight(containerRadius, heightMeters);
		float num2 = num;
		float viscosity = GetViscosity();
		num2 -= totalContainerVolume * Time.deltaTime / (viscosity / 0.1f) * 1.5f;
		if (fluidOverFillPercent > 0f)
		{
			num2 -= totalContainerVolume * fluidOverFillPercent;
		}
		if (num2 < 5f)
		{
			num2 = 0f;
		}
		if (fluidImpactParticleEffectManagerCached != null || (fluidImpactParticleEffectManagerCached = FluidImpactParticleManager.Instance) != null)
		{
			float num3 = num - num2;
			if (num2 < 0f)
			{
				num3 = num;
			}
			if (!(num3 > 0f))
			{
				return;
			}
			FluidImpactParticleManager.FluidImpactDetails fluidImpactDetails = fluidImpactParticleEffectManagerCached.FetchPooledFluidImpactDetails();
			if (fluidImpactDetails != null)
			{
				RemoveParticleQuantity(num3, ref fluidImpactDetails);
				if (!ApplyParticleQuanity(fluidImpactDetails) && fluidImpactDetails != null)
				{
					fluidImpactParticleEffectManagerCached.ReleasePooledFluidImpactDetails(fluidImpactDetails);
				}
			}
			else
			{
				Debug.LogWarning("FluidImpactDetails was returned as null.");
			}
		}
		else
		{
			fluidImpactParticleEffectManagerCached = FluidImpactParticleManager.Instance;
		}
	}

	private float GetViscosity()
	{
		if (currMajorityFluidData != null)
		{
			return currMajorityFluidData.ViscosityAsFluid;
		}
		return 0.1f;
	}

	private void UpdateFluidMovement()
	{
		Vector3 position = fluid.SurfaceTransform.position;
		if (fluidSurfaceLaggedPos != position || fluidFullPercent >= 1f || fluidOverFillPercent > 0f || isParticleCollectionZoneDirty)
		{
			Vector3 worldSurfaceNormal = position - fluidSurfaceLaggedPos;
			worldSurfaceNormal.y = 3f * GetViscosity();
			worldSurfaceNormal.Normalize();
			fluid.UpdateMesh(worldSurfaceNormal);
		}
	}

	private void FixedUpdate()
	{
		Vector3 position = fluid.SurfaceTransform.position;
		if (position != fluidSurfaceLaggedPos || fluidSurfaceVelocity != Vector3.zero)
		{
			float viscosity = GetViscosity();
			fluidSurfaceVelocity += (position - fluidSurfaceLaggedPos) * Time.fixedDeltaTime / (viscosity / 0.1f) * 200f;
			fluidSurfaceVelocity *= 1f - 2.5f * Time.fixedDeltaTime;
			fluidSurfaceLaggedPos += fluidSurfaceVelocity * Time.fixedDeltaTime;
		}
	}

	private bool ShouldPour()
	{
		return fluid.FluidHitRim || fluidOverFillPercent > 0f;
	}

	private void RemoveParticleQuantity(float amount, ref FluidImpactParticleManager.FluidImpactDetails fluidImpactDetails)
	{
		particleCollectionZone.RemoveParticleQuantity(amount, ref fluidImpactDetails);
	}

	private void EmptyParticleQuantities(ref FluidImpactParticleManager.FluidImpactDetails fluidImpactDetails)
	{
		particleCollectionZone.Clear();
	}

	private bool ApplyParticleQuanity(FluidImpactParticleManager.FluidImpactDetails fluidImpactDetails)
	{
		return ApplyParticleQuanity(fluidImpactDetails, lowPointOfRimTransform.position);
	}

	private bool ApplyParticleQuanity(FluidImpactParticleManager.FluidImpactDetails fluidImpactDetails, Vector3 pourFromPosition)
	{
		if (pourFromPosition == lowPointOfRimTransform.position)
		{
			fluidOverflowParticle.transform.localPosition = Vector3.zero;
		}
		else
		{
			fluidOverflowParticle.transform.position = pourFromPosition;
		}
		fluidParticleRaycastEmitter.DispensingFluidQuantity(pourFromPosition, Vector3.down, particleCollectionZone, rb, ref fluidParticleEmitCollisionInfo);
		float distanceToHit = fluidParticleEmitCollisionInfo.distanceToHit;
		float num = fluidOverflowParticle.SetLifetimeUsingDistance(distanceToHit);
		if (fluidImpactParticleEffectManagerCached != null || (fluidImpactParticleEffectManagerCached = FluidImpactParticleManager.Instance) != null)
		{
			if (distanceToHit < 4f && num < 2.5f && currMajorityFluidData != null)
			{
				fluidImpactDetails.pos = pourFromPosition + Vector3.down * distanceToHit;
				fluidImpactDetails.zone = fluidParticleEmitCollisionInfo.zone;
				fluidImpactDetails.time = num + Time.time;
				fluidImpactParticleEffectManagerCached.AddFluidImpact(fluidImpactDetails);
				return true;
			}
			return false;
		}
		fluidImpactParticleEffectManagerCached = FluidImpactParticleManager.Instance;
		return false;
	}

	private void UpdateStateFromParticleCollectionZone(bool shouldUpdateMass)
	{
		float totalQuantity = particleCollectionZone.GetTotalQuantity();
		fluidFullPercent = totalQuantity / totalContainerVolume;
		if (fluidFullPercent > 1f)
		{
			fluidOverFillPercent = fluidFullPercent - 1f;
			fluidFullPercent = 1f;
		}
		else
		{
			fluidOverFillPercent = 0f;
		}
		if (totalQuantity > 0f)
		{
			if (currMajorityFluidData != particleCollectionZone.MajorityFluidData)
			{
				currMajorityFluidData = particleCollectionZone.MajorityFluidData;
				if (!(currMajorityFluidData != null))
				{
				}
			}
			Color color = particleCollectionZone.CalculateCombinedFluidColor();
			if (color != prevFluidColor)
			{
				fluidOverflowParticle.SetColor(color);
				fluid.SetFluidColor(color);
				prevFluidColor = color;
			}
		}
		if (shouldUpdateMass && rb != null)
		{
			UpdateMass();
		}
	}

	private void UpdateMass()
	{
		if (FluidFullPercent < 0.05f)
		{
			rb.centerOfMass = emptyCenterOfMass;
		}
		else
		{
			rb.centerOfMass = liquidCenterOfMass;
		}
		rb.mass = totalContainerVolume * FluidFullPercent * 0.001f + containerMass;
	}

	private void CalculateCenterOfMass()
	{
		emptyCenterOfMass = rb.centerOfMass;
		emptyCenterOfMass = base.transform.InverseTransformPoint(rb.worldCenterOfMass);
		Collider[] componentsInChildren = GetComponentsInChildren<Collider>();
		Bounds bounds = new Bounds(base.transform.position, Vector3.zero);
		Collider[] array = componentsInChildren;
		foreach (Collider collider in array)
		{
			if (!collider.isTrigger)
			{
				bounds.Encapsulate(collider.bounds);
			}
		}
		float y = base.transform.InverseTransformPoint(bounds.center).y - bounds.extents.y + bounds.size.y * centerOfMassOffsetFromBottom;
		Vector3 vector = new Vector3(rb.centerOfMass.x, y, rb.centerOfMass.z);
		liquidCenterOfMass = vector;
		UpdateMass();
	}
}
