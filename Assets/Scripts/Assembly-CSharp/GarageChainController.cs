using System;
using System.Collections;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class GarageChainController : MonoBehaviour
{
	[SerializeField]
	private Transform chainOffsetTransform;

	[SerializeField]
	private Vector3 offsetLowPoint;

	[SerializeField]
	private Vector3 offsetHighPoint;

	[SerializeField]
	private GrabbableSlider chainControlSliderForwards;

	[SerializeField]
	private GrabbableItem chainGrabbableForwards;

	[SerializeField]
	private GrabbableSlider chainControlSliderBackwards;

	[SerializeField]
	private GrabbableItem chainGrabbableBackwards;

	private bool forwardChainIsGrabbed;

	[SerializeField]
	public WorldItem myWorldItem;

	[SerializeField]
	public Animation garageAnimation;

	[SerializeField]
	public AnimationClip garageOpenAnimClip;

	[SerializeField]
	public AnimationClip garageCloseAnimClip;

	[SerializeField]
	private SelectedChangeOutlineController[] outlinesForChain;

	[SerializeField]
	private GameObject chainLinkPrefab;

	[SerializeField]
	private int numberOfChainLinks = 100;

	[SerializeField]
	private float distancePerChainLink = 0.05f;

	[SerializeField]
	private float chainParabolaHeight = 3f;

	[SerializeField]
	private float chainParabolaOffsetHeight = 3.42f;

	[SerializeField]
	private float chainParabolaRadius = 0.5f;

	[SerializeField]
	private ProfitCounterController profitCounter;

	[SerializeField]
	private Transform chainHackUp;

	[SerializeField]
	private Transform chainHackDown;

	private Transform[] spawnedLinks;

	private bool garageDoorIsOpen;

	[SerializeField]
	private float speedMultiplier = 1f;

	[SerializeField]
	private float inertiaDecayTime = 1f;

	private bool isSliding;

	private float lastSliderPosition = 0.5f;

	private float storedInertia;

	private float releasedTime;

	[SerializeField]
	private float pullDistanceRequiredForFullOpen = 2f;

	[SerializeField]
	private float garageDoorLagAmount = 10f;

	private float yankDistance;

	private float smoothedGarageScrub;

	private float distanceYankedDownSinceLowered;

	private bool hasBeenFullyYankedDown;

	[SerializeField]
	private AudioClip[] chainLinkClips;

	[SerializeField]
	private AudioClip chaindropsound;

	[SerializeField]
	private AudioClip chainrisesound;

	[SerializeField]
	private AudioClip garageOpenClip;

	[SerializeField]
	private AudioSourceHelper garageAudioSourceHelper;

	[Range(0f, 1f)]
	[SerializeField]
	private float minGaragePitch = 0.9f;

	[Range(1f, 2f)]
	[SerializeField]
	private float maxGaragePitch = 1.1f;

	private bool isLoopingGarageSoundPlaying;

	private float loopingGarageSoundVolume;

	private float secondsToFadeLoopIn = 0.25f;

	private float secondsToFadeLoopOut = 0.25f;

	private bool wasJobCompleteChainLowered;

	[SerializeField]
	private float linkSFXSpeedMultiplier = 1f;

	private void Start()
	{
		spawnedLinks = new Transform[numberOfChainLinks];
		List<MeshFilter> list = new List<MeshFilter>();
		for (int i = 0; i < numberOfChainLinks; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(chainLinkPrefab);
			spawnedLinks[i] = gameObject.transform;
			gameObject.transform.SetParent(chainOffsetTransform, true);
			list.Add(gameObject.GetComponent<MeshFilter>());
		}
		if (chainHackUp != null)
		{
			list.Add(chainHackUp.GetComponent<MeshFilter>());
		}
		if (chainHackDown != null)
		{
			list.Add(chainHackDown.GetComponent<MeshFilter>());
		}
		UpdateLinkVisuals();
		for (int j = 0; j < outlinesForChain.Length; j++)
		{
			outlinesForChain[j].meshFilters = list.ToArray();
			outlinesForChain[j].Build();
		}
		RaiseChain();
	}

	private void OnEnable()
	{
		JobBoardManager instance = JobBoardManager.instance;
		instance.OnBeganWaitingForConfirmation = (Action)Delegate.Combine(instance.OnBeganWaitingForConfirmation, new Action(LowerChain));
		JobBoardManager instance2 = JobBoardManager.instance;
		instance2.OnJobComplete = (Action<JobStatusController>)Delegate.Combine(instance2.OnJobComplete, new Action<JobStatusController>(JobCompleted));
		GrabbableItem grabbableItem = chainGrabbableForwards;
		grabbableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(grabbableItem.OnGrabbed, new Action<GrabbableItem>(ChainFowardControlGrabbed));
		GrabbableItem grabbableItem2 = chainGrabbableForwards;
		grabbableItem2.OnReleasedWasNotSwappedBetweenHands = (Action<GrabbableItem>)Delegate.Combine(grabbableItem2.OnReleasedWasNotSwappedBetweenHands, new Action<GrabbableItem>(ControlChainReleased));
		GrabbableItem grabbableItem3 = chainGrabbableForwards;
		grabbableItem3.OnReleased = (Action<GrabbableItem>)Delegate.Combine(grabbableItem3.OnReleased, new Action<GrabbableItem>(ControlChainReleased));
		GrabbableItem grabbableItem4 = chainGrabbableBackwards;
		grabbableItem4.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(grabbableItem4.OnGrabbed, new Action<GrabbableItem>(ChainBackwardsControlGrabbed));
		GrabbableItem grabbableItem5 = chainGrabbableBackwards;
		grabbableItem5.OnReleasedWasNotSwappedBetweenHands = (Action<GrabbableItem>)Delegate.Combine(grabbableItem5.OnReleasedWasNotSwappedBetweenHands, new Action<GrabbableItem>(ControlChainReleased));
		GrabbableItem grabbableItem6 = chainGrabbableBackwards;
		grabbableItem6.OnReleased = (Action<GrabbableItem>)Delegate.Combine(grabbableItem6.OnReleased, new Action<GrabbableItem>(ControlChainReleased));
		JobBoardManager instance3 = JobBoardManager.instance;
		instance3.OnSandboxPhaseStarted = (Action)Delegate.Combine(instance3.OnSandboxPhaseStarted, new Action(SandboxStart));
	}

	private void OnDisable()
	{
		JobBoardManager instance = JobBoardManager.instance;
		instance.OnBeganWaitingForConfirmation = (Action)Delegate.Remove(instance.OnBeganWaitingForConfirmation, new Action(LowerChain));
		JobBoardManager instance2 = JobBoardManager.instance;
		instance2.OnJobComplete = (Action<JobStatusController>)Delegate.Remove(instance2.OnJobComplete, new Action<JobStatusController>(JobCompleted));
		GrabbableItem grabbableItem = chainGrabbableForwards;
		grabbableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(grabbableItem.OnGrabbed, new Action<GrabbableItem>(ChainFowardControlGrabbed));
		GrabbableItem grabbableItem2 = chainGrabbableForwards;
		grabbableItem2.OnReleasedWasNotSwappedBetweenHands = (Action<GrabbableItem>)Delegate.Remove(grabbableItem2.OnReleasedWasNotSwappedBetweenHands, new Action<GrabbableItem>(ControlChainReleased));
		GrabbableItem grabbableItem3 = chainGrabbableForwards;
		grabbableItem3.OnReleased = (Action<GrabbableItem>)Delegate.Remove(grabbableItem3.OnReleased, new Action<GrabbableItem>(ControlChainReleased));
		GrabbableItem grabbableItem4 = chainGrabbableBackwards;
		grabbableItem4.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(grabbableItem4.OnGrabbed, new Action<GrabbableItem>(ChainBackwardsControlGrabbed));
		GrabbableItem grabbableItem5 = chainGrabbableBackwards;
		grabbableItem5.OnReleasedWasNotSwappedBetweenHands = (Action<GrabbableItem>)Delegate.Remove(grabbableItem5.OnReleasedWasNotSwappedBetweenHands, new Action<GrabbableItem>(ControlChainReleased));
		GrabbableItem grabbableItem6 = chainGrabbableBackwards;
		grabbableItem6.OnReleased = (Action<GrabbableItem>)Delegate.Remove(grabbableItem6.OnReleased, new Action<GrabbableItem>(ControlChainReleased));
		JobBoardManager instance3 = JobBoardManager.instance;
		instance3.OnSandboxPhaseStarted = (Action)Delegate.Remove(instance3.OnSandboxPhaseStarted, new Action(SandboxStart));
	}

	private void SandboxStart()
	{
		if (!wasJobCompleteChainLowered)
		{
			wasJobCompleteChainLowered = true;
			Debug.Log("mechanic job complete, lowering chain for sandbox car");
			StartCoroutine(InternalLowerChainSandbox(1f));
		}
	}

	private void JobCompleted(JobStatusController job)
	{
		if (!wasJobCompleteChainLowered)
		{
			wasJobCompleteChainLowered = true;
			Debug.Log("mechanic job complete, lowering chain for sandbox car");
			StartCoroutine(InternalLowerChainSandbox(34f));
		}
	}

	private IEnumerator InternalLowerChainSandbox(float t)
	{
		yield return new WaitForSeconds(t);
		LowerChain();
	}

	private void ChainFowardControlGrabbed(GrabbableItem item)
	{
		if (chainGrabbableBackwards.IsCurrInHand && chainGrabbableBackwards.CurrInteractableHand != null)
		{
			chainGrabbableBackwards.CurrInteractableHand.ManuallyReleaseJoint();
		}
		isSliding = true;
		forwardChainIsGrabbed = true;
		chainControlSliderForwards.transform.localPosition = Vector3.zero;
		lastSliderPosition = 0.5f;
	}

	private void ChainBackwardsControlGrabbed(GrabbableItem item)
	{
		if (chainGrabbableForwards.IsCurrInHand && chainGrabbableForwards.CurrInteractableHand != null)
		{
			chainGrabbableForwards.CurrInteractableHand.ManuallyReleaseJoint();
		}
		isSliding = true;
		forwardChainIsGrabbed = false;
		chainControlSliderBackwards.transform.localPosition = Vector3.zero;
		lastSliderPosition = 0.5f;
	}

	private void ControlChainReleased(GrabbableItem item)
	{
		isSliding = false;
		releasedTime = Time.realtimeSinceStartup;
		GetGrabbedControlSlider().transform.localPosition = Vector3.zero;
		lastSliderPosition = 0.5f;
	}

	private void ControlChainSwappedHands(GrabbableItem item)
	{
		isSliding = true;
		GetGrabbedControlSlider().transform.localPosition = Vector3.zero;
		lastSliderPosition = 0.5f;
	}

	private GrabbableSlider GetGrabbedControlSlider()
	{
		return (!forwardChainIsGrabbed) ? chainControlSliderBackwards : chainControlSliderForwards;
	}

	private void LateUpdate()
	{
		if (isSliding)
		{
			ModifyYankDistance(storedInertia = (GetGrabbedControlSlider().NormalizedOffset - lastSliderPosition) * speedMultiplier);
			lastSliderPosition = GetGrabbedControlSlider().NormalizedOffset;
		}
		else if (Mathf.Abs(storedInertia) > 1E-05f)
		{
			storedInertia = Mathf.Lerp(storedInertia, 0f, (Time.realtimeSinceStartup - releasedTime) / inertiaDecayTime);
			ModifyYankDistance(storedInertia);
		}
		float num = distanceYankedDownSinceLowered / pullDistanceRequiredForFullOpen;
		float num2 = Mathf.Abs(num - smoothedGarageScrub);
		if (num2 > 0.0001f)
		{
			float t = 1f;
			if (garageDoorLagAmount > 0f)
			{
				t = 1f / garageDoorLagAmount;
			}
			smoothedGarageScrub = Mathf.Lerp(smoothedGarageScrub, num, t);
			float time = smoothedGarageScrub * garageOpenAnimClip.length;
			garageAnimation[garageOpenAnimClip.name].enabled = true;
			garageAnimation[garageOpenAnimClip.name].weight = 1f;
			garageAnimation[garageOpenAnimClip.name].time = time;
			garageAnimation.Sample();
			garageAnimation[garageOpenAnimClip.name].enabled = false;
		}
		if ((double)num2 > 0.005)
		{
			if (!isLoopingGarageSoundPlaying)
			{
				isLoopingGarageSoundPlaying = true;
				loopingGarageSoundVolume = 0f;
				garageAudioSourceHelper.SetVolume(0f);
				garageAudioSourceHelper.SetLooping(true);
				garageAudioSourceHelper.SetClip(garageOpenClip);
				garageAudioSourceHelper.Play();
				return;
			}
			if (loopingGarageSoundVolume < 1f)
			{
				if (secondsToFadeLoopIn > 0f)
				{
					loopingGarageSoundVolume = Mathf.Clamp(loopingGarageSoundVolume + Time.deltaTime / secondsToFadeLoopIn, 0f, 1f);
				}
				else
				{
					loopingGarageSoundVolume = 1f;
				}
				garageAudioSourceHelper.SetVolume(loopingGarageSoundVolume);
			}
			garageAudioSourceHelper.SetPitch(Mathf.Lerp(minGaragePitch, maxGaragePitch, 0.1f - smoothedGarageScrub));
		}
		else if (loopingGarageSoundVolume > 0f)
		{
			if (secondsToFadeLoopOut > 0f)
			{
				loopingGarageSoundVolume = Mathf.Clamp(loopingGarageSoundVolume - Time.deltaTime / secondsToFadeLoopOut, 0f, 1f);
			}
			else
			{
				loopingGarageSoundVolume = 0f;
			}
			garageAudioSourceHelper.SetVolume(loopingGarageSoundVolume);
			garageAudioSourceHelper.SetPitch(Mathf.Lerp(minGaragePitch, maxGaragePitch, 0.1f - smoothedGarageScrub));
		}
		else
		{
			garageAudioSourceHelper.Stop();
			isLoopingGarageSoundPlaying = false;
		}
	}

	private void ModifyYankDistance(float amt)
	{
		if (!forwardChainIsGrabbed)
		{
			amt *= -1f;
		}
		yankDistance += amt;
		distanceYankedDownSinceLowered += amt;
		distanceYankedDownSinceLowered = Mathf.Clamp(distanceYankedDownSinceLowered, 0f, pullDistanceRequiredForFullOpen);
		float num = distanceYankedDownSinceLowered / pullDistanceRequiredForFullOpen;
		int num2 = Mathf.Clamp(Mathf.FloorToInt(Mathf.Abs(yankDistance * linkSFXSpeedMultiplier) / distancePerChainLink), 0, 3);
		float num3 = ((num2 <= 0) ? 0f : (Time.deltaTime / (float)num2));
		for (int i = 0; i < num2; i++)
		{
			StartCoroutine(PlayChainAudio(num3 * (float)i));
		}
		while (yankDistance >= distancePerChainLink)
		{
			yankDistance -= distancePerChainLink;
		}
		while (yankDistance <= 0f - distancePerChainLink)
		{
			yankDistance += distancePerChainLink;
		}
		UpdateLinkVisuals();
		if (num >= 0.99f)
		{
			if (!hasBeenFullyYankedDown)
			{
				hasBeenFullyYankedDown = true;
				garageDoorIsOpen = true;
				profitCounter.paused = false;
				GameEventsManager.Instance.ItemActionOccurredWithAmount(myWorldItem.Data, "USED_PARTIALLY", 1f);
				GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "USED");
				RaiseChain();
				if (GenieManager.AreAnyJobGenieModesActive() && GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode))
				{
					AutoMechanicManager.Instance.ChainWasPulledInEndlessMode();
				}
				else if (wasJobCompleteChainLowered)
				{
					AutoMechanicManager.Instance.ChainWasPulledInSandboxMode();
				}
			}
		}
		else
		{
			GameEventsManager.Instance.ItemActionOccurredWithAmount(myWorldItem.Data, "USED_PARTIALLY", num);
		}
	}

	private IEnumerator PlayChainAudio(float delay)
	{
		if (delay > 0f)
		{
			yield return new WaitForSeconds(delay);
		}
		AudioManager.Instance.Play(base.transform.position, chainLinkClips[UnityEngine.Random.Range(0, chainLinkClips.Length)], 1f, 1f);
	}

	private void UpdateLinkVisuals()
	{
		for (int i = 0; i < numberOfChainLinks; i++)
		{
			spawnedLinks[i].localPosition = GetPointOnChainParabola(yankDistance + (float)i * distancePerChainLink) - chainOffsetTransform.position;
			spawnedLinks[i].eulerAngles = GetRotationOnChainParabola(yankDistance + (float)i * distancePerChainLink);
		}
		if (chainHackDown != null)
		{
			chainHackDown.localPosition = new Vector3(0f, -3.2687f + yankDistance, chainHackDown.localPosition.z);
		}
		if (chainHackUp != null)
		{
			chainHackUp.localPosition = new Vector3(0f, -3.323f - yankDistance, chainHackUp.localPosition.z);
		}
	}

	private void RaiseChain()
	{
		if (chainControlSliderForwards.Grabbable.IsCurrInHand)
		{
			chainControlSliderForwards.Grabbable.CurrInteractableHand.ManuallyReleaseJoint();
		}
		if (chainControlSliderBackwards.Grabbable.IsCurrInHand)
		{
			chainControlSliderBackwards.Grabbable.CurrInteractableHand.ManuallyReleaseJoint();
		}
		chainControlSliderForwards.Grabbable.enabled = false;
		chainControlSliderBackwards.Grabbable.enabled = false;
		AudioManager.Instance.Play(base.transform.position, chainrisesound, 1f, 1f);
		Go.to(chainOffsetTransform, 2f, new GoTweenConfig().localPosition(offsetHighPoint).setEaseType(GoEaseType.QuadInOut));
	}

	private void LowerChain()
	{
		distanceYankedDownSinceLowered = 0f;
		chainControlSliderForwards.Grabbable.enabled = true;
		chainControlSliderBackwards.Grabbable.enabled = true;
		AudioManager.Instance.Play(base.transform.position, chainrisesound, 1f, 1f);
		smoothedGarageScrub = 0f;
		hasBeenFullyYankedDown = false;
		if (garageDoorIsOpen)
		{
			garageDoorIsOpen = false;
			garageAnimation.Play(garageCloseAnimClip.name);
			profitCounter.Reset();
			StartCoroutine(DoorCloseAudio());
		}
		Go.to(chainOffsetTransform, 2f, new GoTweenConfig().localPosition(offsetLowPoint).setEaseType(GoEaseType.QuadInOut));
	}

	private IEnumerator DoorCloseAudio()
	{
		garageAudioSourceHelper.SetPitch(maxGaragePitch);
		garageAudioSourceHelper.SetVolume(1f);
		garageAudioSourceHelper.Play();
		while (garageAnimation.isPlaying)
		{
			yield return null;
		}
		garageAudioSourceHelper.Stop();
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.grey;
		for (int i = 0; i < numberOfChainLinks; i++)
		{
			float worldDistanceTravelled = (float)i * distancePerChainLink;
			Gizmos.DrawLine(GetPointOnChainParabola(worldDistanceTravelled) + Vector3.left * 0.05f, GetPointOnChainParabola(worldDistanceTravelled) + Vector3.right * 0.05f);
		}
	}

	private Vector3 GetPointOnChainParabola(float worldDistanceTravelled)
	{
		Vector3 position = chainOffsetTransform.position;
		float num = (float)Math.PI * chainParabolaRadius;
		if (worldDistanceTravelled <= chainParabolaHeight)
		{
			return position + Vector3.back * chainParabolaRadius + Vector3.down * (worldDistanceTravelled - chainParabolaOffsetHeight);
		}
		if (worldDistanceTravelled <= chainParabolaHeight + num)
		{
			float num2 = (worldDistanceTravelled - chainParabolaHeight) / num;
			float f = (num2 * 180f - 180f) * ((float)Math.PI / 180f);
			Vector3 vector = position + Vector3.down * chainParabolaHeight;
			float z = vector.z + chainParabolaRadius * Mathf.Cos(f);
			float num3 = vector.y + chainParabolaRadius * Mathf.Sin(f);
			return new Vector3(position.x, num3 + chainParabolaOffsetHeight, z);
		}
		float num4 = worldDistanceTravelled - chainParabolaHeight - num;
		return position - Vector3.back * chainParabolaRadius + Vector3.down * (chainParabolaHeight - num4 - chainParabolaOffsetHeight);
	}

	private Vector3 GetRotationOnChainParabola(float worldDistanceTravelled)
	{
		float num = (float)Math.PI * chainParabolaRadius;
		if (worldDistanceTravelled <= chainParabolaHeight)
		{
			return Vector3.zero;
		}
		if (worldDistanceTravelled <= chainParabolaHeight + num)
		{
			float num2 = (worldDistanceTravelled - chainParabolaHeight) / num;
			return Vector3.left * (num2 * 180f);
		}
		return Vector3.right * 180f;
	}
}
