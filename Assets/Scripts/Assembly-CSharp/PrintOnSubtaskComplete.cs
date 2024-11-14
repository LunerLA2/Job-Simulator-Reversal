using System;
using UnityEngine;

[Serializable]
public class PrintOnSubtaskComplete
{
	[SerializeField]
	private SubtaskData subtask;

	[SerializeField]
	private GameObject objectToPrint;

	public SubtaskData Subtask
	{
		get
		{
			return subtask;
		}
	}

	public GameObject ObjectToPrint
	{
		get
		{
			return objectToPrint;
		}
	}
}
