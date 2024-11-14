using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneRecyclingData", menuName = "SceneRecyclingData")]
public class SceneRecyclingData : ScriptableObject
{
	[SerializeField]
	private List<RecycleInfo> recycleInfos = new List<RecycleInfo>();

	[SerializeField]
	private string fallbackRespawnLocationID = string.Empty;

	public List<RecycleInfo> RecycleInfos
	{
		get
		{
			return recycleInfos;
		}
	}

	public string FallbackRespawnLocationID
	{
		get
		{
			return fallbackRespawnLocationID;
		}
	}

	public void EditorSetFallbackRespawnLocationID(string f)
	{
		fallbackRespawnLocationID = f;
	}
}
