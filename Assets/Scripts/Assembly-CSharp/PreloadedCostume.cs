using UnityEngine;

public class PreloadedCostume
{
	private BotCostumeData data;

	private GameObject sceneObject;

	private Transform originalParent;

	public bool claimed;

	public BotCostumeData Data
	{
		get
		{
			return data;
		}
	}

	public GameObject SceneObject
	{
		get
		{
			return sceneObject;
		}
	}

	public PreloadedCostume(BotCostumeData costume, Transform parent)
	{
		data = costume;
		sceneObject = new GameObject(costume.name);
		originalParent = parent;
		for (int i = 0; i < costume.CostumePieces.Length; i++)
		{
			if (costume.CostumePieces[i].ArtPrefab != null)
			{
				GameObject gameObject = Object.Instantiate(costume.CostumePieces[i].ArtPrefab, Vector3.zero, Quaternion.identity) as GameObject;
				BasePrefabSpawner component = gameObject.GetComponent<BasePrefabSpawner>();
				if (component != null)
				{
					gameObject = component.LastSpawnedPrefabGO;
				}
				gameObject.transform.SetParent(sceneObject.transform, false);
			}
			else
			{
				Debug.LogError("Costume '" + costume.name + "' not set up properly: a CostumePiece is missing its art prefab.");
			}
		}
		sceneObject.transform.SetParent(parent, false);
		sceneObject.SetActive(false);
		claimed = false;
	}

	public void Claim(Transform parent)
	{
		sceneObject.transform.SetParent(parent, false);
		sceneObject.SetActive(true);
		claimed = true;
	}

	public void Release()
	{
		sceneObject.transform.SetParent(originalParent, false);
		sceneObject.SetActive(false);
		if (!data.ContainsDynamicElements)
		{
			claimed = false;
		}
	}
}
