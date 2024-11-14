using System.Collections;
using UnityEngine;

namespace SiliconTrail
{
	public class CinematicTask : Task
	{
		public string CinematicName;

		public string SkipKeyCode;

		private bool skipped;

		protected override IEnumerator Execute()
		{
			program.HideAll();
			Cinematic cinematic = program.ShowCinematic(CinematicName);
			float timeLeft = cinematic.Duration;
			while (timeLeft > 0f && !skipped)
			{
				if (timeLeft != float.PositiveInfinity)
				{
					timeLeft -= Time.deltaTime;
				}
				yield return null;
			}
			program.HideAll();
		}

		public override void OnKeyPress(string code)
		{
			if (code == SkipKeyCode)
			{
				skipped = true;
			}
		}
	}
}
