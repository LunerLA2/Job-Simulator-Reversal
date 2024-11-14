using System;
using System.Collections;
using OwlchemyVR;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class MenuGadgetController : MonoBehaviour
{
	public enum ConfirmationButtonState
	{
		CAMERA = 0,
		RESET = 1,
		EXIT = 2,
		NONE = 3
	}

	[SerializeField]
	private MenuController menuController;

	[SerializeField]
	private Rigidbody rb;

	[SerializeField]
	private PickupableItem pickupableItem;

	[SerializeField]
	private SelectedChangeOutlineController outlineController;

	[SerializeField]
	private TriggerListener triggerListener;

	private bool readyToPoof;

	private Vector3 spawnPosition;

	private Quaternion spawnRotation;

	private ConfirmationButtonState confirmationButtonState = ConfirmationButtonState.NONE;

	[SerializeField]
	private Animator cameraAnimator;

	[SerializeField]
	private Animator resetAnimator;

	[SerializeField]
	private Animator exitAnimator;

	[SerializeField]
	private Animator doorsAnimator;

	[SerializeField]
	private AnimationClip menuRemoteDoors;

	[SerializeField]
	private ButtonController cameraButton;

	[SerializeField]
	private ButtonController resetButton;

	[SerializeField]
	private ButtonController exitButton;

	[SerializeField]
	private AudioClip cameraSound;

	[SerializeField]
	private AudioClip exitSound;

	[SerializeField]
	private AudioClip resetSound;

	[SerializeField]
	private Transform actualConfirmationButton;

	private float safeAnimationTimer;

	private void OnEnable()
	{
		ResetObject();
		menuController.PoofEffect();
	}

	private void ResetObject()
	{
		Debug.Log("ResetMenuGadgetTODO: Reset the variables on the Button childed below");
		readyToPoof = false;
		rb.isKinematic = true;
		rb.Sleep();
		rb.angularVelocity = Vector3.zero;
		rb.velocity = Vector3.zero;
		Vector3 position = GlobalStorage.Instance.MasterHMDAndInputController.TrackedHmdTransform.position;
		Vector3 forward = GlobalStorage.Instance.MasterHMDAndInputController.TrackedHmdTransform.forward;
		forward.y = 0f;
		Vector3 position2 = position + forward.normalized * 0.5f;
		position2.y = position2.y / 4f * 3f;
		Quaternion rotation = Quaternion.LookRotation(forward, Vector3.up);
		rotation.eulerAngles = new Vector3(15f, rotation.eulerAngles.y, 0f);
		base.transform.position = position2;
		base.transform.rotation = rotation;
	}

	private void Start()
	{
		PickupableItem obj = pickupableItem;
		obj.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(obj.OnGrabbed, new Action<GrabbableItem>(Grabbed));
		PickupableItem obj2 = pickupableItem;
		obj2.OnReleased = (Action<GrabbableItem>)Delegate.Combine(obj2.OnReleased, new Action<GrabbableItem>(Released));
		TriggerListener obj3 = triggerListener;
		obj3.OnEnter = (Action<TriggerEventInfo>)Delegate.Combine(obj3.OnEnter, new Action<TriggerEventInfo>(OnTriggerListenerEnter));
		ButtonController buttonController = cameraButton;
		buttonController.OnButtonDown = (UnityAction)Delegate.Combine(buttonController.OnButtonDown, new UnityAction(CameraButtonDown));
		ButtonController buttonController2 = exitButton;
		buttonController2.OnButtonDown = (UnityAction)Delegate.Combine(buttonController2.OnButtonDown, new UnityAction(ExitButtonDown));
		ButtonController buttonController3 = resetButton;
		buttonController3.OnButtonDown = (UnityAction)Delegate.Combine(buttonController3.OnButtonDown, new UnityAction(ResetButtonDown));
	}

	private void OnDestroy()
	{
		PickupableItem obj = pickupableItem;
		obj.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(obj.OnGrabbed, new Action<GrabbableItem>(Grabbed));
		PickupableItem obj2 = pickupableItem;
		obj2.OnReleased = (Action<GrabbableItem>)Delegate.Remove(obj2.OnReleased, new Action<GrabbableItem>(Released));
		TriggerListener obj3 = triggerListener;
		obj3.OnEnter = (Action<TriggerEventInfo>)Delegate.Remove(obj3.OnEnter, new Action<TriggerEventInfo>(OnTriggerListenerEnter));
		ButtonController buttonController = cameraButton;
		buttonController.OnButtonDown = (UnityAction)Delegate.Remove(buttonController.OnButtonDown, new UnityAction(CameraButtonDown));
		ButtonController buttonController2 = exitButton;
		buttonController2.OnButtonDown = (UnityAction)Delegate.Remove(buttonController2.OnButtonDown, new UnityAction(ExitButtonDown));
		ButtonController buttonController3 = resetButton;
		buttonController3.OnButtonDown = (UnityAction)Delegate.Remove(buttonController3.OnButtonDown, new UnityAction(ResetButtonDown));
	}

	private void Grabbed(GrabbableItem item)
	{
		readyToPoof = false;
		outlineController.enabled = false;
	}

	private void Released(GrabbableItem item)
	{
		readyToPoof = true;
		outlineController.enabled = true;
	}

	private void OnTriggerListenerEnter(TriggerEventInfo info)
	{
		if (!info.other.isTrigger && readyToPoof)
		{
			menuController.PoofEffect();
			base.gameObject.SetActive(false);
		}
	}

	private void Update()
	{
		safeAnimationTimer += Time.deltaTime;
	}

	private void CameraButtonDown()
	{
		if (!(safeAnimationTimer > menuRemoteDoors.length * 2f))
		{
			return;
		}
		safeAnimationTimer = 0f;
		switch (confirmationButtonState)
		{
		case ConfirmationButtonState.NONE:
			cameraAnimator.Play("Open");
			StartCoroutine(DoorsOpen());
			confirmationButtonState = ConfirmationButtonState.CAMERA;
			break;
		case ConfirmationButtonState.CAMERA:
			confirmationButtonState = ConfirmationButtonState.NONE;
			cameraAnimator.Play("Close");
			DoorsClose();
			break;
		case ConfirmationButtonState.EXIT:
			if ((bool)exitAnimator)
			{
				exitAnimator.Play("Close");
			}
			if ((bool)cameraAnimator)
			{
				cameraAnimator.Play("Open");
			}
			StartCoroutine(CloseDoorsThenOpenDoors());
			confirmationButtonState = ConfirmationButtonState.CAMERA;
			break;
		case ConfirmationButtonState.RESET:
			if ((bool)resetAnimator)
			{
				resetAnimator.Play("Close");
			}
			if ((bool)cameraAnimator)
			{
				cameraAnimator.Play("Open");
			}
			StartCoroutine(CloseDoorsThenOpenDoors());
			confirmationButtonState = ConfirmationButtonState.CAMERA;
			break;
		}
	}

	private void ExitButtonDown()
	{
		if (safeAnimationTimer > menuRemoteDoors.length * 2f)
		{
			safeAnimationTimer = 0f;
			switch (confirmationButtonState)
			{
			case ConfirmationButtonState.NONE:
				exitAnimator.Play("Open");
				StartCoroutine(DoorsOpen());
				confirmationButtonState = ConfirmationButtonState.EXIT;
				break;
			case ConfirmationButtonState.CAMERA:
				cameraAnimator.Play("Close");
				exitAnimator.Play("Open");
				StartCoroutine(CloseDoorsThenOpenDoors());
				confirmationButtonState = ConfirmationButtonState.EXIT;
				break;
			case ConfirmationButtonState.EXIT:
				exitAnimator.Play("Close");
				DoorsClose();
				confirmationButtonState = ConfirmationButtonState.NONE;
				break;
			case ConfirmationButtonState.RESET:
				resetAnimator.Play("Close");
				exitAnimator.Play("Open");
				StartCoroutine(CloseDoorsThenOpenDoors());
				confirmationButtonState = ConfirmationButtonState.EXIT;
				break;
			}
		}
	}

	private void ResetButtonDown()
	{
		if (safeAnimationTimer > menuRemoteDoors.length * 2f)
		{
			safeAnimationTimer = 0f;
			switch (confirmationButtonState)
			{
			case ConfirmationButtonState.NONE:
				resetAnimator.Play("Open");
				StartCoroutine(DoorsOpen());
				confirmationButtonState = ConfirmationButtonState.RESET;
				break;
			case ConfirmationButtonState.CAMERA:
				cameraAnimator.Play("Close");
				resetAnimator.Play("Open");
				StartCoroutine(CloseDoorsThenOpenDoors());
				confirmationButtonState = ConfirmationButtonState.RESET;
				break;
			case ConfirmationButtonState.EXIT:
				exitAnimator.Play("Close");
				resetAnimator.Play("Open");
				StartCoroutine(CloseDoorsThenOpenDoors());
				confirmationButtonState = ConfirmationButtonState.RESET;
				break;
			case ConfirmationButtonState.RESET:
				resetAnimator.Play("Close");
				DoorsClose();
				confirmationButtonState = ConfirmationButtonState.NONE;
				break;
			}
		}
	}

	private IEnumerator CloseDoorsThenOpenDoors()
	{
		doorsAnimator.gameObject.SetActive(true);
		actualConfirmationButton.gameObject.SetActive(false);
		doorsAnimator.Play("Close");
		yield return new WaitForSeconds(menuRemoteDoors.length + 0.2f);
		doorsAnimator.Play("Open");
		yield return new WaitForSeconds(menuRemoteDoors.length + 0.2f);
		actualConfirmationButton.gameObject.SetActive(true);
		doorsAnimator.gameObject.SetActive(false);
	}

	private IEnumerator DoorsOpen()
	{
		doorsAnimator.gameObject.SetActive(true);
		actualConfirmationButton.gameObject.SetActive(false);
		doorsAnimator.Play("Open");
		yield return new WaitForSeconds(menuRemoteDoors.length + 0.2f);
		actualConfirmationButton.gameObject.SetActive(true);
		doorsAnimator.gameObject.SetActive(false);
	}

	private void DoorsClose()
	{
		doorsAnimator.gameObject.SetActive(true);
		actualConfirmationButton.gameObject.SetActive(false);
		doorsAnimator.Play("Close");
	}

	public void ConfirmationButtonDown()
	{
		switch (confirmationButtonState)
		{
		case ConfirmationButtonState.CAMERA:
			AudioManager.Instance.Play(base.transform.position, cameraSound, 1f, 1f);
			break;
		case ConfirmationButtonState.EXIT:
			AudioManager.Instance.Play(base.transform.position, exitSound, 1f, 1f);
			LevelLoader.Instance.LoadIntroScene();
			break;
		case ConfirmationButtonState.RESET:
			AudioManager.Instance.Play(base.transform.position, resetSound, 1f, 1f);
			LevelLoader.Instance.LoadJob(SceneManager.GetActiveScene().name, 0, GlobalStorage.Instance.CurrentGenieModes);
			break;
		case ConfirmationButtonState.NONE:
			break;
		}
	}
}
