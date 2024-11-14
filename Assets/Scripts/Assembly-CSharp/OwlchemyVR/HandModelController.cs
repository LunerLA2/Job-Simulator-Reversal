using System;
using System.Collections.Generic;
using UnityEngine;

namespace OwlchemyVR
{
	public class HandModelController : MonoBehaviour
	{
		private const int GRAB_OPEN_FULL_PERCENTAGE = 0;

		private const int GRAB_CLOSE_FULL_PERCENTAGE = 1;

		[SerializeField]
		private InteractionHandController interactableHandController;

		[SerializeField]
		private Renderer handRenderer;

		[SerializeField]
		private PoseContainer restingPose;

		[SerializeField]
		private PoseContainer grabbingPose;

		[SerializeField]
		private Transform fingerColliderTransform;

		[SerializeField]
		private Transform fingerColliderRestingPosition;

		[SerializeField]
		private Transform fingerColliderGrabbingPosition;

		private bool isHandAlwaysVisible;

		private bool shouldHandBeVisible = true;

		private bool isForcingHandToBeInvisible;

		private bool isItemWithinRange;

		private bool isClenching;

		private bool isUsingRestingCollider = true;

		private float destGrabPercentage;

		private float currGrabPercentage;

		private float openSpeed = 10f;

		private float closeSpeed = 7f;

		private Dictionary<int, Transform> cachedTransforms = new Dictionary<int, Transform>();

		private void Awake()
		{
		}

		private void OnEnable()
		{
			InteractionHandController interactionHandController = interactableHandController;
			interactionHandController.OnGrabSuccess = (Action<InteractionHandController, GrabbableItem>)Delegate.Combine(interactionHandController.OnGrabSuccess, new Action<InteractionHandController, GrabbableItem>(GrabSuccess));
			InteractionHandController interactionHandController2 = interactableHandController;
			interactionHandController2.OnGrabFailure = (Action<InteractionHandController>)Delegate.Combine(interactionHandController2.OnGrabFailure, new Action<InteractionHandController>(GrabFailure));
			InteractionHandController interactionHandController3 = interactableHandController;
			interactionHandController3.OnReleasedFailure = (Action<InteractionHandController>)Delegate.Combine(interactionHandController3.OnReleasedFailure, new Action<InteractionHandController>(ReleaseNothing));
			InteractionHandController interactionHandController4 = interactableHandController;
			interactionHandController4.OnReleasedGrabbable = (Action<InteractionHandController, GrabbableItem>)Delegate.Combine(interactionHandController4.OnReleasedGrabbable, new Action<InteractionHandController, GrabbableItem>(GrabRelease));
			InteractionHandController interactionHandController5 = interactableHandController;
			interactionHandController5.OnInteractionDeactivation = (Action<InteractionHandController>)Delegate.Combine(interactionHandController5.OnInteractionDeactivation, new Action<InteractionHandController>(InteractionDeactivation));
			InteractionHandController interactionHandController6 = interactableHandController;
			interactionHandController6.OnInteractionReactivation = (Action<InteractionHandController>)Delegate.Combine(interactionHandController6.OnInteractionReactivation, new Action<InteractionHandController>(InteractionReactivation));
		}

		private void OnDisable()
		{
			InteractionHandController interactionHandController = interactableHandController;
			interactionHandController.OnGrabSuccess = (Action<InteractionHandController, GrabbableItem>)Delegate.Remove(interactionHandController.OnGrabSuccess, new Action<InteractionHandController, GrabbableItem>(GrabSuccess));
			InteractionHandController interactionHandController2 = interactableHandController;
			interactionHandController2.OnGrabFailure = (Action<InteractionHandController>)Delegate.Remove(interactionHandController2.OnGrabFailure, new Action<InteractionHandController>(GrabFailure));
			InteractionHandController interactionHandController3 = interactableHandController;
			interactionHandController3.OnReleasedFailure = (Action<InteractionHandController>)Delegate.Remove(interactionHandController3.OnReleasedFailure, new Action<InteractionHandController>(ReleaseNothing));
			InteractionHandController interactionHandController4 = interactableHandController;
			interactionHandController4.OnReleasedGrabbable = (Action<InteractionHandController, GrabbableItem>)Delegate.Remove(interactionHandController4.OnReleasedGrabbable, new Action<InteractionHandController, GrabbableItem>(GrabRelease));
			InteractionHandController interactionHandController5 = interactableHandController;
			interactionHandController5.OnInteractionDeactivation = (Action<InteractionHandController>)Delegate.Remove(interactionHandController5.OnInteractionDeactivation, new Action<InteractionHandController>(InteractionDeactivation));
			InteractionHandController interactionHandController6 = interactableHandController;
			interactionHandController6.OnInteractionReactivation = (Action<InteractionHandController>)Delegate.Remove(interactionHandController6.OnInteractionReactivation, new Action<InteractionHandController>(InteractionReactivation));
		}

		private void GrabSuccess(InteractionHandController interactableHandController, GrabbableItem grabbableItem)
		{
			if (!isHandAlwaysVisible)
			{
				shouldHandBeVisible = false;
				handRenderer.enabled = false;
			}
			DoBlendedPose(1f, restingPose, grabbingPose);
			destGrabPercentage = currGrabPercentage;
		}

		private void GrabFailure(InteractionHandController interactableHandController)
		{
			GrabNothing();
		}

		private void GrabRelease(InteractionHandController interactableHandController, GrabbableItem grabbableItem)
		{
			shouldHandBeVisible = true;
			if (!isForcingHandToBeInvisible)
			{
				handRenderer.enabled = true;
			}
			DoBlendedPose(1f, restingPose, grabbingPose);
			destGrabPercentage = currGrabPercentage;
			SetupCurrGrabAnimationTypeBasedOnWithinItemRange();
		}

		private void InteractionDeactivation(InteractionHandController handController)
		{
			handRenderer.enabled = false;
			shouldHandBeVisible = false;
		}

		private void InteractionReactivation(InteractionHandController handController)
		{
			shouldHandBeVisible = true;
			if (!isForcingHandToBeInvisible)
			{
				handRenderer.enabled = true;
			}
		}

		public void ForceHandToBeInvisible()
		{
			isForcingHandToBeInvisible = true;
			handRenderer.enabled = false;
		}

		public void StopForcingHandToBeInvisible()
		{
			if (isForcingHandToBeInvisible)
			{
				isForcingHandToBeInvisible = false;
				handRenderer.enabled = shouldHandBeVisible;
			}
		}

		public void ToggleForcingHandToBeInvisible()
		{
			if (isForcingHandToBeInvisible)
			{
				StopForcingHandToBeInvisible();
			}
			else
			{
				ForceHandToBeInvisible();
			}
		}

		public void GrabNothing()
		{
			DoBlendedPose(0f, restingPose, grabbingPose);
			destGrabPercentage = 1f;
			isClenching = true;
		}

		private void ReleaseNothing(InteractionHandController handController)
		{
			DoBlendedPose(1f, restingPose, grabbingPose);
			destGrabPercentage = currGrabPercentage;
			isClenching = false;
			SetupCurrGrabAnimationTypeBasedOnWithinItemRange();
		}

		public void InRangeOfInteratableUpdate(bool isInRange)
		{
			isItemWithinRange = isInRange;
			if (!isClenching)
			{
				SetupCurrGrabAnimationTypeBasedOnWithinItemRange();
			}
		}

		private void SetupCurrGrabAnimationTypeBasedOnWithinItemRange()
		{
			if (isItemWithinRange)
			{
				destGrabPercentage = 1f;
			}
			else
			{
				destGrabPercentage = 0f;
			}
		}

		private void Update()
		{
			if (!handRenderer.enabled)
			{
				return;
			}
			float num = currGrabPercentage;
			if (destGrabPercentage < currGrabPercentage)
			{
				currGrabPercentage -= Time.deltaTime * openSpeed;
				if (currGrabPercentage <= 0f)
				{
					currGrabPercentage = 0f;
				}
			}
			else if (destGrabPercentage > currGrabPercentage)
			{
				currGrabPercentage += Time.deltaTime * closeSpeed;
				if (currGrabPercentage >= 1f)
				{
					currGrabPercentage = 1f;
				}
			}
			if (num != currGrabPercentage)
			{
				DoBlendedPose(currGrabPercentage, restingPose, grabbingPose);
			}
		}

		private void DoBlendedPose(float grabPercentage, PoseContainer blendFrom, PoseContainer blendTo)
		{
			foreach (int key in blendTo.storedBoneRotations.Keys)
			{
				Transform cachedTransform = GetCachedTransform(key);
				if (cachedTransform != null)
				{
					cachedTransform.localRotation = Quaternion.Lerp(blendFrom.GetRotationOfTransform(key), blendTo.GetRotationOfTransform(key), grabPercentage);
				}
			}
			float num = 0.5f;
			if (grabPercentage > num && isUsingRestingCollider)
			{
				fingerColliderTransform.localPosition = fingerColliderGrabbingPosition.localPosition;
				fingerColliderTransform.localRotation = fingerColliderGrabbingPosition.localRotation;
				isUsingRestingCollider = false;
			}
			if (grabPercentage < num && !isUsingRestingCollider)
			{
				fingerColliderTransform.localPosition = fingerColliderRestingPosition.localPosition;
				fingerColliderTransform.localRotation = fingerColliderRestingPosition.localRotation;
				isUsingRestingCollider = true;
			}
			currGrabPercentage = grabPercentage;
		}

		private Transform GetCachedTransform(int childId)
		{
			if (!cachedTransforms.ContainsKey(childId))
			{
				string nameForId = PoseContainer.GetNameForId(childId);
				if (nameForId == null)
				{
					nameForId = PoseContainer.GetNameForId(childId);
				}
				Transform transform = base.transform.Find(nameForId);
				cachedTransforms[childId] = transform;
				return transform;
			}
			return cachedTransforms[childId];
		}
	}
}
