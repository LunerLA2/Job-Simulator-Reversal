using System;
using UnityEngine;

public class GCOnStart : MonoBehaviour
{
	private void Start()
	{
		Resources.UnloadUnusedAssets();
		GC.Collect();
	}
}
