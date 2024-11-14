using OwlchemyVR;
using UnityEngine;

public class GlobalStorage : MonoBehaviour
{
	public static readonly bool IsDemoMode;

	public static readonly bool SupportSaving = true;

	public bool isShortModeEnabled;

	public bool isSpectatorModeEnabled;

	private static GlobalStorage _instance;

	public MasterHMDAndInputController MasterHMDAndInputController { get; private set; }

	public Transform ContentRoot { get; private set; }

	public LevelController LevelController { get; private set; }

	public GameStateData GameStateData { get; private set; }

	public int StartingTaskIndex { get; private set; }

	public JobGenieCartridge.GenieModeTypes CurrentGenieModes { get; private set; }

	public GameObject MenuBriefcase { get; private set; }

	public CompanionUIManager CompanionUIManager { get; private set; }

	public static GlobalStorage no_instantiate_instance
	{
		get
		{
			return _instance;
		}
	}

	public static GlobalStorage Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Object.FindObjectOfType<GlobalStorage>();
				if (_instance == null)
				{
					_instance = new GameObject("_GlobalStorage").AddComponent<GlobalStorage>();
				}
			}
			return _instance;
		}
	}

	public void SetMasterHMDAndInput(MasterHMDAndInputController masterHMDAndInputController)
	{
		MasterHMDAndInputController = masterHMDAndInputController;
	}

	public void SetContentRoot(Transform contentRoot)
	{
		ContentRoot = contentRoot;
	}

	public void SetLevelController(LevelController levelController)
	{
		LevelController = levelController;
	}

	public void SetGameStateData(GameStateData data)
	{
		GameStateData = data;
	}

	public void SetStartingTaskIndex(int index)
	{
		StartingTaskIndex = index;
	}

	public void SetGenieModes(JobGenieCartridge.GenieModeTypes currentGenieModes)
	{
		CurrentGenieModes = currentGenieModes;
	}

	public void SetMenuBriefcase(GameObject menuObject)
	{
		MenuBriefcase = menuObject;
	}

	public void SetCompanionUIManager(CompanionUIManager comp)
	{
		CompanionUIManager = comp;
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
		}
	}
}
