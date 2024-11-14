using System.Collections;
using UnityEngine;

public class MuseumManager : MonoBehaviour
{
	[SerializeField]
	private JobCartridgeController jobCart;

	private void Start()
	{
		StartCoroutine(IntroLogic());
	}

	private void Update()
	{
		if ((!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl)) || (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift)))
		{
			return;
		}
		if (Input.GetKeyDown(KeyCode.U))
		{
			for (int i = 0; i < GlobalStorage.Instance.GameStateData.JobsData.Count; i++)
			{
				JobStateData jobStateData = GlobalStorage.Instance.GameStateData.JobsData[i];
				for (int j = 0; j < jobStateData.TasksData.Count; j++)
				{
					jobStateData.TasksData[j].SetIsCompleted(true);
				}
			}
			GameStateController.SaveState();
			Debug.Log("Set all jobs to completed");
			LevelLoader.Instance.LoadIntroScene();
		}
		if (Input.GetKeyDown(KeyCode.Delete))
		{
			SaveLoad.Delete();
			GameStateController.LoadState();
			Debug.Log("Save file deleted");
			LevelLoader.Instance.LoadIntroScene();
		}
	}

	private IEnumerator IntroLogic()
	{
		yield return new WaitForSeconds(2f);
		if (GlobalStorage.Instance.GameStateData.AllJobsComplete())
		{
			if (!GlobalStorage.Instance.GameStateData.HasSeenGameComplete)
			{
				Debug.Log("All jobs are complete, run the Job Genie intro");
				GameEventsManager.Instance.ScriptedCauseOccurred("playJobGenieIntro");
				DayNightFader.instance.ToggleCanUseSwitch(false);
				StartCoroutine(WaitAndReAllowOvertimeSwitch());
				jobCart.SwitchToUpgradedCart(23f);
				jobCart.PlayEnterAnim(31f);
				GlobalStorage.Instance.GameStateData.SetHasSeenGameComplete(true);
				GameStateController.SaveState();
			}
			else
			{
				Debug.Log("Starting playIntroduction after you beat the game");
				GameEventsManager.Instance.ScriptedCauseOccurred("playIntroduction");
				jobCart.PlayEnterAnim(0f);
			}
		}
		else if (GlobalStorage.Instance.GameStateData.HasSavedData())
		{
			Debug.Log("Starting playIntroduction");
			GameEventsManager.Instance.ScriptedCauseOccurred("playIntroduction");
			jobCart.PlayEnterAnim(0f);
		}
		else
		{
			Debug.Log("Starting playFirstTimeIntroduction");
			GameEventsManager.Instance.ScriptedCauseOccurred("playFirstTimeIntroduction");
		}
	}

	private IEnumerator WaitAndReAllowOvertimeSwitch()
	{
		yield return new WaitForSeconds(35f);
		DayNightFader.instance.ToggleCanUseSwitch(true);
	}
}
