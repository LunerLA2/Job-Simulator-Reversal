using System;
using System.Collections;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class ServingStationController : MonoBehaviour
{
	private const float INF_OVERTIME_BELL_MULTIPLIER = 2f;

	[SerializeField]
	private WorldItem bellWorldItem;

	[SerializeField]
	private Transform midwayPoint;

	[SerializeField]
	private Transform defaultTable;

	[SerializeField]
	private AudioClip plateLeavingAudioClip;

	[SerializeField]
	private ServingPlateTableInfo[] tableInfos;

	private Transform nextTablePosition;

	private PageStatusController currentPage;

	[SerializeField]
	private Animation bellAnimation;

	[SerializeField]
	private AnimationClip bellAnimationIn;

	[SerializeField]
	private AnimationClip bellAnimationOut;

	[SerializeField]
	private Transform bellTransform;

	[SerializeField]
	private AudioClip bellRiseSound;

	[SerializeField]
	private AudioClip bellLowerSound;

	[SerializeField]
	private GameObjectPrefabSpawner plateSpawner;

	[SerializeField]
	private Transform endlessModeMidwayLocation;

	[SerializeField]
	private Transform endlessModePlateLocation;

	private ServingPlate currentPlate;

	private bool wantBellForCurrentPage;

	private bool bellIsShown = true;

	private bool canRing;

	private bool canDoPlatesForever;

	private float bellAnimationMultiplier = 1f;

	private List<ServingPlate> servedPlates = new List<ServingPlate>();

	private void Awake()
	{
		nextTablePosition = defaultTable;
		AnimateBell(false);
		canRing = false;
		SpawnPlate();
	}

	private void Start()
	{
		if (GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode))
		{
			bellAnimationMultiplier = 2f;
		}
	}

	private void OnEnable()
	{
		if (JobBoardManager.instance != null)
		{
			JobBoardManager instance = JobBoardManager.instance;
			instance.OnPageShown = (Action<PageStatusController>)Delegate.Combine(instance.OnPageShown, new Action<PageStatusController>(PageShown));
			JobBoardManager instance2 = JobBoardManager.instance;
			instance2.OnSubtaskComplete = (Action<SubtaskStatusController>)Delegate.Combine(instance2.OnSubtaskComplete, new Action<SubtaskStatusController>(SubtaskComplete));
			JobBoardManager instance3 = JobBoardManager.instance;
			instance3.OnSubtaskUncomplete = (Action<SubtaskStatusController>)Delegate.Combine(instance3.OnSubtaskUncomplete, new Action<SubtaskStatusController>(SubtaskUncomplete));
			JobBoardManager instance4 = JobBoardManager.instance;
			instance4.OnJobComplete = (Action<JobStatusController>)Delegate.Combine(instance4.OnJobComplete, new Action<JobStatusController>(JobComplete));
			JobBoardManager instance5 = JobBoardManager.instance;
			instance5.OnSandboxPhaseStarted = (Action)Delegate.Combine(instance5.OnSandboxPhaseStarted, new Action(SandboxStart));
			JobBoardManager instance6 = JobBoardManager.instance;
			instance6.OnTaskComplete = (Action<TaskStatusController>)Delegate.Combine(instance6.OnTaskComplete, new Action<TaskStatusController>(OnTaskSkipped));
		}
	}

	private void PageComplete(PageStatusController obj)
	{
		if (GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode))
		{
			TaskStatusController currentGoal = JobBoardManager.instance.EndlessModeStatusController.GetCurrentGoal();
			PageData currentPageData = JobBoardManager.instance.EndlessModeStatusController.GetCurrentPageData();
			List<PageData> pages = currentGoal.Data.Pages;
			int num = JobBoardManager.instance.EndlessModeStatusController.Data.RequiredPages.Length;
			if (currentPageData != pages[pages.Count - num - 1])
			{
				currentPlate.SendToTable(endlessModeMidwayLocation, endlessModePlateLocation, true);
				TimeManager.Invoke(SpawnPlate, 3f);
			}
			else
			{
				Debug.Log("it happened");
			}
		}
	}

	private void OnDisable()
	{
		if (JobBoardManager.instance != null)
		{
			JobBoardManager instance = JobBoardManager.instance;
			instance.OnPageShown = (Action<PageStatusController>)Delegate.Remove(instance.OnPageShown, new Action<PageStatusController>(PageShown));
			JobBoardManager instance2 = JobBoardManager.instance;
			instance2.OnSubtaskComplete = (Action<SubtaskStatusController>)Delegate.Remove(instance2.OnSubtaskComplete, new Action<SubtaskStatusController>(SubtaskComplete));
			JobBoardManager instance3 = JobBoardManager.instance;
			instance3.OnSubtaskUncomplete = (Action<SubtaskStatusController>)Delegate.Remove(instance3.OnSubtaskUncomplete, new Action<SubtaskStatusController>(SubtaskUncomplete));
			JobBoardManager instance4 = JobBoardManager.instance;
			instance4.OnJobComplete = (Action<JobStatusController>)Delegate.Remove(instance4.OnJobComplete, new Action<JobStatusController>(JobComplete));
			JobBoardManager instance5 = JobBoardManager.instance;
			instance5.OnSandboxPhaseStarted = (Action)Delegate.Remove(instance5.OnSandboxPhaseStarted, new Action(SandboxStart));
			JobBoardManager instance6 = JobBoardManager.instance;
			instance6.OnTaskComplete = (Action<TaskStatusController>)Delegate.Remove(instance6.OnTaskComplete, new Action<TaskStatusController>(OnTaskSkipped));
		}
	}

	private void OnTaskSkipped(TaskStatusController task)
	{
		if (task.IsSkipped)
		{
			TimeManager.Invoke(SpawnPlate, 4f);
		}
	}

	private void SandboxStart()
	{
		if (!canDoPlatesForever)
		{
			Debug.Log("sandbox mode started, begin infinite serving plates");
			canDoPlatesForever = true;
			StartCoroutine(BeginAllowingInfinitePlates(1f));
		}
	}

	private void JobComplete(JobStatusController job)
	{
		if (!canDoPlatesForever)
		{
			Debug.Log("job complete, begin infinite serving plates");
			canDoPlatesForever = true;
			StartCoroutine(BeginAllowingInfinitePlates(5f));
		}
	}

	private IEnumerator BeginAllowingInfinitePlates(float t)
	{
		yield return new WaitForSeconds(t);
		RandomlyChooseOutgoingTargetTable();
		AnimateBell(true);
		canRing = true;
	}

	private void RandomlyChooseOutgoingTargetTable()
	{
		nextTablePosition = null;
		int num = 0;
		while (nextTablePosition == null && num < 50)
		{
			nextTablePosition = tableInfos[UnityEngine.Random.Range(0, tableInfos.Length)].TargetTable;
			num++;
			if (nextTablePosition == endlessModePlateLocation)
			{
				nextTablePosition = null;
				num--;
			}
		}
		if (nextTablePosition == null)
		{
			Debug.LogError("Somehow couldn't find a table to send food out to");
		}
	}

	private void PageShown(PageStatusController page)
	{
		currentPage = page;
		wantBellForCurrentPage = false;
		for (int i = 0; i < tableInfos.Length; i++)
		{
			if (tableInfos[i].DuringPage == currentPage.Data)
			{
				nextTablePosition = tableInfos[i].TargetTable;
				wantBellForCurrentPage = true;
				for (int j = 0; j < page.SubtaskStatusControllerList.Count; j++)
				{
					SubtaskChanged(page.SubtaskStatusControllerList[j]);
				}
				break;
			}
		}
	}

	private void SubtaskComplete(SubtaskStatusController subtask)
	{
		SubtaskChanged(subtask);
	}

	private void SubtaskUncomplete(SubtaskStatusController subtask)
	{
		SubtaskChanged(subtask);
	}

	private void SubtaskChanged(SubtaskStatusController subtask)
	{
		if (!wantBellForCurrentPage || currentPage == null || !currentPage.SubtaskStatusControllerList.Contains(subtask))
		{
			return;
		}
		bool state = true;
		for (int i = 0; i < currentPage.SubtaskStatusControllerList.Count - 1; i++)
		{
			if (!currentPage.SubtaskStatusControllerList[i].IsCompleted)
			{
				state = false;
				break;
			}
		}
		if (!subtask.isSuccess)
		{
			state = false;
		}
		AnimateBell(state);
		canRing = state;
	}

	private void SpawnPlate()
	{
		if (currentPlate != null)
		{
			if (JobBoardManager.instance != null && JobBoardManager.instance.EndlessModeStatusController != null && currentPlate.ToGoBoxAnimator != null && currentPlate.ToGoBoxAnimator.gameObject != null && currentPlate.ToGoBoxAnimator.transform.parent.gameObject == currentPlate.gameObject)
			{
				return;
			}
			UnityEngine.Object.Destroy(currentPlate.gameObject);
			currentPlate = null;
		}
		currentPlate = plateSpawner.SpawnPrefab().GetComponent<ServingPlate>();
	}

	public void BellRung()
	{
		if (currentPlate != null && currentPlate.ReadyToSendAway() && bellIsShown && canRing)
		{
			canRing = false;
			StartCoroutine(InternalBellRung());
		}
	}

	private void EndlessTaskComplete()
	{
		if (!JobBoardManager.instance.EndlessModeStatusController.GetCurrentGoal().IsCompleted)
		{
			JobBoardManager.instance.EndlessModeStatusController.ForceJobComplete(true, false);
		}
	}

	private IEnumerator InternalBellRung()
	{
		GameEventsManager.Instance.ItemActionOccurred(bellWorldItem.Data, "ACTIVATED");
		Transform midPoint = midwayPoint;
		Transform tableToUse = nextTablePosition;
		nextTablePosition = defaultTable;
		bool destroyPlateOnServe = false;
		if (GenieManager.AreAnyJobGenieModesActive() && GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode))
		{
			EndlessTaskComplete();
			yield return new WaitForSeconds(3f);
			AnimateBell(false);
			SpawnPlate();
			yield break;
		}
		for (int i = 0; i < servedPlates.Count; i++)
		{
			if (servedPlates[i].TableTransform == tableToUse)
			{
				servedPlates[i].RemoveFromTable();
				servedPlates.RemoveAt(i);
				i--;
			}
		}
		currentPlate.SendToTable(midPoint, tableToUse, destroyPlateOnServe);
		if (!GenieManager.AreAnyJobGenieModesActive() || !GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode))
		{
			servedPlates.Add(currentPlate);
		}
		currentPlate = null;
		yield return new WaitForSeconds(1f);
		AudioManager.Instance.Play(plateSpawner.transform.position, plateLeavingAudioClip, 0.25f, 1f);
		yield return new WaitForSeconds(1f);
		AnimateBell(false);
		SpawnPlate();
		if (canDoPlatesForever)
		{
			yield return BeginAllowingInfinitePlates(2f);
		}
	}

	private void AnimateBell(bool state)
	{
		if (state != bellIsShown)
		{
			bellIsShown = state;
			bellAnimation.Stop();
			bellAnimation.clip = ((!state) ? bellAnimationOut : bellAnimationIn);
			bellAnimation[bellAnimation.clip.name].speed = bellAnimationMultiplier;
			bellAnimation.Play();
			if (bellIsShown && bellRiseSound != null)
			{
				AudioManager.Instance.Play(bellTransform, bellRiseSound, 1f, 1f);
			}
			else if (bellLowerSound != null)
			{
				AudioManager.Instance.Play(bellTransform, bellLowerSound, 1f, 1f);
			}
		}
	}

	private void OnDrawGizmos()
	{
		if (tableInfos == null)
		{
			return;
		}
		Gizmos.color = Color.yellow;
		for (int i = 0; i < tableInfos.Length; i++)
		{
			if (tableInfos[i].TargetTable != null)
			{
				Gizmos.DrawSphere(tableInfos[i].TargetTable.position, 0.1f);
			}
		}
	}
}
