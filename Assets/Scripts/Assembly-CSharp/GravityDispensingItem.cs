using System;
using OwlchemyVR;
using UnityEngine;

[RequireComponent(typeof(FluidParticleRaycastEmitter))]
public class GravityDispensingItem : MonoBehaviour
{
	private const int HAPTICS_SHACK_PULSE_RATE_MICRO_SEC = 650;

	private const float HAPTICS_SHACK_LENGTH_SECONDS = 0.02f;

	[SerializeField]
	private ParticleSystem particleEffect;

	[SerializeField]
	private float maxDownwardAngle;

	[SerializeField]
	private Transform tipLocation;

	[SerializeField]
	private PickupableItem item;

	[SerializeField]
	private WorldItemData fluidToDispense;

	[SerializeField]
	private bool setParticlesToFluidColor;

	[SerializeField]
	private float fluidTemperatureCelsius = 21f;

	[SerializeField]
	private float dispenseQuantityMLPerSecond = 100f;

	private bool isDispensing;

	[SerializeField]
	private bool forceToBeInHand = true;

	[SerializeField]
	private bool doesRequireShake;

	private Vector3 prevPos;

	private float timeSinceLastRelease;

	[SerializeField]
	private float shakeTime = 0.3f;

	[SerializeField]
	private float requiredDownwardVelocity = 10f;

	private bool wasPrevDirectionNegative;

	[SerializeField]
	protected AudioSourceHelper pouringAudioSrc;

	public AudioClip dispenseSound;

	private float origDispenseQuantityMLPerSecond;

	private float origEmissionRate;

	private HapticInfoObject shakeDispenseHaptic;

	private FluidParticleRaycastEmitter fluidParticleRaycastEmitter;

	private float particleFallingAcceleration;

	private FluidImpactParticleManager fluidImpactParticleEffectManagerCached;

	private FluidParticleEmitCollisionInfo fluidParticleEmitCollisionInfo = new FluidParticleEmitCollisionInfo();

	private bool canPour = true;

	public float DispenseQuantityMLPerSecond
	{
		get
		{
			return dispenseQuantityMLPerSecond;
		}
		set
		{
			dispenseQuantityMLPerSecond = value;
		}
	}

	public bool IsDispensing
	{
		get
		{
			return isDispensing;
		}
	}

	public bool CanPour
	{
		get
		{
			return canPour;
		}
	}

	private void Awake()
	{
		GravityDispensingItemAwake();
	}

	private void OnEnable()
	{
		GravityDispensingItemOnEnable();
	}

	private void OnDisable()
	{
		GravityDispensingItemOnDisable();
	}

	private void ItemReleased(GrabbableItem grabbedItem)
	{
		if (shakeDispenseHaptic.IsRunning && item.CurrInteractableHand != null)
		{
			item.CurrInteractableHand.HapticsController.RemoveHaptic(shakeDispenseHaptic);
		}
	}

	private void Start()
	{
		prevPos = base.transform.position;
		pouringAudioSrc.SetClip(dispenseSound);
		pouringAudioSrc.SetLooping(true);
	}

	private void Update()
	{
		if (canPour)
		{
			UpdateLogic();
		}
	}

	private void UpdateLogic()
	{
		bool flag = false;
		if (!forceToBeInHand)
		{
			flag = true;
		}
		else if (item != null && item.IsCurrInHand)
		{
			flag = true;
		}
		if (flag)
		{
			float num = Vector3.Angle(base.transform.up, Vector3.down);
			if (doesRequireShake)
			{
				Vector3 direction = (base.transform.position - prevPos) / Time.deltaTime;
				prevPos = base.transform.position;
				float y = base.transform.InverseTransformDirection(direction).y;
				if (y < 0f)
				{
					wasPrevDirectionNegative = true;
				}
				if (isDispensing)
				{
					timeSinceLastRelease += Time.deltaTime;
					DoDispensing();
					if (timeSinceLastRelease > shakeTime)
					{
						StopDispensing();
					}
				}
				else
				{
					if (!wasPrevDirectionNegative || !(y > requiredDownwardVelocity) || !(num < maxDownwardAngle))
					{
						return;
					}
					timeSinceLastRelease = 0f;
					DoDispensing();
					wasPrevDirectionNegative = false;
					PlayShakeAudio(y);
					if (item != null)
					{
						if (item.CurrInteractableHand.HapticsController.ContainHaptic(shakeDispenseHaptic))
						{
							shakeDispenseHaptic.Restart();
							return;
						}
						shakeDispenseHaptic.Restart();
						item.CurrInteractableHand.HapticsController.AddNewHaptic(shakeDispenseHaptic);
					}
				}
			}
			else if (num < maxDownwardAngle)
			{
				DoDispensing();
			}
			else
			{
				StopDispensing();
			}
		}
		else if (isDispensing)
		{
			StopDispensing();
		}
	}

	public void SetFluidToDispense(WorldItemData fluid)
	{
		fluidToDispense = fluid;
		if (setParticlesToFluidColor)
		{
			particleEffect.startColor = fluidToDispense.OverallColor;
		}
	}

	protected virtual void DoDispensing()
	{
		DoDispensingLogic();
		if (!pouringAudioSrc.IsPlaying)
		{
			PlayAudio();
		}
	}

	protected virtual void StopDispensing()
	{
		StopDispensingLogic();
		if (pouringAudioSrc.IsPlaying)
		{
			StopAudio();
		}
	}

	protected void DoDispensingLogic()
	{
		float num = Time.deltaTime * dispenseQuantityMLPerSecond;
		ParticleSystem.EmissionModule emission = particleEffect.emission;
		emission.enabled = true;
		particleEffect.emissionRate = dispenseQuantityMLPerSecond / origDispenseQuantityMLPerSecond * origEmissionRate;
		isDispensing = true;
		fluidParticleRaycastEmitter.DispensingFluidQuantity(tipLocation.position, Vector3.down, null, null, ref fluidParticleEmitCollisionInfo);
		float distanceToHit = fluidParticleEmitCollisionInfo.distanceToHit;
		float timeToParticleImpact = FluidParticleRaycastEmitter.GetTimeToParticleImpact(distanceToHit + 0.03f, particleFallingAcceleration, particleEffect.startSpeed * particleEffect.transform.forward.y);
		particleEffect.startLifetime = timeToParticleImpact;
		if ((fluidImpactParticleEffectManagerCached != null || (fluidImpactParticleEffectManagerCached = FluidImpactParticleManager.Instance) != null) && distanceToHit < 4f && timeToParticleImpact < 2.5f)
		{
			FluidImpactParticleManager.FluidImpactDetails fluidImpactDetails = fluidImpactParticleEffectManagerCached.FetchPooledFluidImpactDetails();
			if (fluidImpactDetails != null)
			{
				fluidImpactDetails.pos = tipLocation.position + Vector3.down * distanceToHit;
				fluidImpactDetails.zone = fluidParticleEmitCollisionInfo.zone;
				fluidImpactDetails.time = timeToParticleImpact + Time.time;
				fluidImpactDetails.numOfFluids = 1;
				fluidImpactDetails.fluidQuantities[0].SetFluidDataAndQuatity(fluidToDispense, num);
				fluidImpactDetails.avgColor = fluidToDispense.OverallColor;
				fluidImpactDetails.temperatureCelsius = fluidTemperatureCelsius;
				fluidImpactDetails.totalAmountOfFluid = num;
				fluidImpactParticleEffectManagerCached.AddFluidImpact(fluidImpactDetails);
			}
			else
			{
				Debug.LogWarning("Could not fetch new fluid impact details, impact will not occur");
			}
		}
	}

	protected void StopDispensingLogic()
	{
		ParticleSystem.EmissionModule emission = particleEffect.emission;
		emission.enabled = false;
		isDispensing = false;
	}

	protected void GravityDispensingItemAwake()
	{
		StopAudio();
		origDispenseQuantityMLPerSecond = dispenseQuantityMLPerSecond;
		origEmissionRate = particleEffect.emissionRate;
		if (GenieManager.IsNoGravityEnabled())
		{
			particleEffect.gravityModifier = particleEffect.gravityModifier / 1E-05f * 1f;
		}
		fluidParticleRaycastEmitter = GetComponent<FluidParticleRaycastEmitter>();
		particleFallingAcceleration = Physics.gravity.y * particleEffect.gravityModifier;
		float length = 0.02f;
		shakeDispenseHaptic = new HapticInfoObject(650f, length);
		shakeDispenseHaptic.DeactiveHaptic();
		if (setParticlesToFluidColor)
		{
			particleEffect.startColor = fluidToDispense.OverallColor;
		}
	}

	protected void GravityDispensingItemOnEnable()
	{
		if (item != null)
		{
			PickupableItem pickupableItem = item;
			pickupableItem.OnReleased = (Action<GrabbableItem>)Delegate.Combine(pickupableItem.OnReleased, new Action<GrabbableItem>(ItemReleased));
		}
	}

	protected void GravityDispensingItemOnDisable()
	{
		if (item != null)
		{
			PickupableItem pickupableItem = item;
			pickupableItem.OnReleased = (Action<GrabbableItem>)Delegate.Remove(pickupableItem.OnReleased, new Action<GrabbableItem>(ItemReleased));
		}
		StopDispensing();
	}

	private void PlayShakeAudio(float vel)
	{
		if (!pouringAudioSrc.IsPlaying)
		{
			PlayAudio();
		}
		pouringAudioSrc.SetVolume(Mathf.Clamp(vel * 1.5f, 0.1f, 1f));
	}

	protected void PlayAudio()
	{
		pouringAudioSrc.enabled = true;
		pouringAudioSrc.Play();
	}

	protected void StopAudio()
	{
		pouringAudioSrc.Stop();
		pouringAudioSrc.enabled = false;
	}

	public void SetCanPour(bool pour)
	{
		canPour = pour;
	}
}
