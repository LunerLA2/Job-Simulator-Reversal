using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

[Serializable]
[XmlRoot("SaveData")]
public class SaveStateData
{
	[SerializeField]
	[XmlArray("JobsSaveData")]
	private List<JobSaveData> jobsData = new List<JobSaveData>();

	[XmlIgnore]
	[SerializeField]
	private bool hasSeenGameComplete;

	[XmlElement("HasSeenGameComplete")]
	public string HasSeenGameCompleteSerialize
	{
		get
		{
			return (!hasSeenGameComplete) ? "0" : "1";
		}
		set
		{
			hasSeenGameComplete = value == "1";
		}
	}

	public List<JobSaveData> JobsData
	{
		get
		{
			return jobsData;
		}
	}

	public bool HasSeenGameComplete
	{
		get
		{
			return hasSeenGameComplete;
		}
	}

	public void SetJobsData(List<JobSaveData> jobsData)
	{
		this.jobsData = jobsData;
	}

	public void SetHasSeenGameComplete(bool value)
	{
		hasSeenGameComplete = value;
	}
}
