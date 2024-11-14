using System;
using System.Collections;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class ReceiptHolderController : MonoBehaviour
{
	[SerializeField]
	private GameObjectPrefabSpawner[] hookSpawners;

	[SerializeField]
	private Rigidbody parentRigidbody;

	[SerializeField]
	private AudioClip orderAppearSound;

	[SerializeField]
	private ParticleSystem orderAppearPoof;

	[SerializeField]
	private ParticleSystem orderDisappearPoof;

	[SerializeField]
	private Texture orderSkipTexture;

	private GameObject destroyOnNextConfirmation;

	private List<AttachablePoint> paperAttachPoints = new List<AttachablePoint>();

	private AttachableObject lastDetached;

	private void OnEnable()
	{
		StartCoroutine(WaitAndSubscribe());
	}

	private IEnumerator WaitAndSubscribe()
	{
		yield return null;
		if (JobBoardManager.instance != null)
		{
			JobBoardManager instance = JobBoardManager.instance;
			instance.OnBeganWaitingForConfirmation = (Action)Delegate.Combine(instance.OnBeganWaitingForConfirmation, new Action(JobBoardBeganWaitingForConfirmation));
			JobBoardManager instance2 = JobBoardManager.instance;
			instance2.OnBeganWaitingForSkipAction = (Action)Delegate.Combine(instance2.OnBeganWaitingForSkipAction, new Action(JobBoardBeganWaitingForConfirmation));
			JobBoardManager instance3 = JobBoardManager.instance;
			instance3.OnTaskComplete = (Action<TaskStatusController>)Delegate.Combine(instance3.OnTaskComplete, new Action<TaskStatusController>(OnTaskComplete));
		}
	}

	private void OnDisable()
	{
		StopAllCoroutines();
		if (JobBoardManager.instance != null)
		{
			JobBoardManager instance = JobBoardManager.instance;
			instance.OnBeganWaitingForConfirmation = (Action)Delegate.Remove(instance.OnBeganWaitingForConfirmation, new Action(JobBoardBeganWaitingForConfirmation));
			JobBoardManager instance2 = JobBoardManager.instance;
			instance2.OnBeganWaitingForSkipAction = (Action)Delegate.Remove(instance2.OnBeganWaitingForSkipAction, new Action(JobBoardBeganWaitingForConfirmation));
			JobBoardManager instance3 = JobBoardManager.instance;
			instance3.OnTaskComplete = (Action<TaskStatusController>)Delegate.Remove(instance3.OnTaskComplete, new Action<TaskStatusController>(OnTaskComplete));
		}
	}

	private void Awake()
	{
		for (int i = 0; i < hookSpawners.Length; i++)
		{
			GameObject gameObject = hookSpawners[i].SpawnPrefab();
			ConfigurableJoint component = gameObject.GetComponent<ConfigurableJoint>();
			gameObject.gameObject.SetActive(false);
			component.connectedBody = parentRigidbody;
			gameObject.gameObject.SetActive(true);
			AttachablePoint componentInChildren = gameObject.GetComponentInChildren<AttachablePoint>();
			paperAttachPoints.Add(componentInChildren);
			componentInChildren.OnObjectWasDetached += TicketDetatched;
		}
	}

	private void Start()
	{
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

	private void JobBoardBeganWaitingForConfirmation()
	{
		if (JobBoardManager.instance.EndlessModeStatusController == null)
		{
			Refresh();
		}
		else if (JobBoardManager.instance.EndlessModeStatusController.GetCurrentGoal() != null && !JobBoardManager.instance.EndlessModeStatusController.GetCurrentGoal().IsCompleted)
		{
			Refresh(true);
		}
		else
		{
			Refresh();
		}
	}

	private void Refresh(bool skip = false)
	{
		DestroyOldOrder();
		AttachablePoint attachablePoint = paperAttachPoints[0];
		float num = float.PositiveInfinity;
		for (int i = 0; i < paperAttachPoints.Count; i++)
		{
			if (paperAttachPoints[i].transform.position.z < num)
			{
				num = paperAttachPoints[i].transform.position.z;
				attachablePoint = paperAttachPoints[i];
			}
		}
		attachablePoint.RefillOneItem();
		if (skip)
		{
			attachablePoint.GetAttachedObject(0).GetComponentInChildren<MeshRenderer>().material.mainTexture = orderSkipTexture;
		}
		else
		{
			AudioManager.Instance.Play(attachablePoint.transform.position, orderAppearSound, 1f, 1f);
		}
		orderAppearPoof.transform.position = attachablePoint.transform.position;
		orderAppearPoof.Play();
		destroyOnNextConfirmation = attachablePoint.GetAttachedObject(0).gameObject;
	}

	private void OnTaskComplete(TaskStatusController taskStatus)
	{
		if (JobBoardManager.instance.EndlessModeStatusController != null && !taskStatus.IsSkipped)
		{
			DestroyOldOrder();
		}
	}

	private void DestroyOldOrder()
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
			orderDisappearPoof.transform.position = destroyOnNextConfirmation.transform.position;
			orderDisappearPoof.Play();
			UnityEngine.Object.Destroy(destroyOnNextConfirmation);
			destroyOnNextConfirmation = null;
		}
	}
}
