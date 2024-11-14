using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SkinnedMeshRenderer))]
[ExecuteInEditMode]
public class RibbonController : MonoBehaviour
{
	public RibbonParameters ribbonParams;

	public List<Transform> bones = new List<Transform>();

	private void OnEnable()
	{
		RefreshBones();
		RefreshMesh();
	}

	private void OnDrawGizmos()
	{
		if (bones == null)
		{
			return;
		}
		for (int i = 0; i < bones.Count; i++)
		{
			Transform transform = bones[i];
			if (transform != null)
			{
				Gizmos.color = new Color(1f, 1f, 0f, 0.5f);
				Gizmos.DrawWireSphere(transform.position, 0.005f);
				if (i > 0 && bones[i - 1] != null)
				{
					Gizmos.DrawLine(transform.position, bones[i - 1].position);
				}
			}
		}
	}

	public void RefreshBones()
	{
		SkinnedMeshRenderer component = GetComponent<SkinnedMeshRenderer>();
		if (ribbonParams != null)
		{
			while (bones.Count > ribbonParams.boneCount)
			{
				bones.RemoveAt(bones.Count - 1);
			}
			while (bones.Count < ribbonParams.boneCount)
			{
				bones.Add(null);
			}
		}
		component.bones = bones.ToArray();
	}

	public void RefreshMesh()
	{
		if (ribbonParams != null)
		{
			SkinnedMeshRenderer component = GetComponent<SkinnedMeshRenderer>();
			component.sharedMesh = ribbonParams.RebuildMesh();
			component.updateWhenOffscreen = true;
		}
	}
}
