using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuBrainControlledObject : BrainControlledObject
{
	private const string EFFECT_EXIT_NAME = "exit";

	private const string EFFECT_RESET_NAME = "reset";

	[SerializeField]
	private MenuController menuController;

	[SerializeField]
	private AudioClip exitSound;

	[SerializeField]
	private GameObject exitEffectsPrefab;

	[SerializeField]
	private AudioClip resetSound;

	private string sceneToNotSpawnIn = "Loading_PSVR";

	private bool resettingLevel;

	public override void Appear(BrainData brain)
	{
		if (!(SceneManager.GetActiveScene().name == sceneToNotSpawnIn))
		{
			base.Appear(brain);
			base.gameObject.SetActive(true);
		}
	}

	public override void Disappear()
	{
		base.Disappear();
		base.gameObject.SetActive(false);
	}

	public override void ScriptedEffect(BrainEffect effect)
	{
		if (effect.TextInfo == "exit")
		{
			ExitGame();
		}
		else if (effect.TextInfo == "reset")
		{
			ResetLevel();
		}
		else
		{
			Debug.LogError("MenuBrainControlledObject does not understand the command '" + effect.TextInfo + "'");
		}
	}

	private void OnLevelWasLoaded(int i)
	{
		resettingLevel = false;
	}

	private void ExitGame()
	{
		if (exitSound != null)
		{
			AudioManager.Instance.Play(base.transform.position, exitSound, 1f, 1f);
		}
		Object.Instantiate(exitEffectsPrefab, new Vector3(GlobalStorage.Instance.MasterHMDAndInputController.Head.transform.position.x, 0f, GlobalStorage.Instance.MasterHMDAndInputController.Head.transform.position.z), Quaternion.identity);
		menuController.DisableMenu();
		Invoke("ExitLevelFinish", 0.2f);
	}

	private void ExitLevelFinish()
	{
		LevelLoader.Instance.LoadIntroScene();
	}

	private void ResetLevel()
	{
		if (!resettingLevel)
		{
			resettingLevel = true;
			if (resetSound != null)
			{
				AudioManager.Instance.Play(GlobalStorage.Instance.MasterHMDAndInputController.Head.transform, resetSound, 1f, 1f);
			}
			Invoke("ResetLevelFinish", 1f);
		}
	}

	private void ResetLevelFinish()
	{
		menuController.DisableMenu();
		LevelLoader.Instance.LoadJob(SceneManager.GetActiveScene().name, 0, GlobalStorage.Instance.CurrentGenieModes);
	}
}
