using System.Collections;
using OwlchemyVR;
using UnityEngine;

public class PartsChooserController : ModeContainer
{
	private const float DOORS_CLOSED_DURATION = 0.25f;

	[SerializeField]
	private Animation doorsAnimation;

	[SerializeField]
	private Animation pegBoardAnimation;

	[SerializeField]
	private AnimationClip pegBoardBackwardAnimClip;

	[SerializeField]
	private AnimationClip pegBoardForwardAnimClip;

	[SerializeField]
	private AnimationClip doorsCloseAnimClip;

	[SerializeField]
	private AnimationClip doorsOpenAnimClip;

	[SerializeField]
	private Transform spawnersRoot;

	[SerializeField]
	private Transform interfaceAudioSource;

	[SerializeField]
	private AudioClip pegBoardBackwardSound;

	[SerializeField]
	private AudioClip pegBoardForwardSound;

	[SerializeField]
	private AudioClip doorsCloseSound;

	[SerializeField]
	private AudioClip doorsOpenSound;

	[SerializeField]
	private AudioSourceHelper audioSourceHelper;

	private GameObjectPrefabSpawner[] prefabSpawners;

	private GameObject[] prefabGameObjects;

	private WorldItem[] prefabWorldItems;

	private Transform toolTemporaryParent;

	private void Awake()
	{
		toolTemporaryParent = new GameObject("TemporaryToolParent").transform;
		toolTemporaryParent.SetParent(base.transform);
		toolTemporaryParent.SetToDefaultPosRotScale();
		prefabSpawners = new GameObjectPrefabSpawner[spawnersRoot.childCount];
		prefabGameObjects = new GameObject[prefabSpawners.Length];
		prefabWorldItems = new WorldItem[prefabSpawners.Length];
		for (int i = 0; i < spawnersRoot.childCount; i++)
		{
			prefabSpawners[i] = spawnersRoot.GetChild(i).GetComponent<GameObjectPrefabSpawner>();
			prefabWorldItems[i] = spawnersRoot.GetChild(i).GetComponent<WorldItem>();
			if (GetOrSpawnTool(i) != null)
			{
				prefabGameObjects[i].SetActive(false);
				prefabGameObjects[i].transform.SetParent(toolTemporaryParent);
			}
		}
		for (int j = 0; j < prefabGameObjects.Length; j++)
		{
			prefabGameObjects[j].transform.SetParent(spawnersRoot);
		}
	}

	private GameObject GetOrSpawnTool(int partsIndex)
	{
		if (partsIndex == -1)
		{
			return null;
		}
		if (prefabGameObjects[partsIndex] == null)
		{
			if (prefabSpawners[partsIndex] == null || prefabSpawners[partsIndex].prefab == null)
			{
				return null;
			}
			GameObject gameObject = prefabSpawners[partsIndex].SpawnPrefab();
			if (gameObject != null)
			{
				prefabGameObjects[partsIndex] = gameObject;
			}
		}
		return prefabGameObjects[partsIndex];
	}

	protected override IEnumerator ChangeModeAsync(int newModeIndex)
	{
		if (newModeIndex == modeIndex)
		{
			yield break;
		}
		if (modeIndex != -1)
		{
			GameObject toolGo2 = prefabGameObjects[modeIndex];
			pegBoardAnimation.Stop();
			pegBoardAnimation.Play(pegBoardBackwardAnimClip.name);
			if (pegBoardBackwardSound != null)
			{
				audioSourceHelper.SetClip(pegBoardBackwardSound);
				audioSourceHelper.Play();
			}
			while (pegBoardAnimation.isPlaying)
			{
				yield return null;
			}
			pegBoardAnimation[pegBoardBackwardAnimClip.name].normalizedTime = 1f;
			doorsAnimation.Stop();
			doorsAnimation.Play(doorsCloseAnimClip.name);
			if (doorsCloseSound != null)
			{
				audioSourceHelper.SetClip(doorsCloseSound);
				audioSourceHelper.Play();
			}
			while (doorsAnimation.isPlaying)
			{
				yield return null;
			}
			doorsAnimation[doorsCloseAnimClip.name].normalizedTime = 1f;
			yield return new WaitForSeconds(0.25f);
			if (toolGo2 != null)
			{
				toolGo2.SetActive(false);
			}
			if (prefabWorldItems[modeIndex] != null)
			{
				GameEventsManager.Instance.ItemActionOccurred(prefabWorldItems[modeIndex].Data, "CLOSED");
			}
		}
		if (newModeIndex != -1)
		{
			GetOrSpawnTool(newModeIndex);
			GameObject toolGo = prefabGameObjects[newModeIndex];
			if (toolGo != null)
			{
				toolGo.SetActive(true);
			}
			doorsAnimation.Stop();
			doorsAnimation.Play(doorsOpenAnimClip.name);
			while (doorsAnimation.isPlaying)
			{
				yield return null;
			}
			doorsAnimation[doorsOpenAnimClip.name].normalizedTime = 1f;
			pegBoardAnimation.Stop();
			pegBoardAnimation.Play(pegBoardForwardAnimClip.name);
			if (pegBoardForwardSound != null)
			{
				audioSourceHelper.SetClip(pegBoardForwardSound);
				audioSourceHelper.Play();
			}
			while (pegBoardAnimation.isPlaying)
			{
				yield return null;
			}
			pegBoardAnimation[pegBoardForwardAnimClip.name].normalizedTime = 1f;
			if (prefabWorldItems[newModeIndex] != null)
			{
				GameEventsManager.Instance.ItemActionOccurred(prefabWorldItems[newModeIndex].Data, "OPENED");
			}
		}
	}
}
