using UnityEngine;

namespace OwlchemyVR
{
	[CreateAssetMenu(fileName = "MULTITASK_TaskData", menuName = "MultiTaskData")]
	public class MultiTaskData : TaskData
	{
		[SerializeField]
		private TaskData[] Tasks;

		public override TaskData GetTaskData()
		{
			if (Tasks.Length == 0)
			{
				return null;
			}
			return Tasks[Random.Range(0, Tasks.Length)];
		}
	}
}
