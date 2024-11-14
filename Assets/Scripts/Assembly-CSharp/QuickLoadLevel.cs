using UnityEngine;

public class QuickLoadLevel : MonoBehaviour
{
	[SerializeField]
	private MechanicalPushButtonController button;

	[SerializeField]
	private string sceneName;

	private void OnEnable()
	{
		button.OnButtonPress += ButtonDown;
	}

	private void OnDisable()
	{
		button.OnButtonPress -= ButtonDown;
	}

	private void ButtonDown()
	{
		LevelLoader.Instance.LoadSceneManual(sceneName);
	}
}
