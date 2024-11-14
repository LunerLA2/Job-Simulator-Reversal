using System;
using System.Collections;
using OwlchemyVR;
using UnityEngine;

public class CookingTimerController : MonoBehaviour
{
	private const float MAX_TIME = 60f;

	private const float TIME_PER_CLICK = 1f;

	private const float DEGREES_PER_SECOND = 6f;

	private const float DEGREES_PER_CLICK = 6f;

	[SerializeField]
	private GrabbableItem dialGrabbable;

	[SerializeField]
	private ForceLockJoint dialForceLockJoint;

	[SerializeField]
	private float shakeForce;

	[SerializeField]
	private float shakeInterval;

	[SerializeField]
	private AudioSourceHelper clickSoundSource;

	[SerializeField]
	private AudioSourceHelper ringSoundSource;

	private Rigidbody rb;

	private CookingTimerState state;

	private float prevDialAngle;

	private HapticInfoObject clickHaptic;

	private HapticInfoObject ringHaptic;

	public CookingTimerState State
	{
		get
		{
			return state;
		}
	}

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
		clickHaptic = new HapticInfoObject(800f, 0.2f);
		clickHaptic.DeactiveHaptic();
		ringHaptic = new HapticInfoObject(800f);
		ringHaptic.DeactiveHaptic();
	}

	private void OnEnable()
	{
		GrabbableItem grabbableItem = dialGrabbable;
		grabbableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(grabbableItem.OnGrabbed, new Action<GrabbableItem>(DialGrabbed));
		GrabbableItem grabbableItem2 = dialGrabbable;
		grabbableItem2.OnReleased = (Action<GrabbableItem>)Delegate.Combine(grabbableItem2.OnReleased, new Action<GrabbableItem>(DialReleased));
	}

	private void OnDisable()
	{
		GrabbableItem grabbableItem = dialGrabbable;
		grabbableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(grabbableItem.OnGrabbed, new Action<GrabbableItem>(DialGrabbed));
		GrabbableItem grabbableItem2 = dialGrabbable;
		grabbableItem2.OnReleased = (Action<GrabbableItem>)Delegate.Remove(grabbableItem2.OnReleased, new Action<GrabbableItem>(DialReleased));
	}

	private void Start()
	{
	}

	private float GetDialAngle()
	{
		float num;
		for (num = dialGrabbable.transform.localEulerAngles.z; num <= -360f; num += 360f)
		{
		}
		while (num > 0f)
		{
			num -= 360f;
		}
		return num;
	}

	private void SetDialAngle(float angle)
	{
		dialGrabbable.transform.localEulerAngles = new Vector3(0f, 0f, angle);
		dialForceLockJoint.ResetRotationMemory();
	}

	private float SnapDialToNearestSecond()
	{
		float dialAngle = GetDialAngle();
		dialAngle = Mathf.Round(dialAngle / 6f) * 6f;
		if (dialAngle != prevDialAngle)
		{
			prevDialAngle = dialAngle;
			Click();
		}
		SetDialAngle(dialAngle);
		return dialAngle;
	}

	private void Click()
	{
		clickSoundSource.Play();
		clickHaptic.Restart();
		if (dialGrabbable.IsCurrInHand && !dialGrabbable.CurrInteractableHand.HapticsController.ContainHaptic(clickHaptic))
		{
			dialGrabbable.CurrInteractableHand.HapticsController.AddNewHaptic(clickHaptic);
		}
	}

	private void Update()
	{
		if (state == CookingTimerState.CountingDown)
		{
			float dialAngle = GetDialAngle();
			dialAngle = Mathf.Min(0f, dialAngle + Time.deltaTime * 6f);
			SetDialAngle(dialAngle);
			if (dialAngle == 0f)
			{
				Ring();
			}
		}
		else if (state == CookingTimerState.Stopped && dialGrabbable.IsCurrInHand)
		{
			SnapDialToNearestSecond();
		}
	}

	private void DialGrabbed(GrabbableItem grabbable)
	{
		dialForceLockJoint.lockZRot = false;
		state = CookingTimerState.Stopped;
		if (!grabbable.CurrInteractableHand.HapticsController.ContainHaptic(ringHaptic))
		{
			grabbable.CurrInteractableHand.HapticsController.AddNewHaptic(ringHaptic);
		}
	}

	private void DialReleased(GrabbableItem grabbable)
	{
		dialForceLockJoint.lockZRot = true;
		float num = SnapDialToNearestSecond();
		if (num != 0f)
		{
			state = CookingTimerState.CountingDown;
		}
		else
		{
			state = CookingTimerState.Stopped;
		}
		if (grabbable.CurrInteractableHand.HapticsController.ContainHaptic(ringHaptic))
		{
			grabbable.CurrInteractableHand.HapticsController.RemoveHaptic(ringHaptic);
		}
	}

	private void Ring()
	{
		StartCoroutine(RingAsync());
	}

	public void StopRinging()
	{
		state = CookingTimerState.Stopped;
	}

	private IEnumerator RingAsync()
	{
		state = CookingTimerState.Ringing;
		ringHaptic.Restart();
		ringSoundSource.Play();
		dialGrabbable.enabled = false;
		while (state == CookingTimerState.Ringing)
		{
			yield return new WaitForSeconds(shakeInterval);
			Vector3 force = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f)).normalized * shakeForce;
			rb.AddTorque(force, ForceMode.Impulse);
		}
		dialGrabbable.enabled = true;
		ringSoundSource.Stop();
		ringHaptic.DeactiveHaptic();
	}
}
