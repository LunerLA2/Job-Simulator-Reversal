using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using OwlchemyVR;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
	[SerializeField]
	[Tooltip("Objects that should enable/disable when opening/closing the menu")]
	private List<GameObject> objectsToAppear;

	[SerializeField]
	private List<BusinessCardInfo> businessCardInfos;

	[SerializeField]
	private AttachablePoint businessCardAttachPoint;

	[SerializeField]
	private ParticleSystem poofParticle;

	[SerializeField]
	[Tooltip("The minimum distance from the starting location before an object will reset based on collisions")]
	private float minPoofDistance = 0.3f;

	[SerializeField]
	private AudioClip poofClip;

	private GameObject cameraObject;

	private PickupableItem cameraGrabbable;

	private AttachableObject cameraAttachableObject;

	private TriggerListener cameraTrigger;

	[SerializeField]
	private AttachablePoint cameraAttachPoint;

	private Rigidbody cameraRigidbody;

	private ParticleSystem cameraPoofParticle;

	[SerializeField]
	private ParticleSystem cameraPrevPositionParticle;

	[SerializeField]
	private Rigidbody rb;

	[SerializeField]
	[Tooltip("The hinge on the briefcase, which should be shut")]
	private GrabbableHinge grabbableHinge;

	[SerializeField]
	[Header("Exit Edible")]
	private EdibleItem exitEdible;

	[Tooltip("The trigger listener to determine when to 'poof' back to the initial position")]
	[SerializeField]
	private TriggerListener exitEdibleTrigger;

	private Vector3 exitEdibleLocation;

	private Quaternion exitEdibleRotation;

	[SerializeField]
	private PickupableItem exitEdiblePickupable;

	private Rigidbody exitEdibleRigidbody;

	[SerializeField]
	private ParticleSystem exitEdiblePoofParticle;

	[SerializeField]
	[Tooltip("A particle to play at the position it is being reset from")]
	private ParticleSystem exitEdiblePrevPositionParticle;

	[Header("Reset Drink")]
	[SerializeField]
	private GameObject resetDrink;

	[Tooltip("The trigger listener to determine when to 'poof' back to the initial position")]
	[SerializeField]
	private TriggerListener resetDrinkTrigger;

	private Vector3 resetDrinkLocation;

	private Quaternion resetDrinkRotation;

	[SerializeField]
	private PickupableItem resetDrinkPickupable;

	private Rigidbody resetDrinkRigidbody;

	[SerializeField]
	private ParticleSystem resetDrinkPoofParticle;

	[SerializeField]
	[Tooltip("A particle to play at the position it is being reset from")]
	private ParticleSystem resetDrinkPrevPositionParticle;

	[SerializeField]
	private float minTimeBetweenButtonPresses = 0.33f;

	private float timeSinceLastButtonPress;

	private float timeSincePreviewStarted;

	[SerializeField]
	private MenuPreviewer menuPreviewer;

	private bool menuIsActive;

	private int creditsBusinessCardIndex;

	[SerializeField]
	private AudioClip errorPlacementAudio;

	private MasterHMDAndInputController masterHMDAndInputController;

	public MenuPreviewer MenuPreviewer
	{
		get
		{
			return menuPreviewer;
		}
	}

	private void Awake()
	{
		if (exitEdible != null)
		{
			exitEdibleLocation = exitEdible.transform.localPosition;
			exitEdibleRotation = exitEdible.transform.localRotation;
			exitEdibleRigidbody = exitEdible.GetComponent<Rigidbody>();
		}
		else
		{
			Debug.LogWarning("MenuController doesn't hasn't been assigned a exit edible");
		}
		if (resetDrink != null)
		{
			resetDrinkLocation = resetDrink.transform.localPosition;
			resetDrinkRotation = resetDrink.transform.localRotation;
			resetDrinkRigidbody = resetDrink.GetComponent<Rigidbody>();
		}
		else
		{
			Debug.LogWarning("MenuController doesn't hasn't been assigned a reset drink");
		}
		if ((bool)exitEdibleTrigger)
		{
			TriggerListener triggerListener = exitEdibleTrigger;
			triggerListener.OnEnter = (Action<TriggerEventInfo>)Delegate.Combine(triggerListener.OnEnter, new Action<TriggerEventInfo>(OnExitEdibleTriggerListenerEnter));
		}
		else
		{
			Debug.LogWarning("MenuController doesn't have a ExitEdibleTriggerListener");
		}
		if ((bool)resetDrinkTrigger)
		{
			TriggerListener triggerListener2 = resetDrinkTrigger;
			triggerListener2.OnEnter = (Action<TriggerEventInfo>)Delegate.Combine(triggerListener2.OnEnter, new Action<TriggerEventInfo>(OnResetDrinkTriggerListenerEnter));
		}
		else
		{
			Debug.LogWarning("MenuController doesn't have a ExitEdibleTriggerListener");
		}
		businessCardAttachPoint.OnObjectWasRefilled += BusinessCardRefilled;
		for (int i = 0; i < objectsToAppear.Count; i++)
		{
			objectsToAppear[i].SetActive(false);
		}
		base.gameObject.SetActive(true);
		menuIsActive = false;
		exitEdible.gameObject.SetActive(false);
		grabbableHinge.OnLowerUnlocked += LiftBriefcaseLid;
	}

	private IEnumerator Start()
	{
		while (!GlobalStorage.Instance.MasterHMDAndInputController.IsHMDAndInputReady)
		{
			yield return null;
		}
		masterHMDAndInputController = GlobalStorage.Instance.MasterHMDAndInputController;
	}

	public static void ExportCreditsAsMD(List<BusinessCardInfo> businessCardInfos)
	{
		string text = string.Empty;
		foreach (BusinessCardInfo businessCardInfo in businessCardInfos)
		{
			bool flag = false;
			foreach (BusinessCardletInfo cardletInfo in businessCardInfo.CardletInfos)
			{
				if (!flag && !string.IsNullOrEmpty(cardletInfo.Heading))
				{
					text += string.Format("##{0}\n", cardletInfo.Heading);
					flag = true;
				}
				else if (!string.IsNullOrEmpty(cardletInfo.Heading))
				{
					text += string.Format("###{0}\n", cardletInfo.Heading);
				}
				text += string.Format("{0}\n", cardletInfo.Body);
			}
		}
		File.WriteAllText(Path.Combine(Application.dataPath, "credits.md"), text);
	}

	private void OnDestroy()
	{
		if ((bool)exitEdibleTrigger)
		{
			TriggerListener triggerListener = exitEdibleTrigger;
			triggerListener.OnEnter = (Action<TriggerEventInfo>)Delegate.Remove(triggerListener.OnEnter, new Action<TriggerEventInfo>(OnExitEdibleTriggerListenerEnter));
		}
		if ((bool)resetDrinkTrigger)
		{
			TriggerListener triggerListener2 = resetDrinkTrigger;
			triggerListener2.OnEnter = (Action<TriggerEventInfo>)Delegate.Remove(triggerListener2.OnEnter, new Action<TriggerEventInfo>(OnResetDrinkTriggerListenerEnter));
		}
		if ((bool)cameraGrabbable)
		{
			PickupableItem pickupableItem = cameraGrabbable;
			pickupableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(pickupableItem.OnGrabbed, new Action<GrabbableItem>(CameraGrabbed));
		}
		businessCardAttachPoint.OnObjectWasRefilled -= BusinessCardRefilled;
	}

	private void Update()
	{
		timeSinceLastButtonPress += Time.deltaTime;
		timeSincePreviewStarted += Time.deltaTime;
		if (VRPlatform.GetCurrVRPlatformType() == VRPlatformTypes.Oculus && ((OculusHMDAndInputController)GlobalStorage.Instance.MasterHMDAndInputController).IsPaused)
		{
			return;
		}
		if (cameraObject == null && cameraAttachPoint.NumAttachedObjects >= 1)
		{
			Debug.Log("Menu camera obtained");
			cameraObject = cameraAttachPoint.AttachedObjects[0].gameObject;
			cameraGrabbable = cameraObject.GetComponent<PickupableItem>();
			PickupableItem pickupableItem = cameraGrabbable;
			pickupableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(pickupableItem.OnGrabbed, new Action<GrabbableItem>(CameraGrabbed));
			cameraAttachableObject = cameraObject.GetComponent<AttachableObject>();
			Transform transform = cameraObject.transform.Find("ResetTrigger");
			if ((bool)transform)
			{
				cameraTrigger = transform.gameObject.GetComponent<TriggerListener>();
				TriggerListener triggerListener = cameraTrigger;
				triggerListener.OnEnter = (Action<TriggerEventInfo>)Delegate.Combine(triggerListener.OnEnter, new Action<TriggerEventInfo>(OnCameraTriggerListenerEnter));
			}
			transform = cameraObject.transform.Find("PoofParticle");
			if ((bool)transform)
			{
				cameraPoofParticle = transform.gameObject.GetComponent<ParticleSystem>();
			}
			cameraRigidbody = cameraObject.GetComponent<Rigidbody>();
			cameraObject.SetActive(menuIsActive);
		}
		bool flag = false;
		bool flag2 = false;
		if (masterHMDAndInputController != null && masterHMDAndInputController.LeftHand != null)
		{
			flag = masterHMDAndInputController.LeftHand.HandController.GetButtonDown(HandController.HandControllerButton.Menu);
		}
		if (masterHMDAndInputController != null && masterHMDAndInputController.RightHand != null)
		{
			flag2 = masterHMDAndInputController.RightHand.HandController.GetButtonDown(HandController.HandControllerButton.Menu);
		}
		if ((flag || flag2) && TempBuildSettingsHolder.CurrentBuildType != TempBuildSettingsHolder.DemoType.Office5minDemo && timeSinceLastButtonPress > minTimeBetweenButtonPresses)
		{
			timeSinceLastButtonPress = 0f;
			if (menuIsActive)
			{
				ToggleMenu(flag2);
			}
			else if (IsInSceneThatSupportsMenu())
			{
				menuPreviewer.StartPreviewing(flag2);
				timeSincePreviewStarted = 0f;
			}
			else
			{
				ToggleMenu(flag2);
			}
		}
		if (masterHMDAndInputController != null && masterHMDAndInputController.LeftHand != null)
		{
			flag = masterHMDAndInputController.LeftHand.HandController.GetButtonUp(HandController.HandControllerButton.Menu);
		}
		if (masterHMDAndInputController != null && masterHMDAndInputController.RightHand != null)
		{
			flag2 = masterHMDAndInputController.RightHand.HandController.GetButtonUp(HandController.HandControllerButton.Menu);
		}
		if (flag || flag2 || (menuPreviewer.isActiveAndEnabled && timeSincePreviewStarted > minTimeBetweenButtonPresses))
		{
			bool flag3 = menuPreviewer.isActiveAndEnabled;
			if (menuPreviewer.StopPreviewing(flag2))
			{
				ToggleMenu(flag2);
			}
			else if (flag3 && !menuPreviewer.isActiveAndEnabled)
			{
				AudioManager.Instance.Play(menuPreviewer.GetPosition(), errorPlacementAudio, 1f, 1f);
			}
		}
		if (menuPreviewer.isActiveAndEnabled)
		{
			base.transform.position = menuPreviewer.GetPosition();
			base.transform.rotation = menuPreviewer.GetRotation();
		}
	}

	private void LiftBriefcaseLid(GrabbableHinge hinge)
	{
		if (!exitEdible.gameObject.activeSelf)
		{
			exitEdible.gameObject.SetActive(true);
		}
	}

	private void BusinessCardRefilled(AttachablePoint attachPoint, AttachableObject attachObj)
	{
		BusinessCard component = attachObj.GetComponent<BusinessCard>();
		component.Initialize(businessCardInfos[creditsBusinessCardIndex]);
		creditsBusinessCardIndex = (creditsBusinessCardIndex + 1) % businessCardInfos.Count;
		if (creditsBusinessCardIndex == 0)
		{
			AchievementManager.CompleteAchievement(11);
		}
	}

	public void ResetObject()
	{
		if (rb != null)
		{
			rb.isKinematic = true;
			rb.Sleep();
			rb.angularVelocity = Vector3.zero;
			rb.velocity = Vector3.zero;
		}
		Vector3 position = menuPreviewer.GetPosition();
		Quaternion rotation = menuPreviewer.GetRotation();
		base.transform.position = position;
		base.transform.rotation = rotation;
		resetDrink.transform.localPosition = resetDrinkLocation;
		resetDrink.transform.localRotation = resetDrinkRotation;
		exitEdible.Setup();
		exitEdible.SetNumberOfBitesTaken(0);
		exitEdible.transform.localPosition = exitEdibleLocation;
		exitEdible.transform.localRotation = exitEdibleRotation;
		exitEdible.gameObject.SetActive(false);
	}

	private void ResetHinge()
	{
		if (grabbableHinge != null)
		{
			grabbableHinge.LockLower();
		}
	}

	private void OnExitEdibleTriggerListenerEnter(TriggerEventInfo info)
	{
		if (!info.other.isTrigger && (!(exitEdiblePickupable != null) || !exitEdiblePickupable.IsCurrInHand) && !(Vector3.SqrMagnitude(exitEdible.transform.localPosition - exitEdibleLocation) < minPoofDistance * minPoofDistance))
		{
			exitEdiblePrevPositionParticle.transform.position = exitEdible.transform.position;
			exitEdiblePrevPositionParticle.Play();
			exitEdiblePoofParticle.Play();
			AudioManager.Instance.Play(exitEdible.transform.position, poofClip, 1f, 1f);
			exitEdible.SetNumberOfBitesTaken(0);
			exitEdible.transform.localPosition = exitEdibleLocation;
			exitEdible.transform.localRotation = exitEdibleRotation;
			exitEdibleRigidbody.isKinematic = true;
		}
	}

	private void OnResetDrinkTriggerListenerEnter(TriggerEventInfo info)
	{
		if (!info.other.isTrigger && (!(resetDrinkPickupable != null) || !resetDrinkPickupable.IsCurrInHand) && !(Vector3.SqrMagnitude(resetDrink.transform.localPosition - resetDrinkLocation) < minPoofDistance * minPoofDistance))
		{
			resetDrinkPrevPositionParticle.transform.position = resetDrink.transform.position;
			resetDrinkPrevPositionParticle.Play();
			resetDrinkPoofParticle.Play();
			AudioManager.Instance.Play(resetDrink.transform.position, poofClip, 1f, 1f);
			resetDrink.transform.localPosition = resetDrinkLocation;
			resetDrink.transform.localRotation = resetDrinkRotation;
			resetDrinkRigidbody.isKinematic = true;
		}
	}

	private void OnCameraTriggerListenerEnter(TriggerEventInfo info)
	{
		if (!info.other.isTrigger && (!(cameraGrabbable != null) || !cameraGrabbable.IsCurrInHand) && !(Vector3.SqrMagnitude(cameraObject.transform.position - cameraAttachPoint.transform.position) < minPoofDistance * minPoofDistance))
		{
			cameraPrevPositionParticle.transform.position = cameraObject.transform.position;
			cameraPrevPositionParticle.Play();
			cameraPoofParticle.Play();
			AudioManager.Instance.Play(cameraObject.transform.position, poofClip, 1f, 1f);
			cameraAttachableObject.AttachTo(cameraAttachPoint);
			cameraRigidbody.isKinematic = true;
		}
	}

	public void DisableMenu()
	{
		if (menuIsActive)
		{
			ToggleMenu(false);
		}
		base.gameObject.SetActive(false);
	}

	public void EnableMenu()
	{
		base.gameObject.SetActive(true);
	}

	public bool IsInSceneThatSupportsMenu()
	{
		return SceneManager.GetActiveScene().name != "Museum" && SceneManager.GetActiveScene().name != "Loading_PSVR" && SceneManager.GetActiveScene().name != "DemoEnd_PSVR";
	}

	private void ToggleMenu(bool isRightHand)
	{
		if (SceneManager.GetActiveScene().name == "Museum")
		{
			MuseumPoofEffect(isRightHand);
			return;
		}
		if (menuIsActive)
		{
			ResetHinge();
			for (int i = 0; i < objectsToAppear.Count; i++)
			{
				objectsToAppear[i].SetActive(false);
			}
			if (cameraGrabbable != null && !cameraGrabbable.IsCurrInHand)
			{
				cameraObject.SetActive(false);
			}
			menuIsActive = false;
			return;
		}
		PoofEffect();
		ResetObject();
		for (int j = 0; j < objectsToAppear.Count; j++)
		{
			objectsToAppear[j].SetActive(true);
		}
		if (cameraObject != null)
		{
			cameraObject.SetActive(true);
		}
		menuIsActive = true;
	}

	public void ReturnToMueseum()
	{
		LevelLoader.Instance.LoadIntroScene();
	}

	private void MuseumPoofEffect(bool isRightHand)
	{
		Vector3 position = menuPreviewer.GetPosition(true, isRightHand);
		base.transform.position = position;
		poofParticle.transform.position = position;
		poofParticle.Play();
		AudioManager.Instance.Play(poofParticle.transform.position, poofClip, 1f, 1f);
	}

	public void PoofEffect()
	{
		poofParticle.transform.position = base.transform.position;
		poofParticle.transform.rotation = base.transform.rotation;
		poofParticle.Play();
		if (AudioManager._noCreateInstance != null)
		{
			AudioManager._noCreateInstance.Play(poofParticle.transform.position, poofClip, 1f, 1f);
		}
	}

	private void CameraGrabbed(GrabbableItem item)
	{
		if (!menuIsActive)
		{
			return;
		}
		for (int i = 0; i < cameraAttachPoint.AttachedObjects.Count; i++)
		{
			if (cameraAttachPoint.AttachedObjects[i] == cameraAttachableObject)
			{
				ToggleMenu(false);
				break;
			}
		}
	}
}
