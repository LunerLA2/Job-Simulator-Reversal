using System;
using System.Collections.Generic;
using UnityEngine;

namespace OwlchemyVR
{
	[RequireComponent(typeof(InteractionController))]
	public class InteractionHandController : MonoBehaviour
	{
		public delegate void OnHandVisualChange(bool isController);

		public const float GRAB_MAX_DISTANCE_FROM_GRAB_POINT = 0.12f;

		public const float OCULUS_TOUCH_GRAB_MIN_TRHESHOLD = 0.1f;

		public const float OCULUS_TOUCH_CLICK_MIN_TRHESHOLD = 0.3f;

		private const int numOfFramesRecordedThrowRelase = 4;

		private const float TIME_TO_REENABLE_HAND_COLLIDER = 0.1f;

		private const int HAPTICS_GRAB_PULSE_RATE_MCIRO_SEC = 500;

		private const float HAPTICS_GRAB_LENGTH_SECONDS = 0.02f;

		[SerializeField]
		private HandController handController;

		[SerializeField]
		private Transform cfjTransform;

		[SerializeField]
		private Transform grabPointTransform;

		[SerializeField]
		private GameObject handColliderGO;

		[SerializeField]
		private JointEvents jointEvents;

		[SerializeField]
		private Transform mountOrientation;

		[SerializeField]
		private HapticsController hapticsController;

		[SerializeField]
		private GameObject controllerVisualContainer;

		[SerializeField]
		private OnStayTriggerRecorder handColliderTestCapsuleTriggerRecorder;

		[SerializeField]
		private HandModelController handModelController;

		[SerializeField]
		private Collider[] allHandColliders;

		private ConfigurableJoint cfj;

		private GrabbableItem currClosestGrabblableItem;

		private float distanceFromCurrClosestModItem;

		private GrabbableItem currGrabbedItem;

		private Vector3 grabbedItemCurrVelocity = Vector3.zero;

		private Vector3 grabbedItemPrevPos = Vector3.zero;

		private QuickLookupManager quickLookupManager;

		private List<Vector3> velocities = new List<Vector3>();

		private List<Vector3> handVelocities = new List<Vector3>();

		private Vector3 angularVelocity;

		private float throwForceMultiplier = 1.2f;

		private float timeSinceReleasedItem;

		private Vector3 prevPos;

		private Quaternion prevRot;

		public Action<InteractionHandController, GrabbableItem> OnGrabSuccess;

		public Action<InteractionHandController> OnGrabFailure;

		public Action<InteractionHandController> OnReleasedFailure;

		public Action<InteractionHandController, GrabbableItem> OnReleasedGrabbable;

		public Action<InteractionHandController> OnInteractionDeactivation;

		public Action<InteractionHandController> OnInteractionReactivation;

		private InteractionController interactionController;

		private float distanceHandHasMovedSinceLastSuccessfulGrab;

		private bool isJointBreakLerpComplete;

		private float desiredBreakForce = 1500f;

		private float desiredBreakTorque = 1000f;

		private float desiredBreakForceNewGrabbingSystem = 40f;

		private float desiredBreakTorqueNewGrabbingSystem = 2000f;

		private float lerpedSpringForce;

		private float lerpedDamping;

		private float startingBreakForce = 50000f;

		private float startingBreakTorque = 50000f;

		private Quaternion snapToRotationOffsetFromMountPoint = Quaternion.identity;

		private bool isCurrentlySnappingToRotation;

		private Vector3 snapToPositionOffsetFromMountPoint = Vector3.zero;

		private bool isCurrentlySnappingToPosition;

		private bool isInteractionActive = true;

		[SerializeField]
		private OnStayTriggerRecorder handGrabbleTrigger;

		private Collider handGrabbleTriggerCollider;

		[SerializeField]
		private CollisionEvents handCollisionEventHandler;

		[SerializeField]
		private AnimationClip impactHapticAnimationClip;

		private bool hasDoneFailedGrab;

		private HapticInfoObject hapticHandObjectInteractionInfoObj;

		private bool newGrabbingSystem = true;

		private float baseMaximumJointForce = 500f;

		private float maximumJointForceVelocityMultiplier = 10000f;

		private Quaternion cfjInitLocalRotation;

		private List<Collider> currentCollidersInOtherHandToIgnore = new List<Collider>();

		private bool areCurrentCollidersInOtherHandIgnored;

		private Vector3 previousHandPositionForGrabbing = Vector3.zero;

		private InteractionHandController otherHandInteractionHandController;

		private bool platformControllersVisible;

		public HandController HandController
		{
			get
			{
				return handController;
			}
		}

		public HapticsController HapticsController
		{
			get
			{
				return hapticsController;
			}
		}

		public float DistanceFromCurrClosestModItem
		{
			get
			{
				return distanceFromCurrClosestModItem;
			}
		}

		public bool IsGrabbableCurrInHand
		{
			get
			{
				return currGrabbedItem != null;
			}
		}

		public GrabbableItem CurrGrabbedItem
		{
			get
			{
				return currGrabbedItem;
			}
		}

		public bool IsCurrClosestGrabbleItem
		{
			get
			{
				return currClosestGrabblableItem != null;
			}
		}

		public Transform MountOrientation
		{
			get
			{
				return mountOrientation;
			}
		}

		public Vector3 GrabbedItemCurrVelocity
		{
			get
			{
				return grabbedItemCurrVelocity;
			}
		}

		public event OnHandVisualChange handVisualsChangedEvent;

		public void Awake()
		{
			quickLookupManager = QuickLookupManager.Instance;
			handColliderTestCapsuleTriggerRecorder.enabled = false;
			prevPos = base.transform.position;
			MakeNewCFJ();
			interactionController = GetComponent<InteractionController>();
			hapticsController.Init(handController);
			float length = 0.02f;
			hapticHandObjectInteractionInfoObj = new HapticInfoObject(500f, length);
			hapticHandObjectInteractionInfoObj.SetAsPermanent();
			hapticHandObjectInteractionInfoObj.DeactiveHaptic();
			hapticsController.AddNewHaptic(hapticHandObjectInteractionInfoObj);
			handGrabbleTriggerCollider = handGrabbleTrigger.GetComponent<Collider>();
			if (VRPlatform.GetCurrVRPlatformType() == VRPlatformTypes.Oculus)
			{
				throwForceMultiplier *= 1.2f;
			}
			else if (VRPlatform.GetCurrVRPlatformHardwareType() == VRPlatformHardwareType.WindowsMR)
			{
				throwForceMultiplier *= 1.05f;
			}
		}

		private void OnLevelWasLoaded(int level)
		{
		}

		public void SetControllerVisual(bool isVisible)
		{
			platformControllersVisible = isVisible;
			controllerVisualContainer.SetActive(isVisible);
			if (this.handVisualsChangedEvent != null)
			{
				this.handVisualsChangedEvent(isVisible);
			}
		}

		private void OnEnable()
		{
			JointEvents obj = jointEvents;
			obj.OnJointBreakEvent = (Action<float>)Delegate.Combine(obj.OnJointBreakEvent, new Action<float>(JointBreakJointHasBrokenAndBeenDestroyed));
			InteractionController obj2 = interactionController;
			obj2.OnForceDeselect = (Action<InteractableItem>)Delegate.Combine(obj2.OnForceDeselect, new Action<InteractableItem>(ForceDeselect));
			CollisionEvents collisionEvents = handCollisionEventHandler;
			collisionEvents.OnEnterCollision = (Action<Collision>)Delegate.Combine(collisionEvents.OnEnterCollision, new Action<Collision>(HandCollisionImpact));
		}

		private void OnDisable()
		{
			JointEvents obj = jointEvents;
			obj.OnJointBreakEvent = (Action<float>)Delegate.Remove(obj.OnJointBreakEvent, new Action<float>(JointBreakJointHasBrokenAndBeenDestroyed));
			InteractionController obj2 = interactionController;
			obj2.OnForceDeselect = (Action<InteractableItem>)Delegate.Remove(obj2.OnForceDeselect, new Action<InteractableItem>(ForceDeselect));
			CollisionEvents collisionEvents = handCollisionEventHandler;
			collisionEvents.OnEnterCollision = (Action<Collision>)Delegate.Remove(collisionEvents.OnEnterCollision, new Action<Collision>(HandCollisionImpact));
		}

		public void SetupOtherHand(InteractionHandController otherHand)
		{
			otherHandInteractionHandController = otherHand;
		}

		private void Update()
		{
			if (!isInteractionActive)
			{
				hapticsController.HapticsManualUpdate();
				return;
			}
			if (IsGrabbableCurrInHand)
			{
				if (isCurrentlySnappingToRotation || isCurrentlySnappingToPosition)
				{
					CheckIfSnappingIsComplete();
				}
				if (IsGrabInputButtonUp())
				{
					ReleaseCurrGrabbable();
				}
				else if (!isJointBreakLerpComplete)
				{
					distanceHandHasMovedSinceLastSuccessfulGrab += Vector3.Distance(base.transform.position, previousHandPositionForGrabbing);
					float num = distanceHandHasMovedSinceLastSuccessfulGrab / 0.1f;
					if (newGrabbingSystem)
					{
						cfj.breakForce = Mathf.Lerp(startingBreakForce, desiredBreakForceNewGrabbingSystem * currGrabbedItem.BreakForceMultiplier, num);
						cfj.breakTorque = Mathf.Lerp(startingBreakTorque, desiredBreakTorqueNewGrabbingSystem, num);
					}
					else
					{
						cfj.breakForce = Mathf.Lerp(startingBreakForce, desiredBreakForce * currGrabbedItem.BreakForceMultiplier, num);
						cfj.breakTorque = Mathf.Lerp(startingBreakTorque, desiredBreakTorque, num);
					}
					if (num >= 1f)
					{
						isJointBreakLerpComplete = true;
					}
				}
			}
			if (!IsGrabbableCurrInHand)
			{
				GrabbableItem closestGrabbableItem = GetClosestGrabbableItem();
				if (closestGrabbableItem != currClosestGrabblableItem)
				{
					if (currClosestGrabblableItem != null)
					{
						currClosestGrabblableItem.InteractableItem.DeselectItem(interactionController);
					}
					currClosestGrabblableItem = closestGrabbableItem;
					if (currClosestGrabblableItem != null)
					{
						currClosestGrabblableItem.InteractableItem.SelectItem(interactionController);
					}
				}
				if (IsGrabInputButtonDown())
				{
					TryGrab();
				}
				if (hasDoneFailedGrab && !IsGrabInputButton())
				{
					ReleaseEmptyHand();
					hasDoneFailedGrab = false;
				}
			}
			if (currGrabbedItem == null)
			{
				if (!handColliderGO.activeSelf)
				{
					if (timeSinceReleasedItem > 0.1f)
					{
						if (!handColliderTestCapsuleTriggerRecorder.enabled)
						{
							handColliderTestCapsuleTriggerRecorder.enabled = true;
						}
						else if (IsHandColliderClearOfCollisions())
						{
							ToggleHandColliderGameObjectActiveState(true);
							handColliderTestCapsuleTriggerRecorder.enabled = false;
						}
					}
					else
					{
						timeSinceReleasedItem += Time.deltaTime;
					}
				}
			}
			else
			{
				timeSinceReleasedItem = 0f;
				ToggleHandColliderGameObjectActiveState(false);
			}
			if (currGrabbedItem != null)
			{
				currGrabbedItem.InHandUpdate();
				if (currGrabbedItem != null)
				{
					if (IsSqueezedButtonDown())
					{
						currGrabbedItem.StartUsing();
					}
					else if (IsSqueezedButtonUp())
					{
						currGrabbedItem.StopUsing();
					}
				}
			}
			handModelController.InRangeOfInteratableUpdate(currClosestGrabblableItem != null || currGrabbedItem != null);
			hapticsController.HapticsManualUpdate();
			previousHandPositionForGrabbing = base.transform.position;
		}

		public void TryGrab()
		{
			if (IsGrabbableCurrInHand || hasDoneFailedGrab)
			{
				return;
			}
			if (currClosestGrabblableItem != null)
			{
				GrabClosestModGrabbable();
				return;
			}
			hasDoneFailedGrab = true;
			if (OnGrabFailure != null)
			{
				OnGrabFailure(this);
			}
		}

		public void TryRelease(bool withForce = true)
		{
			if (IsGrabbableCurrInHand)
			{
				ReleaseCurrGrabbable(withForce);
			}
		}

		public void TryToggleGrabState()
		{
			if (IsGrabbableCurrInHand)
			{
				TryRelease();
			}
			else
			{
				TryGrab();
			}
		}

		public void StartUsingGrabbedItem()
		{
			if (currGrabbedItem != null)
			{
				currGrabbedItem.StartUsing();
			}
		}

		public void StopUsingGrabbedItem()
		{
			if (currGrabbedItem != null)
			{
				currGrabbedItem.StopUsing();
			}
		}

		private void LateUpdate()
		{
			CalculateVelocities();
			CalculateInHandItemVelocities();
			if (!newGrabbingSystem || !(currGrabbedItem != null))
			{
				return;
			}
			float magnitude = velocities[velocities.Count - 1].magnitude;
			if (magnitude > 1f)
			{
				if (cfj != null)
				{
					JointDrive xDrive = cfj.xDrive;
					xDrive.maximumForce = baseMaximumJointForce + maximumJointForceVelocityMultiplier;
					cfj.xDrive = xDrive;
					cfj.yDrive = xDrive;
					cfj.zDrive = xDrive;
				}
				else
				{
					Debug.LogWarning("Cfj should never be null here, but it might happen is some very weird cases");
				}
			}
		}

		private void CalculateInHandItemVelocities()
		{
			if (currGrabbedItem != null)
			{
				grabbedItemCurrVelocity = (currGrabbedItem.transform.position - grabbedItemPrevPos) / Time.deltaTime;
				grabbedItemPrevPos = currGrabbedItem.transform.position;
			}
			else
			{
				grabbedItemCurrVelocity = Vector3.zero;
			}
		}

		private void GrabClosestModGrabbable()
		{
			GrabGrabbable(currClosestGrabblableItem);
		}

		private void GrabGrabbable(GrabbableItem grabbableItem)
		{
			currGrabbedItem = grabbableItem;
			string eventDataValue = "null";
			if (grabbableItem.InteractableItem != null && grabbableItem.InteractableItem.WorldItemData != null)
			{
				eventDataValue = grabbableItem.InteractableItem.WorldItemData.ItemName;
			}
			AnalyticsManager.CustomEvent("Grab Item", "Item", eventDataValue);
			if (currGrabbedItem.IsCurrInHand)
			{
				currGrabbedItem.CurrInteractableHand.ReleaseCurrGrabbable(false, true);
			}
			currGrabbedItem.Grab(this);
			currClosestGrabblableItem = null;
			distanceHandHasMovedSinceLastSuccessfulGrab = 0f;
			isJointBreakLerpComplete = false;
			StartHapticBasicHandObjInteraction();
			grabbedItemPrevPos = currGrabbedItem.transform.position;
			if (OnGrabSuccess != null)
			{
				OnGrabSuccess(this, currGrabbedItem);
			}
			if (otherHandInteractionHandController != null)
			{
				otherHandInteractionHandController.PhysicsIgnoreGrabbable(currGrabbedItem);
			}
			handGrabbleTriggerCollider.enabled = false;
		}

		private bool GetIsDistanceOfGrabbableFromGrabPointValid(GrabbableItem item)
		{
			Vector3 b = item.Rigidbody.ClosestPointOnBounds(grabPointTransform.position);
			float num = Vector3.Distance(grabPointTransform.position, b);
			return num < 0.12f;
		}

		public bool IsGrabInputButtonDown()
		{
			if (handController.ControllerType == HandController.HandControllerType.SteamVRCompatibleController && VRPlatform.GetCurrVRPlatformHardwareType() == VRPlatformHardwareType.OculusRift && handController.SteamVRCompatibleController != null)
			{
				return handController.SteamVRCompatibleController.GetGrabAxisValue() > 0.1f && handController.SteamVRCompatibleController.GetGrabAxisValueLastFrame() < 0.1f;
			}
			return handController.GetButtonDown(HandController.HandControllerButton.GrabCustom);
		}

		public bool IsGrabInputButton()
		{
			if (handController.ControllerType == HandController.HandControllerType.SteamVRCompatibleController && VRPlatform.GetCurrVRPlatformHardwareType() == VRPlatformHardwareType.OculusRift && handController.SteamVRCompatibleController != null)
			{
				return handController.SteamVRCompatibleController.GetGrabAxisValue() > 0.1f;
			}
			return handController.GetButton(HandController.HandControllerButton.GrabCustom);
		}

		public bool IsGrabInputButtonUp()
		{
			if (handController.ControllerType == HandController.HandControllerType.SteamVRCompatibleController && VRPlatform.GetCurrVRPlatformHardwareType() == VRPlatformHardwareType.OculusRift && handController.SteamVRCompatibleController != null)
			{
				return handController.SteamVRCompatibleController.GetGrabAxisValue() < 0.1f && handController.SteamVRCompatibleController.GetGrabAxisValueLastFrame() > 0.1f;
			}
			return handController.GetButtonUp(HandController.HandControllerButton.GrabCustom);
		}

		public bool IsSqueezedButton()
		{
			if (handController.ControllerType == HandController.HandControllerType.SteamVRCompatibleController && VRPlatform.GetCurrVRPlatformHardwareType() == VRPlatformHardwareType.OculusRift && handController.SteamVRCompatibleController != null)
			{
				return handController.SteamVRCompatibleController.GetPointerFingerAxisValue() > 0.3f;
			}
			return handController.GetButton(HandController.HandControllerButton.InteractCustom);
		}

		public bool IsSqueezedButtonDown()
		{
			if (handController.ControllerType == HandController.HandControllerType.SteamVRCompatibleController && VRPlatform.GetCurrVRPlatformHardwareType() == VRPlatformHardwareType.OculusRift && handController.SteamVRCompatibleController != null)
			{
				return handController.SteamVRCompatibleController.GetPointerFingerAxisValue() > 0.3f && handController.SteamVRCompatibleController.GetPointerFingerAxisValueLastFrame() < 0.3f;
			}
			return handController.GetButtonDown(HandController.HandControllerButton.InteractCustom);
		}

		public bool IsSqueezedButtonUp()
		{
			if (handController.ControllerType == HandController.HandControllerType.SteamVRCompatibleController && VRPlatform.GetCurrVRPlatformHardwareType() == VRPlatformHardwareType.OculusRift && handController.SteamVRCompatibleController != null)
			{
				return handController.SteamVRCompatibleController.GetPointerFingerAxisValue() < 0.3f && handController.SteamVRCompatibleController.GetPointerFingerAxisValueLastFrame() > 0.3f;
			}
			return handController.GetButtonUp(HandController.HandControllerButton.InteractCustom);
		}

		public bool IsTrackPadTouched()
		{
			return handController.GetButtonTouch(HandController.HandControllerButton.Trackpad);
		}

		public bool IsTrackPadButton()
		{
			return handController.GetButton(HandController.HandControllerButton.Trackpad);
		}

		public bool IsTrackPadButtonDown()
		{
			return handController.GetButtonDown(HandController.HandControllerButton.Trackpad);
		}

		public bool IsTrackPadButtonUp()
		{
			return handController.GetButtonUp(HandController.HandControllerButton.Trackpad);
		}

		public Vector2 GetTrackPadVector2()
		{
			return handController.GetTrackPadPosPercentage();
		}

		private void CalculateVelocities()
		{
			Vector3 position;
			Vector3 item;
			if (currGrabbedItem != null)
			{
				position = base.transform.position;
				item = (position - prevPos) / Time.deltaTime;
				velocities.Add(item);
				if (velocities.Count > 4)
				{
					velocities.RemoveAt(0);
				}
				Quaternion rotation = currGrabbedItem.transform.rotation;
				Quaternion quaternion = rotation * Quaternion.Inverse(prevRot);
				float num = 2f * Mathf.Acos(quaternion.w);
				float x = quaternion.x / Mathf.Sqrt(1f - quaternion.w * quaternion.w);
				float y = quaternion.y / Mathf.Sqrt(1f - quaternion.w * quaternion.w);
				float z = quaternion.z / Mathf.Sqrt(1f - quaternion.w * quaternion.w);
				angularVelocity = new Vector3(x, y, z) * num * (1f / Time.deltaTime);
				prevRot = currGrabbedItem.transform.rotation;
			}
			position = base.transform.position;
			item = (position - prevPos) / Time.deltaTime;
			handVelocities.Add(item);
			if (handVelocities.Count > 4)
			{
				handVelocities.RemoveAt(0);
			}
			prevPos = base.transform.position;
		}

		private void JointBreakJointHasBrokenAndBeenDestroyed(float breakForce)
		{
			cfj = null;
			ReleaseCurrGrabbable();
			MakeNewCFJ();
		}

		public void ManuallyReleaseJoint()
		{
			if (cfj == null)
			{
				MakeNewCFJ();
			}
			SetConfigurableJointSettings(true, null);
			ReleaseCurrGrabbable();
		}

		public void ReleaseCurrGrabbableTempPrepareForReattachOnSameFrame()
		{
			SetConfigurableJointSettings(true, null);
			ReleaseCurrGrabbable(false);
		}

		public void ReGrabGrabbable(GrabbableItem grabbableItem)
		{
			GrabGrabbable(grabbableItem);
		}

		private void ReleaseCurrGrabbable(bool withForce = true, bool wasSwappedBetweenHands = false)
		{
			bool flag = true;
			if (currGrabbedItem != null)
			{
				Vector3 applyVelocity = Vector3.zero;
				Vector3 applyAngVelocity = Vector3.zero;
				if (withForce)
				{
					applyVelocity = CalcCurrentObjectReleaseForce();
					applyAngVelocity = CalcCurrentObjectReleaseAngularForce();
					if (VRPlatform.GetCurrVRPlatformHardwareType() == VRPlatformHardwareType.WindowsMR)
					{
						applyAngVelocity *= 0.8f;
					}
				}
				flag = currGrabbedItem.Release(this, applyVelocity, applyAngVelocity, wasSwappedBetweenHands);
			}
			if (otherHandInteractionHandController != null)
			{
				otherHandInteractionHandController.UnIgnoreOtherHandGrabbleCollidersAndClear();
			}
			if (flag)
			{
				isCurrentlySnappingToPosition = false;
				isCurrentlySnappingToRotation = false;
				if (OnReleasedGrabbable != null)
				{
					OnReleasedGrabbable(this, currGrabbedItem);
				}
				currGrabbedItem = null;
				StartHapticBasicHandObjInteraction();
				handGrabbleTriggerCollider.enabled = true;
			}
		}

		private void ReleaseEmptyHand()
		{
			if (OnReleasedFailure != null)
			{
				OnReleasedFailure(this);
			}
		}

		private void MakeNewCFJ()
		{
			cfj = cfjTransform.gameObject.AddComponent<ConfigurableJoint>();
			cfjInitLocalRotation = cfj.transform.localRotation;
			SetConfigurableJointSettings(true, null);
		}

		private void SetConfigurableJointSettings(bool setStatingBreakForce, GrabbableItem grabbableItem)
		{
			if (newGrabbingSystem)
			{
				SetConfigurableJointSettingsNew(setStatingBreakForce, grabbableItem);
			}
		}

		private void SetConfigurableJointSettingsNew(bool setStatingBreakForce, GrabbableItem grabbableItem)
		{
			if (setStatingBreakForce)
			{
				cfj.breakForce = startingBreakForce;
				cfj.breakTorque = startingBreakTorque;
			}
			if (isCurrentlySnappingToRotation)
			{
				cfj.angularXMotion = ConfigurableJointMotion.Free;
				cfj.angularYMotion = ConfigurableJointMotion.Free;
				cfj.angularZMotion = ConfigurableJointMotion.Free;
				JointDrive jointDrive = default(JointDrive);
				jointDrive.positionSpring = 100f;
				jointDrive.positionDamper = 0f;
				jointDrive.maximumForce = 20f;
				cfj.angularXDrive = jointDrive;
				cfj.angularYZDrive = jointDrive;
			}
			else
			{
				cfj.angularXMotion = ConfigurableJointMotion.Locked;
				cfj.angularYMotion = ConfigurableJointMotion.Locked;
				cfj.angularZMotion = ConfigurableJointMotion.Locked;
				JointDrive jointDrive2 = default(JointDrive);
				cfj.angularXDrive = jointDrive2;
				cfj.angularYZDrive = jointDrive2;
			}
			if (isCurrentlySnappingToPosition)
			{
				cfj.autoConfigureConnectedAnchor = false;
				cfj.anchor = cfj.transform.InverseTransformPoint(MountOrientation.position);
				cfj.connectedAnchor = snapToPositionOffsetFromMountPoint;
				cfj.xMotion = ConfigurableJointMotion.Free;
				cfj.yMotion = ConfigurableJointMotion.Free;
				cfj.zMotion = ConfigurableJointMotion.Free;
				JointDrive jointDrive3 = default(JointDrive);
				jointDrive3.positionSpring = 2500f;
				jointDrive3.positionDamper = 100f;
				jointDrive3.maximumForce = 100f;
				cfj.xDrive = jointDrive3;
				cfj.yDrive = jointDrive3;
				cfj.zDrive = jointDrive3;
			}
			else
			{
				cfj.anchor = Vector3.zero;
				cfj.autoConfigureConnectedAnchor = false;
				if (currGrabbedItem != null)
				{
					Vector3 vector = cfj.transform.InverseTransformPoint(handGrabbleTrigger.transform.position);
					cfj.anchor = vector;
					cfj.connectedAnchor = currGrabbedItem.Rigidbody.transform.InverseTransformPoint(cfj.transform.TransformPoint(vector));
				}
				else
				{
					cfj.connectedAnchor = Vector3.zero;
				}
				cfj.xMotion = ConfigurableJointMotion.Limited;
				cfj.yMotion = ConfigurableJointMotion.Limited;
				cfj.zMotion = ConfigurableJointMotion.Limited;
				SoftJointLimit linearLimit = default(SoftJointLimit);
				linearLimit.limit = 0.14f;
				linearLimit.contactDistance = 0.1f;
				linearLimit.bounciness = 10f;
				cfj.linearLimit = linearLimit;
				JointDrive jointDrive4 = default(JointDrive);
				jointDrive4.positionSpring = 1000000f;
				jointDrive4.positionDamper = 0f;
				jointDrive4.maximumForce = 500f;
				cfj.xDrive = jointDrive4;
				cfj.yDrive = jointDrive4;
				cfj.zDrive = jointDrive4;
			}
			if (isCurrentlySnappingToRotation)
			{
				Quaternion targetLocalRotation = Quaternion.Inverse(cfj.transform.rotation) * (currGrabbedItem.transform.rotation * snapToRotationOffsetFromMountPoint);
				Quaternion rotation = Quaternion.Inverse(cfj.transform.rotation) * MountOrientation.transform.rotation;
				targetLocalRotation *= Quaternion.Inverse(rotation);
				cfj.SetTargetRotationLocal(targetLocalRotation, cfjInitLocalRotation);
			}
			else
			{
				cfj.targetRotation = Quaternion.identity;
			}
			cfj.projectionMode = JointProjectionMode.PositionAndRotation;
			cfj.projectionDistance = 10f;
			cfj.projectionAngle = 180f;
		}

		public void AddConnectedBody(Rigidbody body, GrabbableItem grabbableItem)
		{
			SetConfigurableJointSettings(true, grabbableItem);
			cfj.connectedBody = body;
		}

		public void RemoveConnectedBody()
		{
			if (cfj != null)
			{
				cfj.connectedBody = null;
			}
		}

		private Vector3 CalcCurrentObjectReleaseForce()
		{
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			for (int i = 0; i < velocities.Count; i++)
			{
				Vector3 vector = velocities[i];
				num += vector.x;
				num2 += vector.y;
				num3 += vector.z;
			}
			Vector3 vector2 = new Vector3(num / (float)velocities.Count, num2 / (float)velocities.Count, num3 / (float)velocities.Count);
			return vector2 * throwForceMultiplier;
		}

		private Vector3 CalcCurrentObjectReleaseAngularForce()
		{
			if (!float.IsNaN(angularVelocity.x) && !float.IsNaN(angularVelocity.y) && !float.IsNaN(angularVelocity.z))
			{
				return angularVelocity;
			}
			return Vector3.zero;
		}

		public Vector3 GetRawHandVelocity()
		{
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			for (int i = 0; i < handVelocities.Count; i++)
			{
				Vector3 vector = handVelocities[i];
				num += vector.x;
				num2 += vector.y;
				num3 += vector.z;
			}
			return new Vector3(num / (float)handVelocities.Count, num2 / (float)handVelocities.Count, num3 / (float)handVelocities.Count);
		}

		public Vector3 GetCurrentVelocity()
		{
			return CalcCurrentObjectReleaseForce();
		}

		public Vector3 GetGrabPointPos()
		{
			return grabPointTransform.position;
		}

		public Transform GetGrabPointTransform()
		{
			return grabPointTransform;
		}

		private GrabbableItem GetClosestGrabbableItem()
		{
			GrabbableItem result = null;
			List<Collider> collidersWithinTrigger = handGrabbleTrigger.GetCollidersWithinTrigger();
			float num = float.PositiveInfinity;
			if (collidersWithinTrigger.Count > 0)
			{
				GrabbableItem grabbableItem = null;
				int num2 = -1;
				for (int i = 0; i < collidersWithinTrigger.Count; i++)
				{
					grabbableItem = null;
					Collider collider = collidersWithinTrigger[i];
					num2 = collider.GetInstanceID();
					ColliderGrabbableItemPointer colliderGrabbableItemPointer;
					if (quickLookupManager.HasPointerKeyForColliderInstanceID(num2))
					{
						colliderGrabbableItemPointer = quickLookupManager.GetPointerFromColliderInstanceID(num2);
					}
					else
					{
						colliderGrabbableItemPointer = collider.GetComponent<ColliderGrabbableItemPointer>();
						quickLookupManager.SetPointerForColliderInstanceID(colliderGrabbableItemPointer, num2);
					}
					if (colliderGrabbableItemPointer != null)
					{
						grabbableItem = colliderGrabbableItemPointer.GrabbableItem;
					}
					else
					{
						Rigidbody attachedRigidbody = collider.attachedRigidbody;
						if (attachedRigidbody != null)
						{
							grabbableItem = attachedRigidbody.GetComponent<GrabbableItem>();
						}
					}
					if (!(grabbableItem != null) || !grabbableItem.enabled)
					{
						continue;
					}
					float maxDistance = 100f;
					float num3 = float.PositiveInfinity;
					float b = float.PositiveInfinity;
					RaycastHit hitInfo;
					if (collider.Raycast(new Ray(grabPointTransform.position, (collider.bounds.center - grabPointTransform.position).normalized), out hitInfo, maxDistance))
					{
						num3 = hitInfo.distance;
						if (collider.Raycast(new Ray(grabPointTransform.position, grabPointTransform.forward), out hitInfo, maxDistance))
						{
							b = hitInfo.distance * 0.5f;
						}
					}
					else
					{
						num3 = 0f;
					}
					float num4 = Mathf.Min(num3, b);
					if (num4 < num)
					{
						result = grabbableItem;
						num = num4;
						if (num4 <= 0f && !(num <= 0f))
						{
						}
					}
				}
			}
			return result;
		}

		private float GetDistanceFromColliderUsingWorldPointForward(Collider c, Transform pointTransform)
		{
			return GetDistanceFromColliderToPointUsingDirection(c, pointTransform.position, pointTransform.forward);
		}

		private float GetDistanceFromColliderToPointUsingColliderCenter(Collider c, Vector3 worldPoint)
		{
			Vector3 normalized = (c.bounds.center - worldPoint).normalized;
			return GetDistanceFromColliderToPointUsingDirection(c, worldPoint, normalized);
		}

		private float GetDistanceFromColliderToPointUsingDirection(Collider c, Vector3 worldPoint, Vector3 dir)
		{
			float result = 0f;
			RaycastHit hitInfo;
			if (c.Raycast(new Ray(worldPoint, dir), out hitInfo, 100f))
			{
				result = hitInfo.distance;
			}
			return result;
		}

		public void ForceDeselect(InteractableItem item)
		{
			if (currClosestGrabblableItem.InteractableItem == item)
			{
				currClosestGrabblableItem = null;
			}
			else
			{
				Debug.LogWarning("Could not force deselect as it was not selected in the first place");
			}
		}

		private bool IsHandColliderClearOfCollisions()
		{
			return handColliderTestCapsuleTriggerRecorder.GetCollidersWithinTrigger().Count == 0;
		}

		public void SnapToPosition(Vector3 posOffsetFromMountPoint)
		{
			PickupableItem pickupableItem = currGrabbedItem as PickupableItem;
			if (pickupableItem != null)
			{
				if (!isCurrentlySnappingToPosition)
				{
					isCurrentlySnappingToPosition = true;
					snapToPositionOffsetFromMountPoint = posOffsetFromMountPoint;
				}
				else
				{
					Debug.LogError("Can not snap to position when already snapping to position");
				}
			}
			else
			{
				Debug.LogError("Can not snap non-pickupable to position");
			}
		}

		public void SnapToRotation(Quaternion rotOffsetFromMountPoint)
		{
			PickupableItem pickupableItem = currGrabbedItem as PickupableItem;
			if (pickupableItem != null)
			{
				if (!isCurrentlySnappingToRotation)
				{
					isCurrentlySnappingToRotation = true;
					snapToRotationOffsetFromMountPoint = rotOffsetFromMountPoint;
				}
				else
				{
					Debug.LogError("Can not snap to rotation when already snapping to rotation");
				}
			}
			else
			{
				Debug.LogError("Can not snap non-pickupable to rotation");
			}
		}

		private void CheckIfSnappingIsComplete()
		{
			bool flag = !isCurrentlySnappingToPosition;
			bool flag2 = !isCurrentlySnappingToRotation;
			Vector3 vector = Vector3.zero;
			Quaternion quaternion = Quaternion.identity;
			if (isCurrentlySnappingToPosition)
			{
				vector = MountOrientation.TransformPoint(-snapToPositionOffsetFromMountPoint);
				float num = Vector3.Distance(currGrabbedItem.transform.position, vector);
				if (num < 0.1f)
				{
					flag = true;
				}
			}
			if (isCurrentlySnappingToRotation)
			{
				quaternion = MountOrientation.rotation * Quaternion.Inverse(snapToRotationOffsetFromMountPoint);
				float num2 = Quaternion.Angle(currGrabbedItem.transform.rotation, quaternion);
				if (num2 < 1f)
				{
					flag2 = true;
				}
			}
			if (!flag || !flag2)
			{
				return;
			}
			cfj.connectedBody = null;
			if (currGrabbedItem != null)
			{
				if (isCurrentlySnappingToPosition)
				{
					currGrabbedItem.transform.position = vector;
					currGrabbedItem.Rigidbody.position = vector;
				}
				if (isCurrentlySnappingToRotation)
				{
					currGrabbedItem.transform.rotation = quaternion;
					currGrabbedItem.Rigidbody.rotation = quaternion;
				}
			}
			isCurrentlySnappingToPosition = false;
			isCurrentlySnappingToRotation = false;
			SetConfigurableJointSettings(false, currGrabbedItem);
			if (currGrabbedItem != null)
			{
				cfj.connectedBody = currGrabbedItem.Rigidbody;
			}
		}

		public void ReorientCurrItemInHand(Vector3 position, Quaternion rotation)
		{
			if (currGrabbedItem != null)
			{
				cfj.connectedBody = null;
				currGrabbedItem.transform.position = position;
				currGrabbedItem.Rigidbody.position = position;
				currGrabbedItem.transform.rotation = rotation;
				currGrabbedItem.Rigidbody.rotation = rotation;
				SetConfigurableJointSettings(false, currGrabbedItem);
				cfj.connectedBody = currGrabbedItem.Rigidbody;
			}
		}

		private void ToggleHandColliderGameObjectActiveState(bool isActive)
		{
			if (isActive != handColliderGO.activeSelf)
			{
				handColliderGO.SetActive(isActive);
				if (isActive)
				{
					ReIgnoreOtherHandGrabbableColliders();
				}
				else
				{
					UnIgnoreOtherHandGrabbleColliders();
				}
			}
		}

		public void DeactivateHandInteractions()
		{
			if (!isInteractionActive)
			{
				Debug.LogWarning("DeactivateHandInteractions called more than once.");
			}
			if (currGrabbedItem != null)
			{
				ReleaseCurrGrabbable(false);
			}
			ToggleHandColliderGameObjectActiveState(false);
			isInteractionActive = false;
			if (OnInteractionDeactivation != null)
			{
				OnInteractionDeactivation(this);
			}
		}

		public void ReactivateHandInteractions()
		{
			isInteractionActive = true;
			if (OnInteractionReactivation != null)
			{
				OnInteractionReactivation(this);
			}
		}

		private void HandCollisionImpact(Collision collisionInfo)
		{
		}

		private void StartHapticBasicHandObjInteraction()
		{
			hapticHandObjectInteractionInfoObj.Restart();
		}

		public void PhysicsIgnoreGrabbableInTheOtherHand()
		{
			if (otherHandInteractionHandController != null)
			{
				otherHandInteractionHandController.PhysicsIgnoreGrabbable(currGrabbedItem);
			}
		}

		public void PhysicsIgnoreGrabbable(GrabbableItem grabbableItem)
		{
			BuildCollidersInOtherHandToIgnoreList(grabbableItem);
			if (handColliderGO.activeSelf)
			{
				TogglePhysicsIgnoreOtherHandGrabbableColliders(true);
			}
			areCurrentCollidersInOtherHandIgnored = true;
		}

		public void ReIgnoreOtherHandGrabbableColliders()
		{
			if (areCurrentCollidersInOtherHandIgnored)
			{
				TogglePhysicsIgnoreOtherHandGrabbableColliders(true);
			}
		}

		public void BuildCollidersInOtherHandToIgnoreList(GrabbableItem grabbableItem)
		{
			if (currentCollidersInOtherHandToIgnore.Count > 0)
			{
				Debug.LogWarning("Should never have any currently ignored colliders when trying to ignore more of them");
			}
			if (areCurrentCollidersInOtherHandIgnored)
			{
				Debug.LogWarning("Flag set to other colliders are currently ignore is true, when building a list of new colliders to ignore");
			}
			Collider[] componentsInChildren = grabbableItem.GetComponentsInChildren<Collider>();
			Rigidbody rigidbody = grabbableItem.Rigidbody;
			if (rigidbody != null)
			{
				foreach (Collider collider in componentsInChildren)
				{
					if (!collider.isTrigger && collider.gameObject.layer != 21 && collider.attachedRigidbody == rigidbody)
					{
						currentCollidersInOtherHandToIgnore.Add(collider);
					}
				}
			}
			else
			{
				Debug.LogWarning("Could not ignore grabbable colliders with hand because GrabbableItem did not have a rigidbody", grabbableItem.gameObject);
			}
		}

		public void UnIgnoreOtherHandGrabbleCollidersAndClearInOtherHand()
		{
			if (otherHandInteractionHandController != null)
			{
				otherHandInteractionHandController.UnIgnoreOtherHandGrabbleCollidersAndClear();
			}
		}

		public void UnIgnoreOtherHandGrabbleCollidersAndClear()
		{
			UnIgnoreOtherHandGrabbleColliders();
			currentCollidersInOtherHandToIgnore.Clear();
			areCurrentCollidersInOtherHandIgnored = false;
		}

		public void UnIgnoreOtherHandGrabbleColliders()
		{
			if (areCurrentCollidersInOtherHandIgnored && handColliderGO.activeSelf)
			{
				TogglePhysicsIgnoreOtherHandGrabbableColliders(false);
			}
		}

		public void TogglePhysicsIgnoreOtherHandGrabbableColliders(bool isIgnore)
		{
			if (currentCollidersInOtherHandToIgnore.Count <= 0)
			{
				return;
			}
			for (int i = 0; i < allHandColliders.Length; i++)
			{
				for (int j = 0; j < currentCollidersInOtherHandToIgnore.Count; j++)
				{
					Physics.IgnoreCollision(allHandColliders[i], currentCollidersInOtherHandToIgnore[j], isIgnore);
				}
			}
		}

		public void ForceHandToBeInvisible()
		{
			handModelController.ForceHandToBeInvisible();
		}

		public void StopForcingHandToBeInvisible()
		{
			handModelController.StopForcingHandToBeInvisible();
		}

		public void ToggleForceHandToBeInvisible()
		{
			handModelController.ToggleForcingHandToBeInvisible();
		}
	}
}
