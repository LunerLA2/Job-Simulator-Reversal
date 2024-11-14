using UnityEngine;

public class CleaningBotControlPanel : KitchenTool
{
	[SerializeField]
	private CleaningBot cleaningBotPrefab;

	[SerializeField]
	private string cleaningBotSpawnPos;

	private CleaningBot spawnedCleaningBot;

	[SerializeField]
	private Animator antennaAnimator;

	[SerializeField]
	private JoystickController controlJoystick;

	[SerializeField]
	private LookAtTransform antennaLookAt;

	[SerializeField]
	private GameObject[] objectsWhenBotPoweredOn;

	[SerializeField]
	private VisibilityEvents screenVisibilityEvents;

	private bool screenIsVisible;

	private SecurityStationController securityStation;

	[SerializeField]
	private CleaningBotControlPanelLowRes lowResPanel;

	[SerializeField]
	private bool useLowRes;

	public Transform cleaningBotTransform
	{
		get
		{
			return spawnedCleaningBot.transform;
		}
	}

	public bool ScreenIsVisible
	{
		get
		{
			return screenIsVisible;
		}
	}

	private void Awake()
	{
		securityStation = Object.FindObjectOfType<SecurityStationController>();
		if (securityStation == null)
		{
			Debug.LogError("Couldn't find security station ref", base.gameObject);
		}
		if (useLowRes)
		{
			lowResPanel.Init(this);
			lowResPanel.gameObject.SetActive(true);
		}
	}

	private void Start()
	{
		antennaAnimator.SetBool("IsUp", true);
		if (spawnedCleaningBot == null)
		{
			spawnedCleaningBot = Object.Instantiate(cleaningBotPrefab);
			UniqueObject objectByName = BotUniqueElementManager.Instance.GetObjectByName(cleaningBotSpawnPos);
			spawnedCleaningBot.transform.position = objectByName.transform.position;
			spawnedCleaningBot.transform.rotation = objectByName.transform.rotation;
			spawnedCleaningBot.Setup(controlJoystick, this);
			antennaLookAt.SetTransformToLookAt(spawnedCleaningBot.transform);
		}
		SetPowerState(true);
	}

	private void OnEnable()
	{
		screenVisibilityEvents.OnObjectBecameVisible += ScreenVisibilityEvents_OnObjectBecameVisible;
		screenVisibilityEvents.OnObjectBecameInvisible += ScreenVisibilityEvents_OnObjectBecameInvisible;
	}

	private void OnDisable()
	{
		screenVisibilityEvents.OnObjectBecameVisible -= ScreenVisibilityEvents_OnObjectBecameVisible;
		screenVisibilityEvents.OnObjectBecameInvisible -= ScreenVisibilityEvents_OnObjectBecameInvisible;
	}

	private void ScreenVisibilityEvents_OnObjectBecameInvisible(VisibilityEvents obj)
	{
		screenIsVisible = false;
	}

	private void ScreenVisibilityEvents_OnObjectBecameVisible(VisibilityEvents obj)
	{
		screenIsVisible = true;
	}

	public override void OnDismiss()
	{
		base.OnDismiss();
		SetPowerState(false);
	}

	public override void OnSummon()
	{
		base.OnSummon();
		SetPowerState(true);
	}

	private void SetPowerState(bool s)
	{
		for (int i = 0; i < objectsWhenBotPoweredOn.Length; i++)
		{
			objectsWhenBotPoweredOn[i].SetActive(s);
		}
		if (spawnedCleaningBot != null)
		{
			spawnedCleaningBot.SetPowerState(s);
		}
		if (securityStation != null)
		{
			securityStation.SetCleaningBotState(s);
		}
	}
}
