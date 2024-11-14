using System;
using UnityEngine;

namespace OwlchemyVR2
{
	public class GrabbableSlider : LimitedIndirectGrabbableController
	{
		private Vector3 initialLocalAxis;

		public event Action<GrabbableSlider, bool> OnUpperLocked;

		public event Action<GrabbableSlider, bool> OnLowerLocked;

		public event Action<GrabbableSlider> OnUpperUnlocked;

		public event Action<GrabbableSlider> OnLowerUnlocked;

		public event Action<GrabbableSlider, int> OnSnapIndexSelected;

		protected override float GetSnapTolerance()
		{
			return 0.003f;
		}

		protected override float GetUnlockShiftAmount()
		{
			return 0.001f;
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

		protected override void Start()
		{
			if (base.Axis == AxisTypes.X)
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
			else if (base.Axis == AxisTypes.Y)
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
			else if (base.Axis == AxisTypes.Z)
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
			base.Start();
		}

		protected override ConfigurableJointSaveState GetDefaultJointSettings()
		{
			ConfigurableJointSaveState configurableJointSaveState = new ConfigurableJointSaveState();
			configurableJointSaveState.axis = Vector3.right;
			configurableJointSaveState.secondaryAxis = Vector3.up;
			configurableJointSaveState.xMotion = ((base.Axis == AxisTypes.X) ? ConfigurableJointMotion.Free : ConfigurableJointMotion.Locked);
			configurableJointSaveState.yMotion = ((base.Axis == AxisTypes.Y) ? ConfigurableJointMotion.Free : ConfigurableJointMotion.Locked);
			configurableJointSaveState.zMotion = ((base.Axis == AxisTypes.Z) ? ConfigurableJointMotion.Free : ConfigurableJointMotion.Locked);
			configurableJointSaveState.angularXMotion = ConfigurableJointMotion.Locked;
			configurableJointSaveState.angularYMotion = ConfigurableJointMotion.Locked;
			configurableJointSaveState.angularZMotion = ConfigurableJointMotion.Locked;
			return configurableJointSaveState;
		}

		public override float GetCurrentAxisValue()
		{
			return Vector3.Dot(base.transform.localPosition - initialLocalPos, initialLocalAxis);
		}

		public override void AddAxisForce(float toDesiredAxisValue, float naturalDriveForceMultiplier)
		{
			if (Mathf.Abs(toDesiredAxisValue) > 0.001f)
			{
				Vector3 vector = GetLocalAxisVector() * toDesiredAxisValue * naturalDriveForceMultiplier * 20f;
				if (usePhysics)
				{
					rb.AddForce(base.transform.TransformVector(vector), ForceMode.Force);
					return;
				}
				vector = ConstrainVectorForNonPhysics(vector);
				base.transform.localPosition += vector * Time.deltaTime;
			}
		}

		protected override void ForceOrientTransformToAxisValue(float amount)
		{
			base.transform.localPosition = initialLocalPos + initialLocalAxis * amount;
			base.transform.localRotation = initialLocalRot;
			if (rb != null)
			{
				rb.MovePosition(base.transform.position);
				rb.MoveRotation(base.transform.rotation);
			}
			currentAxisValue = amount;
		}
	}
}
