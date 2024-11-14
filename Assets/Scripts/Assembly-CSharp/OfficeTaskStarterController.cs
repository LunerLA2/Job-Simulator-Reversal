using System;
using System.Collections;
using OwlchemyVR;
using UnityEngine;

public class OfficeTaskStarterController : MonoBehaviour
{
	[SerializeField]
	private Texture SkipTexture;

	[SerializeField]
	private AttachablePoint ticketAttachPoint;

	[SerializeField]
	private AudioClip ticketAppearSound;

	[SerializeField]
	private ParticleSystem ticketAppearPoof;

	[SerializeField]
	private ParticleSystem ticketDisappearPoof;

	[SerializeField]
	private GameObject objectToBlinkWhenNew;

	[SerializeField]
	private float blinkTime = 0.5f;

	private bool isBlinkRunning;

	[SerializeField]
	private Transform ticketAppearPoofPosition;

	private GameObject destroyOnNextConfirmation;

	private AttachableObject lastDetached;

	private void OnEnable()
	{
		JobBoardManager instance = JobBoardManager.instance;
		instance.OnBeganWaitingForConfirmation = (Action)Delegate.Combine(instance.OnBeganWaitingForConfirmation, new Action(BeganWaitingForConfirmation));
		JobBoardManager instance2 = JobBoardManager.instance;
		instance2.OnBeganWaitingForSkipAction = (Action)Delegate.Combine(instance2.OnBeganWaitingForSkipAction, new Action(BeganWaitingForConfirmation));
		JobBoardManager instance3 = JobBoardManager.instance;
		instance3.OnTaskComplete = (Action<TaskStatusController>)Delegate.Combine(instance3.OnTaskComplete, new Action<TaskStatusController>(OnTaskComplete));
		ticketAttachPoint.OnObjectWasAttached += TicketAttached;
		ticketAttachPoint.OnObjectWasDetached += TicketDetatched;
	}

	private void OnDisable()
	{
		JobBoardManager instance = JobBoardManager.instance;
		instance.OnBeganWaitingForConfirmation = (Action)Delegate.Remove(instance.OnBeganWaitingForConfirmation, new Action(BeganWaitingForConfirmation));
		JobBoardManager instance2 = JobBoardManager.instance;
		instance2.OnBeganWaitingForSkipAction = (Action)Delegate.Remove(instance2.OnBeganWaitingForSkipAction, new Action(BeganWaitingForConfirmation));
		JobBoardManager instance3 = JobBoardManager.instance;
		instance3.OnTaskComplete = (Action<TaskStatusController>)Delegate.Remove(instance3.OnTaskComplete, new Action<TaskStatusController>(OnTaskComplete));
		ticketAttachPoint.OnObjectWasAttached -= TicketAttached;
		ticketAttachPoint.OnObjectWasDetached -= TicketDetatched;
	}

	private void Awake()
	{
		objectToBlinkWhenNew.SetActive(false);
	}

	private void TicketAttached(AttachablePoint point, AttachableObject obj)
	{
		lastDetached = obj;
	}

	private void TicketDetatched(AttachablePoint point, AttachableObject obj)
	{
		if (!(obj == lastDetached))
		{
			lastDetached = obj;
			if (GenieManager.AreAnyJobGenieModesActive() && GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode) && JobBoardManager.instance.EndlessModeStatusController.GetCurrentGoal() != null && !JobBoardManager.instance.EndlessModeStatusController.GetCurrentGoal().IsCompleted)
			{
				JobBoardManager.instance.EndlessModeStatusController.ForceJobComplete(false, true);
			}
		}
	}

	private IEnumerator BlinkAnimation()
	{
		while (ticketAttachPoint.NumAttachedObjects > 0)
		{
			objectToBlinkWhenNew.SetActive(true);
			yield return new WaitForSeconds(blinkTime);
			objectToBlinkWhenNew.SetActive(false);
			yield return new WaitForSeconds(blinkTime);
		}
		isBlinkRunning = false;
	}

	private void BeganWaitingForConfirmation()
	{
		DestroyOldTicket();
		ticketAttachPoint.RefillOneItem();
		bool flag = false;
		if (GenieManager.AreAnyJobGenieModesActive() && GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode) && JobBoardManager.instance.EndlessModeStatusController.GetCurrentGoal() != null && !JobBoardManager.instance.EndlessModeStatusController.GetCurrentGoal().IsCompleted)
		{
			GameObject gameObject = ticketAttachPoint.AttachedObjects[0].gameObject;
			gameObject.transform.Find("memo").GetComponent<MeshRenderer>().material.SetTexture("_MainTex", SkipTexture);
			flag = true;
		}
		if (!flag)
		{
			AudioManager.Instance.Play(ticketAttachPoint.transform.position, ticketAppearSound, 1f, 1f);
		}
		ticketAppearPoof.transform.position = ticketAppearPoofPosition.position;
		ticketAppearPoof.Play();
		destroyOnNextConfirmation = ticketAttachPoint.GetAttachedObject(0).gameObject;
		if (!isBlinkRunning)
		{
			isBlinkRunning = true;
			StartCoroutine(BlinkAnimation());
		}
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
		if (ticketAttachPoint.NumAttachedObjects > 0)
		{
			GameObject obj = ticketAttachPoint.AttachedObjects[0].gameObject;
			ticketAttachPoint.AttachedObjects[0].Detach(true, true);
			UnityEngine.Object.Destroy(obj);
		}
	}
}
