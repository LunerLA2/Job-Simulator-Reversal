using System;
using UnityEngine;

public class TSCallbackOnDestroy : MonoBehaviour
{
	public Mesh mesh;

	public Action<Mesh> callWithMeshOnDestroy;

	private void OnDestroy()
	{
		if ((bool)mesh)
		{
			callWithMeshOnDestroy(mesh);
			mesh = null;
		}
	}
}
