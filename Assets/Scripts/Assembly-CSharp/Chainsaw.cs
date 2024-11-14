using System;
using OwlchemyVR;
using UnityEngine;

public class Chainsaw : MonoBehaviour
{
	[SerializeField]
	private PositionalGrabbableController pullTabGrabbable;

	[SerializeField]
	private Transform pullTabTransform;

	[SerializeField]
	private Transform pullCordStartTransform;

	[SerializeField]
	private LineRenderer cordLineRenderer;

	[SerializeField]
	private ParticleSystem startSparkParticle;

	[SerializeField]
	private float startPullTimerAtDistance;

	[SerializeField]
	private float pullTimerLength;

	[SerializeField]
	private float pullSuccessAtDistance;

	[SerializeField]
	private Collider bladeColliderNormal;

	[SerializeField]
	private Collider bladeColliderRunning;

	[SerializeField]
	private Transform bladeBase;

	[SerializeField]
	private MeshRenderer bladeChainRenderer;

	[SerializeField]
	private Rigidbody chainsawRigidbody;

	[SerializeField]
	private Rigidbody pullTabRigidbody;

	[SerializeField]
	private ParticleSystem sparkParticleTop;

	[SerializeField]
	private ParticleSystem sparkParticleBottom;

	[SerializeField]
	private Transform bodyJiggleTransform;

	[SerializeField]
	private Transform bladeJiggleTransform;

	private float bodyJiggleAmount;

	private float bladeJiggleAmount;

	private float pullTimer;

	private bool isPulling;

	private bool isRunning;

	private float bladeForce = 2f;

	private float jiggleSpeed = 60f;

	private float bladeScrollSpeed = 23f;

	private PickupableItem pickupableItem;

	private HapticInfoObject hapticsRunning = new HapticInfoObject(1000f);

	private void Awake()
	{
		pickupableItem = GetComponent<PickupableItem>();
	}

	private void OnEnable()
	{
		PickupableItem obj = pickupableItem;
		obj.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(obj.OnGrabbed, new Action<GrabbableItem>(Grabbed));
		PickupableItem obj2 = pickupableItem;
		obj2.OnReleased = (Action<GrabbableItem>)Delegate.Combine(obj2.OnReleased, new Action<GrabbableItem>(Released));
	}

	private void OnDisable()
	{
		PickupableItem obj = pickupableItem;
		obj.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(obj.OnGrabbed, new Action<GrabbableItem>(Grabbed));
		PickupableItem obj2 = pickupableItem;
		obj2.OnReleased = (Action<GrabbableItem>)Delegate.Remove(obj2.OnReleased, new Action<GrabbableItem>(Released));
	}

	private void Grabbed(GrabbableItem grabbedItem)
	{
		if (isRunning)
		{
			StartChainsawRunningHaptics();
		}
	}

	private void Released(GrabbableItem grabbedItem)
	{
		if (isRunning)
		{
			EndChainsawRunningHaptics();
		}
	}

	private void StartChainsawRunningHaptics()
	{
		pickupableItem.CurrInteractableHand.HapticsController.AddNewHaptic(hapticsRunning);
	}

	private void EndChainsawRunningHaptics()
	{
		pickupableItem.CurrInteractableHand.HapticsController.RemoveHaptic(hapticsRunning);
	}

	private void Update()
	{
		cordLineRenderer.SetPosition(0, pullCordStartTransform.position);
		cordLineRenderer.SetPosition(1, pullTabTransform.position);
		if (pullTabGrabbable.IsCurrInHand)
		{
			float num = Mathf.Abs(Vector3.Distance(pullTabTransform.position, pullCordStartTransform.position));
			if (!isRunning)
			{
				if (!isPulling)
				{
					if (num <= startPullTimerAtDistance)
					{
						BeginPulling();
					}
				}
				else
				{
					pullTimer -= Time.deltaTime;
					if (pullTimer > 0f)
					{
						if (num >= pullSuccessAtDistance)
						{
							StartEngine();
						}
					}
					else
					{
						CancelPulling();
					}
				}
			}
		}
		bodyJiggleTransform.localEulerAngles = Vector3.right * Mathf.PerlinNoise(Time.time * jiggleSpeed, 0f) * bodyJiggleAmount;
		bladeJiggleTransform.localPosition = Vector3.up * ((Mathf.PerlinNoise(0f, Time.time * jiggleSpeed) - 0.5f) * bladeJiggleAmount / 100f);
		if (isRunning)
		{
			bladeChainRenderer.material.mainTextureOffset = Vector2.right * (bladeChainRenderer.material.mainTextureOffset.x + Time.deltaTime * bladeScrollSpeed);
		}
	}

	private void EnteredTop(Collider c)
	{
		if (c.attachedRigidbody != null)
		{
			Rigidbody attachedRigidbody = c.attachedRigidbody;
			if (attachedRigidbody != chainsawRigidbody && attachedRigidbody != pullTabRigidbody)
			{
				Vector3 normalized = (bladeBase.position - bladeColliderNormal.transform.position).normalized;
				attachedRigidbody.AddForce(normalized * (0f - bladeForce), ForceMode.Impulse);
			}
		}
		sparkParticleTop.Play();
	}

	private void EnteredBottom(Collider c)
	{
		if (c.attachedRigidbody != null)
		{
			Rigidbody attachedRigidbody = c.attachedRigidbody;
			if (attachedRigidbody != chainsawRigidbody && attachedRigidbody != pullTabRigidbody)
			{
				Vector3 normalized = (bladeBase.position - bladeColliderNormal.transform.position).normalized;
				attachedRigidbody.AddForce(normalized * bladeForce, ForceMode.Impulse);
			}
		}
		sparkParticleBottom.Play();
	}

	private void BeginPulling()
	{
		isPulling = true;
		pullTimer = pullTimerLength;
	}

	private void CancelPulling()
	{
		isPulling = false;
		pullTimer = 0f;
	}

	private void StartEngine()
	{
		isPulling = false;
		isRunning = true;
		StartChainsawRunningHaptics();
		bodyJiggleAmount = 6f;
		bladeJiggleAmount = 1.1f;
		bladeColliderNormal.enabled = false;
		bladeColliderRunning.enabled = true;
		startSparkParticle.Play();
		Debug.Log("started engine");
	}
}
