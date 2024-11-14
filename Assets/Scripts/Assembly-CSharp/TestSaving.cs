using UnityEngine;

public class TestSaving : MonoBehaviour
{
	private void Start()
	{
		GameStateController.LoadState();
		GameStateController.SaveState();
	}
}
