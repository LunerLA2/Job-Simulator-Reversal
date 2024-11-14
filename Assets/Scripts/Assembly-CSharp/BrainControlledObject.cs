using OwlchemyVR;
using UnityEngine;

public class BrainControlledObject : MonoBehaviour
{
	protected BrainData currentBrainData;

	public BrainData CurrentBrainData
	{
		get
		{
			return currentBrainData;
		}
	}

	public virtual void Appear(BrainData brain)
	{
		currentBrainData = brain;
		if (brain == null)
		{
			Debug.LogError("BrainControlledObject '" + base.gameObject.name + "' ran Setup with no brain");
		}
	}

	public virtual void Disappear()
	{
		currentBrainData = null;
	}

	public virtual WorldItemData GetCurrentWorldItemData()
	{
		return null;
	}

	public virtual void PlayVO(AudioClip clip, BotVoiceController.VOImportance importance)
	{
		NotImplementedError("PlayVO (AudioClip)");
	}

	public virtual void PlayVO(BotVOInfoData voInfo, BotVoiceController.VOImportance importance)
	{
		NotImplementedError("PlayVO (BotVOInfoData)");
	}

	public virtual void StopVO()
	{
		NotImplementedError("StopVO");
	}

	public virtual void Emote(BotFaceEmote emote, Sprite customSprite)
	{
		NotImplementedError("Emote");
	}

	public virtual void MoveAlongPath(string pathName, BrainEffect.PathTypes pathingType, BrainEffect.PathLookTypes pathLookType)
	{
		NotImplementedError("MoveAlongPath");
	}

	public virtual void MoveToWaypoint(string pathName, int waypointIndex, float moveDuration)
	{
		NotImplementedError("MoveToWaypoint");
	}

	public virtual void MoveToPosition(string positionName, float moveDuration)
	{
		NotImplementedError("MoveToPosition");
	}

	public virtual void AddCostumePiece(GameObject piece)
	{
		NotImplementedError("AddCostumePiece");
	}

	public virtual void ChangeCostume(BotCostumeData costumeData)
	{
		NotImplementedError("ChangeCostume");
	}

	public virtual void ChangeVOProfile()
	{
		NotImplementedError("ChangeVOProfile");
	}

	public virtual void ItemsOfInterest(BrainEffect.ItemOfInterestTypes actionType, WorldItemData worldItemData = null, float radius = 1f)
	{
		NotImplementedError("ItemsOfInterest");
	}

	public virtual void GrabItem(string itemName, bool isLarge)
	{
		NotImplementedError("GrabItem");
	}

	public virtual void EjectPrefab(GameObject prefab, BotInventoryController.EjectTypes ejectType, string locationName = "", float forceMultiplier = 1f)
	{
		NotImplementedError("EjectPrefab");
	}

	public virtual void EmptyInventory(BotInventoryController.EmptyTypes emptyType, string locationName = "", bool onlyMostRecentItem = false, WorldItemData optionalWorldItemDataFilter = null)
	{
		NotImplementedError("EmptyInventory");
	}

	public virtual void FloatingHeight(BrainEffect.FloatHeightTypes floatType, float number, string optionalComplexOptions)
	{
		NotImplementedError("FloatingHeight");
	}

	public virtual void LookAt(BrainEffect.LookAtTypes lookAtType, string optionalObjectName = "", float optionalWorldAngle = 0f)
	{
		NotImplementedError("LookAt");
	}

	public virtual void ScriptedEffect(BrainEffect effect)
	{
		NotImplementedError("ScriptedEffect");
	}

	public virtual void StopMoving()
	{
		NotImplementedError("StopMoving");
	}

	public virtual void ChangeParent(string parentUniqueID, BrainEffect.ChangeParentTypes mode, float optionalTweenTime = 0f)
	{
		NotImplementedError("ChangeParent");
	}

	private void NotImplementedError(string effect)
	{
		Debug.LogError("Effect not implemented on '" + base.gameObject.name + "': " + effect);
	}
}
