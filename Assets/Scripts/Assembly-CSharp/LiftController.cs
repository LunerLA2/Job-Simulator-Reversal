using System;
using System.Collections;
using OwlchemyVR;
using UnityEngine;

public class LiftController : MonoBehaviour
{
	public enum LiftRotation
	{
		Front = 0,
		Right = 1,
		Back = 2,
		Left = 3,
		LeftFront = 4,
		LeftBack = 5,
		RightFront = 6,
		RightBack = 7
	}

	private float[] desiredLiftYawsPerRotationSetting = new float[8] { 180f, 90f, 0f, 270f, 270f, 270f, 90f, 90f };

	private ReceiptPrinter receiptPrinter;

	[SerializeField]
	private WorldItemData frontData;

	[SerializeField]
	private WorldItemData rightData;

	[SerializeField]
	private WorldItemData backData;

	[SerializeField]
	private WorldItemData leftData;

	[SerializeField]
	private WorldItemData leftFrontData;

	[SerializeField]
	private WorldItemData leftBackData;

	[SerializeField]
	private WorldItemData rightFrontData;

	[SerializeField]
	private WorldItemData rightBackData;

	[SerializeField]
	private DialSelector selectionDial;

	private LiftRotation currentLiftRotation;

	private LiftRotation targetLiftRotation;

	[SerializeField]
	private float carFloatHeight = 0.5f;

	[SerializeField]
	private float carFloatHeightPSVR = 0.5f;

	[SerializeField]
	private Transform retractPosition;

	[SerializeField]
	private Transform workPosition;

	[SerializeField]
	private Material liftLightOff;

	[SerializeField]
	private Material liftLightOn;

	[SerializeField]
	private Renderer[] liftLights;

	[SerializeField]
	private LifterPainter lifterPainter;

	[SerializeField]
	private Animation lifterAnimation;

	[SerializeField]
	private AnimationClip lifterOpenClip;

	[SerializeField]
	private AnimationClip lifterCloseClip;

	[SerializeField]
	private AnimationClip lifterIdleClip;

	[SerializeField]
	private PageData[] whenThesePagesShownRevealDropButton;

	[SerializeField]
	private Rigidbody dropButtonRB;

	[SerializeField]
	private Transform dropButtonPivot;

	[SerializeField]
	private WorldItem dropButtonWorldItem;

	[SerializeField]
	private AudioClip carRotateClip;

	[SerializeField]
	private AudioClip carReturnClip;

	[SerializeField]
	private AudioClip carUpClip;

	[SerializeField]
	private AudioClip carDownClip;

	private bool waitingForDropButton;

	private VehicleController vehicleInLift;

	private float carInitialHeight;

	private float carInitialX;

	private bool liftIsBusy;

	private bool eventsAreRegistered;

	private bool buttonCoverIsOpen;

	private Coroutine dropCoroutine;

	public Action<VehicleController> OnVehicleWasDropped;

	private ReceiptPrinter ReceiptPrinter
	{
		get
		{
			if (receiptPrinter == null)
			{
				receiptPrinter = BotUniqueElementManager.Instance.GetObjectByName("RetractableReceiptPrinter(Clone)").GetComponent<ReceiptPrinter>();
			}
			return receiptPrinter;
		}
	}

	public LiftRotation CurrentLiftRotation
	{
		get
		{
			return currentLiftRotation;
		}
	}

	public Vector3 currentPosition { get; set; }

	public Vector3 currentRotation { get; set; }

	public Coroutine DropCoroutine
	{
		get
		{
			return dropCoroutine;
		}
	}

	private void Awake()
	{
		currentRotation = new Vector3(0f, 180f, 0f);
		dropButtonRB.isKinematic = true;
		TurnLightOn(0);
	}

	private void Start()
	{
		AutoMechanicManager.Instance.RegisterLiftController(this);
		RegisterJobBoardEvents();
	}

	private void OnEnable()
	{
		RegisterJobBoardEvents();
	}

	private void RegisterJobBoardEvents()
	{
		if (!eventsAreRegistered && JobBoardManager.instance != null)
		{
			eventsAreRegistered = true;
			JobBoardManager instance = JobBoardManager.instance;
			instance.OnPageShown = (Action<PageStatusController>)Delegate.Combine(instance.OnPageShown, new Action<PageStatusController>(PageShown));
			JobBoardManager instance2 = JobBoardManager.instance;
			instance2.OnPageComplete = (Action<PageStatusController>)Delegate.Combine(instance2.OnPageComplete, new Action<PageStatusController>(PageCompleted));
			JobBoardManager instance3 = JobBoardManager.instance;
			instance3.OnTaskComplete = (Action<TaskStatusController>)Delegate.Combine(instance3.OnTaskComplete, new Action<TaskStatusController>(TaskComplete));
		}
	}

	private void OnDisable()
	{
		JobBoardManager instance = JobBoardManager.instance;
		instance.OnPageShown = (Action<PageStatusController>)Delegate.Remove(instance.OnPageShown, new Action<PageStatusController>(PageShown));
		JobBoardManager instance2 = JobBoardManager.instance;
		instance2.OnPageComplete = (Action<PageStatusController>)Delegate.Remove(instance2.OnPageComplete, new Action<PageStatusController>(PageCompleted));
		JobBoardManager instance3 = JobBoardManager.instance;
		instance3.OnTaskComplete = (Action<TaskStatusController>)Delegate.Remove(instance3.OnTaskComplete, new Action<TaskStatusController>(TaskComplete));
		eventsAreRegistered = false;
	}

	private void PageShown(PageStatusController pageStatus)
	{
		if (Array.IndexOf(whenThesePagesShownRevealDropButton, pageStatus.Data) > -1)
		{
			SetDropButtonCoverOpen(true);
			waitingForDropButton = true;
			if (GenieManager.AreAnyJobGenieModesActive() && GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode) && vehicleInLift == null)
			{
				GameEventsManager.Instance.ItemActionOccurred(dropButtonWorldItem.Data, "USED");
				waitingForDropButton = false;
			}
		}
	}

	private void PageCompleted(PageStatusController pageStatus)
	{
		if ((!GenieManager.AreAnyJobGenieModesActive() || !GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode)) && Array.IndexOf(whenThesePagesShownRevealDropButton, pageStatus.Data) > -1)
		{
			waitingForDropButton = false;
			SetDropButtonCoverOpen(false);
			DropCar();
		}
	}

	private void TaskComplete(TaskStatusController taskStatus)
	{
		if (GenieManager.AreAnyJobGenieModesActive() && GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode))
		{
			waitingForDropButton = false;
			SetDropButtonCoverOpen(false);
			ResetDial();
		}
	}

	public void DropButtonPressed()
	{
		if (GenieManager.AreAnyJobGenieModesActive() && GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode) && AutoMechanicManager.CurrentVehicle != null)
		{
			if ((bool)vehicleInLift)
			{
				DropCar();
			}
			else
			{
				LiftVehicle();
			}
		}
		if (waitingForDropButton)
		{
			GameEventsManager.Instance.ItemActionOccurred(dropButtonWorldItem.Data, "USED");
			waitingForDropButton = false;
		}
	}

	private void Update()
	{
		if (!liftIsBusy && targetLiftRotation != currentLiftRotation)
		{
			StartCoroutine(RotateToRoutine(targetLiftRotation));
		}
		if ((bool)vehicleInLift)
		{
			vehicleInLift.transform.position = currentPosition;
			vehicleInLift.transform.eulerAngles = currentRotation;
		}
	}

	public void LiftVehicle()
	{
		if ((bool)vehicleInLift)
		{
			Debug.LogWarning("A Car Is Already Lifted");
			return;
		}
		if (AutoMechanicManager.CurrentVehicle == null)
		{
			Debug.LogWarning("AutoMechanicManager does not a reference to a car!");
			return;
		}
		if (GenieManager.AreAnyJobGenieModesActive() && GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode))
		{
			ReceiptPrinter.AnimatePrinter(false);
		}
		StartCoroutine(LiftRoutine());
	}

	private IEnumerator LiftRoutine()
	{
		if (GenieManager.AreAnyJobGenieModesActive() && GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode))
		{
			GameEventsManager.Instance.ScriptedCauseOccurred("LiftingCar");
		}
		PlayLifterAnimation(lifterOpenClip);
		liftIsBusy = true;
		vehicleInLift = AutoMechanicManager.CurrentVehicle;
		vehicleInLift.ForceCloseDoors();
		currentRotation = vehicleInLift.transform.eulerAngles;
		currentPosition = vehicleInLift.transform.position;
		carInitialHeight = vehicleInLift.transform.position.y;
		carInitialX = vehicleInLift.transform.position.x;
		if (carUpClip != null)
		{
			AudioManager.Instance.Play(vehicleInLift.transform, carUpClip, 1f, 1f);
		}
		Go.to(this, 1f, new GoTweenConfig().vector3YProp("currentPosition", carFloatHeight).setEaseType(GoEaseType.SineInOut));
		yield return new WaitForSeconds(1f);
		if (GenieManager.AreAnyJobGenieModesActive() && GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode) && currentLiftRotation != 0)
		{
			float rotationAngle = desiredLiftYawsPerRotationSetting[0];
			float rotationDelta = currentRotation.y - rotationAngle;
			if (rotationDelta > 180f)
			{
				rotationAngle += 360f;
			}
			else if (rotationDelta < -180f)
			{
				rotationAngle -= 360f;
			}
			Go.to(this, 2f, new GoTweenConfig().vector3YProp("currentRotation", rotationAngle).setEaseType(GoEaseType.SineInOut).onComplete(delegate
			{
				currentRotation = new Vector3(currentRotation.x, desiredLiftYawsPerRotationSetting[0], currentRotation.z);
			}));
			AudioManager.Instance.Play(vehicleInLift.transform, carRotateClip, 1f, 1f);
			yield return new WaitForSeconds(2f);
		}
		ResetDial();
		MoveToWorkPosition(1f, LiftRotation.Front);
		AudioManager.Instance.Play(vehicleInLift.transform, carReturnClip, 1f, 1f);
		currentRotation = new Vector3(currentRotation.x, desiredLiftYawsPerRotationSetting[(int)currentLiftRotation], currentRotation.z);
		selectionDial.Select(0, true);
		yield return new WaitForSeconds(1.1f);
		vehicleInLift.AllowDoorOpening();
		lifterPainter.OpenDoor();
		TurnLightOn(0);
		liftIsBusy = false;
		PlayLifterAnimation(lifterIdleClip);
		if (GenieManager.AreAnyJobGenieModesActive() && GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode))
		{
			SetDropButtonCoverOpen(true);
		}
	}

	public void SetDropButtonCoverOpen(bool open)
	{
		if (buttonCoverIsOpen != open)
		{
			buttonCoverIsOpen = open;
			dropButtonRB.isKinematic = !open;
			if (open)
			{
				Go.to(dropButtonPivot, 0.5f, new GoTweenConfig().localEulerAngles(Vector3.back * 140f).setEaseType(GoEaseType.QuadInOut));
			}
			else
			{
				Go.to(dropButtonPivot, 0.5f, new GoTweenConfig().localEulerAngles(Vector3.forward * 140f, true).setEaseType(GoEaseType.QuadInOut).setDelay(1.5f));
			}
		}
	}

	public void DropCar()
	{
		if (!vehicleInLift)
		{
			Debug.LogWarning("No Car Present To Drop");
		}
		else if (dropCoroutine == null)
		{
			dropCoroutine = StartCoroutine(DropRoutine());
			if (GenieManager.AreAnyJobGenieModesActive() && GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode))
			{
				ReceiptPrinter.AnimatePrinter(true);
			}
		}
	}

	private IEnumerator DropRoutine()
	{
		if (liftIsBusy)
		{
			while (liftIsBusy)
			{
				yield return new WaitForEndOfFrame();
			}
		}
		liftIsBusy = true;
		vehicleInLift.ForceCloseDoors();
		if (currentLiftRotation != LiftRotation.Back)
		{
			yield return StartCoroutine(RotateToRoutine(LiftRotation.Back, false, true));
		}
		liftIsBusy = true;
		Go.to(this, 1f, new GoTweenConfig().vector3YProp("currentPosition", carInitialHeight).setEaseType(GoEaseType.SineInOut));
		PlayLifterAnimation(lifterCloseClip);
		if (carDownClip != null)
		{
			AudioManager.Instance.Play(vehicleInLift.transform, carDownClip, 1f, 1f);
		}
		yield return new WaitForSeconds(1f);
		vehicleInLift.AllowDoorOpening();
		VehicleController droppedVehicle = vehicleInLift;
		vehicleInLift = null;
		lifterPainter.CloseDoor();
		if (OnVehicleWasDropped != null)
		{
			OnVehicleWasDropped(droppedVehicle);
		}
		yield return new WaitForSeconds(1f);
		if (!GenieManager.AreAnyJobGenieModesActive() || !GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode))
		{
			ResetDial();
		}
		dropCoroutine = null;
	}

	private void TurnLightOn(int light)
	{
		for (int i = 0; i < liftLights.Length; i++)
		{
			liftLights[i].material = ((i != light) ? liftLightOff : liftLightOn);
		}
	}

	private void ResetDial()
	{
		DoGameEventForLiftRotation(currentLiftRotation, "CLOSED");
		DoGameEventForLiftRotation(LiftRotation.Front, "OPENED");
		selectionDial.transform.localRotation = Quaternion.identity;
		TurnLightOn(0);
		currentLiftRotation = LiftRotation.Front;
		targetLiftRotation = LiftRotation.Front;
	}

	public void RotateCar(int newLiftRotation)
	{
		targetLiftRotation = (LiftRotation)newLiftRotation;
		TurnLightOn(newLiftRotation);
	}

	private IEnumerator RotateToRoutine(LiftRotation rotateTo, bool becomeFreeWhenDone = true, bool dropRoutine = false)
	{
		if (vehicleInLift == null)
		{
			yield break;
		}
		liftIsBusy = true;
		bool xOnly = false;
		vehicleInLift.ForceCloseDoors();
		DoGameEventForLiftRotation(currentLiftRotation, "CLOSED");
		DoGameEventForLiftRotation(rotateTo, "OPENED");
		if (currentLiftRotation == LiftRotation.LeftBack && rotateTo == LiftRotation.LeftFront)
		{
			xOnly = true;
		}
		if (currentLiftRotation == LiftRotation.LeftFront && rotateTo == LiftRotation.LeftBack)
		{
			xOnly = true;
		}
		if (currentLiftRotation == LiftRotation.RightBack && rotateTo == LiftRotation.RightFront)
		{
			xOnly = true;
		}
		if (currentLiftRotation == LiftRotation.RightFront && rotateTo == LiftRotation.RightBack)
		{
			xOnly = true;
		}
		currentLiftRotation = rotateTo;
		if (!xOnly)
		{
			Go.to(this, 1f, new GoTweenConfig().vector3ZProp("currentPosition", retractPosition.position.z).setEaseType(GoEaseType.SineInOut));
			Go.to(this, 1f, new GoTweenConfig().vector3XProp("currentPosition", carInitialX).setEaseType(GoEaseType.SineInOut));
			float rotationAngle = desiredLiftYawsPerRotationSetting[(int)rotateTo];
			float rotationDelta = currentRotation.y - rotationAngle;
			if (rotationDelta > 180f)
			{
				rotationAngle += 360f;
			}
			else if (rotationDelta < -180f)
			{
				rotationAngle -= 360f;
			}
			LiftRotation rotateTo2 = default(LiftRotation);
			Go.to(this, 2f, new GoTweenConfig().vector3YProp("currentRotation", rotationAngle).setEaseType(GoEaseType.SineInOut).onComplete(delegate
			{
				currentRotation = new Vector3(currentRotation.x, desiredLiftYawsPerRotationSetting[(int)rotateTo2], currentRotation.z);
			}));
			AudioManager.Instance.Play(vehicleInLift.transform, carRotateClip, 1f, 1f);
			yield return new WaitForSeconds(2f);
		}
		if (!dropRoutine)
		{
			MoveToWorkPosition(1f, rotateTo);
			AudioManager.Instance.Play(vehicleInLift.transform, carReturnClip, 1f, 1f);
			yield return new WaitForSeconds(1f);
			vehicleInLift.AllowDoorOpening();
		}
		if (becomeFreeWhenDone)
		{
			yield return null;
			liftIsBusy = false;
		}
	}

	private void DoGameEventForLiftRotation(LiftRotation rotation, string eventName)
	{
		switch (rotation)
		{
		case LiftRotation.Front:
			GameEventsManager.Instance.ItemActionOccurred(frontData, eventName);
			break;
		case LiftRotation.Right:
		case LiftRotation.RightFront:
		case LiftRotation.RightBack:
			GameEventsManager.Instance.ItemActionOccurred(rightData, eventName);
			switch (rotation)
			{
			case LiftRotation.RightFront:
				GameEventsManager.Instance.ItemActionOccurred(rightFrontData, eventName);
				break;
			case LiftRotation.RightBack:
				GameEventsManager.Instance.ItemActionOccurred(rightBackData, eventName);
				break;
			}
			break;
		case LiftRotation.Back:
			GameEventsManager.Instance.ItemActionOccurred(backData, eventName);
			break;
		case LiftRotation.Left:
		case LiftRotation.LeftFront:
		case LiftRotation.LeftBack:
			GameEventsManager.Instance.ItemActionOccurred(leftData, eventName);
			switch (rotation)
			{
			case LiftRotation.LeftFront:
				GameEventsManager.Instance.ItemActionOccurred(leftFrontData, eventName);
				break;
			case LiftRotation.LeftBack:
				GameEventsManager.Instance.ItemActionOccurred(leftBackData, eventName);
				break;
			}
			break;
		}
	}

	private void MoveToWorkPosition(float duration, LiftRotation desiredLiftRotation)
	{
		Vector3 vector = Vector3.zero;
		float num = 0f;
		float num2 = 0.35f;
		switch (desiredLiftRotation)
		{
		case LiftRotation.Front:
			vector = workPosition.position - vehicleInLift.FrontBoundPosition;
			break;
		case LiftRotation.Right:
			vector = workPosition.position - vehicleInLift.RightBoundPosition;
			break;
		case LiftRotation.Back:
			vector = workPosition.position - vehicleInLift.BackBoundPosition;
			break;
		case LiftRotation.Left:
			vector = workPosition.position - vehicleInLift.LeftBoundPosition;
			break;
		case LiftRotation.LeftFront:
			vector = workPosition.position - vehicleInLift.LeftBoundPosition;
			num = num2;
			break;
		case LiftRotation.LeftBack:
			vector = workPosition.position - vehicleInLift.LeftBoundPosition;
			num = 0f - num2;
			break;
		case LiftRotation.RightFront:
			vector = workPosition.position - vehicleInLift.RightBoundPosition;
			num = 0f - num2;
			break;
		case LiftRotation.RightBack:
			vector = workPosition.position - vehicleInLift.RightBoundPosition;
			num = num2;
			break;
		default:
			Debug.LogError("invalid desiredLiftRotation");
			break;
		}
		Go.to(this, duration, new GoTweenConfig().vector3ZProp("currentPosition", vector.z, true).setEaseType(GoEaseType.SineInOut));
		Go.to(this, duration, new GoTweenConfig().vector3XProp("currentPosition", num + carInitialX).setEaseType(GoEaseType.SineInOut));
	}

	private void PlayLifterAnimation(AnimationClip clip)
	{
		if (lifterAnimation != null)
		{
			lifterAnimation.clip = clip;
			lifterAnimation.Play();
		}
		else
		{
			Debug.LogWarning("Lifter animation not setup");
		}
	}
}
