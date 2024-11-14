using System;
using UnityEngine;

[Serializable]
public class JobLevelData : ScriptableObject
{
	private string id;

	[SerializeField]
	private string fullName;

	[SerializeField]
	private string sceneName;

	[SerializeField]
	private JobData jobData;

	public string FullName
	{
		get
		{
			return fullName;
		}
	}

	public string SceneName
	{
		get
		{
			return sceneName;
		}
	}

	public string ID
	{
		get
		{
			return id;
		}
	}

	public JobData JobData
	{
		get
		{
			return jobData;
		}
	}

	private void OnEnable()
	{
		id = base.name;
	}
}
