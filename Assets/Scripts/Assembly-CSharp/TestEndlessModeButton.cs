using OwlchemyVR2;
using UnityEngine;

public class TestEndlessModeButton : MonoBehaviour
{
	[SerializeField]
	private PushableButton button;

	[SerializeField]
	private AudioClip loadJobSound;

	[SerializeField]
	private string levelToLoad;

	private bool wasButtonPressed;

	private void OnEnable()
	{
		button.OnButtonPushed += ButtonPressed;
	}

	private void OnDisable()
	{
		button.OnButtonPushed -= ButtonPressed;
	}

	private void ButtonPressed(PushableButton _button)
	{
		if (!wasButtonPressed)
		{
			wasButtonPressed = true;
			AudioManager.Instance.Play(base.transform.position, loadJobSound, 1f, 1f);
			LevelLoader.Instance.LoadJob(levelToLoad, -1, JobGenieCartridge.GenieModeTypes.EndlessMode);
		}
	}
}
