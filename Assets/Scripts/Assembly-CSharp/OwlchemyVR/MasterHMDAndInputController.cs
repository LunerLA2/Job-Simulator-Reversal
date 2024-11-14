using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OwlchemyVR
{
	public abstract class MasterHMDAndInputController : MonoBehaviour
	{
		protected Transform trackedHmdTransform;

		protected Transform trackedRightHandTransform;

		protected Transform trackedLeftHandTransform;

		private HeadController headController;

		protected InteractionHandController rightHandController;

		protected InteractionHandController leftHandController;

		[SerializeField]
		private Transform camTransform;

		[SerializeField]
		private GameObject headPrefab;

		[SerializeField]
		protected GameObject controllerPrefab;

		protected List<ControllerConfigurator> controllersConfigs = new List<ControllerConfigurator>();

		protected List<InteractionHandController> interactionHandControllers = new List<InteractionHandController>();

		public Action<int> OnAdditionalControllersAddedIndex;

		private int numOfControllers;

		protected bool hasHandednessBeenSet;

		protected bool isHMDAndInputReady;

		public abstract MonoBehaviour[] TrackingScripts { get; }

		public Transform TrackedHmdTransform
		{
			get
			{
				return trackedHmdTransform;
			}
		}

		public Transform TrackedRightHandTransform
		{
			get
			{
				return trackedRightHandTransform;
			}
		}

		public Transform TrackedLeftHandTransform
		{
			get
			{
				return trackedLeftHandTransform;
			}
		}

		public HeadController Head
		{
			get
			{
				return headController;
			}
		}

		public InteractionHandController RightHand
		{
			get
			{
				return rightHandController;
			}
		}

		public InteractionHandController LeftHand
		{
			get
			{
				return leftHandController;
			}
		}

		public Transform CamTransform
		{
			get
			{
				return camTransform;
			}
		}

		public List<InteractionHandController> InteractionHandControllers
		{
			get
			{
				return interactionHandControllers;
			}
		}

		public bool IsHMDAndInputReady
		{
			get
			{
				return isHMDAndInputReady;
			}
		}

		public bool GetIsAnyTrackpadButton()
		{
			for (int i = 0; i < interactionHandControllers.Count; i++)
			{
				if (interactionHandControllers[i].IsTrackPadButton())
				{
					return true;
				}
			}
			return false;
		}

		public virtual void Awake()
		{
			if (!(this is SteamVRHMDAndInputController))
			{
				SteamVR.enabled = false;
			}
			if (headPrefab != null)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(headPrefab);
				gameObject.transform.SetParent(camTransform);
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localRotation = Quaternion.identity;
				headController = gameObject.GetComponent<HeadController>();
			}
		}

		public virtual void Start()
		{
		}

		public InteractionHandController GetHand(Handedness handedness)
		{
			return (handedness != Handedness.Right) ? LeftHand : RightHand;
		}

		protected virtual void OnEnable()
		{
			SteamVR_Utils.Event.Listen("input_focus", OnInputFocusChange);
		}

		protected virtual void OnDisable()
		{
		}

		public virtual void Update()
		{
			if (Input.GetKeyDown(KeyCode.S))
			{
				SwapControllerVisualHandedness();
			}
			if (!hasHandednessBeenSet && numOfControllers == 2)
			{
				SetupCorrectHandedness();
			}
			if ((!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift)) || (!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl)) || !Input.GetKeyDown(KeyCode.H))
			{
				return;
			}
			for (int i = 0; i < interactionHandControllers.Count; i++)
			{
				if (interactionHandControllers[i] != null)
				{
					interactionHandControllers[i].ToggleForceHandToBeInvisible();
				}
			}
		}

		private void OnInputFocusChange(params object[] args)
		{
			bool flag = (bool)args[0];
			Debug.Log("Focus Change:" + flag);
			StartCoroutine(WaitUntilEndOfFrameThenPauseUnpauseGame(!flag));
		}

		private IEnumerator WaitUntilEndOfFrameThenPauseUnpauseGame(bool isToggledToPause)
		{
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
		}

		protected bool DoesControllerIndexExist(int index)
		{
			for (int i = 0; i < controllersConfigs.Count; i++)
			{
				if (controllersConfigs[i].SteamVR_TrackedObj.index == (SteamVR_TrackedObject.EIndex)index)
				{
					return true;
				}
			}
			return false;
		}

		protected void SetupController(int index)
		{
			if (controllersConfigs.Count < 2)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(controllerPrefab);
				gameObject.transform.parent = base.transform;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localRotation = Quaternion.identity;
				ControllerConfigurator component = gameObject.GetComponent<ControllerConfigurator>();
				component.Init(index);
				controllersConfigs.Add(component);
				InteractionHandController component2 = gameObject.GetComponent<InteractionHandController>();
				interactionHandControllers.Add(component2);
				numOfControllers++;
				SetupCorrectHandedness();
			}
			else
			{
				Debug.LogWarning("Tried to setup more than 2 controllers");
				if (OnAdditionalControllersAddedIndex != null)
				{
					OnAdditionalControllersAddedIndex(index);
				}
			}
		}

		public Transform SpawnDummyRightHand()
		{
			if (trackedRightHandTransform != null)
			{
				return trackedRightHandTransform;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(controllerPrefab);
			gameObject.transform.SetParent(base.transform);
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			rightHandController = gameObject.GetComponent<InteractionHandController>();
			trackedRightHandTransform = gameObject.transform;
			ControllerConfigurator component = gameObject.GetComponent<ControllerConfigurator>();
			component.SetHandedness(false);
			gameObject.name = "DummyRightHand";
			return gameObject.transform;
		}

		protected virtual void SetupCorrectHandedness()
		{
		}

		public void SetupRightAndLeftHandPointers()
		{
			if (rightHandController != null)
			{
				rightHandController.SetupOtherHand(leftHandController);
			}
			if (leftHandController != null)
			{
				leftHandController.SetupOtherHand(rightHandController);
			}
		}

		private void SwapControllerVisualHandedness()
		{
			if (numOfControllers == 2 || numOfControllers == 1)
			{
				for (int i = 0; i < controllersConfigs.Count; i++)
				{
					controllersConfigs[i].SetHandedness(!controllersConfigs[i].IsLeftHanded);
				}
				InteractionHandController interactionHandController = rightHandController;
				rightHandController = leftHandController;
				leftHandController = interactionHandController;
				Transform transform = trackedRightHandTransform;
				trackedRightHandTransform = trackedLeftHandTransform;
				trackedLeftHandTransform = transform;
			}
			else
			{
				Debug.LogWarning("Can not manually swap handedness either no controllers are connected or too many");
			}
		}

		public virtual void FadeScreenOut()
		{
		}

		public bool GetButtonDownEitherHand(HandController.HandControllerButton btn)
		{
			return (LeftHand != null && LeftHand.HandController.GetButtonDown(btn)) || (RightHand != null && RightHand.HandController.GetButtonDown(btn));
		}

		public bool GetButtonUpEitherHand(HandController.HandControllerButton btn)
		{
			return (LeftHand != null && LeftHand.HandController.GetButtonUp(btn)) || (RightHand != null && RightHand.HandController.GetButtonUp(btn));
		}

		public bool GetButtonEitherHand(HandController.HandControllerButton btn)
		{
			return (LeftHand != null && LeftHand.HandController.GetButton(btn)) || (RightHand != null && RightHand.HandController.GetButton(btn));
		}
	}
}
