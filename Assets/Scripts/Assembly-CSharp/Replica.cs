using System;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class Replica : MonoBehaviour
{
	private const float MIN_SIZE = 0.02f;

	[SerializeField]
	private Transform corePivot;

	[SerializeField]
	private Transform core;

	[SerializeField]
	private Vector3 maxDimensions;

	[SerializeField]
	private Material replicaMaterial;

	[SerializeField]
	private Material[] modelMaterialsToExclude;

	[SerializeField]
	private GameObject supportPrefab;

	[SerializeField]
	private bool addSupports = true;

	private SelectedChangeOutlineController outlineController;

	private void OnDrawGizmosSelected()
	{
		if (corePivot != null)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireCube(corePivot.position + new Vector3(0f, maxDimensions.y / 2f, 0f), maxDimensions);
		}
	}

	private void Awake()
	{
		outlineController = GetComponent<SelectedChangeOutlineController>();
	}

	public void CopyModels(Transform modelBase, PickupableItem[] models)
	{
		if (models.Length == 0)
		{
			outlineController.Build();
			return;
		}
		Transform[] array = new Transform[models.Length];
		List<Transform> list = new List<Transform>();
		Collider[] array2 = null;
		MeshRenderer[] array3 = null;
		GameObject gameObject = null;
		Collider collider = null;
		Transform transform = null;
		Collider collider2 = null;
		MeshRenderer meshRenderer = null;
		MeshFilter meshFilter = null;
		MeshRenderer meshRenderer2 = null;
		List<MeshFilter> list2 = new List<MeshFilter>(outlineController.meshFilters);
		Bounds bounds = default(Bounds);
		bool flag = false;
		for (int i = 0; i < models.Length; i++)
		{
			array[i] = CopyModelTransform(modelBase, models[i].transform);
			array2 = models[i].GetComponentsInChildren<Collider>();
			for (int j = 0; j < array2.Length; j++)
			{
				collider = array2[j];
				gameObject = collider.gameObject;
				if (collider.enabled && gameObject.activeInHierarchy && !collider.isTrigger && gameObject.layer != 17 && !(collider.bounds.size.sqrMagnitude < 0.02f))
				{
					transform = CopyModelTransform(modelBase, collider.transform);
					list.Add(transform);
					collider2 = CopyColliderOnto(collider, transform.gameObject);
					collider2.gameObject.layer = 8;
					if (!flag)
					{
						flag = true;
						bounds = collider2.bounds;
					}
					else
					{
						bounds.Encapsulate(collider2.bounds);
					}
				}
			}
			array3 = models[i].GetComponentsInChildren<MeshRenderer>();
			for (int k = 0; k < array3.Length; k++)
			{
				meshRenderer = array3[k];
				if (meshRenderer.enabled && meshRenderer.gameObject.activeInHierarchy && !(meshRenderer.bounds.size.sqrMagnitude < 0.0005f) && Array.IndexOf(modelMaterialsToExclude, meshRenderer.sharedMaterial) == -1)
				{
					transform = CopyModelTransform(modelBase, meshRenderer.transform);
					list.Add(transform);
					meshFilter = transform.gameObject.AddComponent<MeshFilter>();
					list2.Add(meshFilter);
					meshFilter.sharedMesh = meshRenderer.GetComponent<MeshFilter>().sharedMesh;
					meshRenderer2 = transform.gameObject.AddComponent<MeshRenderer>();
					meshRenderer2.sharedMaterial = replicaMaterial;
				}
			}
		}
		core.position = bounds.center;
		for (int l = 0; l < list.Count; l++)
		{
			transform = list[l];
			transform.SetParent(core, true);
		}
		for (int m = 0; m < models.Length; m++)
		{
			array[m].SetParent(core, true);
		}
		float num = 1f / Mathf.Max(bounds.size.x / maxDimensions.x, bounds.size.y / maxDimensions.y, bounds.size.z / maxDimensions.z);
		core.localScale *= num;
		core.localPosition = new Vector3(0f, bounds.size.y * num / 2f, 0f);
		Transform transform2 = null;
		Quaternion localRotation = Quaternion.AngleAxis(45f, Vector3.up);
		for (int n = 0; n < models.Length; n++)
		{
			if (addSupports)
			{
				transform2 = UnityEngine.Object.Instantiate(supportPrefab).transform;
				transform2.gameObject.RemoveCloneFromName();
				float num2 = Mathf.Abs(array[n].position.y - corePivot.position.y);
				transform2.position = corePivot.position + new Vector3(0f, num2 / 2.2f, 0f);
				transform2.localRotation = localRotation;
				Vector3 localScale = transform2.localScale;
				localScale.y = num2;
				transform2.localScale = localScale;
				transform2.SetParent(corePivot, true);
				list2.Add(transform2.GetComponent<MeshFilter>());
			}
			UnityEngine.Object.Destroy(array[n].gameObject);
		}
		outlineController.meshFilters = list2.ToArray();
		outlineController.Build();
	}

	private Transform CopyModelTransform(Transform modelBase, Transform t)
	{
		GameObject gameObject = new GameObject();
		gameObject.name = "Piece";
		Transform transform = gameObject.transform;
		transform.position = core.TransformPoint(modelBase.InverseTransformPoint(t.position));
		transform.rotation = core.rotation * (Quaternion.Inverse(modelBase.rotation) * t.rotation);
		transform.localScale = t.lossyScale;
		return transform;
	}

	private Collider CopyColliderOnto(Collider col, GameObject go)
	{
		BoxCollider boxCollider = col as BoxCollider;
		if (boxCollider != null)
		{
			BoxCollider boxCollider2 = go.AddComponent<BoxCollider>();
			boxCollider2.center = boxCollider.center;
			boxCollider2.size = boxCollider.size;
			return boxCollider2;
		}
		SphereCollider sphereCollider = col as SphereCollider;
		if (sphereCollider != null)
		{
			SphereCollider sphereCollider2 = go.AddComponent<SphereCollider>();
			sphereCollider2.center = sphereCollider.center;
			sphereCollider2.radius = sphereCollider.radius;
			return sphereCollider2;
		}
		CapsuleCollider capsuleCollider = col as CapsuleCollider;
		if (capsuleCollider != null)
		{
			CapsuleCollider capsuleCollider2 = go.AddComponent<CapsuleCollider>();
			capsuleCollider2.direction = capsuleCollider.direction;
			capsuleCollider2.center = capsuleCollider.center;
			capsuleCollider2.radius = capsuleCollider.radius;
			capsuleCollider2.height = capsuleCollider.height;
			return capsuleCollider2;
		}
		return null;
	}
}
