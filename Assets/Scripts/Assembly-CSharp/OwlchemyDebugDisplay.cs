using System.Collections;
using UnityEngine;

public class OwlchemyDebugDisplay : MonoBehaviour
{
	public enum MoveKeyCode
	{
		None = 0,
		Trigger = 1,
		Center = 2,
		Start = 3,
		Triangle = 4,
		Circle = 5,
		Cross = 6,
		Square = 7
	}

	public GameObject fpsCounter;

	private int fpsMode;

	private bool lastCross;

	private bool lastCircle;

	private bool lastSquare;

	private bool lastTriangle;

	private FPSDisplay fpsDisplay;

	private HeadController headController;

	private bool GetButton(MoveKeyCode key)
	{
		return false;
	}

	private IEnumerator Start()
	{
		fpsCounter.SetActive(false);
		while (headController == null)
		{
			headController = GlobalStorage.Instance.MasterHMDAndInputController.Head;
			yield return new WaitForEndOfFrame();
		}
		yield return new WaitForEndOfFrame();
		base.transform.parent = headController.transform;
		base.transform.localPosition = Vector3.zero;
		base.transform.localRotation = Quaternion.identity;
		fpsDisplay = fpsCounter.GetComponent<FPSDisplay>();
		FPSCounterModeSwitch(1);
	}

	private void FPSCounterModeSwitch(int mode)
	{
		switch (mode)
		{
		case 0:
			fpsCounter.SetActive(false);
			fpsDisplay.activeRBMode = false;
			fpsDisplay.fpsText.GetComponent<TextMesh>().characterSize = 3f;
			break;
		case 1:
			fpsCounter.SetActive(true);
			fpsDisplay.activeRBMode = false;
			fpsDisplay.fpsText.GetComponent<TextMesh>().characterSize = 3f;
			break;
		case 2:
			fpsCounter.SetActive(true);
			fpsDisplay.activeRBMode = true;
			fpsDisplay.fpsText.GetComponent<TextMesh>().characterSize = 1.2f;
			break;
		}
	}

	private void Update()
	{
	}

	private IEnumerator Reposition()
	{
		Debug.Log("DID REPOSITION IN DEBUG");
		yield return new WaitForEndOfFrame();
		Transform playerHeadTransform = GameObject.Find("Mono Camera").transform;
		Vector3 offsetVector = Vector3.zero + Vector3.up * 1.5f - playerHeadTransform.position;
		playerHeadTransform.root.transform.Translate(offsetVector);
	}

	public static void CompleteAllJobs()
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
}
