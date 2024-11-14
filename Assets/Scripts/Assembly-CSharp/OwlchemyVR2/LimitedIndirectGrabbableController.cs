using System;
using System.Collections;
using OwlchemyVR;
using UnityEngine;

namespace OwlchemyVR2
{
	public class LimitedIndirectGrabbableController : IndirectGrabbableController
	{
		[Header("Joint Settings")]
		[Tooltip("The local axis to use.")]
		[SerializeField]
		private AxisTypes axis;

		[SerializeField]
		[Tooltip("The largest value the chosen local axis should allow.")]
		private float upperLimit = 180f;

		[Tooltip("What should happen when the joint hits the upper limit?\n\nDoNothing - the joint will be allowed to continue moving past the limit\n\nStopInPlace - the joint will stop and remain in place until the next time it is grabbed.\n\nStopUntilLeavesRegion - the joint will stop and remain in place until the player's hands are clear of the specified HandRegion PlayerPartDetector.\n\nStopInPlaceForTime - the joint will stop in place for a set time, after which it will return to being a loose physics object.")]
		[SerializeField]
		private LimitBehaviourTypes upperLimitBehaviour = LimitBehaviourTypes.StopInPlace;

		[Tooltip("If StopInPlaceForTime is used for the upper limit, how long should it be stopped in place for?")]
		[SerializeField]
		private float upperLimitTimeout;

		[Tooltip("If UnlockUpper() is called on this joint, how much ejection force should apply?")]
		[SerializeField]
		private float upperUnlockForce;

		[Tooltip("The smallest value the chosen local axis should allow.")]
		[SerializeField]
		private float lowerLimit = -180f;

		[SerializeField]
		[Tooltip("What should happen when the hinge hits the lower limit?\n\nDoNothing - the hinge will be allowed to continue moving past the limit\n\nStopInPlace - the hinge will stop and remain in place until the next time it is grabbed.\n\nStopUntilLeavesRegion - the hinge will stop and remain in place until the player's hands are clear of the specified HandRegion PlayerPartDetector.\n\nStopInPlaceForTime - the hinge will stop in place for a set time, allowing you to begin moving it back without letting go.")]
		private LimitBehaviourTypes lowerLimitBehaviour = LimitBehaviourTypes.StopInPlace;

		[SerializeField]
		[Tooltip("If StopInPlaceForTime is used for the lower limit, how long should it be stopped in place for?")]
		private float lowerLimitTimeout;

		[Tooltip("If UnlockLower() is called on this joint, how much ejection force should apply?")]
		[SerializeField]
		private float lowerUnlockForce;

		[Tooltip("Represents a relationship between the joint's current normalized amount (x axis), and it's desired normalized amount (y axis), which is used to apply ambient force. For example, a linear slope starting at 0 will produce no force, while a flat line at y=1 will produce constant force towards the upper limit.")]
		[SerializeField]
		private AnimationCurve naturalDriveCurve = new AnimationCurve(new Keyframe(0f, 0f, 1f, 1f), new Keyframe(1f, 1f, 1f, 1f));

		[Tooltip("This value multiplies the force applied by the natural drive curve.")]
		[SerializeField]
		private float naturalDriveForceMultiplier;

		private float lerpedNaturalDriveForceMultiplier = 1f;

		private float driveForceLerpSpeed = 0.25f;

		[SerializeField]
		private PlayerPartDetector ignoreDriveForceWhenHandInZone;

		[SerializeField]
		private ItemCollectionZone ignoreDriveForceWhenHeldItemInZone;

		[SerializeField]
		[Header("Snap Settings")]
		[Tooltip("Allows you to define points that the object will stick to.")]
		private bool useSnapIndexes;

		[SerializeField]
		[Tooltip("Axis values for snap points. For hinges, these are angles in degrees. For sliders, these are positions on the chosen local position axis.")]
		protected float[] snapValues;

		[Tooltip("The force with which to attempt to reach the snap points while being dragged. Lower this number for smoother movement, the object will still snap when released.")]
		[SerializeField]
		private float snapStrength = 10f;

		[Tooltip("Only apply snapStrength's force when within this range of the closest snap point's value.")]
		[SerializeField]
		private float snapWithinRange = 10f;

		[SerializeField]
		[Tooltip("If false, the object will instantly snap to the snap point instead of tweening. The tween uses snapStrength to determine its speed.")]
		private bool tweenToClosestSnapPointWhenReleased = true;

		[Tooltip("Which snap index should the object lock to on Start? Set to -1 to not use an initial index.")]
		[SerializeField]
		private int initialSnapIndex = -1;

		[SerializeField]
		[Tooltip("This sound will play when snapping between snap points.")]
		private AudioClip clickSound;

		private int selectionIndex = -1;

		private int prevClosestIndex = -1;

		[SerializeField]
		[Tooltip("Which limit to place the joint at when the scene starts.")]
		[Header("Optional/Extra Settings")]
		private InitialLockType initialLock;

		[Tooltip("If StopUntilHandLeavesRegion was used above, define the hand region here.")]
		[SerializeField]
		private PlayerPartDetector handRegion;

		[SerializeField]
		[Tooltip("In addition to being stopped in place based on the above logic, should the rigidbody of the joint be set to kinematic when the joint is locked? This can be useful on complex objects where you wish to reduce the number of non-kinematic rbs running at once.")]
		private bool useKinematicLock;

		[SerializeField]
		[Tooltip("If checked, the joint will always be kinematic when not being grabbed, preventing other objects from ever bumping it and removing any drift.")]
		private bool alwaysKinematicWhenNotGrabbed;

		[Tooltip("If no AudioSourceHelper is specified, sounds for this joint will be played using AudioManagers instead.")]
		[Header("Audio Settings")]
		[SerializeField]
		private AudioSourceHelper lockUnlockOwlchemyAudioSource;

		[SerializeField]
		private AudioSourceHelper slideOwlchemyAudioSource;

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

		[SerializeField]
		private float delayBetweenStartAndSlide = 0.1f;

		private float lastAxisValue;

		private bool isLoopingSoundPlaying;

		private bool isStartSoundPlaying;

		private float slideSoundTimeRemaining;

		private float slideSoundDecayTime = 0.175f;

		private float slideStartTime;

		private bool breakFromHandWhenLocked;

		protected Vector3 initialLocalPos;

		protected Quaternion initialLocalRot;

		protected Quaternion invInitialLocalRot;

		protected Rigidbody rb;

		protected float currentAxisValue;

		private bool upperLocked;

		private bool lowerLocked;

		private bool upperSoftLocked;

		private bool lowerSoftLocked;

		private float lockTime;

		private ConfigurableJoint joint;

		private ForceLockJointV2 forceLockJoint;

		private ConfigurableJointSaveState jointSaveState;

		protected bool isTweening;

		public float Range
		{
			get
			{
				return upperLimit - lowerLimit;
			}
		}

		public float NormalizedAxisValue
		{
			get
			{
				return (currentAxisValue - lowerLimit) / Range;
			}
		}

		public AxisTypes Axis
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

		public bool IsUpperSoftLocked
		{
			get
			{
				return upperSoftLocked;
			}
		}

		public bool IsLowerSoftLocked
		{
			get
			{
				return lowerSoftLocked;
			}
		}

		public LimitBehaviourTypes UpperLimitBehaviour
		{
			get
			{
				return upperLimitBehaviour;
			}
		}

		public LimitBehaviourTypes LowerLimitBehaviour
		{
			get
			{
				return lowerLimitBehaviour;
			}
		}

		public InitialLockType InitialLock
		{
			get
			{
				return initialLock;
			}
		}

		public float NaturalDriveForceMultiplier
		{
			get
			{
				return naturalDriveForceMultiplier;
			}
		}

		public int CurrentSnapIndex
		{
			get
			{
				return selectionIndex;
			}
		}

		public bool UseSnapIndexes
		{
			get
			{
				return useSnapIndexes;
			}
		}

		public int InitialSnapIndex
		{
			get
			{
				return initialSnapIndex;
			}
		}

		public AudioSourceHelper SlideOwlchemyAudioSource
		{
			get
			{
				return slideOwlchemyAudioSource;
			}
		}

		public event Action<LimitedIndirectGrabbableController, int> OnReleasedAtSnapIndex;

		protected virtual void DoLowerLockedEvent(bool isInitial)
		{
		}

		protected virtual void DoLowerUnlockedEvent()
		{
		}

		protected virtual void DoUpperLockedEvent(bool isInitial)
		{
		}

		protected virtual void DoUpperUnlockedEvent()
		{
		}

		protected virtual void DoSnapSelectedEvent(int index)
		{
		}

		public virtual float GetCurrentAxisValue()
		{
			return 0f;
		}

		protected virtual void ForceOrientTransformToAxisValue(float amount)
		{
		}

		protected virtual ConfigurableJointSaveState GetDefaultJointSettings()
		{
			return null;
		}

		protected virtual float GetLockTolerance()
		{
			return 0f;
		}

		protected virtual float GetUnlockShiftAmount()
		{
			return 0.25f;
		}

		protected virtual float GetSnapTolerance()
		{
			return 1f;
		}

		protected override void Awake()
		{
			base.Awake();
			if (!usePhysics)
			{
				return;
			}
			joint = GetComponent<ConfigurableJoint>();
			if (joint != null)
			{
				jointSaveState = ConfigurableJointSaveState.CreateFromJoint(joint);
			}
			else
			{
				jointSaveState = GetDefaultJointSettings();
				joint = jointSaveState.GenerateJointOnGameObject(base.gameObject);
			}
			forceLockJoint = GetComponent<ForceLockJointV2>() ?? base.gameObject.AddComponent<ForceLockJointV2>();
			forceLockJoint.CopyLocksFromJoint(joint);
			if ((upperLimitBehaviour == LimitBehaviourTypes.StopUntilHandLeavesRegion || lowerLimitBehaviour == LimitBehaviourTypes.StopUntilHandLeavesRegion) && handRegion == null)
			{
				Debug.LogWarning("Using region-based lock, but no hand region specified. Using permanent lock type instead.");
				if (upperLimitBehaviour == LimitBehaviourTypes.StopUntilHandLeavesRegion)
				{
					upperLimitBehaviour = LimitBehaviourTypes.StopInPlace;
				}
				if (lowerLimitBehaviour == LimitBehaviourTypes.StopUntilHandLeavesRegion)
				{
					lowerLimitBehaviour = LimitBehaviourTypes.StopInPlace;
				}
			}
		}

		protected override void Start()
		{
			base.Start();
			if (usePhysics)
			{
				jointSaveState.connectedBody = ((!(base.transform.parent != null)) ? null : base.transform.parent.GetComponentInParent<Rigidbody>());
			}
			initialLocalPos = base.transform.localPosition;
			initialLocalRot = base.transform.localRotation;
			invInitialLocalRot = Quaternion.Inverse(initialLocalRot);
			if (initialLock == InitialLockType.Upper)
			{
				LockUpperInternal(true);
			}
			else if (initialLock == InitialLockType.Lower)
			{
				LockLowerInternal(true);
			}
			if (usePhysics && alwaysKinematicWhenNotGrabbed)
			{
				rb.isKinematic = true;
			}
			if (useSnapIndexes)
			{
				SetSnapIndex(initialSnapIndex);
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			rb = GetComponent<Rigidbody>();
			if (usePhysics && rb == null)
			{
				Debug.LogError("UsePhysics is checked but no Rigidbody is present on " + base.gameObject.name, base.gameObject);
			}
			else if (!usePhysics && rb != null)
			{
				Debug.LogError("UsePhysics is not checked but a Rigidbody is present on " + base.gameObject.name + ". You will get unexpected results.", base.gameObject);
				rb = null;
			}
			if (base.Grabbable != null)
			{
				GrabbableItem grabbable = base.Grabbable;
				grabbable.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(grabbable.OnGrabbed, new Action<GrabbableItem>(Grabbed));
				GrabbableItem grabbable2 = base.Grabbable;
				grabbable2.OnReleased = (Action<GrabbableItem>)Delegate.Combine(grabbable2.OnReleased, new Action<GrabbableItem>(Released));
			}
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			if (base.Grabbable != null)
			{
				GrabbableItem grabbable = base.Grabbable;
				grabbable.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(grabbable.OnGrabbed, new Action<GrabbableItem>(Grabbed));
				GrabbableItem grabbable2 = base.Grabbable;
				grabbable2.OnReleased = (Action<GrabbableItem>)Delegate.Remove(grabbable2.OnReleased, new Action<GrabbableItem>(Released));
			}
		}

		protected virtual void Grabbed(GrabbableItem grabbable)
		{
			if (usePhysics && rb != null)
			{
				rb.isKinematic = false;
			}
			Unlock(false);
		}

		protected virtual void Released(GrabbableItem grabbable)
		{
			int num = -1;
			if (useSnapIndexes)
			{
				num = GetClosestSelectionIndex();
				if (tweenToClosestSnapPointWhenReleased)
				{
					float tweenTime = 0.5f;
					if (snapStrength > 0f)
					{
						tweenTime = 5f / snapStrength;
					}
					TweenToNormalizedAxisValue((snapValues[num] - lowerLimit) / Range, tweenTime);
				}
				else
				{
					SetSnapIndex(num);
				}
			}
			if (usePhysics && rb != null)
			{
				if (alwaysKinematicWhenNotGrabbed)
				{
					rb.isKinematic = true;
				}
				else
				{
					rb.WakeUp();
				}
			}
			if (num > -1 && useSnapIndexes && this.OnReleasedAtSnapIndex != null)
			{
				this.OnReleasedAtSnapIndex(this, num);
			}
		}

		public void SetSnapIndex(int newIndex, bool suppressEvents = false)
		{
			if (!useSnapIndexes)
			{
				Debug.LogError("Can't Set Snap index if the object is not using snapAngles!", base.gameObject);
			}
			else
			{
				if (newIndex < 0 || newIndex >= snapValues.Length)
				{
					return;
				}
				ForceOrientTransformToAxisValue(snapValues[newIndex]);
				if (newIndex != selectionIndex)
				{
					selectionIndex = newIndex;
					if (!suppressEvents)
					{
						DoSnapSelectedEvent(selectionIndex);
					}
				}
			}
		}

		private Quaternion CalculateLocalRotationFromAngle(float angle)
		{
			if (Axis == AxisTypes.X)
			{
				return Quaternion.Euler(angle, 0f, 0f);
			}
			if (Axis == AxisTypes.Y)
			{
				return Quaternion.Euler(0f, angle, 0f);
			}
			if (Axis == AxisTypes.Z)
			{
				return Quaternion.Euler(0f, 0f, angle);
			}
			return Quaternion.identity;
		}

		private Vector3 CalculateLocalPositionFromAxisValue(float axisValue)
		{
			if (Axis == AxisTypes.X)
			{
				return Vector3.right * axisValue;
			}
			if (Axis == AxisTypes.Y)
			{
				return Vector3.up * axisValue;
			}
			if (Axis == AxisTypes.Z)
			{
				return Vector3.forward * axisValue;
			}
			return Vector3.zero;
		}

		private float GetAngleDelta(float from, float to)
		{
			float num;
			for (num = to - from; num > 180f; num -= 360f)
			{
			}
			for (; num <= -180f; num += 360f)
			{
			}
			return Mathf.Abs(num);
		}

		private int GetClosestSelectionIndex()
		{
			float num = float.MaxValue;
			int num2 = -1;
			for (int i = 0; i < snapValues.Length; i++)
			{
				float angleDelta = GetAngleDelta(currentAxisValue, snapValues[i]);
				if (angleDelta < num)
				{
					num = angleDelta;
					num2 = i;
				}
			}
			if (num2 != prevClosestIndex)
			{
				prevClosestIndex = num2;
				if (clickSound != null)
				{
					AudioManager.Instance.Play(base.transform.position, clickSound, 1f, 1f);
				}
				DoSnapSelectedEvent(num2);
			}
			return num2;
		}

		protected virtual void UnlockedUpdate()
		{
			if (!useSnapIndexes || !base.Grabbable.IsCurrInHand || !(snapStrength > 0f))
			{
				return;
			}
			float num = snapValues[GetClosestSelectionIndex()];
			if (base.HandInputType == GrabbableInputTypes.ConvertPositionToRotation || base.HandInputType == GrabbableInputTypes.PositionAndRotation || base.HandInputType == GrabbableInputTypes.RotationOnly)
			{
				float angleDelta = GetAngleDelta(currentAxisValue, num);
				if (angleDelta <= snapWithinRange)
				{
					Quaternion b = CalculateLocalRotationFromAngle(num);
					base.transform.localRotation = Quaternion.Slerp(base.transform.localRotation, b, Time.deltaTime * snapStrength);
				}
			}
			if (base.HandInputType == GrabbableInputTypes.PositionAndRotation || base.HandInputType == GrabbableInputTypes.PositionOnly)
			{
				float num2 = Mathf.Abs(currentAxisValue - num);
				if (num2 <= snapWithinRange)
				{
					Vector3 b2 = CalculateLocalPositionFromAxisValue(num);
					base.transform.localPosition = Vector3.Lerp(base.transform.localPosition, b2, Time.deltaTime * snapStrength);
				}
			}
		}

		private void FixedUpdate()
		{
			if (isTweening || upperLocked || lowerLocked || (!(grabbableItem == null) && grabbableItem.IsCurrInHand) || !(naturalDriveForceMultiplier > 0f))
			{
				return;
			}
			float num = upperLimit - lowerLimit;
			if (!(num > 0f))
			{
				return;
			}
			if (ignoreDriveForceWhenHandInZone != null || ignoreDriveForceWhenHeldItemInZone != null)
			{
				if (ignoreDriveForceWhenHandInZone == null || ignoreDriveForceWhenHandInZone.DetectedHands.Count == 0)
				{
					if (ignoreDriveForceWhenHeldItemInZone == null || !ignoreDriveForceWhenHeldItemInZone.IsAnyHeldItemInside)
					{
						lerpedNaturalDriveForceMultiplier += Time.deltaTime * driveForceLerpSpeed;
						if (lerpedNaturalDriveForceMultiplier > 1f)
						{
							lerpedNaturalDriveForceMultiplier = 1f;
						}
					}
					else
					{
						lerpedNaturalDriveForceMultiplier -= Time.deltaTime * driveForceLerpSpeed;
						if (lerpedNaturalDriveForceMultiplier < 0f)
						{
							lerpedNaturalDriveForceMultiplier = 0f;
						}
					}
				}
				else
				{
					lerpedNaturalDriveForceMultiplier -= Time.deltaTime * driveForceLerpSpeed;
					if (lerpedNaturalDriveForceMultiplier < 0f)
					{
						lerpedNaturalDriveForceMultiplier = 0f;
					}
				}
			}
			else
			{
				lerpedNaturalDriveForceMultiplier = 1f;
			}
			if (lerpedNaturalDriveForceMultiplier > 0f)
			{
				float toDesiredAxisValue = lowerLimit + naturalDriveCurve.Evaluate((currentAxisValue - lowerLimit) / num) * num - currentAxisValue;
				AddAxisForce(toDesiredAxisValue, naturalDriveForceMultiplier * lerpedNaturalDriveForceMultiplier);
			}
		}

		public virtual void AddAxisForce(float toDesiredAxisValue, float naturalDriveForceMultiplier)
		{
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
						slideOwlchemyAudioSource.SetLooping(false);
						slideOwlchemyAudioSource.SetClip(slideStopSound);
						slideOwlchemyAudioSource.Play();
					}
					else
					{
						slideOwlchemyAudioSource.Stop();
					}
					isLoopingSoundPlaying = false;
					isStartSoundPlaying = false;
					slideStartTime = 0f;
				}
			}
			if (upperLocked && usePhysics)
			{
				lockTime += Time.deltaTime;
				if (lockTime >= upperLimitTimeout && ((upperLimitBehaviour == LimitBehaviourTypes.StopUntilHandLeavesRegion && handRegion.DetectedHands.Count == 0) || upperLimitBehaviour == LimitBehaviourTypes.StopInPlaceForTime))
				{
					UnlockUpper(true, false);
				}
				return;
			}
			if (lowerLocked && usePhysics)
			{
				lockTime += Time.deltaTime;
				if (lockTime >= lowerLimitTimeout && ((lowerLimitBehaviour == LimitBehaviourTypes.StopUntilHandLeavesRegion && handRegion.DetectedHands.Count == 0) || lowerLimitBehaviour == LimitBehaviourTypes.StopInPlaceForTime))
				{
					UnlockLower(true, false);
				}
				return;
			}
			UnlockedUpdate();
			lastAxisValue = currentAxisValue;
			lastAxisValue = GetCurrentAxisValue();
			float lockTolerance = GetLockTolerance();
			currentAxisValue = GetCurrentAxisValue();
			if (slideOwlchemyAudioSource != null)
			{
				float num = Mathf.Abs(currentAxisValue - lastAxisValue);
				if (num > Mathf.Abs(Range / 1000f) && !isLoopingSoundPlaying)
				{
					if (!isStartSoundPlaying && slideStartSound != null)
					{
						isStartSoundPlaying = true;
						slideStartTime = Time.time;
						slideOwlchemyAudioSource.SetLooping(false);
						slideOwlchemyAudioSource.SetClip(slideStartSound);
						slideOwlchemyAudioSource.Play();
					}
					else if (slideStartSound == null || (isStartSoundPlaying && Time.time - slideStartTime >= delayBetweenStartAndSlide && slideLoopSound != null))
					{
						slideOwlchemyAudioSource.SetLooping(true);
						slideOwlchemyAudioSource.SetClip(slideLoopSound);
						slideOwlchemyAudioSource.Play();
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
					slideOwlchemyAudioSource.SetPitch(Mathf.Lerp(minSlidePitch, maxSlidePitch, Mathf.InverseLerp(0f, Range * 0.01f, num)));
					slideOwlchemyAudioSource.SetVolume(Mathf.Lerp(minSlideVol, maxSlideVol, Mathf.InverseLerp(0f, Range * 0.01f, num)));
				}
			}
			if (isTweening)
			{
				return;
			}
			if (grabbableItem == null || !grabbableItem.IsCurrInHand)
			{
				if (upperLimitBehaviour != 0 || !usePhysics)
				{
					if (NormalizeAngle(currentAxisValue) - upperLimit > lockTolerance)
					{
						LockUpper();
					}
					else if (Mathf.Abs(NormalizeAngle(currentAxisValue) - upperLimit) < GetSnapTolerance())
					{
						SoftLockUpper();
					}
					else
					{
						SoftUnlockUpper();
					}
				}
				if (lowerLimitBehaviour != 0 || !usePhysics)
				{
					if (NormalizeAngle(currentAxisValue) - lowerLimit < 0f - lockTolerance)
					{
						LockLower();
					}
					else if (Mathf.Abs(NormalizeAngle(currentAxisValue) - lowerLimit) < GetSnapTolerance())
					{
						SoftLockLower();
					}
					else
					{
						SoftUnlockLower();
					}
				}
				return;
			}
			if (upperLimitBehaviour != 0 || !usePhysics)
			{
				if (NormalizedAxisValue > 1f)
				{
					ForceOrientTransformToAxisValue(upperLimit);
				}
				if (Mathf.Abs(NormalizeAngle(currentAxisValue) - upperLimit) < GetSnapTolerance())
				{
					SoftLockUpper();
				}
				else
				{
					SoftUnlockUpper();
				}
			}
			if (lowerLimitBehaviour != 0 || !usePhysics)
			{
				if (NormalizedAxisValue < 0f)
				{
					ForceOrientTransformToAxisValue(lowerLimit);
				}
				if (Mathf.Abs(NormalizeAngle(currentAxisValue) - lowerLimit) < GetSnapTolerance())
				{
					SoftLockLower();
				}
				else
				{
					SoftUnlockLower();
				}
			}
		}

		private void SoftLockUpper()
		{
			bool flag = upperSoftLocked;
			ForceOrientTransformToAxisValue(upperLimit);
			upperSoftLocked = true;
			if (!flag)
			{
				PlaySoundEffect(upperLockSound);
				DoUpperLockedEvent(false);
			}
		}

		private void SoftUnlockUpper()
		{
			bool flag = upperSoftLocked;
			upperSoftLocked = false;
			if (flag)
			{
				PlaySoundEffect(upperUnlockSound);
				DoUpperUnlockedEvent();
			}
		}

		private void SoftLockLower()
		{
			bool flag = lowerSoftLocked;
			ForceOrientTransformToAxisValue(lowerLimit);
			lowerSoftLocked = true;
			if (!flag)
			{
				PlaySoundEffect(lowerLockSound);
				DoLowerLockedEvent(false);
			}
		}

		private void SoftUnlockLower()
		{
			bool flag = lowerSoftLocked;
			lowerSoftLocked = false;
			if (flag)
			{
				PlaySoundEffect(lowerUnlockSound);
				DoLowerUnlockedEvent();
			}
		}

		protected void PlaySoundEffect(AudioClip clip)
		{
			if (clip != null)
			{
				if (lockUnlockOwlchemyAudioSource != null)
				{
					lockUnlockOwlchemyAudioSource.SetClip(clip);
					lockUnlockOwlchemyAudioSource.Play();
				}
				else
				{
					AudioManager.Instance.Play(base.transform.position, clip, 1f, 1f);
				}
			}
		}

		public void LockUpper()
		{
			LockUpperInternal(false);
		}

		public void LockLower(bool suppressEvents = false)
		{
			LockLowerInternal(false, suppressEvents);
		}

		public void SetNormalizedAmount(float amount)
		{
			if (IsUpperLocked && amount < 1f)
			{
				UnlockUpper(false);
			}
			if (IsLowerLocked && amount > 0f)
			{
				UnlockLower(false);
			}
			amount = Mathf.Clamp01(amount);
			currentAxisValue = Range * amount + LowerLimit;
			ForceOrientTransformToAxisValue(currentAxisValue);
		}

		private void LockUpperInternal(bool isInitial)
		{
			bool flag = !upperLocked && !upperSoftLocked;
			if (breakFromHandWhenLocked && grabbableItem != null && grabbableItem.IsCurrInHand)
			{
				grabbableItem.CurrInteractableHand.TryRelease();
			}
			ForceOrientTransformToAxisValue(upperLimit);
			LockJoint();
			upperLocked = true;
			upperSoftLocked = true;
			lockTime = 0f;
			if (flag)
			{
				if (!isInitial)
				{
					PlaySoundEffect(upperLockSound);
				}
				DoUpperLockedEvent(isInitial);
			}
		}

		private void LockLowerInternal(bool isInitial, bool suppressEvents = false)
		{
			bool flag = !lowerLocked && !lowerSoftLocked;
			if (breakFromHandWhenLocked && grabbableItem != null && grabbableItem.IsCurrInHand)
			{
				grabbableItem.CurrInteractableHand.TryRelease();
			}
			ForceOrientTransformToAxisValue(lowerLimit);
			LockJoint();
			lowerLocked = true;
			lowerSoftLocked = true;
			lockTime = 0f;
			if (flag)
			{
				if (!isInitial)
				{
					PlaySoundEffect(lowerLockSound);
				}
				if (!suppressEvents)
				{
					DoLowerLockedEvent(isInitial);
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

		public void UnlockUpper(bool applyForce = true, bool doSoftUnlock = true)
		{
			if (!upperLocked)
			{
				return;
			}
			ForceOrientTransformToAxisValue(upperLimit - GetUnlockShiftAmount());
			upperLocked = false;
			UnlockJoint();
			lockTime = 0f;
			if (usePhysics)
			{
				if (rb.IsSleeping())
				{
					rb.WakeUp();
				}
				if (applyForce && upperUnlockForce > 0f)
				{
					rb.AddTorque(-GetAxisVector() * upperUnlockForce, ForceMode.Impulse);
				}
			}
			if (doSoftUnlock && (grabbableItem == null || !grabbableItem.IsCurrInHand) && upperSoftLocked)
			{
				SoftUnlockUpper();
			}
		}

		public void UnlockLower(bool applyForce = true, bool doSoftUnlock = true)
		{
			if (!lowerLocked)
			{
				return;
			}
			ForceOrientTransformToAxisValue(lowerLimit + GetUnlockShiftAmount());
			lowerLocked = false;
			UnlockJoint();
			lockTime = 0f;
			if (usePhysics && rb != null)
			{
				if (rb.IsSleeping())
				{
					rb.WakeUp();
				}
				if (applyForce && lowerUnlockForce > 0f)
				{
					rb.AddTorque(GetAxisVector() * lowerUnlockForce, ForceMode.Impulse);
				}
			}
			if (doSoftUnlock && (grabbableItem == null || !grabbableItem.IsCurrInHand) && lowerSoftLocked)
			{
				SoftUnlockLower();
			}
		}

		public Vector3 GetAxisVector()
		{
			if (axis == AxisTypes.X)
			{
				return base.transform.right;
			}
			if (axis == AxisTypes.Y)
			{
				return base.transform.up;
			}
			if (axis == AxisTypes.Z)
			{
				return base.transform.forward;
			}
			return Vector3.zero;
		}

		public Vector3 GetLocalAxisVector()
		{
			if (axis == AxisTypes.X)
			{
				return Vector3.right;
			}
			if (axis == AxisTypes.Y)
			{
				return Vector3.up;
			}
			if (axis == AxisTypes.Z)
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

		protected float NormalizeAngle(float a)
		{
			while (a > 180f)
			{
				a -= 360f;
			}
			while (a <= -180f)
			{
				a += 360f;
			}
			return a;
		}

		private void LockJoint()
		{
			if (usePhysics)
			{
				ConfigurableJointSaveState.CreateFromJoint(joint).RestoreToJoint(joint);
				joint.angularXMotion = ConfigurableJointMotion.Locked;
				joint.angularYMotion = ConfigurableJointMotion.Locked;
				joint.angularZMotion = ConfigurableJointMotion.Locked;
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
		}

		private void UnlockJoint()
		{
			if (!usePhysics)
			{
				return;
			}
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
				if (alwaysKinematicWhenNotGrabbed)
				{
					if (grabbableItem.IsCurrInHand)
					{
						rb.isKinematic = false;
					}
				}
				else
				{
					rb.isKinematic = false;
				}
			}
			rb.WakeUp();
		}

		protected override Vector3 ConstrainVectorForNonPhysics(Vector3 v)
		{
			if (axis == AxisTypes.X)
			{
				return new Vector3(v.x, 0f, 0f);
			}
			if (axis == AxisTypes.Y)
			{
				return new Vector3(0f, v.y, 0f);
			}
			if (axis == AxisTypes.Z)
			{
				return new Vector3(0f, 0f, v.z);
			}
			return v;
		}

		public void TweenToNormalizedAxisValue(float normalizedValue, float tweenTime, float tweenDelay = 0f, GoEaseType easeType = GoEaseType.QuadInOut)
		{
			normalizedValue = Mathf.Clamp01(normalizedValue);
			if (isTweening)
			{
				StopAllCoroutines();
			}
			isTweening = true;
			StartCoroutine(InternalTweenToNormalizedAxisValue(normalizedValue, tweenTime, tweenDelay, easeType));
		}

		private IEnumerator InternalTweenToNormalizedAxisValue(float normalizedValue, float tweenTime, float tweenDelay = 0f, GoEaseType easeType = GoEaseType.QuadInOut)
		{
			Unlock(false);
			grabbableItem.enabled = false;
			bool previousKinematic = false;
			if (grabbableItem.Rigidbody != null)
			{
				previousKinematic = grabbableItem.Rigidbody.isKinematic;
				grabbableItem.Rigidbody.isKinematic = true;
			}
			Debug.Log("Tween from " + NormalizedAxisValue + " to " + normalizedValue);
			float desiredAxisValue = Range * normalizedValue + LowerLimit;
			float t = 0f;
			if (tweenDelay > 0f)
			{
				yield return new WaitForSeconds(tweenDelay);
			}
			if (tweenTime > 0f)
			{
				while (t < tweenTime)
				{
					ForceOrientTransformToAxisValue(Mathf.Lerp(currentAxisValue, desiredAxisValue, Mathf.Lerp(0.01f, 0.99f, t / tweenTime)));
					t += Time.deltaTime;
					yield return null;
				}
			}
			else
			{
				ForceOrientTransformToAxisValue(desiredAxisValue);
			}
			yield return null;
			if (grabbableItem.Rigidbody != null)
			{
				grabbableItem.Rigidbody.isKinematic = previousKinematic;
			}
			grabbableItem.enabled = true;
			SetNormalizedAmount(normalizedValue);
			isTweening = false;
		}
	}
}
