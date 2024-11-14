using UnityEngine;

public class BrainGroupData : ScriptableObject
{
	[SerializeField]
	private BrainData[] brainDatas;

	public BrainData[] BrainDatas
	{
		get
		{
			return brainDatas;
		}
	}
}
