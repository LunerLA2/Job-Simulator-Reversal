using System;
using OwlchemyVR;
using UnityEngine;

public class GameEngineCaseController : MonoBehaviour
{
	[SerializeField]
	private WorldItem caseHingeWorldItem;

	[SerializeField]
	private GrabbableHinge caseHinge;

	[SerializeField]
	private PageData pageForRoaches;

	[SerializeField]
	private GameObjectPrefabSpawner[] roachSpawners;

	private void OnEnable()
	{
		caseHinge.OnLowerLocked += CaseLowerLocked;
		caseHinge.OnLowerUnlocked += CaseLowerUnlocked;
		JobBoardManager instance = JobBoardManager.instance;
		instance.OnPageShown = (Action<PageStatusController>)Delegate.Combine(instance.OnPageShown, new Action<PageStatusController>(PageShown));
	}

	private void OnDisable()
	{
		caseHinge.OnLowerLocked -= CaseLowerLocked;
		caseHinge.OnLowerUnlocked -= CaseLowerUnlocked;
		JobBoardManager instance = JobBoardManager.instance;
		instance.OnPageShown = (Action<PageStatusController>)Delegate.Remove(instance.OnPageShown, new Action<PageStatusController>(PageShown));
	}

	private void CaseLowerLocked(GrabbableHinge hinge, bool isInitial)
	{
		GameEventsManager.Instance.ItemActionOccurred(caseHingeWorldItem.Data, "CLOSED");
	}

	private void CaseLowerUnlocked(GrabbableHinge hinge)
	{
		GameEventsManager.Instance.ItemActionOccurred(caseHingeWorldItem.Data, "OPENED");
	}

	private void PageShown(PageStatusController page)
	{
		if (page.Data == pageForRoaches)
		{
			for (int i = 0; i < roachSpawners.Length; i++)
			{
				roachSpawners[i].SpawnPrefabGO();
			}
		}
	}
}
