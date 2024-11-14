using OwlchemyVR;
using TMPro;
using UnityEngine;

public class TextAdventureComputerProgram : ComputerProgram
{
	private const int NUM_TASKS_TO_ACTIVATED = 3;

	private int numTasksToActivatedCounter;

	[SerializeField]
	private TextMeshProUGUI startScreen;

	[SerializeField]
	private TextMeshProUGUI taskComplete;

	[SerializeField]
	private TextMeshProUGUI display;

	[SerializeField]
	private TextMeshProUGUI input;

	[SerializeField]
	private TextMeshProUGUI inputCarrot;

	[SerializeField]
	private TextAdventureTask[] taskList;

	private int currentTask;

	[SerializeField]
	private WorldItemData worldItemData;

	private string inputBuffer = string.Empty;

	private string inputField = string.Empty;

	private int chosenCommand;

	private bool waitingForCommand;

	private bool atStartScreen = true;

	private bool showingTaskCompleteScreen;

	public override ComputerProgramID ProgramID
	{
		get
		{
			return ComputerProgramID.JobSim1982;
		}
	}

	private void Awake()
	{
		ResetGame();
	}

	private void OnEnable()
	{
		GameEventsManager.Instance.ItemActionOccurred(worldItemData, "OPENED");
		if (hostComputer != null)
		{
			hostComputer.HideCursor();
		}
	}

	private void OnDisable()
	{
		GameEventsManager.Instance.ItemActionOccurred(worldItemData, "CLOSED");
		if (hostComputer != null)
		{
			hostComputer.ShowCursor();
		}
	}

	private void Display(string text)
	{
		inputBuffer = text;
		if (waitingForCommand)
		{
			inputBuffer += "\n";
			for (int i = 0; i < taskList[currentTask].Options.Length; i++)
			{
				inputBuffer += "\n";
				for (int j = 0; j < i + 1; j++)
				{
					inputBuffer += "0";
				}
				inputBuffer = inputBuffer + " - " + taskList[currentTask].Options[i];
			}
		}
		display.text = inputBuffer;
	}

	protected override bool OnKeyPress(string code)
	{
		if (atStartScreen)
		{
			StartGame();
			return true;
		}
		if (waitingForCommand)
		{
			if (code == "0")
			{
				inputField += "0";
				if (inputField.Length > taskList[currentTask].Options.Length)
				{
					inputField = "0";
				}
				input.text = inputField;
			}
			else if (code == "1" && inputField.Length > 0)
			{
				chosenCommand = inputField.Length - 1;
				waitingForCommand = false;
				if (taskList[currentTask].Results.Length == taskList[currentTask].Options.Length)
				{
					Display(taskList[currentTask].Results[chosenCommand]);
					input.text = string.Empty;
					SetInputEnabled(false);
					inputField = string.Empty;
				}
				else
				{
					Debug.LogWarning("You should always have exactly the same number of options and results!");
				}
			}
		}
		else
		{
			numTasksToActivatedCounter++;
			if (numTasksToActivatedCounter >= 3)
			{
				numTasksToActivatedCounter = 0;
				GameEventsManager.Instance.ItemActionOccurred(worldItemData, "ACTIVATED");
			}
			if (showingTaskCompleteScreen)
			{
				HideTaskComplete();
			}
			else
			{
				currentTask = taskList[currentTask].GoToTaskOnComplete[chosenCommand];
			}
			if (!AdvanceTask())
			{
				ResetGame();
				return true;
			}
			if (taskList[currentTask].IsStartOfNewTask && !taskList[currentTask].HasShownCompleteTaskScreen)
			{
				taskList[currentTask].SetHasShownCompleteTaskScreen(true);
				ShowTaskComplete();
				return true;
			}
		}
		return true;
	}

	private void SetInputEnabled(bool enabled)
	{
		input.enabled = enabled;
		inputCarrot.enabled = enabled;
	}

	private void StartGame()
	{
		startScreen.enabled = false;
		display.enabled = true;
		currentTask = 0;
		waitingForCommand = true;
		Display(taskList[currentTask].TextBody);
		atStartScreen = false;
		SetInputEnabled(true);
	}

	private void ResetGame()
	{
		display.enabled = false;
		currentTask = 0;
		waitingForCommand = false;
		atStartScreen = true;
		startScreen.enabled = true;
		SetInputEnabled(false);
		taskComplete.enabled = false;
		for (int i = 0; i < taskList.Length; i++)
		{
			taskList[i].SetHasShownCompleteTaskScreen(false);
		}
	}

	private void ShowTaskComplete()
	{
		showingTaskCompleteScreen = true;
		taskComplete.enabled = true;
		display.enabled = false;
		waitingForCommand = false;
		SetInputEnabled(false);
	}

	private void HideTaskComplete()
	{
		showingTaskCompleteScreen = false;
		taskComplete.enabled = false;
		display.enabled = true;
		waitingForCommand = true;
		SetInputEnabled(true);
	}

	private bool AdvanceTask()
	{
		if (currentTask < taskList.Length)
		{
			waitingForCommand = true;
			Display(taskList[currentTask].TextBody);
			SetInputEnabled(true);
			GameEventsManager.Instance.ItemActionOccurred(worldItemData, "USED");
			return true;
		}
		return false;
	}
}
