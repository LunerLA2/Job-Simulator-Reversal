using System;
using OwlchemyVR;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GrabbableHinge : MonoBehaviour
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

	public enum InitialLockType
	{
		None = 0,
		Upper = 1,
		Lower = 2
	}

	public delegate void UpperLockedHandler(GrabbableHinge hinge, bool isInitial);

	public delegate void LowerLockedHandler(GrabbableHinge hinge, bool isInitial);

	public delegate void UpperUnlockedHandler(GrabbableHinge hinge);

	public delegate void LowerUnlockedHandler(GrabbableHinge hinge);

	private const float OUT_OF_HAND_TOLERANCE = 0f;

	private const float IN_HAND_TOLERANCE = 2.5f;

	[SerializeField]
	private Axis axis;

	[SerializeField]
	public LockType upperLockType = LockType.Permanent;

	[SerializeField]
	public float upperLimit = 180f;

	[SerializeField]
	public float upperLockTimeout;

	[SerializeField]
	public float upperUnlockForce;

	[SerializeField]
	public LockType lowerLockType = LockType.Permanent;

	[SerializeField]
	public float lowerLimit = -180f;

	[SerializeField]
	public float lowerLockTimeout;

	[SerializeField]
	public float lowerUnlockForce;

	[SerializeField]
	private bool breakFromHandWhenLocked = true;

	[SerializeField]
	private InitialLockType initialLock;

	[SerializeField]
	private PlayerPartDetector handRegion;

	[SerializeField]
	private bool useKinematicLock;

	[SerializeField]
	private HapticTransformInfo hapticInfo;

	[SerializeField]
	private AnimationCurve naturalDriveCurve = new AnimationCurve(new Keyframe(0f, 0f, 1f, 1f), new Keyframe(1f, 1f, 1f, 1f));

	[SerializeField]
	private float naturalDriveForceMultiplier = 1f;

	private Vector3 initialLocalPos;

	private Quaternion initialLocalRot;

	private Quaternion invInitialLocalRot;

	private Rigidbody rb;

	private GrabbableItem grabbable;

	private float angle;

	private bool upperLocked;

	private bool lowerLocked;

	private float lockTime;

	private ConfigurableJoint joint;

	private ForceLockJoint forceLockJoint;

	private ConfigurableJointSaveState jointSaveState;

	public float Angle
	{
		get
		{
			return angle;
		}
	}

	public float Range
	{
		get
		{
			return upperLimit - lowerLimit;
		}
	}

	public float NormalizedAngle
	{
		get
		{
			return (angle - lowerLimit) / Range;
		}
	}

	public Axis HingeAxis
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

	public GrabbableItem Grabbable
	{
		get
		{
			return grabbable;
		}
	}

	public InitialLockType InitialLock
	{
		get
		{
			return initialLock;
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
				xMotion = ConfigurableJointMotion.Locked,
				yMotion = ConfigurableJointMotion.Locked,
				zMotion = ConfigurableJointMotion.Locked,
				angularXMotion = ((axis == Axis.X) ? ConfigurableJointMotion.Free : ConfigurableJointMotion.Locked),
				angularYMotion = ((axis == Axis.Y) ? ConfigurableJointMotion.Free : ConfigurableJointMotion.Locked),
				angularZMotion = ((axis == Axis.Z) ? ConfigurableJointMotion.Free : ConfigurableJointMotion.Locked)
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
		jointSaveState.connectedBody = ((!(base.transform.parent != null)) ? null : base.transform.parent.GetComponentInParent<Rigidbody>());
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
	}

	public void LockUpper()
	{
		LockUpperInternal(false);
	}

	public void LockLower(bool suppressEvents = false)
	{
		LockLowerInternal(false, suppressEvents);
	}

	private void LockUpperInternal(bool isInitial)
	{
		bool flag = upperLocked;
		if (breakFromHandWhenLocked && grabbable != null && grabbable.IsCurrInHand)
		{
			grabbable.CurrInteractableHand.TryRelease();
		}
		ForceSetAngle(upperLimit);
		LockJoint();
		upperLocked = true;
		lockTime = 0f;
		if (!flag && this.OnUpperLocked != null)
		{
			this.OnUpperLocked(this, isInitial);
		}
	}

	private void LockLowerInternal(bool isInitial, bool suppressEvents = false)
	{
		bool flag = lowerLocked;
		if (breakFromHandWhenLocked && grabbable != null && grabbable.IsCurrInHand)
		{
			grabbable.CurrInteractableHand.TryRelease();
		}
		ForceSetAngle(lowerLimit);
		LockJoint();
		lowerLocked = true;
		lockTime = 0f;
		if (!flag && !suppressEvents && this.OnLowerLocked != null)
		{
			this.OnLowerLocked(this, isInitial);
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
			ForceSetAngle(upperLimit - 0.25f);
			upperLocked = false;
			UnlockJoint();
			lockTime = 0f;
			if (rb.IsSleeping())
			{
				rb.WakeUp();
			}
			if (applyForce && upperUnlockForce > 0f)
			{
				rb.AddTorque(-GetAxisVector() * upperUnlockForce, ForceMode.Impulse);
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
			ForceSetAngle(lowerLimit + 0.25f);
			lowerLocked = false;
			UnlockJoint();
			lockTime = 0f;
			if (rb.IsSleeping())
			{
				rb.WakeUp();
			}
			if (applyForce && lowerUnlockForce > 0f)
			{
				rb.AddTorque(GetAxisVector() * lowerUnlockForce, ForceMode.Impulse);
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

	private void LateUpdate()
	{
		if (upperLocked)
		{
			lockTime += Time.deltaTime;
			if (lockTime >= upperLockTimeout && ((upperLockType == LockType.UntilHandLeavesRegion && handRegion.DetectedHands.Count == 0) || upperLockType == LockType.Timeout))
			{
				UnlockUpper();
				return;
			}
		}
		else if (lowerLocked)
		{
			lockTime += Time.deltaTime;
			if (lockTime >= lowerLockTimeout && ((lowerLockType == LockType.UntilHandLeavesRegion && handRegion.DetectedHands.Count == 0) || lowerLockType == LockType.Timeout))
			{
				UnlockLower();
				return;
			}
		}
		else
		{
			Quaternion quaternion = invInitialLocalRot * base.transform.localRotation;
			angle = 0f;
			float num = ((!(grabbable != null) || !grabbable.IsCurrInHand) ? 0f : 2.5f);
			if (axis == Axis.X)
			{
				Vector3 to = quaternion * Vector3.up;
				angle = Vector3.Angle(Vector3.up, to) * Mathf.Sign(to.z);
			}
			else if (axis == Axis.Y)
			{
				Vector3 to2 = quaternion * Vector3.forward;
				angle = Vector3.Angle(Vector3.forward, to2) * Mathf.Sign(to2.x);
			}
			else if (axis == Axis.Z)
			{
				Vector3 to3 = quaternion * Vector3.right;
				angle = Vector3.Angle(Vector3.right, to3) * Mathf.Sign(to3.y);
			}
			if (upperLockType != 0 && NormalizeAngle(angle) - upperLimit > num)
			{
				LockUpper();
			}
			if (lowerLockType != 0 && NormalizeAngle(angle) - lowerLimit < 0f - num)
			{
				LockLower();
			}
		}
		hapticInfo.ManualUpdate();
	}

	private void FixedUpdate()
	{
		if (!upperLocked && !lowerLocked && (grabbable == null || !grabbable.IsCurrInHand) && naturalDriveForceMultiplier > 0f)
		{
			float num = upperLimit - lowerLimit;
			float num2 = lowerLimit + naturalDriveCurve.Evaluate((angle - lowerLimit) / num) * num - angle;
			if (Mathf.Abs(num2) > 0.5f)
			{
				rb.AddTorque(GetAxisVector() * num2 * naturalDriveForceMultiplier * 0.1f, ForceMode.Force);
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

	private void ForceSetAngle(float angle)
	{
		base.transform.localPosition = initialLocalPos;
		base.transform.localRotation = initialLocalRot;
		if (axis == Axis.X)
		{
			base.transform.Rotate(angle, 0f, 0f);
		}
		else if (axis == Axis.Y)
		{
			base.transform.Rotate(0f, angle, 0f);
		}
		else if (axis == Axis.Z)
		{
			base.transform.Rotate(0f, 0f, angle);
		}
		rb.MovePosition(base.transform.position);
		rb.MoveRotation(base.transform.rotation);
		this.angle = angle;
	}

	private float NormalizeAngle(float a)
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
		ConfigurableJointSaveState.CreateFromJoint(joint).RestoreToJoint(joint);
		joint.angularXMotion = ConfigurableJointMotion.Locked;
		joint.angularYMotion = ConfigurableJointMotion.Locked;
		joint.angularZMotion = ConfigurableJointMotion.Locked;
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

	public void SetNormalizedAngle(float anglePercent)
	{
		anglePercent = Mathf.Clamp01(anglePercent);
		angle = Range * anglePercent + lowerLimit;
		ForceSetAngle(angle);
	}
}
