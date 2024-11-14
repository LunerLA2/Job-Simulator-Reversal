using System.Collections;
using System.Collections.Generic;
using OwlchemyVR;
using OwlchemyVR.BuildSystem;
using PSC;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
	public const string USE_BAKED_SCENES_KEY = "use_baked_scenes";

	public const string BAKED_SCENE_FOLDER_NAME = "Baked_Scenes";

	public const string BAKED_SCENE_PATH = "Assets/Baked_Scenes";

	public const string BASE_SCENE_PATH = "Scenes/Jobs";

	private static LevelLoader _instance;

	private Coroutine returnToLauncherRoutine;

	private Coroutine levelLoadRoutine;

	public static LevelLoader Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Object.FindObjectOfType<LevelLoader>();
				if (_instance == null)
				{
					_instance = new GameObject("_LevelLoader").AddComponent<LevelLoader>();
				}
			}
			return _instance;
		}
	}

	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
		}
		else if (_instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		if (Application.isPlaying && Application.isEditor)
		{
		}
	}

	public void Init()
	{
	}

	public void ReturnToLauncher(string additionalArguments = "")
	{
		Debug.LogWarning("Calling Return to Launcher Routine");
		if (returnToLauncherRoutine == null)
		{
			returnToLauncherRoutine = StartCoroutine(ReturnToLauncherRoutine(additionalArguments));
		}
	}

	private IEnumerator ReturnToLauncherRoutine(string additionalArguments = "")
	{
		ScreenFader.Instance.FadeOut(2.5f);
		yield return new WaitForSeconds(2.5f);
		returnToLauncherRoutine = null;
		Debug.LogError("Only PS4 Has A Launcher");
	}

	public void LoadIntroScene()
	{
		GlobalStorage.Instance.SetStartingTaskIndex(0);
		GlobalStorage.Instance.SetGenieModes(JobGenieCartridge.GenieModeTypes.None);
		LoadSceneManual("Museum");
	}

	public void LoadJob(string jobSceneName, int startingTaskIndex, JobGenieCartridge.GenieModeTypes genieModes)
	{
		GlobalStorage.Instance.SetStartingTaskIndex(startingTaskIndex);
		GlobalStorage.Instance.SetGenieModes(genieModes);
		LoadSceneManual(jobSceneName);
	}

	public void LoadSceneManualWithoutFade(string sceneName)
	{
		LoadScene(sceneName);
	}

	public void LoadSceneManual(string sceneName, float fadeTime = 2.25f, float delay = 0f)
	{
		StartCoroutine(LoadSceneManualRoutine(sceneName, fadeTime, delay));
	}

	private IEnumerator LoadSceneManualRoutine(string sceneName, float fadeTime, float delay)
	{
		yield return new WaitForSeconds(delay);
		ScreenFader.Instance.FadeOut(fadeTime);
		yield return new WaitForSeconds(fadeTime);
		string sceneNameLoadedString = ((!GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode)) ? sceneName : (sceneName + "-Overtime"));
		AnalyticsManager.CustomEvent("Job Loaded", "Job", sceneNameLoadedString);
		AnalyticsManager.CustomEvent("Overtime Loaded", "Job", sceneNameLoadedString);
		AnalyticsManager.CustomEvent("Job Time", new Dictionary<string, object>
		{
			{
				"Job",
				SceneManager.GetActiveScene().name
			},
			{
				"Time",
				Time.timeSinceLevelLoad
			}
		});
		LoadScene(sceneName);
	}

	private void LoadScene(string sceneName)
	{
		TimeManager.Clear();
		BotVoiceController.ClearAllGlobalCurrentlyPlaying();
		if (GlobalStorage.no_instantiate_instance != null)
		{
			MasterHMDAndInputController masterHMDAndInputController = GlobalStorage.no_instantiate_instance.MasterHMDAndInputController;
			if (masterHMDAndInputController != null)
			{
				if (masterHMDAndInputController.Head != null)
				{
					masterHMDAndInputController.Head.Cleanup();
				}
				if (masterHMDAndInputController.LeftHand != null)
				{
					masterHMDAndInputController.LeftHand.TryRelease();
					masterHMDAndInputController.LeftHand.HapticsController.ManuallyClearHaptics();
				}
				if (masterHMDAndInputController.RightHand != null)
				{
					masterHMDAndInputController.RightHand.TryRelease();
					masterHMDAndInputController.RightHand.HapticsController.ManuallyClearHaptics();
				}
			}
		}
		Debug.Log("about to load scene named " + sceneName);
		if (ShouldUseBakedScenes())
		{
			Debug.Log("Using baked scenes!");
			SceneManager.LoadScene(GetBakedScenePathForLoad(sceneName));
		}
		else
		{
			Debug.Log("NOT using baked scenes!");
			SceneManager.LoadScene(sceneName);
		}
	}

	private bool ShouldUseBakedScenes()
	{
		if (Application.isEditor)
		{
			return PlayerPrefs.GetInt("use_baked_scenes", 0) == 1;
		}
		return RuntimeBuildSettings.UseBakedScenes;
	}

	public static string GetBakedScenePathForLoad(string sceneName, LayoutConfiguration layoutConfig = null)
	{
		if (layoutConfig == null)
		{
			layoutConfig = Room.defaultLayoutToLoad;
		}
		if (layoutConfig == null)
		{
			Debug.LogError("No current room layout configuration! Can't build a path to a baked scene!");
			return null;
		}
		return "Baked_Scenes/" + layoutConfig.name + "/" + sceneName;
	}

	private void OnApplicationQuit()
	{
		Analytics.CustomEvent("Job Time", new Dictionary<string, object>
		{
			{
				"Job",
				SceneManager.GetActiveScene().name
			},
			{
				"Time",
				Time.timeSinceLevelLoad
			},
			{
				"Overtime Enabled",
				GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode)
			}
		});
		AnalyticsManager.CustomEvent("Session Time", "Time", Time.realtimeSinceStartup);
	}
}
