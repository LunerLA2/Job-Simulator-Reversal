using System.Collections;
using UnityEngine;

namespace SiliconTrail
{
	public class TravelTask : Task
	{
		private const float INITIAL_PROGRESS_OFFSET = 0f;

		private const float TRAVEL_SPEED = 0.75f;

		private const float PRE_TRAVEL_IDLE_TIME = 0.2f;

		private const float POST_TRAVEL_IDLE_TIME = 0.5f;

		public int Distance = 1;

		protected override IEnumerator Execute()
		{
			program.HideAll();
			program.ShowTrail();
			int currentPlace = campaign.PlaceIndex;
			int target = Mathf.Min(currentPlace + Distance, campaign.NumPlaces - 1);
			float progress = currentPlace;
			float idleTime2 = 0f;
			while (idleTime2 < 0.2f)
			{
				program.SetTrailProgress(progress);
				idleTime2 += Time.deltaTime;
				yield return null;
			}
			while (progress < (float)target)
			{
				program.SetTrailProgress(progress);
				progress = Mathf.Min(target, progress + Time.deltaTime * 0.75f);
				yield return null;
			}
			idleTime2 = 0f;
			while (idleTime2 < 0.5f)
			{
				program.SetTrailProgress(progress);
				idleTime2 += Time.deltaTime;
				yield return null;
			}
			campaign.IncrementPlace(Distance);
			program.QueueTask(new ResolveDayTask());
			program.QueueTask(new PlaceTask());
		}
	}
}
