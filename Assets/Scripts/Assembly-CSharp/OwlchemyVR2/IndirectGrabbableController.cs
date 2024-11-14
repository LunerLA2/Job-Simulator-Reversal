using System;
using OwlchemyVR;
using UnityEngine;

namespace OwlchemyVR2
{
	[RequireComponent(typeof(GrabbableItem))]
	public class IndirectGrabbableController : MonoBehaviour
	{
		public enum AxisTypes
		{
			X = 0,
			Y = 1,
			Z = 2
		}

		public enum LimitBehaviourTypes
		{
			DoNothing = 0,
			StopInPlace = 1,
			StopUntilHandLeavesRegion = 2,
			StopInPlaceForTime = 3
		}

		public enum InitialLockType
		{
			None = 0,
			Upper = 1,
			Lower = 2
		}

		public enum GrabbableInputTypes
		{
			PositionOnly = 0,
			RotationOnly = 1,
			PositionAndRotation = 2,
			ConvertPositionToRotation = 3
		}

		private const float SNAP_SPEED_SCALE = 20f;

		private const float MAX_SNAP_SPEED = 15f;

		private const float MAX_OFFSET = 0.15f;

		private const float ANGULAR_SNAP_SPEED_SCALE = 0.5f;

		private const float MAX_ANGULAR_SNAP_SPEED = 30f;

		[Tooltip("If false, this object will act purely based on math rather than a Rigidbody")]
		[Header("Core Settings")]
		[SerializeField]
		protected bool usePhysics = true;

		[SerializeField]
		[Tooltip("Multiplies the force that is applied to the grabbable to attempt to reach your hand.")]
		private float followHandStrengthMultiplier = 1f;

		[Tooltip("Multiplies the distance at which the grabbable will break from your hand. Increase this if players frequently have the grabbable slip out of their hand when trying to move it.")]
		[SerializeField]
		private float breakDistanceToleranceMultiplier = 1f;

		[Tooltip("PositionOnly: Use the delta of the hand's position to manipulate the RB (ex: Levers, Sliders)\n\nRotationOnly: Use the delta of the hand's rotation to manipulate the RB (ex: Twistable dials)\n\nPositionAndRotation: Use both (ex: Complex custom designs)")]
		[SerializeField]
		private GrabbableInputTypes handInputType;

		[Tooltip("An object containing settings for haptic feedback")]
		[SerializeField]
		private HapticTransformInfo hapticInfo;

		protected GrabbableItem grabbableItem;

		private Vector3 handPosRelToSelfAtGrab;

		private Quaternion rotRelToHandAtGrab;

		private Transform handTransform;

		public bool UsePhysics
		{
			get
			{
				return usePhysics;
			}
		}

		public GrabbableInputTypes HandInputType
		{
			get
			{
				return handInputType;
			}
		}

		public GrabbableItem Grabbable
		{
			get
			{
				if (grabbableItem == null)
				{
					grabbableItem = GetComponent<GrabbableItem>();
					if (grabbableItem == null)
					{
						Debug.LogError("IndirectGrabbableController couldn't find its required OwlchemyVR.GrabbableItem: " + base.gameObject.name, base.gameObject);
					}
				}
				return grabbableItem;
			}
		}

		public void EditorSetHandInputType(GrabbableInputTypes t)
		{
			handInputType = t;
		}

		protected virtual void Awake()
		{
			grabbableItem = GetComponent<GrabbableItem>();
			if (grabbableItem == null)
			{
				Debug.LogError("IndirectGrabbableController couldn't find its required OwlchemyVR.GrabbableItem: " + base.gameObject.name, base.gameObject);
			}
			hapticInfo.ManualAwake();
		}

		protected virtual void Start()
		{
			if (grabbableItem.Rigidbody != null)
			{
				grabbableItem.Rigidbody.maxAngularVelocity = 30f;
			}
		}

		protected virtual void OnEnable()
		{
			GrabbableItem obj = grabbableItem;
			obj.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(obj.OnGrabbed, new Action<GrabbableItem>(Grabbed));
			GrabbableItem obj2 = grabbableItem;
			obj2.OnGrabbedUpdate = (Action<GrabbableItem>)Delegate.Combine(obj2.OnGrabbedUpdate, new Action<GrabbableItem>(GrabbedUpdate));
		}

		protected virtual void OnDisable()
		{
			GrabbableItem obj = grabbableItem;
			obj.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(obj.OnGrabbed, new Action<GrabbableItem>(Grabbed));
			GrabbableItem obj2 = grabbableItem;
			obj2.OnGrabbedUpdate = (Action<GrabbableItem>)Delegate.Remove(obj2.OnGrabbedUpdate, new Action<GrabbableItem>(GrabbedUpdate));
		}

		private void Grabbed(GrabbableItem item)
		{
			handTransform = grabbableItem.CurrInteractableHand.GetGrabPointTransform();
			handPosRelToSelfAtGrab = base.transform.InverseTransformPoint(handTransform.position);
			rotRelToHandAtGrab = Quaternion.Inverse(handTransform.rotation) * base.transform.rotation;
		}

		private void GrabbedUpdate(GrabbableItem item)
		{
			if (handTransform == null)
			{
				return;
			}
			Vector3 vector = base.transform.InverseTransformPoint(handTransform.position);
			Vector3 vector2 = vector - handPosRelToSelfAtGrab;
			if (vector2.magnitude > 0.15f * breakDistanceToleranceMultiplier)
			{
				grabbableItem.CurrInteractableHand.TryRelease(false);
				return;
			}
			bool flag = !usePhysics || !grabbableItem.Rigidbody.isKinematic;
			if ((handInputType == GrabbableInputTypes.PositionOnly || handInputType == GrabbableInputTypes.PositionAndRotation) && flag)
			{
				Vector3 vector3 = vector2 * 20f * followHandStrengthMultiplier;
				float magnitude = vector3.magnitude;
				if (magnitude > 15f * followHandStrengthMultiplier)
				{
					vector3 *= 15f * followHandStrengthMultiplier / magnitude;
				}
				if (usePhysics)
				{
					grabbableItem.Rigidbody.velocity = base.transform.TransformVector(vector3);
				}
				else
				{
					vector3 = ConstrainVectorForNonPhysics(vector3);
					grabbableItem.transform.Translate(vector3 * Time.deltaTime, Space.Self);
				}
			}
			if (handInputType == GrabbableInputTypes.RotationOnly || handInputType == GrabbableInputTypes.PositionAndRotation || handInputType == GrabbableInputTypes.ConvertPositionToRotation)
			{
				float num = followHandStrengthMultiplier * ((!usePhysics) ? 100f : 1f);
				Quaternion identity = Quaternion.identity;
				identity = ((handInputType != GrabbableInputTypes.ConvertPositionToRotation) ? (handTransform.rotation * rotRelToHandAtGrab) : (base.transform.rotation * Quaternion.FromToRotation(handPosRelToSelfAtGrab, vector)));
				float angle;
				Vector3 axis;
				(Quaternion.Inverse(base.transform.rotation) * identity).ToAngleAxis(out angle, out axis);
				while (angle > 180f)
				{
					angle -= 360f;
				}
				for (; angle < -180f; angle += 360f)
				{
				}
				if (flag && Mathf.Abs(angle) > 0.05f)
				{
					if (usePhysics)
					{
						float num2 = Mathf.Min(30f, angle * 0.5f) * num;
						Vector3 vector4 = axis * num2;
						grabbableItem.Rigidbody.angularVelocity = base.transform.TransformVector(vector4);
					}
					else
					{
						float num3 = angle * 0.5f;
						Vector3 vector5 = ConstrainVectorForNonPhysics(axis * num3);
						num3 = Mathf.Min(30f, vector5.magnitude) * num;
						axis = vector5 / num3;
						base.transform.Rotate(axis, num3 * Time.deltaTime, Space.Self);
					}
				}
			}
			if (hapticInfo != null)
			{
				hapticInfo.ManualUpdate();
			}
		}

		protected virtual Vector3 ConstrainVectorForNonPhysics(Vector3 angularVelocity)
		{
			return angularVelocity;
		}
	}
}
