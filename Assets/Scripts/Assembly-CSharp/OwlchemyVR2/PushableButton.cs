using System;
using OwlchemyVR;
using UnityEngine;

namespace OwlchemyVR2
{
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(WorldItem))]
	public class PushableButton : MonoBehaviour
	{
		public enum Axis
		{
			X = 0,
			Y = 1,
			Z = 2
		}

		public enum RestingPositionType
		{
			UpperLimit = 0,
			LowerLimit = 1
		}

		private const float TOLERANCE = 0.0015f;

		[Header("Button Settings")]
		[Tooltip("The local axis the button should slide along.")]
		[SerializeField]
		private Axis axis = Axis.Z;

		[SerializeField]
		[Tooltip("This position on the chosen local axis is the highest the button will go.")]
		private float upperLimit;

		[SerializeField]
		[Tooltip("This position on the chosen local axis is the lowest the button will go.")]
		private float lowerLimit = -0.05f;

		[Tooltip("This determines whether the button's upper or lower limit is considered its resting position. You may need to switch this if a button happens to be oriented backwards.")]
		[SerializeField]
		private RestingPositionType restingPosition;

		[Tooltip("After being fully pushed, the button will freeze in place for this many seconds before returning to resting position.")]
		[SerializeField]
		private float timeToStayPushed = 0.25f;

		[Tooltip("After returning to resting position, the button will freeze in place for this many seconds before allowing the next push. Use to reduce unwanted rapid presses.")]
		[SerializeField]
		private float timeBeforeAllowingNextPush;

		[SerializeField]
		[Tooltip("This is the force/speed at which the button will attempt to return to resting position when partially pressed or after a complete press.")]
		private float returnToRestingPositionForce = 10f;

		[Header("Audio Settings")]
		[SerializeField]
		[Tooltip("If no AudioSourceHelper is specified, the button will use AudioManager to play its sounds instead.")]
		private AudioSourceHelper optionalCustomOwlchemyAudioSource;

		[SerializeField]
		public AudioClip soundWhenPushed;

		private Vector3 initialLocalPos;

		private Quaternion initialLocalRot;

		private Vector3 initialLocalAxis;

		private Rigidbody rb;

		private float offset;

		private bool upperLocked;

		private bool lowerLocked;

		private float lockTime;

		private ConfigurableJoint joint;

		private ForceLockJointV2 forceLockJoint;

		private ConfigurableJointSaveState jointSaveState;

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
				return Mathf.Clamp01((offset - lowerLimit) / Range);
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

		public event Action<PushableButton> OnButtonPushed;

		public event Action<PushableButton> OnButtonReturnedToRestingPosition;

		private void Awake()
		{
			rb = GetComponent<Rigidbody>();
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
			forceLockJoint = GetComponent<ForceLockJointV2>() ?? base.gameObject.AddComponent<ForceLockJointV2>();
			forceLockJoint.CopyLocksFromJoint(joint);
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
			if (restingPosition == RestingPositionType.UpperLimit)
			{
				LockUpper();
			}
			else if (restingPosition == RestingPositionType.LowerLimit)
			{
				LockLower();
			}
		}

		private void LateUpdate()
		{
			if (upperLocked)
			{
				lockTime += Time.deltaTime;
				if (lockTime >= ((restingPosition != 0) ? timeToStayPushed : timeBeforeAllowingNextPush))
				{
					UnlockUpper();
				}
				return;
			}
			if (lowerLocked)
			{
				lockTime += Time.deltaTime;
				if (lockTime >= ((restingPosition != 0) ? timeBeforeAllowingNextPush : timeToStayPushed))
				{
					UnlockLower();
				}
				return;
			}
			offset = Vector3.Dot(base.transform.localPosition, initialLocalAxis);
			if (offset - upperLimit > 0.0015f)
			{
				LockUpper();
			}
			if (offset - lowerLimit < -0.0015f)
			{
				LockLower();
			}
		}

		private void FixedUpdate()
		{
			if (upperLocked || lowerLocked || !(returnToRestingPositionForce > 0f) || !(returnToRestingPositionForce > 0f))
			{
				return;
			}
			float num = returnToRestingPositionForce;
			bool flag = true;
			if (restingPosition == RestingPositionType.LowerLimit)
			{
				num *= NormalizedOffset;
				if (NormalizedOffset <= 0.01f)
				{
					flag = false;
				}
			}
			else
			{
				num *= 1f - NormalizedOffset;
				if (NormalizedOffset >= 0.99f)
				{
					flag = false;
				}
			}
			if (flag)
			{
				rb.AddForce(GetAxisVector() * num * ((restingPosition != 0) ? (-1f) : 1f), ForceMode.Force);
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
			ForceSetOffset(upperLimit);
			LockJoint();
			upperLocked = true;
			lockTime = 0f;
			if (restingPosition == RestingPositionType.UpperLimit)
			{
				if (this.OnButtonReturnedToRestingPosition != null)
				{
					this.OnButtonReturnedToRestingPosition(this);
				}
				return;
			}
			if (!flag)
			{
				PlayPressedSound();
			}
			if (this.OnButtonPushed != null)
			{
				this.OnButtonPushed(this);
			}
		}

		private void LockLower(bool isInitial)
		{
			bool flag = lowerLocked;
			ForceSetOffset(lowerLimit);
			LockJoint();
			lowerLocked = true;
			lockTime = 0f;
			if (restingPosition == RestingPositionType.LowerLimit)
			{
				if (this.OnButtonReturnedToRestingPosition != null)
				{
					this.OnButtonReturnedToRestingPosition(this);
				}
				return;
			}
			if (!flag)
			{
				PlayPressedSound();
			}
			if (this.OnButtonPushed != null)
			{
				this.OnButtonPushed(this);
			}
		}

		private void PlayPressedSound()
		{
			if (soundWhenPushed != null)
			{
				if (optionalCustomOwlchemyAudioSource != null)
				{
					optionalCustomOwlchemyAudioSource.SetClip(soundWhenPushed);
					optionalCustomOwlchemyAudioSource.Play();
				}
				else
				{
					AudioManager.Instance.Play(base.transform.position, soundWhenPushed, 1f, 1f);
				}
			}
		}

		public void UnlockUpper()
		{
			if (!upperLocked)
			{
				return;
			}
			if (restingPosition != 0)
			{
				ForceSetOffset(upperLimit - 0.001f);
				if (rb.IsSleeping())
				{
					rb.WakeUp();
				}
			}
			upperLocked = false;
			UnlockJoint();
			lockTime = 0f;
		}

		public void UnlockLower()
		{
			if (!lowerLocked)
			{
				return;
			}
			if (restingPosition != RestingPositionType.LowerLimit)
			{
				ForceSetOffset(lowerLimit + 0.001f);
				if (rb.IsSleeping())
				{
					rb.WakeUp();
				}
			}
			lowerLocked = false;
			UnlockJoint();
			lockTime = 0f;
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
			base.transform.localPosition = initialLocalAxis * offset;
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
			rb.WakeUp();
		}
	}
}
