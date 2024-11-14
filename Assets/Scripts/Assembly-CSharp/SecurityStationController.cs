using System;
using OwlchemyVR;
using UnityEngine;

public class SecurityStationController : MonoBehaviour
{
	[SerializeField]
	private WorldItem worldItem;

	[SerializeField]
	private GameObject[] screens;

	[SerializeField]
	private Camera securityCamera;

	[SerializeField]
	private GrabbableSlider cameraSlider;

	[SerializeField]
	private Transform cameraPivotTransform;

	[SerializeField]
	private Vector3 cameraRotationAtLower;

	[SerializeField]
	private Vector3 cameraRotationAtUpper;

	[SerializeField]
	private MeshRenderer readyLightRenderer;

	[SerializeField]
	private Material lightOnMaterial;

	[SerializeField]
	private Material lightOffMaterial;

	[SerializeField]
	private AttachablePoint ticketAttachPoint;

	[SerializeField]
	private AudioClip ticketAppearSound;

	[SerializeField]
	private ParticleSystem ticketAppearPoof;

	[SerializeField]
	private ParticleSystem ticketDisappearPoof;

	private GameObject destroyOnNextConfirmation;

	private bool screensOn;

	[SerializeField]
	private float cameraUpdateTime = 0.4f;

	[SerializeField]
	private float cameraUpdateTimeWhenGrabbed = 0.05f;

	[SerializeField]
	private MeshRenderer screenStatic;

	[SerializeField]
	private GameObject screenNonStatic;

	private bool cleaningBotState;

	[SerializeField]
	private VisibilityEvents screenVisibilityEvents;

	private bool isScreenVisible;

	private float lastCameraUpdate;

	private bool cameraIsGrabbed;

	private float staticFrame;

	private AttachableObject lastDetached;

	[SerializeField]
	private Texture SkipTexture;

	private void Awake()
	{
		screensOn = false;
		for (int i = 0; i < screens.Length; i++)
		{
			screens[i].SetActive(false);
		}
		securityCamera.gameObject.SetActive(false);
		screenNonStatic.gameObject.SetActive(true);
		screenStatic.gameObject.SetActive(false);
	}

	private void Update()
	{
		if (screensOn && isScreenVisible)
		{
			float num = 0f;
			num = ((!cameraIsGrabbed) ? cameraUpdateTime : cameraUpdateTimeWhenGrabbed);
			if (Time.time - lastCameraUpdate >= num)
			{
				securityCamera.Render();
				lastCameraUpdate = Time.time;
			}
		}
	}

	private void Start()
	{
		JobBoardManager instance = JobBoardManager.instance;
		instance.OnBeganWaitingForConfirmation = (Action)Delegate.Combine(instance.OnBeganWaitingForConfirmation, new Action(BeganWaitingForConfirmation));
		JobBoardManager instance2 = JobBoardManager.instance;
		instance2.OnBeganWaitingForSkipAction = (Action)Delegate.Combine(instance2.OnBeganWaitingForSkipAction, new Action(BeganWaitingForConfirmation));
		JobBoardManager instance3 = JobBoardManager.instance;
		instance3.OnTaskComplete = (Action<TaskStatusController>)Delegate.Combine(instance3.OnTaskComplete, new Action<TaskStatusController>(OnTaskComplete));
		GrabbableItem grabbable = cameraSlider.Grabbable;
		grabbable.OnGrabbedUpdate = (Action<GrabbableItem>)Delegate.Combine(grabbable.OnGrabbedUpdate, new Action<GrabbableItem>(UpdateSliderPosition));
		GrabbableItem grabbable2 = cameraSlider.Grabbable;
		grabbable2.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(grabbable2.OnGrabbed, new Action<GrabbableItem>(SliderGrabbed));
		GrabbableItem grabbable3 = cameraSlider.Grabbable;
		grabbable3.OnReleased = (Action<GrabbableItem>)Delegate.Combine(grabbable3.OnReleased, new Action<GrabbableItem>(SliderReleased));
		ticketAttachPoint.OnObjectWasAttached += TicketAttached;
		ticketAttachPoint.OnObjectWasDetached += TicketDetached;
		screenVisibilityEvents.OnObjectBecameInvisible += ScreenVisibilityEvents_OnObjectBecameInvisible;
		screenVisibilityEvents.OnObjectBecameVisible += ScreenVisibilityEvents_OnObjectBecameVisible;
	}

	private void OnDestroy()
	{
		JobBoardManager instance = JobBoardManager.instance;
		instance.OnBeganWaitingForConfirmation = (Action)Delegate.Remove(instance.OnBeganWaitingForConfirmation, new Action(BeganWaitingForConfirmation));
		JobBoardManager instance2 = JobBoardManager.instance;
		instance2.OnBeganWaitingForSkipAction = (Action)Delegate.Remove(instance2.OnBeganWaitingForSkipAction, new Action(BeganWaitingForConfirmation));
		JobBoardManager instance3 = JobBoardManager.instance;
		instance3.OnTaskComplete = (Action<TaskStatusController>)Delegate.Remove(instance3.OnTaskComplete, new Action<TaskStatusController>(OnTaskComplete));
		GrabbableItem grabbable = cameraSlider.Grabbable;
		grabbable.OnGrabbedUpdate = (Action<GrabbableItem>)Delegate.Remove(grabbable.OnGrabbedUpdate, new Action<GrabbableItem>(UpdateSliderPosition));
		GrabbableItem grabbable2 = cameraSlider.Grabbable;
		grabbable2.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(grabbable2.OnGrabbed, new Action<GrabbableItem>(SliderGrabbed));
		GrabbableItem grabbable3 = cameraSlider.Grabbable;
		grabbable3.OnReleased = (Action<GrabbableItem>)Delegate.Remove(grabbable3.OnReleased, new Action<GrabbableItem>(SliderReleased));
		ticketAttachPoint.OnObjectWasAttached -= TicketAttached;
		ticketAttachPoint.OnObjectWasDetached -= TicketDetached;
		screenVisibilityEvents.OnObjectBecameInvisible -= ScreenVisibilityEvents_OnObjectBecameInvisible;
		screenVisibilityEvents.OnObjectBecameVisible -= ScreenVisibilityEvents_OnObjectBecameVisible;
	}

	private void ScreenVisibilityEvents_OnObjectBecameVisible(VisibilityEvents obj)
	{
		isScreenVisible = true;
	}

	private void ScreenVisibilityEvents_OnObjectBecameInvisible(VisibilityEvents obj)
	{
		isScreenVisible = false;
	}

	public void SetCleaningBotState(bool state)
	{
	}

	private void TicketAttached(AttachablePoint point, AttachableObject obj)
	{
		readyLightRenderer.material = lightOnMaterial;
	}

	private void TicketDetached(AttachablePoint point, AttachableObject obj)
	{
		readyLightRenderer.material = lightOffMaterial;
		if (!(obj == lastDetached))
		{
			lastDetached = obj;
			if (GenieManager.AreAnyJobGenieModesActive() && GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode) && JobBoardManager.instance.EndlessModeStatusController.GetCurrentGoal() != null && !JobBoardManager.instance.EndlessModeStatusController.GetCurrentGoal().IsCompleted)
			{
				JobBoardManager.instance.EndlessModeStatusController.ForceJobComplete(false, true);
			}
		}
	}

	private void BeganWaitingForConfirmation()
	{
		DestroyOldTicket();
		ticketAttachPoint.RefillOneItem();
		if (JobBoardManager.instance.EndlessModeStatusController != null && JobBoardManager.instance.EndlessModeStatusController.GetCurrentGoal() != null && !JobBoardManager.instance.EndlessModeStatusController.GetCurrentGoal().IsCompleted)
		{
			GameObject gameObject = ticketAttachPoint.AttachedObjects[0].gameObject;
			gameObject.transform.Find("CSS_Ticket").GetComponent<MeshRenderer>().material.SetTexture("_MainTex", SkipTexture);
		}
		else
		{
			AudioManager.Instance.Play(ticketAttachPoint.transform.position, ticketAppearSound, 1f, 1f);
		}
		readyLightRenderer.material = lightOnMaterial;
		ticketAppearPoof.transform.position = ticketAttachPoint.transform.position;
		ticketAppearPoof.Play();
		destroyOnNextConfirmation = ticketAttachPoint.GetAttachedObject(0).gameObject;
	}

	private void OnTaskComplete(TaskStatusController taskStatus)
	{
		if (JobBoardManager.instance.EndlessModeStatusController != null && !taskStatus.IsSkipped)
		{
			DestroyOldTicket();
		}
	}

	private void DestroyOldTicket()
	{
		if (destroyOnNextConfirmation != null)
		{
			GrabbableItem component = destroyOnNextConfirmation.GetComponent<GrabbableItem>();
			if (component != null && component.IsCurrInHand && component.CurrInteractableHand != null)
			{
				component.CurrInteractableHand.ManuallyReleaseJoint();
			}
			AttachableObject component2 = destroyOnNextConfirmation.GetComponent<AttachableObject>();
			if (component2 != null && component2.CurrentlyAttachedTo != null)
			{
				component2.Detach(true, true);
			}
			ticketDisappearPoof.transform.position = destroyOnNextConfirmation.transform.position;
			ticketDisappearPoof.Play();
			UnityEngine.Object.Destroy(destroyOnNextConfirmation);
			destroyOnNextConfirmation = null;
		}
	}

	private void SliderGrabbed(GrabbableItem item)
	{
		cameraIsGrabbed = true;
	}

	private void SliderReleased(GrabbableItem item)
	{
		cameraIsGrabbed = false;
	}

	private void UpdateSliderPosition(GrabbableItem item)
	{
		cameraPivotTransform.localEulerAngles = Vector3.Lerp(cameraRotationAtLower, cameraRotationAtUpper, cameraSlider.NormalizedOffset);
	}

	public void ToggleScreens()
	{
		screensOn = !screensOn;
		for (int i = 0; i < screens.Length; i++)
		{
			screens[i].SetActive(screensOn);
		}
		if (screensOn)
		{
			GameEventsManager.Instance.ItemActionOccurred(worldItem.Data, "ACTIVATED");
			lastCameraUpdate = 0f;
		}
		else
		{
			GameEventsManager.Instance.ItemActionOccurred(worldItem.Data, "DEACTIVATED");
		}
	}
}
