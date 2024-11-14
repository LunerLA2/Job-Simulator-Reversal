using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class BotManager : MonoBehaviour
{
	private static BotManager _instance;

	[SerializeField]
	private Bot botPrefab;

	[SerializeField]
	private BotFaceController botFaceControllerPrefab;

	private BrainGroupStatusController brainGroupStatus;

	private Transform botParent;

	private ObjectPool<Bot> botPool;

	private Transform faceParent;

	private ObjectPool<BotFaceController> uniqueFacePool;

	private BotFaceController simpleFace;

	private int botsUsingSimpleFace;

	private Transform costumeParent;

	private List<PreloadedCostume> preloadedCostumes;

	private EndlessModeData endlessModeData;

	public static BotManager Instance
	{
		get
		{
			return _instance;
		}
	}

	public Transform BotParent
	{
		get
		{
			return botParent;
		}
	}

	public BrainGroupStatusController BrainGroupStatus
	{
		get
		{
			return brainGroupStatus;
		}
	}

	private void Awake()
	{
		if (_instance != this)
		{
			if (_instance != null)
			{
				Debug.LogWarning("There were 2 instances of BotManager in your scene!");
				Object.Destroy(_instance);
			}
			_instance = this;
		}
		InitializeBasics();
	}

	private void InitializeBasics()
	{
		botParent = new GameObject("Bots").transform;
		botParent.SetParent(base.transform, false);
		botPool = new ObjectPool<Bot>(botPrefab, 6, true, true, botParent, Vector3.zero);
		faceParent = new GameObject("Faces").transform;
		faceParent.SetParent(base.transform, false);
		uniqueFacePool = new ObjectPool<BotFaceController>(botFaceControllerPrefab, 3, true, true, faceParent, Vector3.down);
		simpleFace = Object.Instantiate(botFaceControllerPrefab, Vector3.down + Vector3.right, Quaternion.identity) as BotFaceController;
		simpleFace.gameObject.name = "SimpleBotFace";
		simpleFace.transform.SetParent(faceParent);
		simpleFace.SetAsSimple(true);
		simpleFace.gameObject.SetActive(false);
		faceParent.position += Vector3.down * 5f;
		preloadedCostumes = new List<PreloadedCostume>();
	}

	public void InitializeJob(JobData jobData)
	{
		costumeParent = new GameObject("Costumes").transform;
		costumeParent.SetParent(base.transform, false);
		preloadedCostumes = new List<PreloadedCostume>();
		if (jobData != null)
		{
			for (int i = 0; i < jobData.DesiredBrainGroup.BrainDatas.Length; i++)
			{
				if (jobData.DesiredBrainGroup.BrainDatas[i].BrainType == BrainData.BrainTypes.Bot)
				{
					BotCostumeData costumeData = jobData.DesiredBrainGroup.BrainDatas[i].GetCostumeData();
					if (costumeData != null)
					{
						preloadedCostumes.Add(new PreloadedCostume(costumeData, costumeParent));
					}
					else
					{
						Debug.LogError("Costume wasn't set for '" + jobData.DesiredBrainGroup.BrainDatas[i].name + "'");
					}
				}
			}
		}
		if (jobData != null)
		{
			BrainGroupData desiredBrainGroup = jobData.DesiredBrainGroup;
			brainGroupStatus = new BrainGroupStatusController(desiredBrainGroup);
			brainGroupStatus.Begin();
		}
		else
		{
			Debug.LogError("BotManager initialized with no jobData");
		}
	}

	public void InitializeEndlessMode(EndlessModeData modeData)
	{
		costumeParent = new GameObject("Costumes").transform;
		costumeParent.SetParent(base.transform, false);
		preloadedCostumes = new List<PreloadedCostume>();
		endlessModeData = modeData;
		if (endlessModeData != null)
		{
			BrainGroupData desiredBrainGroup = endlessModeData.DesiredBrainGroup;
			brainGroupStatus = new BrainGroupStatusController(desiredBrainGroup);
			brainGroupStatus.Begin();
		}
		else
		{
			Debug.LogError("BotManager initialized with no endlessModeData");
		}
	}

	public Bot GetBot()
	{
		return botPool.Fetch(Vector3.forward * 500f, Quaternion.identity);
	}

	public void ReleaseBot(Bot bot)
	{
		botPool.Release(bot);
	}

	public BotFaceController GetUniqueFace()
	{
		BotFaceController botFaceController = uniqueFacePool.Fetch();
		botFaceController.Reset();
		return botFaceController;
	}

	public void ReleaseUniqueFace(BotFaceController face)
	{
		face.Reset();
		uniqueFacePool.Release(face);
	}

	public BotFaceController GetSimpleFace()
	{
		if (Bot.USE_NON_RENDERTEXTURE_SIMPLEFACE)
		{
			simpleFace.gameObject.SetActive(false);
		}
		else
		{
			simpleFace.gameObject.SetActive(true);
		}
		botsUsingSimpleFace++;
		return simpleFace;
	}

	public void ReleaseSimpleFace()
	{
		botsUsingSimpleFace--;
		if (botsUsingSimpleFace <= 0)
		{
			simpleFace.gameObject.SetActive(false);
		}
	}

	public PreloadedCostume GetCostume(BotCostumeData data)
	{
		for (int i = 0; i < preloadedCostumes.Count; i++)
		{
			if (preloadedCostumes[i].Data == data && !preloadedCostumes[i].claimed)
			{
				preloadedCostumes[i].claimed = true;
				return preloadedCostumes[i];
			}
		}
		PreloadedCostume preloadedCostume = new PreloadedCostume(data, costumeParent);
		preloadedCostumes.Add(preloadedCostume);
		return preloadedCostume;
	}

	public BotCostumeData GetRandomCostume()
	{
		if (endlessModeData == null)
		{
			Debug.LogError("Failed to get random BotCostumeData because there is no EndlessModeData set", base.gameObject);
			return null;
		}
		CostumePiece costumePiece = null;
		List<CostumePiece> list = new List<CostumePiece>();
		if (endlessModeData.BotBodyOptions != null && Random.value >= 0.5f)
		{
			costumePiece = endlessModeData.BotBodyOptions.CostumePiecePrefabs[Random.Range(0, endlessModeData.BotBodyOptions.CostumePiecePrefabs.Length)];
			list.Add(costumePiece);
		}
		AttachableObject attachableObject = null;
		if (endlessModeData.BotGlassesOptions != null && Random.value >= 0.5f)
		{
			attachableObject = endlessModeData.BotGlassesOptions.CostumePiecePrefabs[Random.Range(0, endlessModeData.BotGlassesOptions.CostumePiecePrefabs.Length)].ArtPrefab.GetComponent<AttachableObject>();
		}
		AttachableObject hat = null;
		if ((endlessModeData.BotHatOptions != null && Random.value >= 0.5f) || (endlessModeData.BotHatOptions != null && attachableObject == null))
		{
			hat = endlessModeData.BotHatOptions.CostumePiecePrefabs[Random.Range(0, endlessModeData.BotHatOptions.CostumePiecePrefabs.Length)].ArtPrefab.GetComponent<AttachableObject>();
		}
		return BotCostumeData.CreateInstance(list.ToArray(), attachableObject, hat, Random.ColorHSV(0f, 1f, 0.5f, 0.8f, 0.7f, 0.85f, 1f, 1f));
	}

	public void ScriptedCauseOccurred(string msg)
	{
		if (brainGroupStatus != null)
		{
			brainGroupStatus.ScriptedCauseHappened(msg);
		}
	}

	public void ActionOccurred(ActionEventData actionEventData)
	{
		if (brainGroupStatus != null)
		{
			brainGroupStatus.ActionOccurred(actionEventData);
		}
	}

	public void ItemActionOccurred(ActionEventData actionEventData, WorldItemData worldItemData)
	{
		if (brainGroupStatus != null)
		{
			brainGroupStatus.ItemActionOccurred(actionEventData, worldItemData);
		}
	}

	public void ItemActionOccurredWithAmount(ActionEventData actionEventData, WorldItemData worldItemData, float amount)
	{
		if (brainGroupStatus != null)
		{
			brainGroupStatus.ItemActionOccurredWithAmount(actionEventData, worldItemData, amount);
		}
	}

	public void ItemAppliedToItemActionOccurred(ActionEventData actionEventData, WorldItemData worldItemData, WorldItemData appliedToWorldItemData)
	{
		if (brainGroupStatus != null)
		{
			brainGroupStatus.ItemAppliedToItemActionOccurred(actionEventData, worldItemData, appliedToWorldItemData);
		}
	}

	public void ItemAppliedToItemActionOccurredWithAmount(ActionEventData actionEventData, WorldItemData worldItemData, WorldItemData appliedToWorldItemData, float amount)
	{
		if (brainGroupStatus != null)
		{
			brainGroupStatus.ItemAppliedToItemActionOccurredWithAmount(actionEventData, worldItemData, appliedToWorldItemData, amount);
		}
	}
}
