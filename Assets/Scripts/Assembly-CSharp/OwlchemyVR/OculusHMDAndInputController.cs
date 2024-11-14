using System.Collections;
using UnityEngine;

namespace OwlchemyVR
{
	public class OculusHMDAndInputController : MasterHMDAndInputController
	{
		private const string PREF_KEY_OCULUSHANDTYPEA = "oculusHandTypeA";

		[SerializeField]
		private OVRCameraRig ovrCameraRig;

		[SerializeField]
		private OVRScreenFade ovrScreenFade;

		[SerializeField]
		private OculusWorldMover oculusWorldMover;

		private float prevTimeScale = 1f;

		private bool isPaused;

		private bool isInFocus = true;

		private bool isMounted = true;

		private float trackerAnchorPrevPosX;

		private float trackerAnchorPrevPosZ;

		private bool hasTrackerAnchorPrevBeenSet;

		private bool isWaitingOnRecenterCorrection;

		[SerializeField]
		private Transform ovrCenterEyeTransform;

		public bool IsPaused
		{
			get
			{
				return isPaused;
			}
		}

		public OVRScreenFade OvrScreenFade
		{
			get
			{
				return ovrScreenFade;
			}
		}

		public override MonoBehaviour[] TrackingScripts
		{
			get
			{
				return null;
			}
		}

		public Transform OvrCenterEyeTransform
		{
			get
			{
				return ovrCenterEyeTransform;
			}
		}

		public override void Start()
		{
			OVRManager.VrFocusLost += OVRManager_VrFocusLost;
			OVRManager.VrFocusAcquired += OVRManager_VrFocusAcquired;
			OVRManager.HMDMounted += OVRManager_HMDMounted;
			OVRManager.HMDUnmounted += OVRManager_HMDUnmounted;
			trackedHmdTransform = Camera.main.transform;
			AnalyticsManager.CustomEvent("Headset Connected", "Connected", OVRPlugin.hmdPresent);
			SetupOculusHandControllers();
			ovrCameraRig.UpdatedAnchors += UpdateAnchorsTransform;
		}

		private void OVRManager_HMDMounted()
		{
			Debug.Log("HMD Mounted");
			isMounted = true;
			TrySetIsPaused(false);
		}

		private void OVRManager_HMDUnmounted()
		{
			Debug.Log("HMD Unmounted");
			isMounted = false;
			TrySetIsPaused(true);
		}

		private void OVRManager_VrFocusAcquired()
		{
			Debug.Log("VR Focus Gained");
			isInFocus = true;
			TrySetIsPaused(false);
		}

		private void OVRManager_VrFocusLost()
		{
			Debug.Log("VR Focus Lost");
			isInFocus = false;
			TrySetIsPaused(true);
		}

		protected override void OnEnable()
		{
			OVRManager.display.RecenteredPose += PoseRecentered;
		}

		protected override void OnDisable()
		{
			OVRManager.display.RecenteredPose -= PoseRecentered;
		}

		private void UpdateAnchorsTransform(OVRCameraRig camRig)
		{
			StartCoroutine(WaitOneFrameToUpdateTrackerAnchorPosition());
		}

		private IEnumerator WaitOneFrameToUpdateTrackerAnchorPosition()
		{
			yield return null;
			ovrCameraRig.UpdatedAnchors -= UpdateAnchorsTransform;
			hasTrackerAnchorPrevBeenSet = true;
			Vector3 trackerAnchorPos = ovrCameraRig.trackerAnchor.position;
			trackerAnchorPrevPosX = trackerAnchorPos.x;
			trackerAnchorPrevPosZ = trackerAnchorPos.z;
		}

		private void PoseRecentered()
		{
			Debug.Log("Pose Recentered called, we now need to autocorret for that");
			if (hasTrackerAnchorPrevBeenSet)
			{
				if (oculusWorldMover.IsGuardianSetup)
				{
					if (!isWaitingOnRecenterCorrection)
					{
						isWaitingOnRecenterCorrection = true;
						ovrCameraRig.UpdatedAnchors += ForceRecenterCorrection;
					}
				}
				else
				{
					Debug.Log("Do not modify recenter because guardian has not been setup");
				}
			}
			else
			{
				Debug.LogWarning("Unable to handle pos recenter event because initial tracker state was not set");
			}
		}

		private void ForceRecenterCorrection(OVRCameraRig camRig)
		{
			ovrCameraRig.UpdatedAnchors -= ForceRecenterCorrection;
			StartCoroutine(WaitAndCorrectRecenter());
		}

		private IEnumerator WaitAndCorrectRecenter()
		{
			yield return null;
			isWaitingOnRecenterCorrection = false;
			Vector3 trackerAnchorPos2 = ovrCameraRig.trackerAnchor.position;
			Debug.Log("PrevX:" + trackerAnchorPrevPosX + ", PrevZ:" + trackerAnchorPrevPosZ);
			Debug.Log("Current Tracker Anchor Pos:" + trackerAnchorPos2.ToStringPrecise());
			Vector3 offsetDelta = new Vector3(trackerAnchorPos2.x - trackerAnchorPrevPosX, 0f, trackerAnchorPos2.z - trackerAnchorPrevPosZ);
			trackerAnchorPos2 = ovrCameraRig.trackerAnchor.position;
			trackerAnchorPrevPosX = trackerAnchorPos2.x;
			trackerAnchorPrevPosZ = trackerAnchorPos2.z;
			Debug.Log("OffsetDelta to fix:" + offsetDelta.ToStringPrecise());
		}

		private void SetupOculusHandControllers()
		{
			Debug.Log("setup Oculus hand controllers now");
			Transform transform = Object.Instantiate(controllerPrefab).transform;
			Transform transform2 = Object.Instantiate(controllerPrefab).transform;
			transform.position = Vector3.zero;
			transform.rotation = Quaternion.identity;
			transform2.position = Vector3.zero;
			transform2.rotation = Quaternion.identity;
			transform.SetParent(ovrCameraRig.rightHandAnchor, false);
			transform2.SetParent(ovrCameraRig.leftHandAnchor, false);
			ControllerConfigurator component = transform.GetComponent<ControllerConfigurator>();
			controllersConfigs.Add(component);
			ControllerConfigurator component2 = transform2.GetComponent<ControllerConfigurator>();
			controllersConfigs.Add(component2);
			component.SetHandedness(false);
			component2.SetHandedness(true);
			component2.GetComponent<OculusTouch_IndividualController>().Setup(true);
			component.GetComponent<OculusTouch_IndividualController>().Setup(false);
			leftHandController = component2.GetComponent<InteractionHandController>();
			rightHandController = component.GetComponent<InteractionHandController>();
			SetupRightAndLeftHandPointers();
			isHMDAndInputReady = true;
		}

		public override void Update()
		{
			base.Update();
		}

		public override void FadeScreenOut()
		{
			base.FadeScreenOut();
			ScreenFader.Instance.FadeOut(1f);
			Debug.Log("Screen Fader commented out");
		}

		private void TrySetIsPaused(bool value)
		{
			if (isPaused == value)
			{
				Debug.Log("isPaused is already in sync with this request to set it to " + value);
				return;
			}
			if (value)
			{
				Debug.Log("starting pause coroutine; trying to pause the game...");
				StartCoroutine(WaitUntilEndOfFrameThenPauseUnpauseGame(true));
				return;
			}
			if (isMounted && isInFocus)
			{
				Debug.Log("starting pause coroutine; trying to UNpause the game...");
				StartCoroutine(WaitUntilEndOfFrameThenPauseUnpauseGame(false));
				return;
			}
			Debug.Log("TrySetPause is doing nothing! This is the state: value = " + value + " | isMounted = " + isMounted + " | isInFocus = " + isInFocus);
		}

		private IEnumerator WaitUntilEndOfFrameThenPauseUnpauseGame(bool isToggledToPause)
		{
			if (isPaused == isToggledToPause)
			{
				Debug.Log("pause coroutine quit because the state is already correct");
				yield break;
			}
			isPaused = isToggledToPause;
			Debug.Log("Pause routine is running now, setting pause to: " + isToggledToPause);
			if (isToggledToPause)
			{
				prevTimeScale = Time.timeScale;
				Time.timeScale = 0f;
			}
			else
			{
				GlobalStorage.Instance.MenuBriefcase.GetComponent<MenuController>().MenuPreviewer.StopPreviewing(false);
			}
			yield return new WaitForEndOfFrame();
			if (isToggledToPause)
			{
				AudioManager.Instance.PauseAllSounds();
			}
			else
			{
				AudioManager.Instance.UnPauseAllSounds();
			}
			for (int i = 0; i < interactionHandControllers.Count; i++)
			{
				if (interactionHandControllers[i] != null)
				{
					interactionHandControllers[i].HapticsController.SetIsPaused(isToggledToPause);
					if (isToggledToPause)
					{
						interactionHandControllers[i].ForceHandToBeInvisible();
					}
					else
					{
						interactionHandControllers[i].StopForcingHandToBeInvisible();
					}
				}
			}
			if (!isToggledToPause)
			{
				Time.timeScale = prevTimeScale;
			}
		}
	}
}
