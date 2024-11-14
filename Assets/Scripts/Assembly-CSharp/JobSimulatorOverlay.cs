using UnityEngine;

public class JobSimulatorOverlay : MonoBehaviour
{
	public CompanionCam smoothCam;

	private bool streamerModeEnabled;

	private Camera cam;

	private void Start()
	{
	}

	private void OnGUI()
	{
		GUI.BeginGroup(new Rect(0f, 0f, Screen.width / 8, Screen.height));
		GUI.Box(new Rect(0f, 0f, Screen.width / 8, Screen.height), "Job Simulator Options");
		if (GUI.Button(new Rect(0f, Screen.height - 50, 220f, 30f), (!streamerModeEnabled) ? "ENABLE STREAMER MODE" : "DISABLE STREAMER MODE!"))
		{
			StreamerMode();
		}
		if (streamerModeEnabled)
		{
			GUI.Label(new Rect(0f, Screen.height - 120, 240f, 30f), "STREAMER MODE ENABLED!");
		}
		GUI.EndGroup();
	}

	private void StreamerMode()
	{
		streamerModeEnabled = !streamerModeEnabled;
	}
}
