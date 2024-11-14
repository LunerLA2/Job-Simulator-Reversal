using System.Collections;
using System.Collections.Generic;

namespace SiliconTrail
{
	public abstract class CampaignEvent : WeightedProbableThing
	{
		protected SiliconTrailComputerProgram program;

		protected Campaign campaign;

		protected List<Task> tasksOnFinish;

		protected bool isFinished;

		protected virtual bool DoEvent()
		{
			return true;
		}

		private IEnumerator DoEventWrapper()
		{
			isFinished = false;
			if (!DoEvent())
			{
				while (!isFinished)
				{
					yield return null;
				}
			}
			program.QueueTasks(tasksOnFinish);
		}

		public void DoEvent(SiliconTrailComputerProgram program, params Task[] tasksOnFinish)
		{
			this.program = program;
			campaign = program.Campaign;
			this.tasksOnFinish = new List<Task>(tasksOnFinish);
			this.program.StartCoroutine(DoEventWrapper());
		}
	}
}
