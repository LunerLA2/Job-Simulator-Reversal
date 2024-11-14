using System.Collections.Generic;
using NobleMuffins.MuffinSlicer;
using OwlchemyVR;
using UnityEngine;

public class Sliceable : MonoBehaviour, ISliceable
{
	public bool currentlySliceable = true;

	public int cutCount;

	public bool refreshColliders = true;

	public TurboSlice.InfillConfiguration[] infillers = new TurboSlice.InfillConfiguration[0];

	public bool channelNormals = true;

	public bool channelTangents;

	public bool channelUV2;

	public GameObject explicitlySelectedMeshHolder;

	public Object alternatePrefab;

	public bool alwaysCloneFromAlternate;

	public GameObject meshHolder
	{
		get
		{
			if (explicitlySelectedMeshHolder == null)
			{
				Component component = GetComponent<Renderer>();
				if (component != null)
				{
					explicitlySelectedMeshHolder = component.gameObject;
				}
			}
			return explicitlySelectedMeshHolder;
		}
	}

	public GameObject[] Slice(Vector3 positionInWorldSpace, Vector3 normalInWorldSpace)
	{
		if (currentlySliceable)
		{
			Matrix4x4 worldToLocalMatrix = base.transform.worldToLocalMatrix;
			Vector3 point = worldToLocalMatrix.MultiplyPoint3x4(positionInWorldSpace);
			Vector3 normalized = worldToLocalMatrix.MultiplyVector(normalInWorldSpace).normalized;
			Vector4 planeInLocalSpace = MuffinSliceCommon.planeFromPointAndNormal(point, normalized);
			GameObject[] array = TurboSlice.instance.splitByPlane(base.gameObject, planeInLocalSpace, true);
			for (int i = 0; i < array.Length; i++)
			{
				ModelOutlineContainer[] componentsInChildren = array[i].GetComponentsInChildren<ModelOutlineContainer>(true);
				for (int j = 0; j < componentsInChildren.Length; j++)
				{
					componentsInChildren[j].gameObject.SetActive(true);
					componentsInChildren[j].SetMesh(array[i].GetComponent<MeshFilter>().mesh);
				}
			}
			return array;
		}
		return new GameObject[1] { base.gameObject };
	}

	public void handleSlice(GameObject[] results)
	{
		AbstractSliceHandler[] components = base.gameObject.GetComponents<AbstractSliceHandler>();
		AbstractSliceHandler[] array = components;
		foreach (AbstractSliceHandler abstractSliceHandler in array)
		{
			abstractSliceHandler.handleSlice(results);
		}
	}

	public bool cloneAlternate(Dictionary<string, bool> hierarchyPresence)
	{
		if (alternatePrefab == null)
		{
			return false;
		}
		if (alwaysCloneFromAlternate)
		{
			return true;
		}
		AbstractSliceHandler[] components = base.gameObject.GetComponents<AbstractSliceHandler>();
		AbstractSliceHandler[] array = components;
		foreach (AbstractSliceHandler abstractSliceHandler in array)
		{
			if (abstractSliceHandler.cloneAlternate(hierarchyPresence))
			{
				return true;
			}
		}
		return false;
	}
}
