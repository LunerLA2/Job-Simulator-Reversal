using UnityEngine;

public class CleaningBotControlPanelLowRes : MonoBehaviour
{
	private const string pink = "Pink";

	private const string blue = "Blue";

	[SerializeField]
	private GameObject lowResBot;

	[SerializeField]
	private CleaningBotControlPanel controlPanel;

	[SerializeField]
	private Transform stainParent;

	[SerializeField]
	private LowResStainController blueStain;

	[SerializeField]
	private LowResStainController pinkStain;

	private FloorStainController[] stains;

	public void Init(CleaningBotControlPanel control)
	{
		controlPanel = control;
		stains = Object.FindObjectsOfType<FloorStainController>();
		for (int i = 0; i < stains.Length; i++)
		{
			LowResStainController lowResStainController = null;
			if (stains[i].name.Contains("Pink"))
			{
				lowResStainController = Object.Instantiate(pinkStain, stainParent) as LowResStainController;
			}
			else if (stains[i].name.Contains("Blue"))
			{
				lowResStainController = Object.Instantiate(blueStain, stainParent) as LowResStainController;
			}
			lowResStainController.transform.localPosition = stains[i].transform.position;
			lowResStainController.transform.localScale = Vector3.one * 10f;
			lowResStainController.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
			lowResStainController.Init(stains[i]);
		}
	}

	private void Update()
	{
		if (!(controlPanel == null))
		{
			Vector3 position = controlPanel.cleaningBotTransform.position;
			Vector3 localEulerAngles = controlPanel.cleaningBotTransform.localEulerAngles;
			Vector3 localPosition = new Vector3(position.x, 0f, position.z);
			lowResBot.transform.localPosition = localPosition;
			Vector3 localEulerAngles2 = new Vector3(90f, localEulerAngles.y, 0f);
			lowResBot.transform.localEulerAngles = localEulerAngles2;
		}
	}
}
