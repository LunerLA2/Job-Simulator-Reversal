using System;
using System.Collections;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class JobBotToy : MonoBehaviour
{
	[SerializeField]
	private List<AudioClip> talkClips;

	[SerializeField]
	private List<AudioClip> talkClipsNight;

	private ElementSequence<AudioClip> talkClipSequence;

	private ElementSequence<AudioClip> talkClipSequenceNight;

	[SerializeField]
	private Transform pullTabTransform;

	[SerializeField]
	private SpringJoint pullTabJoint;

	[SerializeField]
	private Collider pullTabCollider;

	[SerializeField]
	private GrabbableItem pullTabGrabbable;

	[SerializeField]
	private Transform pullTabBarrier;

	[SerializeField]
	private LineRenderer stringRenderer;

	[SerializeField]
	private ParticleSystem sparks;

	[SerializeField]
	private float activationPullThreshold = 0.15f;

	[SerializeField]
	private float activationReleaseThreshold = 0.1f;

	[SerializeField]
	private float maxNonGrabDistance = 0.1f;

	[SerializeField]
	private float maxGrabDistance = 0.65f;

	[SerializeField]
	private AudioSourceHelper talkAudio;

	[SerializeField]
	private AudioSource talkDummyAudio;

	private Vector3 initialPullTabLocalPos;

	private int cachedPullTabLayer;

	private float pullDistance;

	private bool pulledPastThreshold;

	private bool isTalking;

	[SerializeField]
	private HapticTransformInfo hapticsForMovement;

	private HapticInfoObject loopingHaptics;

	private void Awake()
	{
		talkClipSequence = new ElementSequence<AudioClip>(talkClips.ToArray());
		talkClipSequenceNight = new ElementSequence<AudioClip>(talkClipsNight.ToArray());
		initialPullTabLocalPos = pullTabTransform.localPosition;
		cachedPullTabLayer = pullTabCollider.gameObject.layer;
		hapticsForMovement.ManualAwake();
		loopingHaptics = new HapticInfoObject(0f);
	}

	private void OnEnable()
	{
		GrabbableItem grabbableItem = pullTabGrabbable;
		grabbableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(grabbableItem.OnGrabbed, new Action<GrabbableItem>(PullTabGrabbed));
		GrabbableItem grabbableItem2 = pullTabGrabbable;
		grabbableItem2.OnReleased = (Action<GrabbableItem>)Delegate.Combine(grabbableItem2.OnReleased, new Action<GrabbableItem>(PullTabReleased));
	}

	private void OnDisable()
	{
		GrabbableItem grabbableItem = pullTabGrabbable;
		grabbableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(grabbableItem.OnGrabbed, new Action<GrabbableItem>(PullTabGrabbed));
		GrabbableItem grabbableItem2 = pullTabGrabbable;
		grabbableItem2.OnReleased = (Action<GrabbableItem>)Delegate.Remove(grabbableItem2.OnReleased, new Action<GrabbableItem>(PullTabReleased));
	}

	private void Update()
	{
		hapticsForMovement.ManualUpdate();
		UpdateStringLength();
		UpdateHaptics();
		UpdatePullTab();
		UpdateStringRenderer();
		UpdateActivation();
	}

	private void UpdateStringLength()
	{
		pullDistance = (pullTabTransform.localPosition - initialPullTabLocalPos).magnitude;
	}

	private void PullTabGrabbed(GrabbableItem item)
	{
		item.CurrInteractableHand.HapticsController.AddNewHaptic(loopingHaptics);
	}

	private void PullTabReleased(GrabbableItem item)
	{
		item.CurrInteractableHand.HapticsController.RemoveHaptic(loopingHaptics);
	}

	private void UpdateHaptics()
	{
		if (pullTabGrabbable.IsCurrInHand)
		{
			UpdateHapticValueBasedOnAmount();
		}
	}

	private void UpdateHapticValueBasedOnAmount()
	{
		float num = Mathf.Clamp(Vector3.Distance(pullTabGrabbable.transform.position, base.transform.position) / 1f, 0f, 1f) * 600f;
		if (num < 150f)
		{
			num = 0f;
		}
		loopingHaptics.SetCurrPulseRateMicroSec(num);
	}

	private void UpdatePullTab()
	{
		bool flag = true;
		if (pullTabGrabbable.IsCurrInHand)
		{
			flag = false;
			if (pullDistance > maxGrabDistance)
			{
				pullTabGrabbable.CurrInteractableHand.TryRelease();
			}
		}
		else if (pullTabBarrier.InverseTransformPoint(pullTabTransform.position).z <= 0f)
		{
			flag = false;
		}
		else if (pullDistance > maxNonGrabDistance)
		{
			flag = false;
		}
		int num = ((!flag) ? 17 : cachedPullTabLayer);
		if (pullTabCollider.gameObject.layer != num)
		{
			pullTabCollider.gameObject.layer = num;
		}
	}

	private void UpdateStringRenderer()
	{
		stringRenderer.SetPosition(0, stringRenderer.transform.position);
		stringRenderer.SetPosition(1, pullTabTransform.TransformPoint(pullTabJoint.anchor));
	}

	private void UpdateActivation()
	{
		if (pulledPastThreshold)
		{
			if (!(pullDistance < activationPullThreshold))
			{
				return;
			}
			if (pullTabGrabbable.IsCurrInHand)
			{
				pulledPastThreshold = false;
			}
			else if (pullDistance < activationReleaseThreshold)
			{
				pulledPastThreshold = false;
				if (!isTalking)
				{
					StartCoroutine(TalkAsync());
				}
			}
		}
		else if (pullDistance >= activationPullThreshold)
		{
			pulledPastThreshold = true;
		}
	}

	private IEnumerator TalkAsync()
	{
		isTalking = true;
		sparks.Play();
		AudioClip talkClip = talkClipSequence.GetNext();
		if (JobBoardManager.instance != null && JobBoardManager.instance.EndlessModeStatusController != null)
		{
			talkClip = talkClipSequenceNight.GetNext();
		}
		talkAudio.SetClip(talkClip);
		talkAudio.Play();
		talkDummyAudio.clip = talkClip;
		talkDummyAudio.Play();
		float talkTime = 0f;
		while (talkTime < talkClip.length)
		{
			talkTime += Time.deltaTime;
			yield return null;
		}
		talkAudio.Stop();
		talkDummyAudio.Stop();
		isTalking = false;
	}
}
