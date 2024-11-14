using System;
using OwlchemyVR;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GrabbableSlider : MonoBehaviour
{
	public enum Axis
	{
		X = 0,
		Y = 1,
		Z = 2
	}

	public enum LockType
	{
		None = 0,
		Permanent = 1,
		UntilHandLeavesRegion = 2,
		Timeout = 3
	}

	private enum InitialLock
	{
		None = 0,
		Upper = 1,
		Lower = 2
	}

	public delegate void UpperLockedHandler(GrabbableSlider slider, bool isInitial);

	public delegate void LowerLockedHandler(GrabbableSlider slider, bool isInitial);

	public delegate void UpperUnlockedHandler(GrabbableSlider slider);

	public delegate void LowerUnlockedHandler(GrabbableSlider slider);

	private const float OUT_OF_HAND_TOLERANCE = 0f;

	private const float IN_HAND_TOLERANCE = 0.0075f;

	[SerializeField]
	private Axis axis;

	[SerializeField]
	private LockType upperLockType = LockType.Permanent;

	[SerializeField]
	private float upperLimit = 0.5f;

	[SerializeField]
	private float upperLockTimeout;

	[SerializeField]
	private float upperUnlockForce;

	[SerializeField]
	private LockType lowerLockType = LockType.Permanent;

	[SerializeField]
	private float lowerLimit = -0.5f;

	[SerializeField]
	private float lowerLockTimeout;

	[SerializeField]
	private float lowerUnlockForce;

	[SerializeField]
	private InitialLock initialLock;

	[SerializeField]
	private PlayerPartDetector handRegion;

	[SerializeField]
	private bool useKinematicLock;

	[SerializeField]
	private bool breakFromHandWhenLocked = true;

	[SerializeField]
	private HapticTransformInfo hapticInfo;

	[SerializeField]
	private AnimationCurve naturalDriveCurve = new AnimationCurve(new Keyframe(0f, 0f, 1f, 1f), new Keyframe(1f, 1f, 1f, 1f));

	[SerializeField]
	private float naturalDriveForceMultiplier = 1f;

	[SerializeField]
	private bool recreateJointOnLockAndUnlock;

	[SerializeField]
	private AudioSourceHelper slideAudioSource;

	[SerializeField]
	private AudioSourceHelper lockUnlockAudioSource;

	[SerializeField]
	private AudioClip upperLockSound;

	[SerializeField]
	private AudioClip upperUnlockSound;

	[SerializeField]
	private AudioClip lowerLockSound;

	[SerializeField]
	private AudioClip lowerUnlockSound;

	[SerializeField]
	private AudioClip slideStartSound;

	[SerializeField]
	private AudioClip slideLoopSound;

	[SerializeField]
	private AudioClip slideStopSound;

	[SerializeField]
	private float minSlidePitch = 0.8f;

	[SerializeField]
	private float maxSlidePitch = 1f;

	[SerializeField]
	private float minSlideVol = 0.4f;

	[SerializeField]
	private float maxSlideVol = 1f;

	private Vector3 initialLocalPos;

	private Quaternion initialLocalRot;

	private Vector3 initialLocalAxis;

	private Rigidbody rb;

	private GrabbableItem grabbable;

	private float offset;

	private bool upperLocked;

	private bool lowerLocked;

	private float lockTime;

	private ConfigurableJoint joint;

	private ForceLockJoint forceLockJoint;

	private ConfigurableJointSaveState jointSaveState;

	private float lastOffset;

	private bool isLoopingSoundPlaying;

	private bool isStartSoundPlaying;

	private float slideSoundTimeRemaining;

	private float slideSoundDecayTime = 0.175f;

	private float slideStartTime;

	[SerializeField]
	private float delayBetweenStartAndSlide = 0.1f;

	public float Offset
	{
		get
		{
			return offset;
		}
	}

	public float Range
	{
		get
		{
			return upperLimit - lowerLimit;
		}
	}

	public float NormalizedOffset
	{
		get
		{
			return (offset - lowerLimit) / Range;
		}
	}

	public Axis SliderAxis
	{
		get
		{
			return axis;
		}
	}

	public float UpperLimit
	{
		get
		{
			return upperLimit;
		}
	}

	public float LowerLimit
	{
		get
		{
			return lowerLimit;
		}
	}

	public bool IsUpperLocked
	{
		get
		{
			return upperLocked;
		}
	}

	public bool IsLowerLocked
	{
		get
		{
			return lowerLocked;
		}
	}

	public LockType UpperLockType
	{
		get
		{
			return upperLockType;
		}
	}

	public LockType LowerLockType
	{
		get
		{
			return lowerLockType;
		}
	}

	public GrabbableItem Grabbable
	{
		get
		{
			return grabbable;
		}
	}

	public event UpperLockedHandler OnUpperLocked;

	public event LowerLockedHandler OnLowerLocked;

	public event UpperUnlockedHandler OnUpperUnlocked;

	public event LowerUnlockedHandler OnLowerUnlocked;

	private void Awake()
	{
		joint = GetComponent<ConfigurableJoint>();
		if (joint != null)
		{
			jointSaveState = ConfigurableJointSaveState.CreateFromJoint(joint);
		}
		else
		{
			jointSaveState = new ConfigurableJointSaveState
			{
				axis = Vector3.right,
				secondaryAxis = Vector3.up,
				xMotion = ((axis == Axis.X) ? ConfigurableJointMotion.Free : ConfigurableJointMotion.Locked),
				yMotion = ((axis == Axis.Y) ? ConfigurableJointMotion.Free : ConfigurableJointMotion.Locked),
				zMotion = ((axis == Axis.Z) ? ConfigurableJointMotion.Free : ConfigurableJointMotion.Locked),
				angularXMotion = ConfigurableJointMotion.Locked,
				angularYMotion = ConfigurableJointMotion.Locked,
				angularZMotion = ConfigurableJointMotion.Locked
			};
			joint = jointSaveState.GenerateJointOnGameObject(base.gameObject);
		}
		forceLockJoint = GetComponent<ForceLockJoint>() ?? base.gameObject.AddComponent<ForceLockJoint>();
		forceLockJoint.CopyLocksFromJoint(joint);
		if ((upperLockType == LockType.UntilHandLeavesRegion || lowerLockType == LockType.UntilHandLeavesRegion) && handRegion == null)
		{
			Debug.LogWarning("Using region-based lock, but no hand region specified. Using permanent lock type instead.");
			if (upperLockType == LockType.UntilHandLeavesRegion)
			{
				upperLockType = LockType.Permanent;
			}
			if (lowerLockType == LockType.UntilHandLeavesRegion)
			{
				lowerLockType = LockType.Permanent;
			}
		}
		hapticInfo.ManualAwake();
	}

	private void OnEnable()
	{
		rb = GetComponent<Rigidbody>();
		grabbable = GetComponent<GrabbableItem>();
		if (grabbable != null)
		{
			GrabbableItem grabbableItem = grabbable;
			grabbableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(grabbableItem.OnGrabbed, new Action<GrabbableItem>(Grabbed));
			GrabbableItem grabbableItem2 = grabbable;
			grabbableItem2.OnReleased = (Action<GrabbableItem>)Delegate.Combine(grabbableItem2.OnReleased, new Action<GrabbableItem>(Released));
		}
	}

	private void OnDisable()
	{
		if (grabbable != null)
		{
			GrabbableItem grabbableItem = grabbable;
			grabbableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(grabbableItem.OnGrabbed, new Action<GrabbableItem>(Grabbed));
			GrabbableItem grabbableItem2 = grabbable;
			grabbableItem2.OnReleased = (Action<GrabbableItem>)Delegate.Remove(grabbableItem2.OnReleased, new Action<GrabbableItem>(Released));
		}
	}

	private void Start()
	{
		initialLocalPos = base.transform.localPosition;
		initialLocalRot = base.transform.localRotation;
		jointSaveState.connectedBody = ((!(base.transform.parent != null)) ? null : base.transform.parent.GetComponentInParent<Rigidbody>());
		if (axis == Axis.X)
		{
			if (base.transform.parent != null)
			{
				initialLocalAxis = base.transform.parent.InverseTransformDirection(base.transform.right);
			}
			else
			{
				initialLocalAxis = base.transform.right;
			}
		}
		else if (axis == Axis.Y)
		{
			if (base.transform.parent != null)
			{
				initialLocalAxis = base.transform.parent.InverseTransformDirection(base.transform.up);
			}
			else
			{
				initialLocalAxis = base.transform.up;
			}
		}
		else if (axis == Axis.Z)
		{
			if (base.transform.parent != null)
			{
				initialLocalAxis = base.transform.parent.InverseTransformDirection(base.transform.forward);
			}
			else
			{
				initialLocalAxis = base.transform.forward;
			}
		}
		if (initialLock == InitialLock.Upper)
		{
			LockUpper();
		}
		else if (initialLock == InitialLock.Lower)
		{
			LockLower();
		}
	}

	public void LockUpper()
	{
		LockUpper(false);
	}

	public void LockLower()
	{
		LockLower(false);
	}

	private void LockUpper(bool isInitial)
	{
		bool flag = upperLocked;
		if (breakFromHandWhenLocked && grabbable != null && grabbable.IsCurrInHand)
		{
			grabbable.CurrInteractableHand.TryRelease();
		}
		ForceSetOffset(upperLimit);
		LockJoint();
		upperLocked = true;
		lockTime = 0f;
		if (!flag)
		{
			if (lockUnlockAudioSource != null && upperLockSound != null)
			{
				lockUnlockAudioSource.SetClip(upperLockSound);
				lockUnlockAudioSource.Play();
			}
			if (this.OnUpperLocked != null)
			{
				this.OnUpperLocked(this, isInitial);
			}
		}
	}

	private void LockLower(bool isInitial)
	{
		bool flag = lowerLocked;
		if (breakFromHandWhenLocked && grabbable != null && grabbable.IsCurrInHand)
		{
			grabbable.CurrInteractableHand.TryRelease();
		}
		ForceSetOffset(lowerLimit);
		LockJoint();
		lowerLocked = true;
		lockTime = 0f;
		if (!flag)
		{
			if (lockUnlockAudioSource != null && lowerLockSound != null)
			{
				lockUnlockAudioSource.SetClip(lowerLockSound);
				lockUnlockAudioSource.Play();
			}
			if (this.OnLowerLocked != null)
			{
				this.OnLowerLocked(this, isInitial);
			}
		}
	}

	public void Unlock(bool applyForce = true)
	{
		if (upperLocked)
		{
			UnlockUpper(applyForce);
		}
		else if (lowerLocked)
		{
			UnlockLower(applyForce);
		}
	}

	public void UnlockUpper(bool applyForce = true)
	{
		if (upperLocked)
		{
			ForceSetOffset(upperLimit - 0.001f);
			upperLocked = false;
			UnlockJoint();
			lockTime = 0f;
			if (rb.IsSleeping())
			{
				rb.WakeUp();
			}
			if (applyForce && upperUnlockForce > 0f)
			{
				rb.AddForce(-GetAxisVector() * upperUnlockForce, ForceMode.Impulse);
			}
			if (lockUnlockAudioSource != null && upperUnlockSound != null)
			{
				lockUnlockAudioSource.SetClip(upperUnlockSound);
				lockUnlockAudioSource.Play();
			}
			if (this.OnUpperUnlocked != null)
			{
				this.OnUpperUnlocked(this);
			}
		}
	}

	public void UnlockLower(bool applyForce = true)
	{
		if (lowerLocked)
		{
			ForceSetOffset(lowerLimit + 0.001f);
			lowerLocked = false;
			UnlockJoint();
			lockTime = 0f;
			if (rb.IsSleeping())
			{
				rb.WakeUp();
			}
			if (applyForce && lowerUnlockForce > 0f)
			{
				rb.AddForce(GetAxisVector() * lowerUnlockForce, ForceMode.Impulse);
			}
			if (lockUnlockAudioSource != null && lowerUnlockSound != null)
			{
				lockUnlockAudioSource.SetClip(lowerUnlockSound);
				lockUnlockAudioSource.Play();
			}
			if (this.OnLowerUnlocked != null)
			{
				this.OnLowerUnlocked(this);
			}
		}
	}

	private void Grabbed(GrabbableItem grabbable)
	{
		Unlock(false);
	}

	private void Released(GrabbableItem grabbable)
	{
		rb.WakeUp();
	}

	private void Update()
	{
		hapticInfo.ManualUpdate();
	}

	private void LateUpdate()
	{
		if (isLoopingSoundPlaying || isStartSoundPlaying)
		{
			slideSoundTimeRemaining -= Time.deltaTime;
			if (slideSoundTimeRemaining <= 0f)
			{
				if (slideStopSound != null)
				{
					slideAudioSource.SetClip(slideStopSound);
					slideAudioSource.SetLooping(false);
					slideAudioSource.Play();
				}
				else
				{
					slideAudioSource.FadeOut(0.01f);
				}
				isLoopingSoundPlaying = false;
				isStartSoundPlaying = false;
				slideStartTime = 0f;
			}
		}
		if (upperLocked)
		{
			lockTime += Time.deltaTime;
			if (lockTime >= upperLockTimeout && ((upperLockType == LockType.UntilHandLeavesRegion && handRegion.DetectedHands.Count == 0) || upperLockType == LockType.Timeout))
			{
				UnlockUpper();
			}
			return;
		}
		if (lowerLocked)
		{
			lockTime += Time.deltaTime;
			if (lockTime >= lowerLockTimeout && ((lowerLockType == LockType.UntilHandLeavesRegion && handRegion.DetectedHands.Count == 0) || lowerLockType == LockType.Timeout))
			{
				UnlockLower();
			}
			return;
		}
		lastOffset = offset;
		offset = Vector3.Dot(base.transform.localPosition - initialLocalPos, initialLocalAxis);
		if (slideAudioSource != null)
		{
			float num = Mathf.Abs(offset - lastOffset);
			if (num > Mathf.Abs(Range / 1000f) && !isLoopingSoundPlaying)
			{
				if (!isStartSoundPlaying && slideStartSound != null)
				{
					slideAudioSource.SetClip(slideStartSound);
					slideAudioSource.SetLooping(false);
					isStartSoundPlaying = true;
					slideStartTime = Time.time;
					slideAudioSource.Play();
				}
				else if ((slideStartSound == null || (isStartSoundPlaying && Time.time - slideStartTime >= delayBetweenStartAndSlide)) && slideLoopSound != null)
				{
					slideAudioSource.SetClip(slideLoopSound);
					slideAudioSource.SetLooping(true);
					slideAudioSource.Play();
					isLoopingSoundPlaying = true;
				}
				else if (slideLoopSound == null)
				{
					isLoopingSoundPlaying = true;
				}
			}
			if (num > Mathf.Abs(Range / 1000f) && (isStartSoundPlaying || isLoopingSoundPlaying))
			{
				slideSoundTimeRemaining = slideSoundDecayTime;
				slideAudioSource.SetPitch(Mathf.Lerp(minSlidePitch, maxSlidePitch, Mathf.InverseLerp(0f, Range * 0.01f, num)));
				slideAudioSource.SetVolume(Mathf.Lerp(minSlideVol, maxSlideVol, Mathf.InverseLerp(0f, Range * 0.01f, num)));
			}
		}
		float num2 = ((!(grabbable != null) || !grabbable.IsCurrInHand) ? 0f : 0.0075f);
		if (upperLockType != 0 && offset - upperLimit > num2)
		{
			LockUpper();
		}
		if (lowerLockType != 0 && offset - lowerLimit < 0f - num2)
		{
			LockLower();
		}
	}

	private void FixedUpdate()
	{
		if (!upperLocked && !lowerLocked && (grabbable == null || !grabbable.IsCurrInHand) && naturalDriveForceMultiplier > 0f)
		{
			float num = upperLimit - lowerLimit;
			float num2 = lowerLimit + naturalDriveCurve.Evaluate((offset - lowerLimit) / num) * num - offset;
			if (Mathf.Abs(num2) > 0.001f)
			{
				rb.AddForce(GetAxisVector() * num2 * naturalDriveForceMultiplier * 20f, ForceMode.Force);
			}
		}
	}

	public Vector3 GetAxisVector()
	{
		if (axis == Axis.X)
		{
			return base.transform.right;
		}
		if (axis == Axis.Y)
		{
			return base.transform.up;
		}
		if (axis == Axis.Z)
		{
			return base.transform.forward;
		}
		return Vector3.zero;
	}

	public Vector3 GetLocalAxisVector()
	{
		if (axis == Axis.X)
		{
			return Vector3.right;
		}
		if (axis == Axis.Y)
		{
			return Vector3.up;
		}
		if (axis == Axis.Z)
		{
			return Vector3.forward;
		}
		return Vector3.zero;
	}

	private Vector3 GetLocalSecondaryAxisVector(Vector3 primaryAxis)
	{
		if (primaryAxis == Vector3.right)
		{
			return Vector3.up;
		}
		return Vector3.right;
	}

	private void ForceSetOffset(float offset)
	{
		base.transform.localPosition = initialLocalPos + initialLocalAxis * offset;
		base.transform.localRotation = initialLocalRot;
		rb.MovePosition(base.transform.position);
		rb.MoveRotation(base.transform.rotation);
		this.offset = offset;
	}

	private void LockJoint()
	{
		ConfigurableJointSaveState.CreateFromJoint(joint).RestoreToJoint(joint);
		joint.xMotion = ConfigurableJointMotion.Locked;
		joint.yMotion = ConfigurableJointMotion.Locked;
		joint.zMotion = ConfigurableJointMotion.Locked;
		forceLockJoint.CopyLocksFromJoint(joint);
		forceLockJoint.ResetMemory();
		if (useKinematicLock)
		{
			rb.isKinematic = true;
		}
	}

	private void UnlockJoint()
	{
		Vector3 localPosition = base.transform.localPosition;
		Quaternion localRotation = base.transform.localRotation;
		base.transform.localPosition = initialLocalPos;
		base.transform.localRotation = initialLocalRot;
		jointSaveState.RestoreToJoint(joint);
		base.transform.localPosition = localPosition;
		base.transform.localRotation = localRotation;
		forceLockJoint.CopyLocksFromJoint(joint);
		forceLockJoint.ResetMemory();
		if (useKinematicLock)
		{
			rb.isKinematic = false;
		}
		rb.WakeUp();
	}

	public void SetUpperLockType(LockType newType)
	{
		upperLockType = newType;
		if (upperLockType == LockType.UntilHandLeavesRegion && handRegion == null)
		{
			Debug.LogWarning("Using region-based lock, but no hand region specified. Using permanent lock type instead.");
			upperLockType = LockType.Permanent;
		}
	}

	public void SetLowerLockType(LockType newType)
	{
		lowerLockType = newType;
		if (lowerLockType == LockType.UntilHandLeavesRegion && handRegion == null)
		{
			Debug.LogWarning("Using region-based lock, but no hand region specified. Using permanent lock type instead.");
			lowerLockType = LockType.Permanent;
		}
	}

	public void SetNormalizedAngle(float percent)
	{
		percent = Mathf.Clamp01(percent);
		ForceSetOffset(Mathf.Lerp(lowerLimit, upperLimit, percent));
	}
}
