using System;
using System.Collections;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class GameDeveloperDataStorage : MonoBehaviour
{
	public enum GameSetting
	{
		RPG = 0,
		Horror = 1,
		Sports = 2
	}

	public static GameDeveloperDataStorage Instance;

	[SerializeField]
	private GameObject gameLevelHolder;

	[SerializeField]
	private PageData putOnHMDPage;

	[SerializeField]
	private WorldItemData worldItemDataToActivateOnComplete;

	[SerializeField]
	private SubtaskData subtaskToCheckDesignDoc;

	[SerializeField]
	private BrainData brainToGrabDesignDoc;

	[SerializeField]
	private WorldItemData designDocRPGData;

	[SerializeField]
	private WorldItemData designDocHorrorData;

	[SerializeField]
	private WorldItemData designDocSportsData;

	[SerializeField]
	private SubtaskData subtaskToCheckArtwork;

	[SerializeField]
	private BrainData brainToGrabArtwork;

	[SerializeField]
	private WorldItemData artworkWorldItemData;

	[SerializeField]
	private GameObjectPrefabSpawner[] defaultBossObjects;

	[SerializeField]
	private MeshRenderer bossFaceRenderer;

	[SerializeField]
	private ItemCollectionZone objectZone;

	[SerializeField]
	private Transform objectRespawnLocation;

	[SerializeField]
	private Vector3 objectRespawnDistance = Vector3.one;

	[SerializeField]
	private Animation defaultBossAnimation;

	[SerializeField]
	private AnimationClip bossDeathClip;

	[SerializeField]
	private Transform bossHeadTransform;

	[SerializeField]
	private Transform bossLeftHandTransform;

	[SerializeField]
	private Transform bossRightHandTransform;

	[SerializeField]
	private float mocapScaleFactor = 1f;

	[SerializeField]
	private GameObject[] artRPG;

	[SerializeField]
	private GameObject[] artSports;

	[SerializeField]
	private GameObject[] artHorror;

	[SerializeField]
	private GameObject hitEffect;

	[SerializeField]
	private AudioClip hitSound;

	[SerializeField]
	private ItemCollectionZone hitbox;

	private int hits;

	private MoCappableObject headMocapInfo;

	private MoCappableObject leftHandMocapInfo;

	private MoCappableObject rightHandMocapInfo;

	private bool usingDefaultBossObjects = true;

	private int mocapFrame;

	private bool isAnimatingMocap;

	private bool hasStarted;

	public GameSetting gameSetting;

	public Texture bossTexture;

	public List<MoCappableObject> mocapObjects = new List<MoCappableObject>();

	public List<PickupableItem> bossObjects = new List<PickupableItem>();

	private List<PickupableItem> itemsPendingRespawn = new List<PickupableItem>();

	private bool shouldCheck;

	private bool hasDied;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		gameLevelHolder.SetActive(false);
		if (GenieManager.AreAnyJobGenieModesActive() && GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.OfficeModMode))
		{
			shouldCheck = true;
		}
		if (OfficeManager.Instance.AlwaysLoadGameDevJob)
		{
			shouldCheck = true;
		}
	}

	private void OnEnable()
	{
		JobBoardManager instance = JobBoardManager.instance;
		instance.OnSubtaskComplete = (Action<SubtaskStatusController>)Delegate.Combine(instance.OnSubtaskComplete, new Action<SubtaskStatusController>(SubtaskCompleted));
		JobBoardManager instance2 = JobBoardManager.instance;
		instance2.OnPageComplete = (Action<PageStatusController>)Delegate.Combine(instance2.OnPageComplete, new Action<PageStatusController>(PageCompleted));
		ItemCollectionZone itemCollectionZone = objectZone;
		itemCollectionZone.OnItemsInCollectionRemoved = (Action<ItemCollectionZone, PickupableItem>)Delegate.Combine(itemCollectionZone.OnItemsInCollectionRemoved, new Action<ItemCollectionZone, PickupableItem>(ItemRemovedFromZone));
		ItemCollectionZone itemCollectionZone2 = hitbox;
		itemCollectionZone2.OnItemsInCollectionAdded = (Action<ItemCollectionZone, PickupableItem>)Delegate.Combine(itemCollectionZone2.OnItemsInCollectionAdded, new Action<ItemCollectionZone, PickupableItem>(Hit));
	}

	private void OnDisable()
	{
		JobBoardManager instance = JobBoardManager.instance;
		instance.OnSubtaskComplete = (Action<SubtaskStatusController>)Delegate.Remove(instance.OnSubtaskComplete, new Action<SubtaskStatusController>(SubtaskCompleted));
		JobBoardManager instance2 = JobBoardManager.instance;
		instance2.OnPageComplete = (Action<PageStatusController>)Delegate.Remove(instance2.OnPageComplete, new Action<PageStatusController>(PageCompleted));
		ItemCollectionZone itemCollectionZone = objectZone;
		itemCollectionZone.OnItemsInCollectionRemoved = (Action<ItemCollectionZone, PickupableItem>)Delegate.Remove(itemCollectionZone.OnItemsInCollectionRemoved, new Action<ItemCollectionZone, PickupableItem>(ItemRemovedFromZone));
		ItemCollectionZone itemCollectionZone2 = hitbox;
		itemCollectionZone2.OnItemsInCollectionAdded = (Action<ItemCollectionZone, PickupableItem>)Delegate.Remove(itemCollectionZone2.OnItemsInCollectionAdded, new Action<ItemCollectionZone, PickupableItem>(Hit));
	}

	private void ItemRemovedFromZone(ItemCollectionZone zone, PickupableItem item)
	{
		if (!itemsPendingRespawn.Contains(item))
		{
			itemsPendingRespawn.Add(item);
			StartCoroutine(ItemRemoved(item));
		}
	}

	private IEnumerator ItemRemoved(PickupableItem item)
	{
		yield return new WaitForSeconds(3f);
		if (itemsPendingRespawn.Contains(item))
		{
			itemsPendingRespawn.Remove(item);
		}
		if (!objectZone.ItemsInCollection.Contains(item))
		{
			SpawnItem(item);
		}
	}

	private void SpawnItem(PickupableItem prefab)
	{
		if (!(prefab == null))
		{
			PickupableItem pickupableItem = UnityEngine.Object.Instantiate(prefab);
			pickupableItem.transform.position = objectRespawnLocation.position;
			pickupableItem.transform.SetParent(GlobalStorage.Instance.ContentRoot, true);
			Vector3 vector = new Vector3(UnityEngine.Random.Range((0f - objectRespawnDistance.x) / 2f, objectRespawnDistance.x / 2f), UnityEngine.Random.Range((0f - objectRespawnDistance.y) / 2f, objectRespawnDistance.y / 2f), UnityEngine.Random.Range((0f - objectRespawnDistance.z) / 2f, objectRespawnDistance.z / 2f));
			pickupableItem.transform.position += vector;
			pickupableItem.enabled = true;
			Rigidbody[] componentsInChildren = pickupableItem.GetComponentsInChildren<Rigidbody>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].isKinematic = false;
				componentsInChildren[i].useGravity = true;
			}
			pickupableItem.transform.localScale = Vector3.one;
			pickupableItem.gameObject.SetActive(true);
		}
	}

	private void Update()
	{
		if (isAnimatingMocap)
		{
			bossHeadTransform.localPosition = headMocapInfo.GetPositionForFrame(mocapFrame) * mocapScaleFactor;
			bossHeadTransform.localRotation = headMocapInfo.GetRotationForFrame(mocapFrame);
			bossLeftHandTransform.localPosition = leftHandMocapInfo.GetPositionForFrame(mocapFrame) * mocapScaleFactor;
			bossLeftHandTransform.localRotation = leftHandMocapInfo.GetRotationForFrame(mocapFrame);
			bossRightHandTransform.localPosition = rightHandMocapInfo.GetPositionForFrame(mocapFrame) * mocapScaleFactor;
			bossRightHandTransform.localRotation = rightHandMocapInfo.GetRotationForFrame(mocapFrame);
			mocapFrame++;
		}
	}

	private void PageCompleted(PageStatusController page)
	{
		if (shouldCheck && page.Data == putOnHMDPage && !hasStarted)
		{
			hasStarted = true;
			StartCoroutine(BeginGame());
		}
	}

	private IEnumerator BeginGame()
	{
		ScreenFader.Instance.FadeOut(0.3f);
		yield return new WaitForSeconds(1f);
		MasterHMDAndInputController masterHMDAndInput = GlobalStorage.no_instantiate_instance.MasterHMDAndInputController;
		if (masterHMDAndInput != null)
		{
			Debug.Log("Cleanup head");
			masterHMDAndInput.Head.Cleanup();
			if (masterHMDAndInput.LeftHand != null)
			{
				Debug.Log("LeftHandCleanup");
				masterHMDAndInput.LeftHand.TryRelease();
			}
			if (masterHMDAndInput.RightHand != null)
			{
				Debug.Log("RightHandCleanup");
				masterHMDAndInput.RightHand.TryRelease();
			}
		}
		for (int n = 0; n < artRPG.Length; n++)
		{
			artRPG[n].SetActive(gameSetting == GameSetting.RPG);
		}
		for (int m = 0; m < artSports.Length; m++)
		{
			artSports[m].SetActive(gameSetting == GameSetting.Sports);
		}
		for (int l = 0; l < artHorror.Length; l++)
		{
			artHorror[l].SetActive(gameSetting == GameSetting.Horror);
		}
		GlobalStorage.Instance.MasterHMDAndInputController.transform.position = Vector3.forward * 300f;
		gameLevelHolder.SetActive(true);
		if (usingDefaultBossObjects)
		{
			bossObjects.Clear();
			for (int k = 0; k < defaultBossObjects.Length; k++)
			{
				GameObject go = defaultBossObjects[k].SpawnPrefab();
				bossObjects.Add(go.GetComponent<PickupableItem>());
			}
		}
		for (int j = 0; j < bossObjects.Count; j++)
		{
			bossObjects[j].enabled = false;
			if (bossObjects[j].Rigidbody != null)
			{
				bossObjects[j].Rigidbody.isKinematic = true;
			}
			bossObjects[j].gameObject.SetActive(false);
			SpawnItem(bossObjects[j]);
		}
		bossFaceRenderer.material.mainTexture = bossTexture;
		for (int i = 0; i < mocapObjects.Count; i++)
		{
			if (mocapObjects[i].MoCapType == MoCappableObject.MoCapTypes.Head)
			{
				headMocapInfo = mocapObjects[i];
			}
			else if (mocapObjects[i].MoCapType == MoCappableObject.MoCapTypes.LeftHand)
			{
				leftHandMocapInfo = mocapObjects[i];
			}
			else if (mocapObjects[i].MoCapType == MoCappableObject.MoCapTypes.RightHand)
			{
				rightHandMocapInfo = mocapObjects[i];
			}
		}
		if (mocapObjects.Count < 3 || headMocapInfo == null || leftHandMocapInfo == null || rightHandMocapInfo == null)
		{
			defaultBossAnimation.Play();
		}
		else
		{
			isAnimatingMocap = true;
		}
		ScreenFader.Instance.FadeIn(1f);
		yield return new WaitForSeconds(2f);
		GameEventsManager.Instance.ScriptedCauseOccurred("GameStart" + gameSetting);
	}

	private void Hit(ItemCollectionZone zone, PickupableItem item)
	{
		hits++;
		UnityEngine.Object.Instantiate(hitEffect, item.transform.position, Quaternion.identity);
		AudioManager.Instance.Play(item.transform.position, hitSound, 1f, UnityEngine.Random.Range(0.7f, 1.2f));
		UnityEngine.Object.Destroy(item.gameObject);
		GameEventsManager.Instance.ScriptedCauseOccurred("BossHit" + hits);
		if (!hasDied && hits >= 3)
		{
			hasDied = true;
			StartCoroutine(Die());
		}
	}

	private IEnumerator Die()
	{
		isAnimatingMocap = false;
		defaultBossAnimation.clip = bossDeathClip;
		defaultBossAnimation.Play();
		yield return new WaitForSeconds(2f);
		GameEventsManager.Instance.ItemActionOccurred(worldItemDataToActivateOnComplete, "ACTIVATED");
	}

	public void RegisterBossObject(PickupableItem item)
	{
		if (usingDefaultBossObjects)
		{
			usingDefaultBossObjects = false;
			bossObjects.Clear();
		}
		item.enabled = false;
		if (item.Rigidbody != null)
		{
			item.Rigidbody.isKinematic = true;
		}
		item.gameObject.SetActive(false);
		item.gameObject.transform.localScale = Vector3.one;
		if (!bossObjects.Contains(item))
		{
			bossObjects.Add(item);
		}
	}

	private void SubtaskCompleted(SubtaskStatusController subtask)
	{
		if (!shouldCheck)
		{
			return;
		}
		List<BrainStatusController> brainStatusControllers = BotManager.Instance.BrainGroupStatus.BrainStatusControllers;
		for (int i = 0; i < brainStatusControllers.Count; i++)
		{
			if (subtask.Data == subtaskToCheckDesignDoc && brainStatusControllers[i].Brain == brainToGrabDesignDoc)
			{
				Bot bot = brainStatusControllers[i].Controlled as Bot;
				if (!(bot != null))
				{
					continue;
				}
				for (int j = 0; j < bot.InventoryController.CurrentInventory.Count; j++)
				{
					BotInventoryEntry botInventoryEntry = bot.InventoryController.CurrentInventory[j];
					if (botInventoryEntry.GrabbableItem != null)
					{
						if (botInventoryEntry.GrabbableItem.InteractableItem.WorldItemData == designDocRPGData)
						{
							gameSetting = GameSetting.RPG;
							Debug.Log("Game setting set to RPG");
						}
						if (botInventoryEntry.GrabbableItem.InteractableItem.WorldItemData == designDocSportsData)
						{
							gameSetting = GameSetting.Sports;
							Debug.Log("Game setting set to Sports");
						}
						if (botInventoryEntry.GrabbableItem.InteractableItem.WorldItemData == designDocHorrorData)
						{
							gameSetting = GameSetting.Horror;
							Debug.Log("Game setting set to Horror");
						}
					}
				}
			}
			else
			{
				if (!(subtask.Data == subtaskToCheckArtwork) || !(brainStatusControllers[i].Brain == brainToGrabArtwork))
				{
					continue;
				}
				Bot bot2 = brainStatusControllers[i].Controlled as Bot;
				if (!(bot2 != null))
				{
					continue;
				}
				for (int k = 0; k < bot2.InventoryController.CurrentInventory.Count; k++)
				{
					BotInventoryEntry botInventoryEntry2 = bot2.InventoryController.CurrentInventory[k];
					if (!(botInventoryEntry2.GrabbableItem != null) || !(botInventoryEntry2.GrabbableItem.InteractableItem.WorldItemData == artworkWorldItemData))
					{
						continue;
					}
					FramedPhoto component = botInventoryEntry2.GrabbableItem.gameObject.GetComponent<FramedPhoto>();
					if (component != null)
					{
						bossTexture = component.GetCustomPictureTexture();
						if (bossTexture != null)
						{
							Debug.Log("successfully grabbed picture texture");
						}
						else
						{
							Debug.LogError("grabbed picture texture but it was null");
						}
					}
				}
			}
		}
	}

	private void OnDrawGizmos()
	{
		if (objectRespawnLocation != null)
		{
			Gizmos.color = Color.cyan;
			Gizmos.DrawWireCube(objectRespawnLocation.position, objectRespawnDistance);
		}
	}
}
