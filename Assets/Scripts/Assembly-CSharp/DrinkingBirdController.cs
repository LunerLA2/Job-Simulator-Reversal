using System;
using System.Collections;
using UnityEngine;

public class DrinkingBirdController : MonoBehaviour
{
	public float DrinkCycleDuration = 10f;

	public float WeightMoveDistance;

	[SerializeField]
	private Rigidbody birdRigidbody;

	[SerializeField]
	private Transform weight;

	[SerializeField]
	private AnimationCurve weightCurve;

	private Vector3 originalWeightPosition;

	private float weightTime;

	[SerializeField]
	private Transform headPivot;

	[SerializeField]
	private TriggerListener beakTrigger;

	private bool drinkingInProgress;

	private bool foundFluid;

	private GoTween headSpazTween;

	private GoTweenConfig spazConfig;

	private GoTweenConfig resetConfig;

	[SerializeField]
	private Transform beakFluidRaycast;

	private LayerMask layerMask = LayerMaskHelper.EverythingBut(20);

	private FluidImpactParticleManager.FluidImpactDetails impactDetails;

	private void OnDrawGizmos()
	{
		Vector3 vector = ((!Application.isPlaying) ? weight.position : weight.parent.TransformPoint(originalWeightPosition));
		Vector3 vector2 = vector + weight.forward * WeightMoveDistance;
		Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
		Gizmos.DrawWireSphere(vector, 0.0025f);
		Gizmos.DrawLine(vector, vector2);
		Gizmos.DrawWireSphere(vector2, 0.0025f);
		Gizmos.DrawWireSphere(weight.position, 0.01f);
		Gizmos.DrawRay(beakFluidRaycast.position, beakFluidRaycast.forward);
	}

	private void OnEnable()
	{
		TriggerListener triggerListener = beakTrigger;
		triggerListener.OnEnter = (Action<TriggerEventInfo>)Delegate.Combine(triggerListener.OnEnter, new Action<TriggerEventInfo>(OnBeakEnter));
		TriggerListener triggerListener2 = beakTrigger;
		triggerListener2.OnExit = (Action<TriggerEventInfo>)Delegate.Combine(triggerListener2.OnExit, new Action<TriggerEventInfo>(OnBeakExit));
	}

	private void OnDisable()
	{
		TriggerListener triggerListener = beakTrigger;
		triggerListener.OnEnter = (Action<TriggerEventInfo>)Delegate.Remove(triggerListener.OnEnter, new Action<TriggerEventInfo>(OnBeakEnter));
		TriggerListener triggerListener2 = beakTrigger;
		triggerListener2.OnExit = (Action<TriggerEventInfo>)Delegate.Remove(triggerListener2.OnExit, new Action<TriggerEventInfo>(OnBeakExit));
	}

	private void OnBeakEnter(TriggerEventInfo info)
	{
		if (info.other.gameObject.layer == 20)
		{
			LookForFluid();
			if (foundFluid)
			{
				StartHeadSpaz();
			}
		}
	}

	private void OnBeakExit(TriggerEventInfo info)
	{
		StopHeadSpaz();
	}

	private void Awake()
	{
		birdRigidbody.sleepThreshold = 0f;
		originalWeightPosition = weight.localPosition;
		weightCurve.postWrapMode = WrapMode.Loop;
		impactDetails = new FluidImpactParticleManager.FluidImpactDetails();
		spazConfig = new GoTweenConfig().localEulerAngles(new Vector3(25f, 0f, 0f)).setIterations(-1, GoLoopType.PingPong).setEaseType(GoEaseType.SineInOut);
		resetConfig = new GoTweenConfig().localEulerAngles(new Vector3(45f, 0f, 0f)).setIterations(1, GoLoopType.PingPong).setEaseType(GoEaseType.SineInOut);
	}

	private void Update()
	{
		weightTime += Time.deltaTime / DrinkCycleDuration;
		weight.localPosition = originalWeightPosition + Vector3.forward * weightCurve.Evaluate(weightTime) * WeightMoveDistance;
	}

	private void StartHeadSpaz()
	{
		if (!drinkingInProgress)
		{
			headSpazTween = Go.to(headPivot.transform, 0.1f, spazConfig);
			drinkingInProgress = true;
		}
	}

	private void StopHeadSpaz()
	{
		if (drinkingInProgress)
		{
			headSpazTween.pause();
			headSpazTween = Go.to(headPivot.transform, 0.1f, resetConfig);
			drinkingInProgress = false;
		}
	}

	private void LookForFluid()
	{
		foundFluid = false;
		RaycastHit[] array = Physics.RaycastAll(beakFluidRaycast.position, beakFluidRaycast.forward, 4f, layerMask);
		if (array.Length > 1)
		{
			for (int i = 0; i < array.Length; i++)
			{
				for (int j = 0; j < array.Length - 1; j++)
				{
					if (array[j].distance > array[j + 1].distance)
					{
						RaycastHit raycastHit = array[j + 1];
						array[j + 1] = array[j];
						array[j] = raycastHit;
					}
				}
			}
		}
		foreach (RaycastHit raycastHit in array)
		{
			ParticleImpactZonePointer component = raycastHit.collider.GetComponent<ParticleImpactZonePointer>();
			ParticleCollectionZone particleCollectionZone = ((!component) ? null : component.ParticleImpactZone.ParticleCollectionZone);
			if ((bool)particleCollectionZone && particleCollectionZone.GetTotalQuantity() > 0f)
			{
				Debug.Log("Is removing fluid");
				StartCoroutine(RemoveFluidFromTarget(particleCollectionZone));
				foundFluid = true;
			}
		}
	}

	private IEnumerator RemoveFluidFromTarget(ParticleCollectionZone collectionZone)
	{
		while (drinkingInProgress && collectionZone.GetTotalQuantity() > 0f)
		{
			collectionZone.RemoveParticleQuantity(10f, ref impactDetails);
			yield return new WaitForEndOfFrame();
		}
	}
}
