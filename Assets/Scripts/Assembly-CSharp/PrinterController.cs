using System;
using System.Collections;
using OwlchemyVR;
using UnityEngine;

public class PrinterController : MonoBehaviour
{
	[SerializeField]
	private WorldItem printerWorldItem;

	[SerializeField]
	private Animation scanAnimation;

	[SerializeField]
	private float actualScanTime = 2f;

	[SerializeField]
	private ItemCollectionZone itemCollectionZone;

	[SerializeField]
	private PlayerPartDetector playerPartDetector;

	[SerializeField]
	private GameObject paperPlanePrefab;

	[SerializeField]
	private GameObject lifelessLeftHandPrefab;

	[SerializeField]
	private GameObject lifelessRightHandPrefab;

	[SerializeField]
	private GameObject lifelessHeadPrefab;

	[SerializeField]
	private Transform spawnResultPosition;

	[SerializeField]
	private Transform spawnResultMoveTo;

	private Vector3 spawnResultMoveToPositionCache;

	[SerializeField]
	private AudioClip copySound;

	[SerializeField]
	private AudioClip printSound;

	[SerializeField]
	private PrintOnSubtaskComplete[] printObjectsOnSubtaskComplete;

	[SerializeField]
	private SwapWorldItemWithAlternate[] itemsToSwapOut;

	private bool isBusy;

	public bool IsBusy
	{
		get
		{
			return isBusy;
		}
	}

	private void Awake()
	{
		itemCollectionZone.SetIsSafeForAllPickupables(true);
		spawnResultMoveToPositionCache = spawnResultMoveTo.position;
	}

	private void OnEnable()
	{
		if (JobBoardManager.instance != null)
		{
			JobBoardManager instance = JobBoardManager.instance;
			instance.OnSubtaskComplete = (Action<SubtaskStatusController>)Delegate.Combine(instance.OnSubtaskComplete, new Action<SubtaskStatusController>(SubtaskCompleted));
		}
	}

	private void OnDisable()
	{
		if (JobBoardManager.instance != null)
		{
			JobBoardManager instance = JobBoardManager.instance;
			instance.OnSubtaskComplete = (Action<SubtaskStatusController>)Delegate.Remove(instance.OnSubtaskComplete, new Action<SubtaskStatusController>(SubtaskCompleted));
		}
	}

	private void SubtaskCompleted(SubtaskStatusController subtask)
	{
		for (int i = 0; i < printObjectsOnSubtaskComplete.Length; i++)
		{
			if (printObjectsOnSubtaskComplete[i].Subtask == subtask.Data)
			{
				PrintObject(printObjectsOnSubtaskComplete[i].ObjectToPrint);
				break;
			}
		}
	}

	public void CopyButtonPressed()
	{
		CopyObject();
	}

	public void CopyObject()
	{
		if (!isBusy)
		{
			isBusy = true;
			StartCoroutine(CopyObjectAsync());
		}
	}

	private IEnumerator CopyObjectAsync()
	{
		GameObject original = null;
		AudioManager.Instance.Play(base.transform.position, copySound, 1f, 1f);
		scanAnimation.Play();
		yield return new WaitForSeconds(actualScanTime);
		if (itemCollectionZone.ItemsInCollection.Count > 0)
		{
			original = itemCollectionZone.ItemsInCollection[0].gameObject;
			WorldItem ogWorldItem = original.GetComponent<WorldItem>();
			if (ogWorldItem != null)
			{
				for (int i = 0; i < itemsToSwapOut.Length; i++)
				{
					if (ogWorldItem.Data == itemsToSwapOut[i].SwapWorldItem)
					{
						original = itemsToSwapOut[i].AlternateGameObject;
					}
				}
			}
		}
		else if (playerPartDetector.DetectedHands.Count > 0)
		{
			InteractionHandController hand = playerPartDetector.DetectedHands[0];
			original = ((!(hand == GlobalStorage.Instance.MasterHMDAndInputController.RightHand)) ? lifelessLeftHandPrefab : lifelessRightHandPrefab);
		}
		else if (playerPartDetector.DetectedHead != null)
		{
			original = lifelessHeadPrefab;
		}
		while (scanAnimation.isPlaying)
		{
			yield return null;
		}
		isBusy = false;
		if (original != null)
		{
			PrintObject(original);
		}
	}

	public void PrintObject(GameObject prefab, WorldItemData widOverride = null, Action<GameObject> customizer = null)
	{
		if (!isBusy)
		{
			isBusy = true;
			StartCoroutine(PrintObjectAsync(prefab, widOverride, customizer));
		}
	}

	private IEnumerator PrintObjectAsync(GameObject prefab, WorldItemData widOverride = null, Action<GameObject> customizer = null)
	{
		if (prefab != null)
		{
			GameObject copied = UnityEngine.Object.Instantiate(prefab, spawnResultPosition.position, prefab.transform.rotation) as GameObject;
			BasePrefabSpawner copiedB = copied.GetComponent<BasePrefabSpawner>();
			if (copiedB != null)
			{
				copied = copiedB.LastSpawnedPrefabGO;
			}
			GrabbableItem grabItem = copied.GetComponent<GrabbableItem>();
			if (grabItem != null)
			{
				if (widOverride != null)
				{
					grabItem.InteractableItem.WorldItem.ManualSetData(widOverride);
				}
				Rigidbody rb = copied.GetComponent<Rigidbody>();
				if (rb != null)
				{
					bool useGravity = grabItem.CachedUseGravity;
					if (!useGravity)
					{
						Rigidbody originalRb = prefab.GetComponent<Rigidbody>();
						if (originalRb != null)
						{
							useGravity = originalRb.useGravity || useGravity;
						}
					}
					rb.useGravity = useGravity;
				}
			}
			BasePrefabSpawner b = copied.GetComponent<BasePrefabSpawner>();
			if (b != null)
			{
				copied = b.LastSpawnedPrefabGO;
			}
			copied.transform.SetParent(GlobalStorage.Instance.ContentRoot, true);
			copied.transform.localScale = prefab.transform.localScale;
			if (copied.GetComponent<AttachableObject>() != null)
			{
				yield return null;
				copied.GetComponent<AttachableObject>().Detach();
				copied.transform.position = spawnResultPosition.position;
				copied.transform.rotation = prefab.transform.rotation;
			}
			if (customizer != null)
			{
				customizer(copied);
			}
			AudioManager.Instance.Play(base.transform.position, printSound, 1f, 1f);
			if (grabItem != null && grabItem.Rigidbody != null)
			{
				grabItem.Rigidbody.isKinematic = true;
			}
			AdjustMoveToBasedOnObjectSize(prefab);
			Go.to(copied.transform, 1f, new GoTweenConfig().position(spawnResultMoveTo.position));
			yield return new WaitForSeconds(1.01f);
			if (grabItem != null && grabItem.Rigidbody != null)
			{
				grabItem.Rigidbody.isKinematic = false;
			}
			WorldItem worldItem = copied.GetComponentInChildren<WorldItem>();
			if (worldItem != null)
			{
				GameEventsManager.Instance.ItemAppliedToItemActionOccurred(printerWorldItem.Data, worldItem.Data, "CREATED_BY");
				GameEventsManager.Instance.ItemActionOccurred(worldItem.Data, "CREATED");
			}
			GameEventsManager.Instance.ItemActionOccurred(printerWorldItem.Data, "USED");
		}
		spawnResultMoveTo.position = spawnResultMoveToPositionCache;
		isBusy = false;
	}

	public void PlaneButtonPressed()
	{
		PrintObject(paperPlanePrefab);
	}

	public void AdjustMoveToBasedOnObjectSize(GameObject prefab)
	{
		Collider[] componentsInChildren = prefab.GetComponentsInChildren<Collider>();
		if (componentsInChildren != null)
		{
			Bounds bounds = new Bounds(prefab.transform.position, Vector3.zero);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				bounds.Encapsulate(componentsInChildren[i].bounds);
			}
			if (JobBoardManager.instance != null && JobBoardManager.instance.EndlessModeStatusController != null)
			{
				spawnResultMoveTo.position += new Vector3(0f, bounds.extents.y / base.transform.localScale.y, 0f);
			}
			else
			{
				spawnResultMoveTo.position += new Vector3(0f, bounds.extents.y / 2f, 0f);
			}
		}
	}
}
