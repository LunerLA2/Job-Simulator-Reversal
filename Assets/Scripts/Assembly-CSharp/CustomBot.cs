using OwlchemyVR;
using UnityEngine;

public class CustomBot : MonoBehaviour
{
	[SerializeField]
	private bool appearOnStart = true;

	[SerializeField]
	private BotCostumeData defaultCostume;

	private Bot bot;

	private bool botAppeared;

	public bool IsActive
	{
		get
		{
			return bot != null && botAppeared;
		}
	}

	private void Start()
	{
		if (appearOnStart)
		{
			Appear();
		}
	}

	public void Appear()
	{
		if (bot == null)
		{
			bot = BotManager.Instance.GetBot();
			bot.SetupBotNoBrain(defaultCostume);
			bot.transform.position = base.transform.position;
			bot.transform.rotation = base.transform.rotation;
			botAppeared = true;
		}
	}

	public void Disappear()
	{
		if (bot != null)
		{
			bot.Disappear();
			BotManager.Instance.ReleaseBot(bot);
			bot = null;
			botAppeared = false;
		}
	}

	public void PlayVO(AudioClip clip, BotVoiceController.VOImportance importance)
	{
		if (bot != null)
		{
			bot.PlayVO(clip, importance);
		}
		else
		{
			Debug.LogWarning(GetInactiveWarningMessage("play VO"));
		}
	}

	public void PlayVO(BotVOInfoData info, BotVoiceController.VOImportance importance)
	{
		if (bot != null)
		{
			bot.PlayVO(info, importance);
		}
		else
		{
			Debug.LogWarning(GetInactiveWarningMessage("play VO"));
		}
	}

	public void StopVO()
	{
		if (bot != null)
		{
			bot.StopVO();
		}
		else
		{
			Debug.LogWarning(GetInactiveWarningMessage("stop VO"));
		}
	}

	public void Emote(BotFaceEmote emote, Sprite customFaceGraphic = null)
	{
		if (bot != null)
		{
			bot.Emote(emote, customFaceGraphic);
		}
		else
		{
			Debug.LogWarning(GetInactiveWarningMessage("set emotion"));
		}
	}

	public void ForceRefreshFaceSettings()
	{
		if (bot != null)
		{
			bot.ForceRefreshFace();
		}
	}

	public void MoveAlongPath(string pathName, BrainEffect.PathTypes pathType, BrainEffect.PathLookTypes pathLookType)
	{
		if (bot != null)
		{
			bot.MoveAlongPath(pathName, pathType, pathLookType);
		}
		else
		{
			Debug.LogWarning(GetInactiveWarningMessage("move along path"));
		}
	}

	public void MoveToWaypoint(string pathName, int waypointIndex, float moveTime)
	{
		if (bot != null)
		{
			bot.MoveToWaypoint(pathName, waypointIndex, moveTime);
		}
		else
		{
			Debug.LogWarning(GetInactiveWarningMessage("move to waypoint"));
		}
	}

	public void MoveToPosition(string positionName, float moveTime)
	{
		if (bot != null)
		{
			bot.MoveToPosition(positionName, moveTime);
		}
		else
		{
			Debug.LogWarning(GetInactiveWarningMessage("move to position"));
		}
	}

	public void AddCostumePiece(GameObject costumePiece)
	{
		if (bot != null)
		{
			bot.AddCostumePiece(costumePiece);
		}
		else
		{
			Debug.LogWarning(GetInactiveWarningMessage("add costume piece"));
		}
	}

	public void ChangeCostume(BotCostumeData costume)
	{
		if (bot != null)
		{
			bot.ChangeCostume(costume);
		}
		else
		{
			Debug.LogWarning(GetInactiveWarningMessage("change costume"));
		}
	}

	public void ItemsOfInterest(BrainEffect.ItemOfInterestTypes actionType, WorldItemData worldItemData = null, float radius = 1f)
	{
		if (bot != null)
		{
			bot.ItemsOfInterest(actionType, worldItemData, radius);
		}
		else
		{
			Debug.LogWarning(GetInactiveWarningMessage("change items of interest"));
		}
	}

	public void GrabItem(string itemID, bool isLargeItem = false)
	{
		if (bot != null)
		{
			bot.GrabItem(itemID, isLargeItem);
		}
		else
		{
			Debug.LogWarning(GetInactiveWarningMessage("grab item"));
		}
	}

	public void EjectPrefab(GameObject prefabToEject, BotInventoryController.EjectTypes ejectType, string ejectToPosition = "", float forceMultiplier = 1f)
	{
		if (bot != null)
		{
			bot.EjectPrefab(prefabToEject, ejectType, ejectToPosition, forceMultiplier);
		}
		else
		{
			Debug.LogWarning(GetInactiveWarningMessage("eject prefab"));
		}
	}

	public void EmptyInventory(BotInventoryController.EmptyTypes emptyType, string optionalLocationID = "", bool onlyMostRecentItem = false)
	{
		if (bot != null)
		{
			bot.EmptyInventory(emptyType, optionalLocationID, onlyMostRecentItem);
		}
		else
		{
			Debug.LogWarning(GetInactiveWarningMessage("empty inventory"));
		}
	}

	public void FloatingHeight(BrainEffect.FloatHeightTypes floatType, float number, string optionalComplexOptions)
	{
		if (bot != null)
		{
			bot.FloatingHeight(floatType, number, optionalComplexOptions);
		}
		else
		{
			Debug.LogWarning(GetInactiveWarningMessage("set floating height"));
		}
	}

	public void LookAt(BrainEffect.LookAtTypes lookAtType, string optionalObjectName = "", float optionalWorldAngle = 0f)
	{
		if (bot != null)
		{
			bot.LookAt(lookAtType, optionalObjectName, optionalWorldAngle);
		}
		else
		{
			Debug.LogWarning(GetInactiveWarningMessage("change lookAt"));
		}
	}

	private string GetInactiveWarningMessage(string action)
	{
		return "Can't " + action + " if the bot hasn't appeared: " + base.gameObject.name;
	}

	public void SetDefaultCostume(BotCostumeData costume)
	{
		defaultCostume = costume;
	}
}
