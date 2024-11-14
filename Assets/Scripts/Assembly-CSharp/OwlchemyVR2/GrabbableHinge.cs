using System;
using UnityEngine;

namespace OwlchemyVR2
{
	public class GrabbableHinge : LimitedIndirectGrabbableController
	{
		private enum HingeActivationDirection
		{
			Positive = 0,
			Negative = 1
		}

		[SerializeField]
		[Tooltip("Normalized angle to consider activated. This is for cases where you need an event for a lever being pulled, but don't want to wait until it is fully locked. 0 is lower limit, 1 is upper.")]
		[Range(0f, 1f)]
		[Header("Activation Event Settings")]
		private float normalizedAngleToActivateAt = 0.75f;

		[Range(0f, 1f)]
		[Tooltip("After being activated, the lever will reset and be allowed to activate again when the normalized angle falls back beyond this value. 0 is lower limit, 1 is upper.")]
		[SerializeField]
		private float normalizedAngleToResetAt = 0.4f;

		[SerializeField]
		private AudioClip activateSound;

		[SerializeField]
		private AudioClip resetSound;

		private HingeActivationDirection activationDirection;

		private bool isActivated;

		public AudioClip ActivateSound
		{
			get
			{
				return activateSound;
			}
		}

		public AudioClip ResetSound
		{
			get
			{
				return resetSound;
			}
		}

		public event Action<GrabbableHinge, bool> OnUpperLocked;

		public event Action<GrabbableHinge, bool> OnLowerLocked;

		public event Action<GrabbableHinge> OnUpperUnlocked;

		public event Action<GrabbableHinge> OnLowerUnlocked;

		public event Action<GrabbableHinge, int> OnSnapIndexSelected;

		public event Action<GrabbableHinge> OnHingeActivated;

		public event Action<GrabbableHinge> OnHingeReset;

		protected override float GetUnlockShiftAmount()
		{
			return 0f;
		}

		protected override float GetSnapTolerance()
		{
			return (usePhysics || !(grabbableItem != null) || !grabbableItem.IsCurrInHand) ? 1f : 0f;
		}

		public void SetActivateSound(AudioClip sound)
		{
			activateSound = sound;
		}

		public void SetResetSound(AudioClip sound)
		{
			resetSound = sound;
		}

		protected override void Awake()
		{
			base.Awake();
			for (int i = 0; i < snapValues.Length; i++)
			{
				snapValues[i] = Mathf.Repeat(snapValues[i], 360f);
			}
			if (normalizedAngleToActivateAt >= normalizedAngleToResetAt)
			{
				activationDirection = HingeActivationDirection.Positive;
			}
			else
			{
				activationDirection = HingeActivationDirection.Negative;
			}
		}

		protected override ConfigurableJointSaveState GetDefaultJointSettings()
		{
			ConfigurableJointSaveState configurableJointSaveState = new ConfigurableJointSaveState();
			configurableJointSaveState.axis = Vector3.right;
			configurableJointSaveState.secondaryAxis = Vector3.up;
			configurableJointSaveState.xMotion = ConfigurableJointMotion.Locked;
			configurableJointSaveState.yMotion = ConfigurableJointMotion.Locked;
			configurableJointSaveState.zMotion = ConfigurableJointMotion.Locked;
			configurableJointSaveState.angularXMotion = ((base.Axis == AxisTypes.X) ? ConfigurableJointMotion.Free : ConfigurableJointMotion.Locked);
			configurableJointSaveState.angularYMotion = ((base.Axis == AxisTypes.Y) ? ConfigurableJointMotion.Free : ConfigurableJointMotion.Locked);
			configurableJointSaveState.angularZMotion = ((base.Axis == AxisTypes.Z) ? ConfigurableJointMotion.Free : ConfigurableJointMotion.Locked);
			return configurableJointSaveState;
		}

		public override float GetCurrentAxisValue()
		{
			float result = 0f;
			Quaternion quaternion = invInitialLocalRot * base.transform.localRotation;
			if (base.Axis == AxisTypes.X)
			{
				Vector3 to = quaternion * Vector3.up;
				result = Vector3.Angle(Vector3.up, to) * Mathf.Sign(to.z);
			}
			else if (base.Axis == AxisTypes.Y)
			{
				Vector3 to2 = quaternion * Vector3.forward;
				result = Vector3.Angle(Vector3.forward, to2) * Mathf.Sign(to2.x);
			}
			else if (base.Axis == AxisTypes.Z)
			{
				Vector3 to3 = quaternion * Vector3.right;
				result = Vector3.Angle(Vector3.right, to3) * Mathf.Sign(to3.y);
			}
			return result;
		}

		protected override void DoLowerLockedEvent(bool isInitial)
		{
			if (this.OnLowerLocked != null)
			{
				this.OnLowerLocked(this, isInitial);
			}
		}

		protected override void DoUpperLockedEvent(bool isInitial)
		{
			if (this.OnUpperLocked != null)
			{
				this.OnUpperLocked(this, isInitial);
			}
		}

		protected override void DoLowerUnlockedEvent()
		{
			if (this.OnLowerUnlocked != null)
			{
				this.OnLowerUnlocked(this);
			}
		}

		protected override void DoUpperUnlockedEvent()
		{
			if (this.OnUpperUnlocked != null)
			{
				this.OnUpperUnlocked(this);
			}
		}

		protected override void DoSnapSelectedEvent(int index)
		{
			if (this.OnSnapIndexSelected != null)
			{
				this.OnSnapIndexSelected(this, index);
			}
		}

		protected override void UnlockedUpdate()
		{
			base.UnlockedUpdate();
			if (isActivated)
			{
				if ((activationDirection == HingeActivationDirection.Positive && base.NormalizedAxisValue <= normalizedAngleToResetAt) || (activationDirection == HingeActivationDirection.Negative && base.NormalizedAxisValue >= normalizedAngleToResetAt))
				{
					isActivated = false;
					PlaySoundEffect(resetSound);
					if (this.OnHingeReset != null)
					{
						this.OnHingeReset(this);
					}
				}
			}
			else if ((activationDirection == HingeActivationDirection.Positive && base.NormalizedAxisValue >= normalizedAngleToActivateAt) || (activationDirection == HingeActivationDirection.Negative && base.NormalizedAxisValue <= normalizedAngleToActivateAt))
			{
				isActivated = true;
				PlaySoundEffect(activateSound);
				if (this.OnHingeActivated != null)
				{
					this.OnHingeActivated(this);
				}
			}
		}

		public override void AddAxisForce(float toDesiredAxisValue, float naturalDriveForceMultiplier)
		{
			if (Mathf.Abs(toDesiredAxisValue) > 0.5f)
			{
				Vector3 vector = GetLocalAxisVector() * toDesiredAxisValue * naturalDriveForceMultiplier * 0.1f;
				if (usePhysics)
				{
					rb.AddTorque(base.transform.TransformVector(vector), ForceMode.Force);
					return;
				}
				vector = ConstrainVectorForNonPhysics(vector);
				float magnitude = vector.magnitude;
				Vector3 vector2 = vector / magnitude;
				base.transform.Rotate(vector2, magnitude * Time.deltaTime * 100f);
			}
		}

		protected override void ForceOrientTransformToAxisValue(float angle)
		{
			base.transform.localPosition = initialLocalPos;
			base.transform.localRotation = initialLocalRot;
			if (base.Axis == AxisTypes.X)
			{
				base.transform.Rotate(angle, 0f, 0f);
			}
			else if (base.Axis == AxisTypes.Y)
			{
				base.transform.Rotate(0f, angle, 0f);
			}
			else if (base.Axis == AxisTypes.Z)
			{
				base.transform.Rotate(0f, 0f, angle);
			}
			if (rb != null)
			{
				rb.MovePosition(base.transform.position);
				rb.MoveRotation(base.transform.rotation);
			}
			currentAxisValue = angle;
		}
	}
}
