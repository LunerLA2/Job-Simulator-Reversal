using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OwlchemyVR;
using UnityEngine;

public class KitchenToolStasher : ModeContainer
{
	private class StashedItemInfo
	{
		public PickupableItem Item;

		public Transform CachedParent;

		public bool WasKinematic;
	}

	private const float FLAPS_CLOSED_DURATION = 0.25f;

	[SerializeField]
	private Transform toolSpawnersRoot;

	[SerializeField]
	private Animation liftAnimation;

	[SerializeField]
	private Animation flapsAnimation;

	[SerializeField]
	private AnimationClip liftRaiseAnimClip;

	[SerializeField]
	private AnimationClip liftLowerAnimClip;

	[SerializeField]
	private AnimationClip flapsOpenAnimClip;

	[SerializeField]
	private AnimationClip flapsCloseAnimClip;

	[SerializeField]
	private Transform counterLeft;

	[SerializeField]
	private Transform counterRight;

	[SerializeField]
	private Transform counterFront;

	[SerializeField]
	private Transform counterBack;

	[SerializeField]
	private ItemCollectionZone itemCollectionZone;

	[SerializeField]
	private Transform stashedItemsRoot;

	[SerializeField]
	private Transform interfaceAudioSource;

	[SerializeField]
	private AudioClip errorSound;

	[SerializeField]
	private AudioClip toolLoweringSound;

	[SerializeField]
	private AudioClip doorsClosingSound;

	[SerializeField]
	private AudioClip toolRaisingSound;

	public Action<int> OnWillSwitchToToolIndex;

	private GameObjectPrefabSpawner[] toolSpawners;

	private KitchenTool[] tools;

	private GameObject[] toolGos;

	private List<StashedItemInfo>[] stashedItems;

	private Vector3[] cachedDesiredLocalToolPositions;

	private Quaternion[] cachedDesiredLocalToolRotations;

	private Transform toolTemporaryParent;

	public KitchenTool SelectedTool
	{
		get
		{
			if (modeIndex != -1)
			{
				return tools[modeIndex];
			}
			return null;
		}
	}

	public void ForceRemoveItemFromStashing(PickupableItem item)
	{
		if (itemCollectionZone.ItemsInCollection.Contains(item))
		{
			itemCollectionZone.ItemsInCollection.Remove(item);
		}
	}

	private void Awake()
	{
		toolTemporaryParent = new GameObject("TemporaryToolParent").transform;
		toolTemporaryParent.SetParent(base.transform);
		toolTemporaryParent.SetToDefaultPosRotScale();
		liftAnimation.transform.localPosition = Vector3.zero;
		toolSpawners = new GameObjectPrefabSpawner[toolSpawnersRoot.childCount];
		for (int i = 0; i < toolSpawnersRoot.childCount; i++)
		{
			toolSpawners[i] = toolSpawnersRoot.GetChild(i).GetComponent<GameObjectPrefabSpawner>();
		}
		tools = new KitchenTool[toolSpawners.Length];
		toolGos = new GameObject[toolSpawners.Length];
		stashedItems = new List<StashedItemInfo>[tools.Length];
	}

	protected override void Start()
	{
		PreloadAllTools();
		base.Start();
	}

	private void PreloadAllTools()
	{
		cachedDesiredLocalToolPositions = new Vector3[toolSpawners.Length];
		cachedDesiredLocalToolRotations = new Quaternion[toolSpawners.Length];
		for (int i = 0; i < tools.Length; i++)
		{
			stashedItems[i] = new List<StashedItemInfo>();
			KitchenTool orSpawnTool = GetOrSpawnTool(i);
			if (orSpawnTool != null)
			{
				cachedDesiredLocalToolPositions[i] = orSpawnTool.transform.localPosition;
				cachedDesiredLocalToolRotations[i] = orSpawnTool.transform.localRotation;
				orSpawnTool.gameObject.SetActive(false);
				orSpawnTool.transform.SetParent(toolTemporaryParent);
				orSpawnTool.SetParentChooser(this);
			}
			else
			{
				Debug.LogError("ToolStasher (" + base.gameObject.name + ") tool #" + i + " isn't set up with a KitchenTool script - chooser will not work correctly");
			}
		}
	}

	private KitchenTool GetOrSpawnTool(int toolIndex)
	{
		if (toolIndex == -1)
		{
			return null;
		}
		if (toolGos[toolIndex] == null)
		{
			if (toolSpawners[toolIndex] == null || toolSpawners[toolIndex].prefab == null)
			{
				return null;
			}
			GameObject gameObject = toolSpawners[toolIndex].SpawnPrefab();
			if (gameObject != null)
			{
				toolGos[toolIndex] = gameObject;
				tools[toolIndex] = gameObject.GetComponent<KitchenTool>();
			}
		}
		return tools[toolIndex];
	}

	protected override IEnumerator ChangeModeAsync(int newModeIndex)
	{
		if (newModeIndex == modeIndex)
		{
			yield break;
		}
		bool doingStashing = true;
		if (modeIndex != -1)
		{
			doingStashing = tools[modeIndex].UseItemStashingLogic;
		}
		if (doingStashing)
		{
			PickupableItem[] itemsInZone = itemCollectionZone.ItemsInCollection.Where((PickupableItem item) => item != null && item.Rigidbody != null && !item.Rigidbody.isKinematic).ToArray();
			for (int j = 0; j < itemsInZone.Length; j++)
			{
				itemsInZone[j].Rigidbody.AddForce(0f, 0.01f, 0f, ForceMode.Impulse);
			}
		}
		if (OnWillSwitchToToolIndex != null)
		{
			OnWillSwitchToToolIndex(newModeIndex);
		}
		if (modeIndex != -1)
		{
			GameObject toolGo = toolGos[modeIndex];
			KitchenTool tool = tools[modeIndex];
			if (tool != null)
			{
				if (tool.IsToolBusy)
				{
					AudioManager.Instance.Play(interfaceAudioSource.position, errorSound, 1f, 1f);
					modeChangeFailed = true;
					yield break;
				}
				GameEventsManager.Instance.ItemActionOccurred(tool.GetComponent<WorldItem>().Data, "CLOSED");
				tool.OnDismiss();
			}
			liftAnimation.Stop();
			liftAnimation.Play(liftLowerAnimClip.name);
			if (toolLoweringSound != null)
			{
				AudioManager.Instance.Play(interfaceAudioSource.position, toolLoweringSound, 1f, 1f);
			}
			while (liftAnimation.isPlaying)
			{
				yield return null;
			}
			liftAnimation[liftLowerAnimClip.name].normalizedTime = 1f;
			flapsAnimation.Stop();
			flapsAnimation.Play(flapsCloseAnimClip.name);
			if (doorsClosingSound != null)
			{
				AudioManager.Instance.Play(interfaceAudioSource.position, doorsClosingSound, 1f, 2.3f);
			}
			while (flapsAnimation.isPlaying)
			{
				yield return null;
			}
			flapsAnimation[flapsCloseAnimClip.name].normalizedTime = 1f;
			yield return new WaitForSeconds(0.25f);
			if (toolGo != null)
			{
				toolGo.SetActive(false);
				toolGo.transform.SetParent(toolTemporaryParent, true);
			}
			bool doStashing = true;
			if (tool != null)
			{
				doStashing = tool.UseItemStashingLogic;
				tool.WasCompletelyDismissed();
			}
			if (doStashing)
			{
				PickupableItem[] stashableItems = itemCollectionZone.ItemsInCollection.Where((PickupableItem item) => item != null).ToArray();
				foreach (PickupableItem item4 in stashableItems)
				{
					StashedItemInfo itemInfo3 = new StashedItemInfo
					{
						Item = item4,
						CachedParent = item4.transform.parent
					};
					if (item4.IsCurrInHand)
					{
						item4.CurrInteractableHand.TryRelease();
					}
					item4.transform.SetParent(stashedItemsRoot, true);
					item4.gameObject.SetActive(false);
					if (item4.Rigidbody != null)
					{
						itemInfo3.WasKinematic = item4.Rigidbody.isKinematic;
						item4.Rigidbody.isKinematic = true;
					}
					stashedItems[modeIndex].Add(itemInfo3);
				}
			}
		}
		if (newModeIndex == -1)
		{
			yield break;
		}
		KitchenTool tool2 = GetOrSpawnTool(newModeIndex);
		GameObject toolGo2 = toolGos[newModeIndex];
		bool getStashed = true;
		if (tool2 != null)
		{
			tool2.BeganBeingSummoned();
			getStashed = tool2.UseItemStashingLogic;
		}
		if (toolGo2 != null)
		{
			toolGo2.transform.SetParent(toolSpawnersRoot, true);
			toolGo2.transform.localPosition = cachedDesiredLocalToolPositions[newModeIndex];
			toolGo2.transform.localRotation = cachedDesiredLocalToolRotations[newModeIndex];
			toolGo2.SetActive(true);
		}
		if (getStashed)
		{
			for (int k = 0; k < stashedItems[newModeIndex].Count; k++)
			{
				StashedItemInfo itemInfo2 = stashedItems[newModeIndex][k];
				PickupableItem item3 = itemInfo2.Item;
				if (item3 != null)
				{
					item3.gameObject.SetActive(true);
				}
			}
		}
		Vector3 holePos = base.transform.position;
		Vector3 holeSize = Vector3.zero;
		if (tool2 != null && tool2.CounterHole != null)
		{
			holePos = tool2.CounterHole.transform.TransformPoint(tool2.CounterHole.center);
			holeSize = tool2.CounterHole.transform.localScale;
			holeSize.x *= tool2.CounterHole.size.x;
			holeSize.z *= tool2.CounterHole.size.z;
		}
		ClampCounterAround(holePos, holeSize);
		flapsAnimation.Stop();
		flapsAnimation.Play(flapsOpenAnimClip.name);
		while (flapsAnimation.isPlaying)
		{
			yield return null;
		}
		flapsAnimation[flapsOpenAnimClip.name].normalizedTime = 1f;
		liftAnimation.Stop();
		liftAnimation.Play(liftRaiseAnimClip.name);
		if (toolRaisingSound != null)
		{
			AudioManager.Instance.Play(interfaceAudioSource.position, toolRaisingSound, 1f, 1f);
		}
		while (liftAnimation.isPlaying)
		{
			yield return null;
		}
		liftAnimation[liftRaiseAnimClip.name].normalizedTime = 1f;
		if (getStashed)
		{
			for (int i = 0; i < stashedItems[newModeIndex].Count; i++)
			{
				StashedItemInfo itemInfo = stashedItems[newModeIndex][i];
				PickupableItem item2 = itemInfo.Item;
				if (item2 != null)
				{
					item2.transform.SetParent(itemInfo.CachedParent);
					if (item2.Rigidbody != null)
					{
						item2.Rigidbody.isKinematic = itemInfo.WasKinematic;
					}
				}
			}
			stashedItems[newModeIndex].Clear();
		}
		if (tool2 != null)
		{
			GameEventsManager.Instance.ItemActionOccurred(tool2.GetComponent<WorldItem>().Data, "OPENED");
			tool2.OnSummon();
		}
	}

	private void ClampCounterAround(Vector3 holeCenter, Vector3 holeSize)
	{
		if (counterLeft != null)
		{
			Vector3 localScale = counterLeft.localScale;
			localScale.x = Vector3.Dot(holeCenter - counterLeft.position, base.transform.right) - holeSize.x / 2f;
			counterLeft.localScale = localScale;
		}
		if (counterRight != null)
		{
			Vector3 localScale2 = counterRight.localScale;
			localScale2.x = Vector3.Dot(holeCenter - counterRight.position, -base.transform.right) - holeSize.x / 2f;
			counterRight.localScale = localScale2;
		}
		if (counterFront != null)
		{
			Vector3 localScale3 = counterFront.localScale;
			localScale3.z = Vector3.Dot(holeCenter - counterFront.position, base.transform.forward) - holeSize.z / 2f;
			counterFront.localScale = localScale3;
		}
		if (counterBack != null)
		{
			Vector3 localScale4 = counterBack.localScale;
			localScale4.z = Vector3.Dot(holeCenter - counterBack.position, -base.transform.forward) - holeSize.z / 2f;
			counterBack.localScale = localScale4;
		}
	}
}
