using UnityEngine;

namespace OwlchemyVR
{
	public class PositionalGrabbableController : GrabbableItem
	{
		private enum ReleaseGrabTypes
		{
			StickToLastPosition = 0,
			ReactivatePhysics = 1
		}

		[SerializeField]
		private bool releaseOnTriggerUp = true;

		[SerializeField]
		private bool overrideReleaseOnTriggerUpForOculus;

		[SerializeField]
		private bool releaseOnTriggerUpOculus = true;

		private bool needsToBreak;

		[SerializeField]
		private float breakDistUp = 0.5f;

		[SerializeField]
		private float breakDistSide = 0.1f;

		[SerializeField]
		private bool xAxisMoveEnabled;

		[SerializeField]
		private bool yAxisMoveEnabled;

		[SerializeField]
		private bool zAxisMoveEnabled;

		[SerializeField]
		private bool usePositionConstraints = true;

		[SerializeField]
		private Vector3 minimumLocalPositions = Vector3.zero;

		[SerializeField]
		private Vector3 maximumLocalPositions = Vector3.zero;

		[SerializeField]
		private ReleaseGrabTypes releaseGrabType;

		[SerializeField]
		private bool useHapticsWhileDragging;

		[SerializeField]
		private float hapticsActivateSensitivity = 0.01f;

		[SerializeField]
		private int hapticsRateMicroSec = 650;

		private float hapticsLengthSeconds = 0.02f;

		private HapticInfoObject hapticObject;

		[SerializeField]
		private SpringJoint optionalSpringJoint;

		[SerializeField]
		private Rigidbody body;

		private Vector3 originalLocalPosition;

		private Vector3 grabOriginLocal = Vector3.zero;

		private bool useSpring;

		private float originalSpringAmount;

		private Vector3 lastBodyPosition;

		public override void Awake()
		{
			base.Awake();
			hapticObject = new HapticInfoObject(hapticsRateMicroSec, hapticsLengthSeconds);
			hapticObject.DeactiveHaptic();
		}

		public override void Start()
		{
			base.Start();
			originalLocalPosition = base.transform.localPosition;
			if (optionalSpringJoint != null)
			{
				useSpring = true;
				originalSpringAmount = optionalSpringJoint.spring;
			}
		}

		private void Update()
		{
			if (base.IsCurrInHand)
			{
				Vector3 position = body.transform.TransformPoint(GetDesiredPositionOfGrabbedItem());
				body.transform.position = position;
				body.MovePosition(position);
			}
		}

		private void LateUpdate()
		{
			if (useHapticsWhileDragging && base.IsCurrInHand && Vector3.Distance(body.transform.position, lastBodyPosition) >= hapticsActivateSensitivity)
			{
				hapticObject.Restart();
				if (!base.CurrInteractableHand.HapticsController.ContainHaptic(hapticObject))
				{
					base.CurrInteractableHand.HapticsController.AddNewHaptic(hapticObject);
				}
				lastBodyPosition = body.transform.position;
			}
			if (usePositionConstraints)
			{
				ClampRigidbodyToBounds(body);
			}
			if (base.IsCurrInHand)
			{
				Vector3 vector = body.transform.position - base.CurrInteractableHand.transform.position;
				bool flag = false;
				if (vector.y <= 0f - breakDistUp)
				{
					flag = true;
				}
				if (vector.y >= breakDistSide)
				{
					flag = true;
				}
				if (Mathf.Abs(vector.x) >= breakDistSide || Mathf.Abs(vector.z) >= breakDistSide)
				{
					flag = true;
				}
				if (flag)
				{
					needsToBreak = true;
					base.CurrInteractableHand.ManuallyReleaseJoint();
				}
			}
		}

		private void ClampRigidbodyToBounds(Rigidbody r)
		{
			Vector3 localPosition = r.transform.localPosition;
			localPosition = GetClampedPosition(r.transform.localPosition);
			if (localPosition != r.transform.localPosition)
			{
				r.transform.localPosition = localPosition;
			}
		}

		private Vector3 GetClampedPosition(Vector3 pos)
		{
			Vector3 vector = minimumLocalPositions;
			Vector3 vector2 = maximumLocalPositions;
			Vector3 result = pos;
			if (xAxisMoveEnabled)
			{
				if (pos.x < vector.x)
				{
					result.x = vector.x;
				}
				if (pos.x > vector2.x)
				{
					result.x = vector2.x;
				}
			}
			else
			{
				result.x = originalLocalPosition.x;
			}
			if (yAxisMoveEnabled)
			{
				if (pos.y < vector.y)
				{
					result.y = vector.y;
				}
				if (pos.y > vector2.y)
				{
					result.y = vector2.y;
				}
			}
			else
			{
				result.y = originalLocalPosition.y;
			}
			if (zAxisMoveEnabled)
			{
				if (pos.z < vector.z)
				{
					result.z = vector.z;
				}
				if (pos.z > vector2.z)
				{
					result.z = vector2.z;
				}
			}
			else
			{
				result.z = originalLocalPosition.z;
			}
			return result;
		}

		private Vector3 GetDesiredPositionOfGrabbedItem()
		{
			Vector3 vector = base.transform.InverseTransformPoint(base.CurrInteractableHand.GetGrabPointPos());
			Vector3 result = vector - grabOriginLocal;
			if (!xAxisMoveEnabled)
			{
				result.x = originalLocalPosition.x;
			}
			if (!yAxisMoveEnabled)
			{
				result.y = originalLocalPosition.y;
			}
			if (!zAxisMoveEnabled)
			{
				result.z = originalLocalPosition.z;
			}
			return result;
		}

		public override void Grab(InteractionHandController interactableHand)
		{
			if (useSpring)
			{
				optionalSpringJoint.spring = 0f;
			}
			body.isKinematic = true;
			base.Grab(interactableHand);
			grabOriginLocal = base.transform.InverseTransformPoint(base.CurrInteractableHand.GetGrabPointPos());
			lastBodyPosition = body.transform.position;
		}

		public override bool Release(InteractionHandController interactableHand, Vector3 applyVelocity, Vector3 applyAngVelocity, bool wasSwappedBetweenHands = false)
		{
			bool flag = releaseOnTriggerUp;
			if (VRPlatform.GetCurrVRPlatformHardwareType() == VRPlatformHardwareType.OculusRift && overrideReleaseOnTriggerUpForOculus)
			{
				flag = releaseOnTriggerUpOculus;
			}
			if (!flag && !needsToBreak && !wasSwappedBetweenHands)
			{
				return false;
			}
			needsToBreak = false;
			if (hapticObject.IsRunning && interactableHand != null)
			{
				interactableHand.HapticsController.RemoveHaptic(hapticObject);
			}
			Vector3 grabbedItemCurrVelocity = base.CurrInteractableHand.GrabbedItemCurrVelocity;
			base.Release(interactableHand, applyVelocity, applyAngVelocity, wasSwappedBetweenHands);
			if (releaseGrabType == ReleaseGrabTypes.ReactivatePhysics)
			{
				body.isKinematic = false;
				body.velocity += grabbedItemCurrVelocity;
				if (useSpring)
				{
					optionalSpringJoint.spring = originalSpringAmount;
				}
			}
			return true;
		}
	}
}
