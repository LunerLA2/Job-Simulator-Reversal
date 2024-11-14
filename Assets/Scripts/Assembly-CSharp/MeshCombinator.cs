using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeshCombinator : MonoBehaviour
{
	private const float MaxVertices = 65000f;

	public bool CombineAutomatically;

	public List<GameObject> CombinedGameObjects;

	private List<GameObject> objectsUsed;

	public MeshCombinator()
	{
		CombineAutomatically = false;
		CombinedGameObjects = new List<GameObject>();
		objectsUsed = new List<GameObject>();
	}

	private void Start()
	{
		if (CombineAutomatically)
		{
			if (CombinedGameObjects == null)
			{
				CombinedGameObjects = new List<GameObject>();
			}
			Combine();
		}
	}

	public void Combine()
	{
		Dictionary<string, List<MeshFilter>> enabledMeshes = GetEnabledMeshes();
		foreach (KeyValuePair<string, List<MeshFilter>> item2 in enabledMeshes)
		{
			int num = item2.Value.Sum((MeshFilter filter) => filter.transform.GetComponent<MeshFilter>().sharedMesh.vertexCount);
			int num2 = Mathf.CeilToInt((float)num / 65000f);
			int num3 = item2.Value.Count / num2;
			for (int i = 0; i < num2; i++)
			{
				List<Material> list = new List<Material>();
				List<CombineInstance> list2 = new List<CombineInstance>();
				for (int j = 0; j < num3; j++)
				{
					Transform transformFromFiltersPair = GetTransformFromFiltersPair(item2, i, num3, j);
					MeshRenderer component = transformFromFiltersPair.gameObject.GetComponent<MeshRenderer>();
					for (int k = 0; k < component.sharedMaterials.Length; k++)
					{
						if (!list.Contains(component.sharedMaterials[k]))
						{
							list.Add(component.sharedMaterials[k]);
						}
					}
					list2.Add(new CombineInstance
					{
						mesh = transformFromFiltersPair.GetComponent<MeshFilter>().sharedMesh,
						transform = transformFromFiltersPair.localToWorldMatrix
					});
					SaveToObjectsUsed(transformFromFiltersPair.gameObject);
				}
				GameObject item = SetupCombinedGameObject(list2, list, item2.Value.FirstOrDefault(), i);
				CombinedGameObjects.Add(item);
			}
		}
		DisableRenderers(objectsUsed);
	}

	private GameObject SetupCombinedGameObject(List<CombineInstance> combineInstances, List<Material> materials, MeshFilter nameFilter, int nameIndex)
	{
		GameObject gameObject = new GameObject();
		gameObject.name = string.Concat("_Combined Mesh [", nameFilter, "]_", nameIndex);
		gameObject.transform.parent = base.transform;
		AddMeshFilterToCombinedGameObject(combineInstances.ToArray(), gameObject);
		AddMeshRendererToCombinedGameObject(materials, gameObject);
		return gameObject;
	}

	private static void AddMeshRendererToCombinedGameObject(List<Material> sharedMaterials, GameObject combinedGameObject)
	{
		MeshRenderer meshRenderer = combinedGameObject.AddComponent<MeshRenderer>();
		meshRenderer.sharedMaterials = sharedMaterials.ToArray();
	}

	private static void AddMeshFilterToCombinedGameObject(CombineInstance[] combineInstances, GameObject combinedGameObject)
	{
		MeshFilter meshFilter = combinedGameObject.AddComponent<MeshFilter>();
		meshFilter.mesh = new Mesh();
		meshFilter.sharedMesh.CombineMeshes(combineInstances);
	}

	private static Transform GetTransformFromFiltersPair(KeyValuePair<string, List<MeshFilter>> filtersPair, int i, int filtersInSplit, int j)
	{
		return filtersPair.Value[i * filtersInSplit + j].transform;
	}

	private void SaveToObjectsUsed(GameObject gameObject)
	{
		objectsUsed.Add(gameObject);
	}

	public void UnCombine()
	{
		EnableRenderers(objectsUsed);
		foreach (GameObject combinedGameObject in CombinedGameObjects)
		{
			Object.DestroyImmediate(combinedGameObject);
		}
		CombinedGameObjects.Clear();
	}

	private Dictionary<string, List<MeshFilter>> GetEnabledMeshes()
	{
		MeshFilter[] componentsInChildren = base.transform.GetComponentsInChildren<MeshFilter>();
		return GetEnabledMeshesFromFilters(componentsInChildren);
	}

	private Dictionary<string, List<MeshFilter>> GetEnabledMeshesFromFilters(MeshFilter[] filters)
	{
		Dictionary<string, List<MeshFilter>> dictionary = new Dictionary<string, List<MeshFilter>>();
		foreach (MeshFilter meshFilter in filters)
		{
			MeshRenderer component = meshFilter.GetComponent<MeshRenderer>();
			if (!IsRendererEnabled(component))
			{
				continue;
			}
			foreach (string item in component.sharedMaterials.Select((Material material) => material.name))
			{
				if (dictionary.ContainsKey(item))
				{
					dictionary[item].Add(meshFilter);
					continue;
				}
				dictionary.Add(item, new List<MeshFilter> { meshFilter });
			}
		}
		return dictionary;
	}

	private static bool IsRendererEnabled(MeshRenderer renderer)
	{
		return renderer != null && renderer.enabled;
	}

	private void EnableRenderers(List<GameObject> gameObjects)
	{
		foreach (GameObject gameObject in gameObjects)
		{
			gameObject.GetComponent<Renderer>().enabled = true;
		}
	}

	private void DisableRenderers(List<GameObject> gameObjects)
	{
		foreach (GameObject gameObject in gameObjects)
		{
			gameObject.GetComponent<Renderer>().enabled = false;
		}
	}
}
