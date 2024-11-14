using System.Collections.Generic;
using UnityEngine;

public class JobLevelsListData : ScriptableObject
{
	[SerializeField]
	private List<JobLevelData> activeJobs;

	public List<JobLevelData> ActiveJobs
	{
		get
		{
			return activeJobs;
		}
	}
}
