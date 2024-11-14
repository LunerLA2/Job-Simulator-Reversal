using UnityEngine;

public class JobBoardBrainControlledObject : BrainControlledObject
{
	public const bool ALLOW_ALT_M_AND_N = true;

	private const float BOARD_MIN_Y = 1f;

	private const float BOARD_MAX_Y = 2.5f;

	private const float BOARD_HEIGHT_ADJUST_SPEED = 0.3f;

	public static int JOBBOARD_RENDERTEXTURE_WIDTH = 800;

	public static int JOBBOARD_RENDERTEXTURE_ANTIALIAS_LEVEL;

	[SerializeField]
	private GameObjectPrefabSpawner contentsSpawner;

	[SerializeField]
	private MeshRenderer screenMeshRenderer;

	[SerializeField]
	private AudioSourceHelper audioSrcHelper;

	[SerializeField]
	private BotLocomotion locomotionController;

	[SerializeField]
	private VisibilityEvents visibilityEvents;

	[SerializeField]
	private JobScoreBoard scoreboard;

	private Vector3 defaultPosition;

	private Transform playerHead;

	private bool forcePlayerHeadHeight;

	private float desiredFloatHeight;

	private void Awake()
	{
		JobBoardManager.instance.SetAudioSrcHelper(audioSrcHelper);
		defaultPosition = base.transform.position;
		desiredFloatHeight = 0f;
		contentsSpawner.LastSpawnedPrefabGO.transform.SetParent(GlobalStorage.Instance.ContentRoot, true);
		contentsSpawner.LastSpawnedPrefabGO.transform.localScale = Vector3.one;
		RenderTexture renderTexture = new RenderTexture(JOBBOARD_RENDERTEXTURE_WIDTH, Mathf.RoundToInt((float)JOBBOARD_RENDERTEXTURE_WIDTH / 1.5686275f), 0);
		if (JOBBOARD_RENDERTEXTURE_ANTIALIAS_LEVEL > 0)
		{
			renderTexture.antiAliasing = JOBBOARD_RENDERTEXTURE_ANTIALIAS_LEVEL;
		}
		JobBoardManager.instance.SetMainRenderTexture(renderTexture);
		JobBoardManager.instance.SetVisibilityEvents(visibilityEvents);
		screenMeshRenderer.material.mainTexture = renderTexture;
		screenMeshRenderer.material.SetTexture("_EmissionMap", renderTexture);
		Debug.LogWarning("Be aware that you are running a build with the Alt+M and Alt+N shortcuts enabled!");
	}

	private void Update()
	{
		AlignEyeLevel();
		if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)))
		{
			if (Input.GetKeyDown(KeyCode.M))
			{
				JobBoardManager.instance.TestingForceCurrentSubtaskComplete();
			}
			if (Input.GetKeyDown(KeyCode.N))
			{
				JobBoardManager.instance.TestingForceCurrentTaskComplete();
			}
		}
	}

	public override void Appear(BrainData brain)
	{
		base.Appear(brain);
		base.transform.position = defaultPosition;
		if (JobBoardManager.instance != null && JobBoardManager.instance.EndlessModeStatusController != null && scoreboard != null)
		{
			scoreboard.gameObject.SetActive(true);
		}
	}

	public override void Disappear()
	{
		base.Disappear();
		defaultPosition = base.transform.position;
		desiredFloatHeight = 0f;
		base.transform.position = Vector3.forward * 1000f;
	}

	public override void FloatingHeight(BrainEffect.FloatHeightTypes floatType, float number, string optionalComplexOptions)
	{
		forcePlayerHeadHeight = floatType == BrainEffect.FloatHeightTypes.MatchPlayer;
		desiredFloatHeight = number;
	}

	private void AlignEyeLevel()
	{
		if (playerHead == null)
		{
			playerHead = GlobalStorage.Instance.MasterHMDAndInputController.TrackedHmdTransform;
		}
		float num = desiredFloatHeight + defaultPosition.y - base.transform.position.y;
		if (playerHead != null && forcePlayerHeadHeight)
		{
			num = playerHead.transform.position.y - base.transform.position.y;
		}
		Vector3 localPosition = base.transform.localPosition;
		localPosition.y = Mathf.Lerp(localPosition.y, localPosition.y + num, 0.3f * Time.deltaTime);
		localPosition.y = Mathf.Clamp(localPosition.y, 1f, 2.5f);
		base.transform.localPosition = localPosition;
	}

	public override void MoveToPosition(string positionName, float moveDuration)
	{
		UniqueObject objectByName = BotUniqueElementManager.Instance.GetObjectByName(positionName);
		if (objectByName != null)
		{
			MoveToWithoutY(objectByName.transform.position, objectByName.transform.rotation, moveDuration);
		}
		else
		{
			Debug.LogError("JobBoard failed to move to position '" + objectByName.name + "' because it couldn't be found.");
		}
	}

	public override void MoveToWaypoint(string pathName, int waypointIndex, float moveTime)
	{
		BotPathWaypoint waypointOfPath = BotUniqueElementManager.Instance.GetWaypointOfPath(pathName, waypointIndex);
		if (waypointOfPath != null)
		{
			MoveToWithoutY(waypointOfPath.transform.position, waypointOfPath.transform.rotation, moveTime);
			return;
		}
		Debug.LogError("JobBoard failed to move to waypoint " + waypointIndex + " of path '" + pathName + "' because it couldn't be found.");
	}

	public override void MoveAlongPath(string pathName, BrainEffect.PathTypes pathMovementType, BrainEffect.PathLookTypes pathLookType)
	{
		BotPath pathByName = BotUniqueElementManager.Instance.GetPathByName(pathName);
		if (pathByName != null)
		{
			locomotionController.DoPath(pathByName, pathMovementType, pathLookType);
		}
		else
		{
			Debug.LogError("JobBoard failed to move along path '" + pathName + "' because it couldn't be found.");
		}
	}

	private void MoveToWithoutY(Vector3 position, Quaternion rotation, float time)
	{
		position.y = defaultPosition.y;
		if (time > 0f)
		{
			Go.to(base.transform, time, new GoTweenConfig().position(position).setEaseType(GoEaseType.QuadInOut).rotation(rotation));
		}
		else
		{
			base.transform.position = position;
		}
	}

	public override void ScriptedEffect(BrainEffect effect)
	{
		switch (effect.TextInfo)
		{
		case "ReadyForEndlessTaskEnd":
			JobBoardManager.instance.EndlessModeStatusController.readyToBeginNextGoal = true;
			break;
		case "NotReadyForEndlessTaskEnd":
			JobBoardManager.instance.EndlessModeStatusController.readyToBeginNextGoal = false;
			break;
		case "CompleteTaskWithoutSuccess":
			JobBoardManager.instance.EndlessModeStatusController.GetCurrentGoal().ForceComplete(false);
			break;
		}
	}
}
