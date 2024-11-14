using OwlchemyVR;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
	private static CoreGameLoader coreGameLoader;

	private string menuBriefcasePrefabName = "MenuBriefcase";

	private string menuBriefcasePrefabSmallName = "MenuBriefcasePSVR";

	[SerializeField]
	private JobGenieCartridge.GenieModeTypes editorGenieModeTypesEnabled;

	private void Awake()
	{
		GlobalStorage.Instance.SetContentRoot(base.transform);
		GlobalStorage.Instance.SetLevelController(this);
		if (GlobalStorage.Instance.GameStateData == null)
		{
			GameStateController.LoadState();
		}
		TemperatureManager.Instance.Init();
		LoadCoreIfNeeded();
		GlobalStorage.Instance.MenuBriefcase.GetComponent<UniqueObject>().Reregister();
		GlobalStorage.Instance.MenuBriefcase.GetComponent<MenuController>().EnableMenu();
		GenieManager.Init();
	}

	private void Start()
	{
		AudioManager.Instance.NewSceneLoaded(SceneManager.GetActiveScene().name);
		GenieDependentObject[] componentsInChildren = base.gameObject.GetComponentsInChildren<GenieDependentObject>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].CheckGenieSettings();
		}
	}

	private void LoadCoreIfNeeded()
	{
		if (coreGameLoader == null)
		{
			Debug.Log("Load Core!");
			string empty = string.Empty;
			VersionInfoStorage versionInfoStorage = VersionInfoStorage.LoadVersionInfoStorageFromResources();
			if (versionInfoStorage != null)
			{
				empty += versionInfoStorage.GetBuildNumber();
				Debug.Log("Build Number: " + versionInfoStorage.GetBuildNumber());
			}
			else
			{
				empty += "0.0.0";
				Debug.LogError("NO BUILD NUMBER WAS FOUND");
			}
			VRPlatformTypes currVRPlatformType = VRPlatform.GetCurrVRPlatformType();
			VRPlatformHardwareType currVRPlatformHardwareType = VRPlatform.GetCurrVRPlatformHardwareType();
			AnalyticsManager.CustomEvent("Platform", "Platform Type", currVRPlatformType.ToString());
			AnalyticsManager.CustomEvent("VR Hardware", "VR Hardware Type", currVRPlatformHardwareType.ToString());
			AnalyticsManager.CustomEvent("Operating System", "OS Type", SystemInfo.operatingSystem);
			if (Application.isEditor)
			{
				empty += "-Editor";
			}
			if (!Application.isEditor)
			{
			}
			string coreLoaderNameForCurrConfig = VRPlatform.GetCoreLoaderNameForCurrConfig();
			coreGameLoader = Resources.Load<CoreGameLoader>("CoreGameLoader-" + coreLoaderNameForCurrConfig);
			if (coreGameLoader == null)
			{
				Debug.LogError("Could not find core game loader:" + currVRPlatformType);
				return;
			}
			coreGameLoader.Load();
			Object.DontDestroyOnLoad(coreGameLoader.gameObject);
			GlobalStorage.Instance.SetMasterHMDAndInput(coreGameLoader.MasterHMDAndInputController);
			LevelLoader.Instance.Init();
			Object @object = null;
			@object = Resources.Load(menuBriefcasePrefabSmallName);
			GameObject gameObject = Object.Instantiate(@object) as GameObject;
			Object.DontDestroyOnLoad(gameObject);
			GlobalStorage.Instance.SetMenuBriefcase(gameObject);
		}
	}

	public void CleanupLevel()
	{
		foreach (Transform item in base.transform)
		{
			Object.Destroy(item.gameObject);
		}
	}

	public void CleanupNonLevelItems()
	{
	}
}
