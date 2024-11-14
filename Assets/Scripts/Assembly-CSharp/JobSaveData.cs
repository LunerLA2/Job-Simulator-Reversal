using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

[Serializable]
public class JobSaveData
{
	[XmlArray("Tasks")]
	[SerializeField]
	private List<TaskSaveData> tasks = new List<TaskSaveData>();

	private int highestStreak;

	[XmlAttribute("id")]
	public string ID { get; private set; }

	public List<TaskSaveData> Tasks
	{
		get
		{
			return tasks;
		}
	}

	public int HighestStreak
	{
		get
		{
			return highestStreak;
		}
	}

	[XmlElement("HighestStreak")]
	public string HighestStreakSerialize
	{
		get
		{
			return highestStreak.ToString();
		}
		set
		{
			int result;
			if (int.TryParse(value, out result))
			{
				highestStreak = result;
			}
			else
			{
				highestStreak = 0;
			}
		}
	}

	public void SetID(string id)
	{
		ID = id;
	}

	public void SetTasks(List<TaskSaveData> tasks)
	{
		this.tasks = tasks;
	}

	public void SetHighestStreak(int highestStreak)
	{
		this.highestStreak = highestStreak;
	}
}
