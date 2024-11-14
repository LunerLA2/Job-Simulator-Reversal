using System.Collections;

namespace SiliconTrail
{
	public class SurfingTask : Task
	{
		private SurfingMinigame surfingMinigame;

		protected override IEnumerator Execute()
		{
			program.HideAll();
			program.ShowChoice("Up", "Down");
			surfingMinigame = program.ShowSurfingMinigame();
			while (!surfingMinigame.IsOver)
			{
				yield return null;
			}
			program.HideAll();
			campaign.IncrementMoney(surfingMinigame.TotalDataCollected);
			program.QueueTask(new InfoTask
			{
				GraphicName = "Torrent",
				InfoText = "You collected " + surfingMinigame.TotalDataCollected + " " + program.DataUnit + " of memory!"
			});
		}

		public override void OnKeyPress(string code)
		{
			GameEventsManager.Instance.ItemActionOccurred(program.WorldItemData, "USED");
			program.UsedWID();
			surfingMinigame.OnKeyPress(code);
		}
	}
}
