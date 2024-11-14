using System;
using UnityEngine;

namespace OwlchemyVR
{
	public class HandPoser : MonoBehaviour
	{
		private enum HandState
		{
			Pointing = 0,
			Resting = 1,
			Grabbing = 2
		}

		private const string PREF_KEY_OCULUSHANDTYPEA = "OculusHandType";

		[SerializeField]
		private GameObject handPoseModel;

		[SerializeField]
		private Animator a;

		[SerializeField]
		private Renderer handRenderer;

		[SerializeField]
		private InteractionHandController interactionHandController;

		[SerializeField]
		private SteamVRCompatible_IndividualController steamVRTouch_IndividualController;

		[SerializeField]
		private RuntimeAnimatorController animControllerA;

		[SerializeField]
		private RuntimeAnimatorController animControllerB;

		[SerializeField]
		private Collider fingerCollider;

		[SerializeField]
		private Transform fingerColliderRestLocation;

		[SerializeField]
		private Transform fingerColliderGrabLocation;

		[SerializeField]
		private Transform fingerColliderPointLocation;

		private HandController handController;

		private bool isHandAlwaysVisible;

		private float currentPoint = 1f;

		private float currentThumb;

		private float lerpSpeed = 23f;

		private HandState handState;

		private void Awake()
		{
		}

		private void Start()
		{
			handController = interactionHandController.HandController;
			a = handPoseModel.GetComponent<Animator>();
		}

		private void OnEnable()
		{
			InteractionHandController obj = interactionHandController;
			obj.OnGrabSuccess = (Action<InteractionHandController, GrabbableItem>)Delegate.Combine(obj.OnGrabSuccess, new Action<InteractionHandController, GrabbableItem>(GrabSuccess));
			InteractionHandController obj2 = interactionHandController;
			obj2.OnReleasedGrabbable = (Action<InteractionHandController, GrabbableItem>)Delegate.Combine(obj2.OnReleasedGrabbable, new Action<InteractionHandController, GrabbableItem>(GrabRelease));
			InteractionHandController obj3 = interactionHandController;
			obj3.OnInteractionDeactivation = (Action<InteractionHandController>)Delegate.Combine(obj3.OnInteractionDeactivation, new Action<InteractionHandController>(InteractionDeactivation));
			InteractionHandController obj4 = interactionHandController;
			obj4.OnInteractionReactivation = (Action<InteractionHandController>)Delegate.Combine(obj4.OnInteractionReactivation, new Action<InteractionHandController>(InteractionReactivation));
		}

		private void OnDisable()
		{
			InteractionHandController obj = interactionHandController;
			obj.OnGrabSuccess = (Action<InteractionHandController, GrabbableItem>)Delegate.Remove(obj.OnGrabSuccess, new Action<InteractionHandController, GrabbableItem>(GrabSuccess));
			InteractionHandController obj2 = interactionHandController;
			obj2.OnReleasedGrabbable = (Action<InteractionHandController, GrabbableItem>)Delegate.Remove(obj2.OnReleasedGrabbable, new Action<InteractionHandController, GrabbableItem>(GrabRelease));
			InteractionHandController obj3 = interactionHandController;
			obj3.OnInteractionDeactivation = (Action<InteractionHandController>)Delegate.Remove(obj3.OnInteractionDeactivation, new Action<InteractionHandController>(InteractionDeactivation));
			InteractionHandController obj4 = interactionHandController;
			obj4.OnInteractionReactivation = (Action<InteractionHandController>)Delegate.Remove(obj4.OnInteractionReactivation, new Action<InteractionHandController>(InteractionReactivation));
		}

		private void GrabSuccess(InteractionHandController interactableHandController, GrabbableItem grabbableItem)
		{
			if (!isHandAlwaysVisible)
			{
				handRenderer.enabled = false;
			}
		}

		private void GrabRelease(InteractionHandController interactableHandController, GrabbableItem grabbableItem)
		{
			handRenderer.enabled = true;
		}

		private void HandModeB()
		{
			float num = 0f;
			float b = 0f;
			float value = 1f;
			bool flag = true;
			num = handController.GetButtonRaw(HandController.HandControllerButton.GrabCustom);
			bool flag2 = handController.GetCustomHandControllerButtonToRealButton(HandController.HandControllerButton.GrabCustom) == HandController.HandControllerButton.Trigger;
			flag = !flag2 && ((num > 0.5f) ? true : false);
			if (IsThumbUp())
			{
				b = 1f;
			}
			currentPoint = Mathf.Lerp(b: IsPointerFlexed() ? Mathf.Clamp(1f - num, 0.1f, 0.7f) : (flag ? Mathf.Clamp(value, 0.1f, 1f) : ((!flag2) ? Mathf.Clamp(value, 0.1f, 0.7f) : 0.1f)), a: currentPoint, t: lerpSpeed * Time.deltaTime);
			currentThumb = Mathf.Lerp(currentThumb, b, lerpSpeed * Time.deltaTime);
			if (currentPoint > 0.5f && currentPoint < 0.8f && handState != HandState.Resting)
			{
				fingerCollider.transform.localPosition = fingerColliderRestLocation.localPosition;
				fingerCollider.transform.localRotation = fingerColliderRestLocation.localRotation;
				handState = HandState.Resting;
			}
			if (currentPoint < 0.5f && handState != HandState.Grabbing)
			{
				fingerCollider.transform.localPosition = fingerColliderGrabLocation.localPosition;
				fingerCollider.transform.localRotation = fingerColliderGrabLocation.localRotation;
				handState = HandState.Grabbing;
			}
			if (currentPoint > 0.8f && handState != 0)
			{
				fingerCollider.transform.localPosition = fingerColliderPointLocation.localPosition;
				fingerCollider.transform.localRotation = fingerColliderPointLocation.localRotation;
				handState = HandState.Pointing;
			}
			a.SetFloat("Grab", num);
			a.SetLayerWeight(1, currentPoint);
			a.SetFloat("Point", currentPoint);
			a.SetFloat("ThumbUp", currentThumb);
			a.SetLayerWeight(2, currentThumb);
		}

		private bool IsThumbUp()
		{
			return (VRPlatform.GetCurrVRPlatformType() == VRPlatformTypes.Oculus && !handController.GetButtonTouch(HandController.HandControllerButton.Joystick) && !handController.GetButtonTouch(HandController.HandControllerButton.AButton) && !handController.GetButtonTouch(HandController.HandControllerButton.BButton) && !handController.GetButtonTouch(HandController.HandControllerButton.XButton) && !handController.GetButtonTouch(HandController.HandControllerButton.YButton)) || (VRPlatform.GetCurrVRPlatformType() == VRPlatformTypes.SteamVR && !steamVRTouch_IndividualController.IsThumbTouching());
		}

		private bool IsPointerFlexed()
		{
			return (VRPlatform.GetCurrVRPlatformType() == VRPlatformTypes.Oculus && handController.GetButtonTouch(HandController.HandControllerButton.InteractCustom)) || (VRPlatform.GetCurrVRPlatformType() == VRPlatformTypes.SteamVR && steamVRTouch_IndividualController.IsTriggerTouching());
		}

		private void Update()
		{
			a.runtimeAnimatorController = animControllerB;
			HandModeB();
		}

		private void InteractionDeactivation(InteractionHandController controller)
		{
			Debug.Log("InteractionDeactivated");
			handRenderer.enabled = false;
		}

		private void InteractionReactivation(InteractionHandController controller)
		{
			Debug.Log("InteractionActivated");
			handRenderer.enabled = true;
		}
	}
}
