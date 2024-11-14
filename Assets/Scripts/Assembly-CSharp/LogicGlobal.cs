using UnityEngine;

public class LogicGlobal : MonoBehaviour
{
	private void Start()
	{
	}

	public static void GlobalGUI()
	{
		GUILayout.Label("Press 1-4 to select different sample scenes");
		GUILayout.Space(20f);
	}

	private void Update()
	{
	}
}
