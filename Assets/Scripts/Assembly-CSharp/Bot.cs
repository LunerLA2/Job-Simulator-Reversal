using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using OwlchemyVR;
using UnityEngine;

public class Bot : BrainControlledObject
{
	private const float DEFAULT_BOT_MIN_Y = 0.5f;

	private const float DEFAULT_BOT_MAX_Y = 2f;

	private const float BOT_LOOK_AT_SPEED = 2.5f;

	private const float BOT_HEIGHT_ADJUST_SPEED = 1f;

	private float playerMatchMinY = 0.5f;

	private float playerMatchMaxY = 2f;

	[Header("References")]
	[SerializeField]
	protected BotVoiceController voiceController;

	[SerializeField]
	protected BotLocomotion locomotionController;

	[SerializeField]
	private BotInventoryController inventoryController;

	[SerializeField]
	private BotSightController sightController;

	[SerializeField]
	private VisibilityEvents faceVisibilityEvents;

	[SerializeField]
	private VisibilityEvents bodyVisibilityEvents;

	[SerializeField]
	private WorldItem worldItem;

	[SerializeField]
	private UniqueObject uniqueObject;

	[SerializeField]
	private Transform heightChangeTransform;

	[SerializeField]
	private Transform eyeLevel;

	[SerializeField]
	[Header("Costume and Visuals")]
	private Transform costumeArtParent;

	[SerializeField]
	private AttachablePoint glassesAttachPoint;

	[SerializeField]
	private AttachablePoint hatAttachPoint;

	private PreloadedCostume currentCostume;

	private List<Transform> manuallyAddedCostumePieces = new List<Transform>();

	private List<AttachablePoint> attachpointsOnBot = new List<AttachablePoint>();

	[SerializeField]
	private MeshRenderer screenGlowRenderer;

	[SerializeField]
	private MeshRenderer[] faceScreenRenderers;

	[SerializeField]
	private GameObject faceRendererRenderTexture;

	[SerializeField]
	private GameObject faceRendererNonRenderTexture;

	[SerializeField]
	private MeshRenderer bodyRenderer;

	[SerializeField]
	private GameObject hoverFX;

	private Texture storedDefaultBodyTexture;

	[SerializeField]
	private Animation impactAnimation;

	[SerializeField]
	private ParticleSystem impactParticle;

	[SerializeField]
	private AudioClip[] impactZapAudio;

	[SerializeField]
	private float minZapPitch = 0.8f;

	[SerializeField]
	private float maxZapPitch = 1f;

	[SerializeField]
	[Header("Movement Audio")]
	private AudioClip movementLoopSound;

	[SerializeField]
	private float minMovementPitch = 0.8f;

	[SerializeField]
	private float maxMovementPitch = 1f;

	[SerializeField]
	private float secondsToFadeLoopIn = 1f;

	[SerializeField]
	private float secondsToFadeLoopOut = 1f;

	[SerializeField]
	private AudioSourceHelper movementAudioSource;

	private Vector3 lastPosition;

	private bool isLoopingMovementSoundPlaying;

	private float loopingMovementSoundVolume = 1f;

	[Header("Optimization")]
	[SerializeField]
	private GameObject[] objectsToDisableWhenOptimized;

	[SerializeField]
	private GameObject[] objectsToEnableWhenOptimized;

	private Transform[] optimizedObjectsCachedParents;

	private bool isOptimized;

	private Transform playerHead;

	private BrainEffect.FloatHeightTypes floatMode;

	private float floatWorldHeight;

	private float floatMinBobHeight;

	private float floatMaxBobHeight = 1f;

	private float floatMinBobTime = 3f;

	private float floatMaxBobTime = 5f;

	private float floatNextBobTime;

	private float floatCurrentBobHeight;

	private float floatSineFrequency = 5f;

	private float floatSineLowBound;

	private float floatSineHighBound = 1f;

	private bool isMoving;

	private bool faceIsVisible;

	private BrainEffect.LookAtTypes lookAtMode;

	private float desiredLookYawAngle;

	private Transform lookAtTransform;

	private BotFaceController faceController;

	private bool hasUniqueFace;

	private BotCostumeData costumeData;

	private BotVOProfileData currentVOProfile;

	private Coroutine skipVOAudioSequence;

	private Coroutine introVOAudioSequence;

	private Coroutine successVOAudioSequence;

	public static bool USE_NON_RENDERTEXTURE_SIMPLEFACE
	{
		get
		{
			return false;
		}
	}

	public BotInventoryController InventoryController
	{
		get
		{
			return inventoryController;
		}
	}

	public BotCostumeData CostumeData
	{
		get
		{
			return costumeData;
		}
	}

	private void OnEnable()
	{
		BotVoiceController botVoiceController = voiceController;
		botVoiceController.OnVOWasPlayed = (Action<VOContainer>)Delegate.Combine(botVoiceController.OnVOWasPlayed, new Action<VOContainer>(VoiceBeganTalking));
		BotVoiceController botVoiceController2 = voiceController;
		botVoiceController2.OnAudioPlayComplete = (Action<BotVoiceController>)Delegate.Combine(botVoiceController2.OnAudioPlayComplete, new Action<BotVoiceController>(VoiceStoppedTalking));
		BotVoiceController botVoiceController3 = voiceController;
		botVoiceController3.OnBotEmoteWasTriggered = (Action<BotFaceEmote, Sprite>)Delegate.Combine(botVoiceController3.OnBotEmoteWasTriggered, new Action<BotFaceEmote, Sprite>(VOTriggeredEmotion));
		BotVoiceController botVoiceController4 = voiceController;
		botVoiceController4.OnAudioSequencePlayComplete = (Action<BotVoiceController, Coroutine>)Delegate.Combine(botVoiceController4.OnAudioSequencePlayComplete, new Action<BotVoiceController, Coroutine>(OnAudioSequencePlayComplete));
		faceVisibilityEvents.OnObjectBecameVisible += FaceBecameVisible;
		faceVisibilityEvents.OnObjectBecameInvisible += FaceBecameInvisible;
	}

	private void OnDisable()
	{
		BotVoiceController botVoiceController = voiceController;
		botVoiceController.OnVOWasPlayed = (Action<VOContainer>)Delegate.Remove(botVoiceController.OnVOWasPlayed, new Action<VOContainer>(VoiceBeganTalking));
		BotVoiceController botVoiceController2 = voiceController;
		botVoiceController2.OnAudioPlayComplete = (Action<BotVoiceController>)Delegate.Remove(botVoiceController2.OnAudioPlayComplete, new Action<BotVoiceController>(VoiceStoppedTalking));
		BotVoiceController botVoiceController3 = voiceController;
		botVoiceController3.OnBotEmoteWasTriggered = (Action<BotFaceEmote, Sprite>)Delegate.Remove(botVoiceController3.OnBotEmoteWasTriggered, new Action<BotFaceEmote, Sprite>(VOTriggeredEmotion));
		BotVoiceController botVoiceController4 = voiceController;
		botVoiceController4.OnAudioSequencePlayComplete = (Action<BotVoiceController, Coroutine>)Delegate.Remove(botVoiceController4.OnAudioSequencePlayComplete, new Action<BotVoiceController, Coroutine>(OnAudioSequencePlayComplete));
		faceVisibilityEvents.OnObjectBecameVisible -= FaceBecameVisible;
		faceVisibilityEvents.OnObjectBecameInvisible -= FaceBecameInvisible;
	}

	private void FaceBecameVisible(VisibilityEvents e)
	{
		faceIsVisible = true;
		UpdateFaceVisibilitySetting();
	}

	private void FaceBecameInvisible(VisibilityEvents e)
	{
		faceIsVisible = false;
		UpdateFaceVisibilitySetting();
	}

	private void UpdateFaceVisibilitySetting()
	{
		if (faceController != null)
		{
			faceController.SetIsVisible(faceIsVisible);
		}
	}

	public override void Appear(BrainData brain)
	{
		base.Appear(brain);
		movementAudioSource.SetPitch(UnityEngine.Random.Range(minMovementPitch, maxMovementPitch));
		base.transform.SetParent(BotManager.Instance.BotParent, true);
		base.transform.localRotation = Quaternion.identity;
		base.transform.localScale = Vector3.one;
		hoverFX.SetActive(true);
		optimizedObjectsCachedParents = new Transform[objectsToEnableWhenOptimized.Length];
		if (brain != null)
		{
			ChangeCostume(brain.GetCostumeData());
			ManualSetWorldItemData(brain.GetWorldItemData());
			base.gameObject.name = brain.name;
			BotUniqueElementManager.Instance.RegisterObject(uniqueObject);
			for (int i = 0; i < objectsToDisableWhenOptimized.Length; i++)
			{
				objectsToDisableWhenOptimized[i].SetActive(!brain.IsOptimizedBackgroundBot);
			}
			for (int j = 0; j < objectsToEnableWhenOptimized.Length; j++)
			{
				objectsToEnableWhenOptimized[j].SetActive(brain.IsOptimizedBackgroundBot);
				optimizedObjectsCachedParents[j] = objectsToEnableWhenOptimized[j].transform.parent;
				if (!brain.IsOptimizedBackgroundBot)
				{
					objectsToEnableWhenOptimized[j].transform.SetParent(GlobalStorage.Instance.ContentRoot);
				}
			}
			isOptimized = brain.IsOptimizedBackgroundBot;
		}
		else
		{
			for (int k = 0; k < objectsToDisableWhenOptimized.Length; k++)
			{
				objectsToDisableWhenOptimized[k].SetActive(true);
			}
			for (int l = 0; l < objectsToEnableWhenOptimized.Length; l++)
			{
				objectsToEnableWhenOptimized[l].SetActive(false);
				optimizedObjectsCachedParents[l] = objectsToEnableWhenOptimized[l].transform.parent;
				objectsToEnableWhenOptimized[l].transform.SetParent(GlobalStorage.Instance.ContentRoot);
			}
			isOptimized = false;
		}
		inventoryController.ClearItemsOfInterest();
		sightController.ClearItemsOfInterest();
		inventoryController.SetRadiusMultiplier(1f);
		sightController.SetRadiusMultiplier(1f);
		SetFace(false);
		LookAt(BrainEffect.LookAtTypes.Player, string.Empty, 0f);
		FloatingHeight(BrainEffect.FloatHeightTypes.MatchPlayer, 0f, string.Empty);
		playerMatchMinY = 0.5f;
		playerMatchMaxY = 2f;
		lastPosition = base.transform.position;
		movementAudioSource.enabled = false;
		isLoopingMovementSoundPlaying = false;
		if (playerHead == null)
		{
			playerHead = GlobalStorage.Instance.MasterHMDAndInputController.TrackedHmdTransform;
		}
		if (playerHead != null && playerHead.position != base.transform.position)
		{
			base.transform.rotation = Quaternion.LookRotation(playerHead.position - base.transform.position, base.transform.up);
			base.transform.localEulerAngles = new Vector3(0f, base.transform.localEulerAngles.y, 0f);
		}
	}

	public void SetupBotNoBrain(BotCostumeData costume)
	{
		base.transform.localScale = Vector3.one;
		currentBrainData = null;
		if (costume != null)
		{
			ChangeCostume(costume);
		}
		SetFace(false);
	}

	public override void Disappear()
	{
		base.Disappear();
		BotUniqueElementManager.Instance.UnregisterObject(uniqueObject);
		base.transform.SetParent(BotManager.Instance.BotParent, true);
		hoverFX.SetActive(true);
		base.transform.localRotation = Quaternion.identity;
		base.transform.localScale = Vector3.one;
		inventoryController.Cleanup();
		inventoryController.SetRadiusMultiplier(1f);
		inventoryController.SetItemFilteringType(BotInventoryController.ItemFilteringTypes.OnlyTakeItemsOfInterest);
		sightController.SetRadiusMultiplier(1f);
		ReleaseFace();
		playerMatchMinY = 0.5f;
		playerMatchMaxY = 2f;
		if (objectsToDisableWhenOptimized != null && optimizedObjectsCachedParents != null)
		{
			for (int i = 0; i < objectsToEnableWhenOptimized.Length; i++)
			{
				objectsToEnableWhenOptimized[i].transform.SetParent(optimizedObjectsCachedParents[i]);
				objectsToEnableWhenOptimized[i].transform.SetToDefaultPosRotScale();
			}
		}
	}

	public override WorldItemData GetCurrentWorldItemData()
	{
		return worldItem.Data;
	}

	public void SetFace(bool isUnique)
	{
		if (isUnique)
		{
			if (!hasUniqueFace || faceController == null)
			{
				ReleaseFace();
				faceController = BotManager.Instance.GetUniqueFace();
				hasUniqueFace = true;
				BotFaceController botFaceController = faceController;
				botFaceController.OnFaceFinishedUniqueAction = (Action)Delegate.Combine(botFaceController.OnFaceFinishedUniqueAction, new Action(UniqueFaceFinishedBeingUnique));
				UpdateFaceVisibilitySetting();
			}
		}
		else
		{
			if (hasUniqueFace)
			{
				ReleaseFace();
			}
			faceController = BotManager.Instance.GetSimpleFace();
			hasUniqueFace = false;
			UpdateFaceVisibilitySetting();
		}
		for (int i = 0; i < faceScreenRenderers.Length; i++)
		{
			if (faceScreenRenderers[i].gameObject != faceRendererNonRenderTexture)
			{
				faceScreenRenderers[i].material.mainTexture = faceController.RenderTexture;
				faceScreenRenderers[i].material.SetTexture("_EmissionMap", faceController.RenderTexture);
			}
		}
		if (USE_NON_RENDERTEXTURE_SIMPLEFACE)
		{
			faceRendererNonRenderTexture.SetActive(!isUnique);
			faceRendererRenderTexture.SetActive(isUnique);
		}
		else
		{
			faceRendererNonRenderTexture.SetActive(false);
			faceRendererRenderTexture.SetActive(true);
		}
		UpdateFaceVisibilitySetting();
		UpdateFaceColours();
	}

	private void ReleaseFace()
	{
		if (faceController != null)
		{
			BotFaceController botFaceController = faceController;
			botFaceController.OnFaceFinishedUniqueAction = (Action)Delegate.Remove(botFaceController.OnFaceFinishedUniqueAction, new Action(UniqueFaceFinishedBeingUnique));
			if (hasUniqueFace)
			{
				faceController.gameObject.name = "BotFace";
				BotManager.Instance.ReleaseUniqueFace(faceController);
			}
			else
			{
				BotManager.Instance.ReleaseSimpleFace();
			}
			hasUniqueFace = false;
			faceController = null;
		}
	}

	private void UniqueFaceFinishedBeingUnique()
	{
		SetFace(false);
	}

	private void UpdateFaceColours()
	{
		if (hasUniqueFace)
		{
			for (int i = 0; i < faceScreenRenderers.Length; i++)
			{
				faceScreenRenderers[i].material.color = Color.white;
				faceScreenRenderers[i].material.SetColor("_EmissionColor", Color.white);
			}
			faceController.AssignAsUniqueFaceToBot(this);
		}
		else if (costumeData != null)
		{
			for (int j = 0; j < faceScreenRenderers.Length; j++)
			{
				faceScreenRenderers[j].material.color = costumeData.MainScreenColor;
				faceScreenRenderers[j].material.SetColor("_EmissionColor", costumeData.MainScreenColor);
			}
		}
	}

	public override void MoveAlongPath(string pathName, BrainEffect.PathTypes pathMovementType, BrainEffect.PathLookTypes pathLookType)
	{
		BotPath pathByName = BotUniqueElementManager.Instance.GetPathByName(pathName);
		if (pathByName != null)
		{
			MoveAlongPath(pathByName, pathMovementType, pathLookType);
			return;
		}
		Debug.LogError("Bot '" + base.name + "' failed to move along path '" + pathName + "' because it couldn't be found.");
	}

	private void MoveAlongPath(BotPath path, BrainEffect.PathTypes pathMovementType, BrainEffect.PathLookTypes pathLookType)
	{
		locomotionController.DoPath(path, pathMovementType, pathLookType);
	}

	public override void MoveToWaypoint(string pathName, int waypointIndex, float moveTime)
	{
		if (base.gameObject.activeSelf)
		{
			BotPathWaypoint waypointOfPath = BotUniqueElementManager.Instance.GetWaypointOfPath(pathName, waypointIndex);
			if (waypointOfPath != null)
			{
				MoveToWithoutY(waypointOfPath.transform.position, moveTime, 0f);
				return;
			}
			Debug.LogError("Bot '" + base.name + "' failed to move to waypoint " + waypointIndex + " of path '" + pathName + "' because it couldn't be found.");
		}
	}

	public override void MoveToPosition(string positionName, float moveTime)
	{
		if (base.gameObject.activeSelf)
		{
			UniqueObject objectByName = BotUniqueElementManager.Instance.GetObjectByName(positionName);
			if (objectByName != null)
			{
				objectByName.WasMovedToByBot(this);
				MoveToWithoutY(objectByName.transform.position, moveTime, 0f);
				return;
			}
			Debug.LogError("Bot '" + base.name + "' failed to move to position '" + objectByName.name + "' because it couldn't be found.");
		}
	}

	private void MoveToWithoutY(Vector3 position, float time, float delay)
	{
		position.y = 0f;
		StartCoroutine(InternalMoveToWithoutY(position, time, delay));
	}

	private IEnumerator InternalMoveToWithoutY(Vector3 position, float time, float delay)
	{
		yield return new WaitForSeconds(delay);
		if (time > 0f)
		{
			Go.to(base.transform, time, new GoTweenConfig().position(position).setEaseType(GoEaseType.QuadInOut));
		}
		else
		{
			base.transform.position = position;
		}
	}

	public override void StopMoving()
	{
		locomotionController.CancelLocomotion();
		Go.killAllTweensWithTarget(base.transform);
	}

	public override void PlayVO(BotVOInfoData info, BotVoiceController.VOImportance importance)
	{
		if (base.gameObject.activeSelf)
		{
			voiceController.PlayAudio(info, importance);
		}
	}

	public override void PlayVO(AudioClip clip, BotVoiceController.VOImportance importance)
	{
		if (base.gameObject.activeSelf)
		{
			voiceController.PlayAudio(clip, importance);
		}
	}

	public override void StopVO()
	{
		if (base.gameObject.activeSelf)
		{
			voiceController.CancelMyCurrentVO();
		}
	}

	private void VOTriggeredEmotion(BotFaceEmote emote, Sprite customGraphic)
	{
		Emote(emote, customGraphic);
	}

	public override void Emote(BotFaceEmote emote, Sprite customSprite)
	{
		if (base.gameObject.activeSelf)
		{
			SetFace(true);
			if (emote == BotFaceEmote.CustomGraphic)
			{
				faceController.SetCustomGraphic(customSprite);
			}
			else
			{
				faceController.SetEmote(emote);
			}
		}
	}

	public void ForceRefreshFace()
	{
		if (faceController != null)
		{
			faceController.ForceRefreshSettings();
		}
	}

	public AudioSource GetTalkingAudioSource()
	{
		return voiceController.GetTalkingAudioSource();
	}

	private void VoiceBeganTalking(VOContainer vo)
	{
		SetFace(true);
		faceController.BeginTalking();
	}

	private void VoiceStoppedTalking(BotVoiceController voice)
	{
		faceController.StopTalking();
	}

	public override void ChangeVOProfile()
	{
		if (JobBoardManager.instance != null && JobBoardManager.instance.EndlessModeStatusController != null)
		{
			currentVOProfile = JobBoardManager.instance.EndlessModeStatusController.GetRandomBotVOProfile();
			if (currentVOProfile != null && currentVOProfile.Costume != null)
			{
				ChangeCostume(currentVOProfile.Costume);
			}
		}
		else
		{
			base.ChangeVOProfile();
		}
	}

	public override void ChangeCostume(BotCostumeData costume)
	{
		ClearExistingCostume();
		if (storedDefaultBodyTexture == null)
		{
			storedDefaultBodyTexture = bodyRenderer.material.mainTexture;
		}
		if (costume == null)
		{
			costume = ((!(currentVOProfile != null) || !(currentVOProfile.Costume != null)) ? BotManager.Instance.GetRandomCostume() : currentVOProfile.Costume);
		}
		costumeData = costume;
		UpdateFaceColours();
		screenGlowRenderer.material.SetColor("_TintColor", new Color(costume.MainScreenColor.r, costume.MainScreenColor.g, costume.MainScreenColor.b, costume.MainScreenColor.a / 10f));
		if (costume.MainTextureOverride != null)
		{
			bodyRenderer.material.mainTexture = costume.MainTextureOverride;
		}
		else
		{
			bodyRenderer.material.mainTexture = storedDefaultBodyTexture;
		}
		base.transform.localScale = Vector3.one * costume.MasterScale;
		if (costume.GlassesPrefab != null)
		{
			AttachableObject attachableObject = UnityEngine.Object.Instantiate(costume.GlassesPrefab);
			attachableObject.AttachTo(glassesAttachPoint);
		}
		if (costume.HatPrefab != null)
		{
			AttachableObject attachableObject2 = UnityEngine.Object.Instantiate(costume.HatPrefab);
			attachableObject2.AttachTo(hatAttachPoint);
		}
		currentCostume = BotManager.Instance.GetCostume(costume);
		currentCostume.Claim(costumeArtParent);
		AttachablePoint[] componentsInChildren = currentCostume.SceneObject.GetComponentsInChildren<AttachablePoint>();
		attachpointsOnBot.Add(glassesAttachPoint);
		glassesAttachPoint.OnObjectWasDetached += ItemDetachedFromCostumeAttachpoint;
		attachpointsOnBot.Add(hatAttachPoint);
		hatAttachPoint.OnObjectWasDetached += ItemDetachedFromCostumeAttachpoint;
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			attachpointsOnBot.Add(componentsInChildren[i]);
			componentsInChildren[i].OnObjectWasDetached += ItemDetachedFromCostumeAttachpoint;
		}
	}

	private void ClearExistingCostume()
	{
		if (currentCostume != null)
		{
			currentCostume.Release();
		}
		for (int i = 0; i < manuallyAddedCostumePieces.Count; i++)
		{
			UnityEngine.Object.Destroy(manuallyAddedCostumePieces[i].gameObject);
		}
		manuallyAddedCostumePieces.Clear();
		if (glassesAttachPoint.NumAttachedObjects > 0)
		{
			AttachableObject attachableObject = glassesAttachPoint.AttachedObjects[0];
			attachableObject.Detach(true, true);
			UnityEngine.Object.Destroy(attachableObject.gameObject);
		}
		if (hatAttachPoint.NumAttachedObjects > 0)
		{
			AttachableObject attachableObject2 = hatAttachPoint.AttachedObjects[0];
			attachableObject2.Detach(true, true);
			UnityEngine.Object.Destroy(attachableObject2.gameObject);
		}
		for (int j = 0; j < attachpointsOnBot.Count; j++)
		{
			attachpointsOnBot[j].OnObjectWasDetached -= ItemDetachedFromCostumeAttachpoint;
		}
		attachpointsOnBot.Clear();
	}

	private void ItemDetachedFromCostumeAttachpoint(AttachablePoint point, AttachableObject obj)
	{
		inventoryController.ManuallyAddRecentlyEjectedItem(obj.PickupableItem.Rigidbody);
		GameEventsManager.Instance.ItemAppliedToItemActionOccurred(obj.PickupableItem.InteractableItem.WorldItemData, worldItem.Data, "REMOVED_FROM");
	}

	public override void AddCostumePiece(GameObject piece)
	{
		if (base.gameObject.activeSelf)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(piece, Vector3.zero, Quaternion.identity) as GameObject;
			BasePrefabSpawner component = gameObject.GetComponent<BasePrefabSpawner>();
			if (component != null)
			{
				gameObject = component.LastSpawnedPrefabGO;
			}
			gameObject.transform.SetParent(costumeArtParent, false);
			manuallyAddedCostumePieces.Add(gameObject.transform);
		}
	}

	public override void ItemsOfInterest(BrainEffect.ItemOfInterestTypes actionType, WorldItemData worldItemData = null, float floatInfo = 1f)
	{
		if (actionType == BrainEffect.ItemOfInterestTypes.AddGlobalItem || actionType == BrainEffect.ItemOfInterestTypes.AddSightItem)
		{
			sightController.AddItemOfInterest(worldItemData);
		}
		if (actionType == BrainEffect.ItemOfInterestTypes.AddGlobalItem || actionType == BrainEffect.ItemOfInterestTypes.AddInventoryItem)
		{
			inventoryController.AddItemOfInterest(worldItemData);
		}
		if (actionType == BrainEffect.ItemOfInterestTypes.RemoveGlobalItem || actionType == BrainEffect.ItemOfInterestTypes.RemoveSightItem)
		{
			sightController.RemoveItemOfInterest(worldItemData);
		}
		if (actionType == BrainEffect.ItemOfInterestTypes.RemoveGlobalItem || actionType == BrainEffect.ItemOfInterestTypes.RemoveInventoryItem)
		{
			inventoryController.RemoveItemOfInterest(worldItemData);
		}
		if (actionType == BrainEffect.ItemOfInterestTypes.ClearAllGlobalItems || actionType == BrainEffect.ItemOfInterestTypes.ClearAllSightItems)
		{
			sightController.ClearItemsOfInterest();
		}
		if (actionType == BrainEffect.ItemOfInterestTypes.ClearAllGlobalItems || actionType == BrainEffect.ItemOfInterestTypes.ClearAllInventoryItems)
		{
			inventoryController.ClearItemsOfInterest();
		}
		if (actionType == BrainEffect.ItemOfInterestTypes.ChangeGlobalRadiusMultiplier || actionType == BrainEffect.ItemOfInterestTypes.ChangeSightRadiusMultiplier)
		{
			sightController.SetRadiusMultiplier(floatInfo);
		}
		if (actionType == BrainEffect.ItemOfInterestTypes.ChangeGlobalRadiusMultiplier || actionType == BrainEffect.ItemOfInterestTypes.ChangeInventoryRadiusMultiplier)
		{
			inventoryController.SetRadiusMultiplier(floatInfo);
		}
		if (actionType == BrainEffect.ItemOfInterestTypes.ChangeFilteringType)
		{
			inventoryController.SetItemFilteringType((BotInventoryController.ItemFilteringTypes)Mathf.RoundToInt(floatInfo));
		}
	}

	public override void GrabItem(string itemName, bool isLargeItem = false)
	{
		if (base.gameObject.activeSelf)
		{
			UniqueObject objectByName = BotUniqueElementManager.Instance.GetObjectByName(itemName);
			if (objectByName != null)
			{
				objectByName.WasPulledIntoInventoryOfBot(this);
				inventoryController.AddItemToInventory(objectByName.transform, isLargeItem);
			}
		}
	}

	public override void EjectPrefab(GameObject prefab, BotInventoryController.EjectTypes ejectType, string optionalLocationName = "", float forceMultiplier = 1f)
	{
		if (base.gameObject.activeSelf)
		{
			Transform ejectToLocation = null;
			if (optionalLocationName != string.Empty)
			{
				UniqueObject objectByName = BotUniqueElementManager.Instance.GetObjectByName(optionalLocationName);
				ejectToLocation = objectByName.transform;
			}
			GameObject gameObject = inventoryController.EjectPrefabAndReturnInstance(prefab, ejectType, ejectToLocation, forceMultiplier);
			WorldItem component = gameObject.GetComponent<WorldItem>();
			if (component != null)
			{
				GameEventsManager.Instance.ItemAppliedToItemActionOccurred(worldItem.Data, component.Data, "CREATED_BY");
			}
		}
	}

	public override void EmptyInventory(BotInventoryController.EmptyTypes emptyType, string optionalLocationName = "", bool onlyMostRecentItem = false, WorldItemData optionalWorldItemDataFilter = null)
	{
		if (base.gameObject.activeSelf)
		{
			Transform removeToLocation = null;
			if (optionalLocationName != string.Empty)
			{
				UniqueObject objectByName = BotUniqueElementManager.Instance.GetObjectByName(optionalLocationName);
				removeToLocation = objectByName.transform;
			}
			if (emptyType == BotInventoryController.EmptyTypes.DestroyHeldOutItems)
			{
				inventoryController.ClearHeldOutItems();
			}
			else if (onlyMostRecentItem)
			{
				inventoryController.RemoveMostRecentItemFromInventory(emptyType, removeToLocation, optionalWorldItemDataFilter);
			}
			else
			{
				inventoryController.RemoveAllItemsFromInventory(emptyType, removeToLocation, optionalWorldItemDataFilter);
			}
		}
	}

	public override void FloatingHeight(BrainEffect.FloatHeightTypes _floatType, float number, string optionalComplexOptions)
	{
		floatMode = _floatType;
		floatWorldHeight = number;
		bool flag = false;
		if (floatMode == BrainEffect.FloatHeightTypes.RandomBob)
		{
			if (optionalComplexOptions.Contains("|"))
			{
				string[] array = optionalComplexOptions.Split('|');
				if (array.Length == 4)
				{
					float.TryParse(array[0], NumberStyles.Float, CultureInfo.InvariantCulture, out floatMinBobHeight);
					float.TryParse(array[1], NumberStyles.Float, CultureInfo.InvariantCulture, out floatMaxBobHeight);
					float.TryParse(array[2], NumberStyles.Float, CultureInfo.InvariantCulture, out floatMinBobTime);
					float.TryParse(array[3], NumberStyles.Float, CultureInfo.InvariantCulture, out floatMaxBobTime);
					flag = true;
				}
			}
		}
		else if (floatMode == BrainEffect.FloatHeightTypes.SineWave)
		{
			if (optionalComplexOptions.Contains("|"))
			{
				string[] array2 = optionalComplexOptions.Split('|');
				if (array2.Length == 3)
				{
					float.TryParse(array2[0], NumberStyles.Float, CultureInfo.InvariantCulture, out floatSineFrequency);
					float.TryParse(array2[1], NumberStyles.Float, CultureInfo.InvariantCulture, out floatSineLowBound);
					float.TryParse(array2[2], NumberStyles.Float, CultureInfo.InvariantCulture, out floatSineHighBound);
					flag = true;
				}
			}
		}
		else if (floatMode == BrainEffect.FloatHeightTypes.MatchPlayer)
		{
			if (optionalComplexOptions.Contains("|"))
			{
				string[] array3 = optionalComplexOptions.Split('|');
				if (array3.Length == 2)
				{
					float.TryParse(array3[0], NumberStyles.Float, CultureInfo.InvariantCulture, out playerMatchMinY);
					float.TryParse(array3[1], NumberStyles.Float, CultureInfo.InvariantCulture, out playerMatchMaxY);
					flag = true;
				}
			}
		}
		else
		{
			flag = true;
		}
		if (flag)
		{
		}
	}

	public override void LookAt(BrainEffect.LookAtTypes lookAtType, string optionalObjectName = "", float optionalWorldAngle = 0f)
	{
		lookAtMode = lookAtType;
		switch (lookAtType)
		{
		case BrainEffect.LookAtTypes.WorldAngle:
			desiredLookYawAngle = optionalWorldAngle;
			break;
		case BrainEffect.LookAtTypes.Object:
		case BrainEffect.LookAtTypes.Bot:
		{
			UniqueObject objectByName = BotUniqueElementManager.Instance.GetObjectByName(optionalObjectName);
			if (objectByName != null)
			{
				objectByName.WasLookedAtByBot(this);
				lookAtTransform = objectByName.transform;
			}
			else
			{
				Debug.LogError("Can't look at object '" + optionalObjectName + "' because it doesn't exist!");
				lookAtTransform = null;
			}
			break;
		}
		case BrainEffect.LookAtTypes.Nothing:
			lookAtTransform = null;
			break;
		}
	}

	public override void ChangeParent(string parentUniqueID, BrainEffect.ChangeParentTypes mode, float optionalTweenTime = 0f)
	{
		Go.killAllTweensWithTarget(base.transform);
		if (parentUniqueID != string.Empty)
		{
			UniqueObject objectByName = BotUniqueElementManager.Instance.GetObjectByName(parentUniqueID);
			base.transform.SetParent(objectByName.transform, true);
			if (currentBrainData != null && currentBrainData.HideHoverFxWhileParented)
			{
				hoverFX.SetActive(false);
			}
		}
		else
		{
			base.transform.SetParent(BotManager.Instance.BotParent, true);
			hoverFX.SetActive(true);
		}
		if (mode == BrainEffect.ChangeParentTypes.SnapLocalPosAndRotToZero || mode == BrainEffect.ChangeParentTypes.SnapLocalPosToZero)
		{
			base.transform.localPosition = Vector3.zero;
		}
		if (mode == BrainEffect.ChangeParentTypes.SnapLocalPosAndRotToZero || mode == BrainEffect.ChangeParentTypes.SnapLocalRotToZero)
		{
			base.transform.localRotation = Quaternion.identity;
		}
		switch (mode)
		{
		case BrainEffect.ChangeParentTypes.TweenLocalPosAndRotToZero:
			Go.to(base.transform, optionalTweenTime, new GoTweenConfig().localPosition(Vector3.zero).localRotation(Quaternion.identity));
			break;
		case BrainEffect.ChangeParentTypes.TweenLocalPosToZero:
			Go.to(base.transform, optionalTweenTime, new GoTweenConfig().localPosition(Vector3.zero));
			break;
		case BrainEffect.ChangeParentTypes.TweenLocalRotToZero:
			Go.to(base.transform, optionalTweenTime, new GoTweenConfig().localRotation(Quaternion.identity));
			break;
		}
	}

	public void ManualSetWorldItemData(WorldItemData wi)
	{
		worldItem.ManualSetData(wi);
	}

	private void Update()
	{
		if (playerHead == null)
		{
			playerHead = GlobalStorage.Instance.MasterHMDAndInputController.TrackedHmdTransform;
		}
		LookAtUpdate();
		FloatHeightUpdate();
	}

	private void LateUpdate()
	{
		if (movementAudioSource != null)
		{
			float num = 0.001f;
			float num2 = Vector3.Distance(lastPosition, base.transform.position);
			isMoving = num2 > num;
			if (isMoving)
			{
				if (!isLoopingMovementSoundPlaying)
				{
					if (movementLoopSound != null)
					{
						isLoopingMovementSoundPlaying = true;
						loopingMovementSoundVolume = 0f;
						movementAudioSource.enabled = true;
						movementAudioSource.SetVolume(0f);
						movementAudioSource.SetClip(movementLoopSound);
						movementAudioSource.SetLooping(true);
						movementAudioSource.Play();
					}
				}
				else if (loopingMovementSoundVolume < 1f)
				{
					if (secondsToFadeLoopIn > 0f)
					{
						loopingMovementSoundVolume = Mathf.Clamp(loopingMovementSoundVolume + Time.deltaTime / secondsToFadeLoopIn, 0f, 1f);
					}
					else
					{
						loopingMovementSoundVolume = 1f;
					}
					movementAudioSource.SetVolume(loopingMovementSoundVolume);
				}
			}
			else if (isLoopingMovementSoundPlaying)
			{
				if (loopingMovementSoundVolume > 0f)
				{
					if (secondsToFadeLoopOut > 0f)
					{
						loopingMovementSoundVolume = Mathf.Clamp(loopingMovementSoundVolume - Time.deltaTime / secondsToFadeLoopOut, 0f, 1f);
					}
					else
					{
						loopingMovementSoundVolume = 0f;
					}
					movementAudioSource.SetVolume(loopingMovementSoundVolume);
				}
				else
				{
					movementAudioSource.Stop();
					movementAudioSource.enabled = false;
					isLoopingMovementSoundPlaying = false;
				}
			}
		}
		lastPosition = base.transform.position;
	}

	private void LookAtUpdate()
	{
		if (lookAtMode == BrainEffect.LookAtTypes.Nothing || (isOptimized && Time.frameCount % 3 != 0) || !bodyVisibilityEvents.IsVisible)
		{
			return;
		}
		float num = 2.5f;
		Quaternion b = Quaternion.identity;
		if (lookAtMode == BrainEffect.LookAtTypes.Player)
		{
			if (playerHead != null)
			{
				if (playerHead.position == base.transform.position)
				{
					return;
				}
				b = Quaternion.LookRotation(playerHead.position - base.transform.position, base.transform.up);
			}
		}
		else if (lookAtMode == BrainEffect.LookAtTypes.WorldAngle)
		{
			b = Quaternion.Euler(0f, desiredLookYawAngle, 0f);
			num = 1f;
		}
		else if (lookAtMode == BrainEffect.LookAtTypes.Object || lookAtMode == BrainEffect.LookAtTypes.Bot)
		{
			if (lookAtTransform == null || lookAtTransform.position == base.transform.position)
			{
				return;
			}
			b = Quaternion.LookRotation(lookAtTransform.position - base.transform.position, base.transform.up);
		}
		if (Mathf.Abs(base.transform.eulerAngles.y - b.eulerAngles.y) >= ((!isOptimized) ? num : (num * 5f)))
		{
			Quaternion quaternion = Quaternion.Lerp(base.transform.rotation, b, 2.5f * Time.deltaTime);
			base.transform.eulerAngles = new Vector3(0f, quaternion.eulerAngles.y, 0f);
		}
	}

	private void FloatHeightUpdate()
	{
		float num = floatWorldHeight;
		bool flag = false;
		if (isOptimized && Time.frameCount % 3 != 2)
		{
			return;
		}
		if (playerHead != null && floatMode == BrainEffect.FloatHeightTypes.MatchPlayer)
		{
			num = playerHead.transform.position.y;
			flag = true;
		}
		if (floatMode == BrainEffect.FloatHeightTypes.RandomBob)
		{
			if (Time.time >= floatNextBobTime)
			{
				floatCurrentBobHeight = UnityEngine.Random.Range(floatMinBobHeight, floatMaxBobHeight);
				floatNextBobTime = Time.time + UnityEngine.Random.Range(floatMinBobTime, floatMaxBobTime);
			}
			num = floatCurrentBobHeight;
		}
		else if (floatMode == BrainEffect.FloatHeightTypes.SineWave && floatSineFrequency != 0f)
		{
			num = Mathf.Lerp(floatSineLowBound, floatSineHighBound, (Mathf.Sin(Time.time * 2f / floatSineFrequency) + 1f) / 2f);
		}
		float num2 = num - eyeLevel.position.y;
		Vector3 localPosition = heightChangeTransform.localPosition;
		float num3 = Mathf.Lerp(localPosition.y, Mathf.Clamp(localPosition.y + num2, (!flag) ? 0.5f : playerMatchMinY, (!flag) ? 2f : playerMatchMaxY), 1f * Time.deltaTime);
		if ((double)Mathf.Abs(num3 - localPosition.y) >= ((!isOptimized) ? 0.009999999776482582 : 0.1) * (double)Time.deltaTime)
		{
			localPosition.y = num3;
			heightChangeTransform.localPosition = localPosition;
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if ((currentBrainData != null && ((isMoving && !currentBrainData.CanBeHitWhileTweening) || (locomotionController.IsInTransit && !currentBrainData.CanBeHitWhileInTransit) || (base.transform.parent != BotManager.Instance.BotParent && !currentBrainData.CanBeHitWhileParented))) || !(collision.collider.attachedRigidbody != null) || collision.collider.attachedRigidbody.transform.IsChildOf(base.transform) || collision.collider.attachedRigidbody.GetComponent<Bot>() != null)
		{
			return;
		}
		PickupableItem component = collision.collider.attachedRigidbody.GetComponent<PickupableItem>();
		string value = "null";
		if (component != null)
		{
			if (component.IsCurrInHand)
			{
				return;
			}
			if (component.InteractableItem != null && component.InteractableItem.WorldItemData != null)
			{
				value = component.InteractableItem.WorldItemData.ItemName;
			}
		}
		if (impactAnimation != null && !impactAnimation.isPlaying)
		{
			if (impactZapAudio != null)
			{
				AudioManager.Instance.Play(base.transform.position, impactZapAudio[UnityEngine.Random.Range(0, impactZapAudio.Length)], 1f, UnityEngine.Random.Range(minZapPitch, maxZapPitch));
			}
			if (impactParticle != null)
			{
				impactParticle.Play();
			}
			impactAnimation.Play();
			GameEventsManager.Instance.ItemActionOccurred(worldItem.Data, "BOT_HIT");
			if (base.CurrentBrainData != null && currentBrainData.name == "MuseumJobBot")
			{
				AchievementManager.CompleteAchievement(10);
			}
			AnalyticsManager.CustomEvent("Bot Hit", new Dictionary<string, object>
			{
				{
					"Bot",
					base.gameObject.name
				},
				{ "Item", value }
			});
		}
	}

	public override void ScriptedEffect(BrainEffect effect)
	{
		string textInfo = effect.TextInfo;
		BotVOProfileData.VOLine vOLine = null;
		List<BotVOEmoteEvent> emoteEvents = new List<BotVOEmoteEvent>();
		BotVOEmoteEvent botVOEmoteEvent = new BotVOEmoteEvent();
		botVOEmoteEvent.InternalSetEmote(BotFaceEmote.Happy);
		BotVOEmoteEvent botVOEmoteEvent2 = new BotVOEmoteEvent();
		botVOEmoteEvent2.InternalSetEmote(BotFaceEmote.Sad);
		switch (textInfo)
		{
		case "PlayEndlessStoreTaskIntroVO":
		case "PlayEndlessTaskIntroVO":
			PlayEndlessTaskIntroVO(effect.FloatInfo);
			break;
		case "PlayEndlessSubtaskCompletedVO":
		{
			if (!(currentVOProfile != null))
			{
				break;
			}
			vOLine = currentVOProfile.GetRandomPageCompleteLine();
			TaskStatusController currentGoal = JobBoardManager.instance.EndlessModeStatusController.GetCurrentGoal();
			for (int i = 1; i < JobBoardManager.instance.EndlessModeStatusController.Data.RequiredPages.Length; i++)
			{
				if (currentGoal != null && currentGoal.PageStatusControllerList != null && currentGoal.CurrPageIndex < currentGoal.PageStatusControllerList.Count && currentGoal.PageStatusControllerList[currentGoal.CurrPageIndex].Data == JobBoardManager.instance.EndlessModeStatusController.Data.RequiredPages[i] && vOLine != null)
				{
					vOLine = null;
				}
			}
			if (vOLine != null && currentGoal.CurrPageIndex < currentGoal.PageStatusControllerList.Count - (1 + JobBoardManager.instance.EndlessModeStatusController.Data.RequiredPages.Length))
			{
				VOContainer vOContainer2 = vOLine.GetVOContainer(BotVoiceController.VOImportance.DontPlayIfSelfSpeaking);
				if (vOContainer2 != null && vOContainer2.Clip != null)
				{
					voiceController.PlayAudio(vOContainer2);
				}
			}
			break;
		}
		case "PlayEndlessTaskSuccessVO":
		{
			if (!(currentVOProfile != null))
			{
				break;
			}
			List<VOContainer> list = new List<VOContainer>();
			vOLine = currentVOProfile.GetRandomSuccessLine();
			if (vOLine != null)
			{
				VOContainer vOContainer = vOLine.GetVOContainer();
				if (vOContainer != null)
				{
					list.Add(vOContainer);
					emoteEvents.Add(botVOEmoteEvent);
					successVOAudioSequence = voiceController.PlayAudioSequence(list.ToArray(), emoteEvents.ToArray());
				}
			}
			break;
		}
		case "PlayEndlessTaskFailVO":
		{
			if (!(currentVOProfile != null))
			{
				break;
			}
			List<VOContainer> list2 = new List<VOContainer>();
			vOLine = currentVOProfile.GetRandomSkipLine();
			if (vOLine != null)
			{
				VOContainer vOContainer3 = vOLine.GetVOContainer();
				if (vOContainer3 != null)
				{
					list2.Add(vOContainer3);
					emoteEvents.Add(botVOEmoteEvent2);
					skipVOAudioSequence = voiceController.PlayAudioSequence(list2.ToArray(), emoteEvents.ToArray());
				}
			}
			break;
		}
		case "PlayEndlessOfficeTaskSuccessVO":
		{
			if (!(currentVOProfile != null))
			{
				break;
			}
			List<VOContainer> successLines = new List<VOContainer>();
			TaskStatusController currentGoal3 = JobBoardManager.instance.EndlessModeStatusController.GetCurrentGoal();
			for (int l = 0; l < currentGoal3.Data.Pages.Count; l++)
			{
				if (successLines.Count > 0)
				{
					successLines.Add(currentVOProfile.GetRandomSentenceConjunctionLine().GetVOContainer());
				}
				AddSuccessVOAndEmotesForPage(currentGoal3.Data.Pages[l], ref successLines, ref emoteEvents);
			}
			if (successLines.Count > 0)
			{
				successVOAudioSequence = voiceController.PlayAudioSequence(successLines.ToArray(), emoteEvents.ToArray());
				break;
			}
			vOLine = currentVOProfile.GetRandomSuccessLine();
			if (vOLine != null)
			{
				VOContainer vOContainer5 = vOLine.GetVOContainer();
				if (vOContainer5 != null)
				{
					successLines.Add(vOContainer5);
					emoteEvents.Add(botVOEmoteEvent);
					successVOAudioSequence = voiceController.PlayAudioSequence(successLines.ToArray(), emoteEvents.ToArray());
				}
			}
			break;
		}
		case "PlayEndlessOfficeTaskSkipVO":
		{
			if (!(currentVOProfile != null))
			{
				break;
			}
			List<VOContainer> failureLines = new List<VOContainer>();
			TaskStatusController currentGoal2 = JobBoardManager.instance.EndlessModeStatusController.GetCurrentGoal();
			if (!currentGoal2.IsSkipped)
			{
				for (int j = 0; j < currentGoal2.Data.Pages.Count - JobBoardManager.instance.EndlessModeStatusController.Data.RequiredPages.Length; j++)
				{
					if (failureLines.Count > 0)
					{
						failureLines.Add(currentVOProfile.GetRandomSentenceConjunctionLine().GetVOContainer());
					}
					AddFailureVOAndEmotesForPage(currentGoal2.Data.Pages[j], ref failureLines, ref emoteEvents);
				}
			}
			if (failureLines.Count == 0)
			{
				for (int k = 0; k < currentGoal2.Data.Pages.Count - JobBoardManager.instance.EndlessModeStatusController.Data.RequiredPages.Length; k++)
				{
					if (failureLines.Count > 0)
					{
						failureLines.Add(currentVOProfile.GetRandomSentenceConjunctionLine().GetVOContainer());
					}
					AddSkipVOAndEmotesForPage(currentGoal2.Data.Pages[k], ref failureLines, ref emoteEvents);
				}
			}
			if (failureLines.Count > 0)
			{
				skipVOAudioSequence = voiceController.PlayAudioSequence(failureLines.ToArray(), emoteEvents.ToArray());
				break;
			}
			vOLine = currentVOProfile.GetRandomSkipLine();
			if (vOLine != null)
			{
				VOContainer vOContainer4 = vOLine.GetVOContainer();
				if (vOContainer4 != null)
				{
					failureLines.Add(vOContainer4);
					emoteEvents.Add(botVOEmoteEvent2);
					skipVOAudioSequence = voiceController.PlayAudioSequence(failureLines.ToArray(), emoteEvents.ToArray());
				}
			}
			break;
		}
		case "PlayEndlessTaskRequestVOForCurrentNonRequiredPage":
		{
			if (!(currentVOProfile != null))
			{
				break;
			}
			TaskStatusController currentGoal4 = JobBoardManager.instance.EndlessModeStatusController.GetCurrentGoal();
			if (currentGoal4 == null || currentGoal4.CurrPageIndex < currentGoal4.Data.Pages.Count - JobBoardManager.instance.EndlessModeStatusController.Data.RequiredPages.Length)
			{
				List<VOContainer> requestLines2 = new List<VOContainer>();
				AddRequestVOAndEmotesForCurrentPage(ref requestLines2, ref emoteEvents);
				if (requestLines2.Count > 0)
				{
					introVOAudioSequence = voiceController.PlayAudioSequence(requestLines2.ToArray(), emoteEvents.ToArray());
				}
			}
			break;
		}
		case "PlayEndlessTaskRequestVOForCurrentPage":
			if (currentVOProfile != null)
			{
				List<VOContainer> requestLines = new List<VOContainer>();
				AddRequestVOAndEmotesForCurrentPage(ref requestLines, ref emoteEvents);
				if (requestLines.Count > 0)
				{
					introVOAudioSequence = voiceController.PlayAudioSequence(requestLines.ToArray(), emoteEvents.ToArray());
				}
			}
			break;
		case "RandomPercentChance":
			if (effect.FloatInfo > UnityEngine.Random.Range(0f, 1f))
			{
				GameEventsManager.Instance.ScriptedCauseOccurred("RandomPercentSuccess");
			}
			else
			{
				GameEventsManager.Instance.ScriptedCauseOccurred("RandomPercentFailure");
			}
			break;
		case "ChangeFaceColor":
			if (costumeData != null)
			{
				BotCostumeData botCostumeData = ScriptableObject.CreateInstance<BotCostumeData>();
				botCostumeData.DuplicateData(costumeData);
				botCostumeData.SetFaceColor(effect.CostumeDataInfo.MainScreenColor);
				costumeData = botCostumeData;
			}
			else
			{
				Debug.LogError("costumeData not set on ScriptedEffect ChangeFaceColor on " + base.gameObject.name);
			}
			break;
		case "DeregisterWorldItemData":
			if (effect.WorldItemDataInfo != null && BotUniqueElementManager._instanceNoCreate != null)
			{
				UniqueObject objectByName = BotUniqueElementManager._instanceNoCreate.GetObjectByName(effect.WorldItemDataInfo.ItemName);
				if (objectByName != null)
				{
					UnityEngine.Object.Destroy(objectByName);
				}
			}
			break;
		case "EjectPrefabBasedOnCurrentSubtaskWorldItemData":
		{
			GameObject prefab = GetWorldItemWithIconDataBasedOnCurrentPage().Prefab;
			if (prefab != null)
			{
				if (effect.WorldItemDataInfo != null)
				{
					EjectPrefab(prefab, BotInventoryController.EjectTypes.TweenToLocation, effect.WorldItemDataInfo.name);
				}
				else
				{
					EjectPrefab(prefab, BotInventoryController.EjectTypes.HoldOutToPlayer, string.Empty);
				}
			}
			break;
		}
		case "AddItemToInventoryBasedOnCurrentPage":
		{
			WorldItemWithIconData worldItemWithIconDataBasedOnCurrentPage = GetWorldItemWithIconDataBasedOnCurrentPage();
			if (worldItemWithIconDataBasedOnCurrentPage != null)
			{
				ItemsOfInterest(BrainEffect.ItemOfInterestTypes.AddInventoryItem, worldItemWithIconDataBasedOnCurrentPage.WorldItemData);
			}
			break;
		}
		case "RunFizzBuzzOnScore":
			if (JobBoardManager.instance != null && JobBoardManager.instance.EndlessModeStatusController != null && !JobBoardManager.instance.EndlessModeStatusController.ShouldGetPromotion())
			{
				GameEventsManager.Instance.ScriptedCauseOccurred("Fizz");
			}
			break;
		default:
			Debug.LogError("Scripted Effect '" + effect.TextInfo + "' not implemented on '" + base.gameObject.name + "': " + effect, base.gameObject);
			break;
		}
	}

	private bool AddRequestVOAndEmotesForCurrentPage(ref List<VOContainer> requestLines, ref List<BotVOEmoteEvent> emoteEvents)
	{
		bool result = false;
		TaskStatusController currentGoal = JobBoardManager.instance.EndlessModeStatusController.GetCurrentGoal();
		if (currentGoal != null)
		{
			PageStatusController currentPage = currentGoal.GetCurrentPage();
			if (currentPage != null)
			{
				result = AddRequestVOAndEmotesForPage(currentPage.Data, ref requestLines, ref emoteEvents);
			}
		}
		return result;
	}

	private bool AddIntroVOAndEmotesForPage(PageData page, ref List<VOContainer> introLines, ref List<BotVOEmoteEvent> emoteEvents, bool useDynamicDataIfAvailable = false)
	{
		if (currentVOProfile != null && page != null)
		{
			BotVOProfileData.VOPair randomIntroVOPairForPage = currentVOProfile.GetRandomIntroVOPairForPage(page.name);
			return AddVOAndEmotesForPage(randomIntroVOPairForPage, ref introLines, ref emoteEvents, page, useDynamicDataIfAvailable);
		}
		return false;
	}

	private bool AddRequestVOAndEmotesForPage(PageData page, ref List<VOContainer> requestLines, ref List<BotVOEmoteEvent> emoteEvents)
	{
		if (currentVOProfile != null && page != null)
		{
			BotVOProfileData.VOPair randomRequestVOPairForPage = currentVOProfile.GetRandomRequestVOPairForPage(page.name);
			return AddVOAndEmotesForPage(randomRequestVOPairForPage, ref requestLines, ref emoteEvents, page);
		}
		return false;
	}

	private bool AddSuccessVOAndEmotesForPage(PageData page, ref List<VOContainer> successLines, ref List<BotVOEmoteEvent> emoteEvents, bool useDynamicDataIfAvailable = false)
	{
		if (currentVOProfile != null && page != null)
		{
			BotVOProfileData.VOPair randomSuccessVOPairForPage = currentVOProfile.GetRandomSuccessVOPairForPage(page.name);
			return AddVOAndEmotesForPage(randomSuccessVOPairForPage, ref successLines, ref emoteEvents, page, useDynamicDataIfAvailable);
		}
		return false;
	}

	private bool AddSkipVOAndEmotesForPage(PageData page, ref List<VOContainer> skipLines, ref List<BotVOEmoteEvent> emoteEvents, bool useDynamicDataIfAvailable = false)
	{
		if (currentVOProfile != null && page != null)
		{
			BotVOProfileData.VOPair randomSkipVOPairForPage = currentVOProfile.GetRandomSkipVOPairForPage(page.name);
			return AddVOAndEmotesForPage(randomSkipVOPairForPage, ref skipLines, ref emoteEvents, page, useDynamicDataIfAvailable);
		}
		return false;
	}

	private bool AddFailureVOAndEmotesForPage(PageData page, ref List<VOContainer> failureLines, ref List<BotVOEmoteEvent> emoteEvents, bool useDynamicDataIfAvailable = false)
	{
		if (currentVOProfile != null && page != null)
		{
			BotVOProfileData.VOPair randomFailureVOPairForPage = currentVOProfile.GetRandomFailureVOPairForPage(page.name);
			return AddVOAndEmotesForPage(randomFailureVOPairForPage, ref failureLines, ref emoteEvents, page, useDynamicDataIfAvailable);
		}
		return false;
	}

	private bool AddVOAndEmotesForPage(BotVOProfileData.VOPair pair, ref List<VOContainer> desiredLines, ref List<BotVOEmoteEvent> emoteEvents, PageData page = null, bool useDynamicDataIfAvailable = true)
	{
		bool result = false;
		if (currentVOProfile != null)
		{
			BotVOEmoteEvent botVOEmoteEvent = new BotVOEmoteEvent();
			botVOEmoteEvent.InternalSetEmote(BotFaceEmote.Idle);
			BotVOEmoteEvent botVOEmoteEvent2 = new BotVOEmoteEvent();
			botVOEmoteEvent2.InternalSetEmote(BotFaceEmote.Techno);
			if (pair != null)
			{
				result = true;
				VOContainer vOContainer = pair.prefixLine.GetVOContainer();
				if (vOContainer != null)
				{
					desiredLines.Add(vOContainer);
					emoteEvents.Add(botVOEmoteEvent);
				}
				if (useDynamicDataIfAvailable && page != null)
				{
					AddDynamicDataToLinesForPage(page, ref desiredLines, ref emoteEvents);
				}
				vOContainer = pair.suffixLine.GetVOContainer();
				if (vOContainer != null)
				{
					desiredLines.Add(vOContainer);
					emoteEvents.Add(botVOEmoteEvent);
				}
			}
		}
		return result;
	}

	private bool AddDynamicDataToLinesForPage(PageData page, ref List<VOContainer> requestLines, ref List<BotVOEmoteEvent> emoteEvents)
	{
		bool result = false;
		BotVOEmoteEvent botVOEmoteEvent = new BotVOEmoteEvent();
		botVOEmoteEvent.InternalSetEmote(BotFaceEmote.Idle);
		BotVOEmoteEvent botVOEmoteEvent2 = new BotVOEmoteEvent();
		botVOEmoteEvent2.InternalSetEmote(BotFaceEmote.Techno);
		PageNameItemMatchedPair pageNameItemMatchedPairForPage = GetPageNameItemMatchedPairForPage(page);
		if (pageNameItemMatchedPairForPage != null)
		{
			result = true;
			BotVOProfileData.VOClause randomRequestVOClauseForPage = currentVOProfile.GetRandomRequestVOClauseForPage(page.name);
			if (randomRequestVOClauseForPage != null && randomRequestVOClauseForPage.relationToSubject == BotVOProfileData.VOClause.RelationToSubject.before)
			{
				AddDynamicClausesToLines(randomRequestVOClauseForPage, pageNameItemMatchedPairForPage, ref requestLines, ref emoteEvents);
			}
			WorldItemWithIconData worldItemWithIconData = pageNameItemMatchedPairForPage.WorldItemWithIconData;
			if (worldItemWithIconData != null)
			{
				requestLines.Add(new VOContainer(worldItemWithIconData.GetItemNameAudioClip(currentVOProfile.VoiceType), BotVoiceController.VOImportance.OverrideOnlySelf));
				emoteEvents.Add(botVOEmoteEvent2);
			}
			if (randomRequestVOClauseForPage != null && randomRequestVOClauseForPage.relationToSubject == BotVOProfileData.VOClause.RelationToSubject.after)
			{
				AddDynamicClausesToLines(randomRequestVOClauseForPage, pageNameItemMatchedPairForPage, ref requestLines, ref emoteEvents);
			}
		}
		return result;
	}

	private void AddDynamicClausesToLines(BotVOProfileData.VOClause clause, PageNameItemMatchedPair matchedPairForPage, ref List<VOContainer> lines, ref List<BotVOEmoteEvent> emoteEvents)
	{
		BotVOEmoteEvent botVOEmoteEvent = new BotVOEmoteEvent();
		botVOEmoteEvent.InternalSetEmote(BotFaceEmote.Idle);
		BotVOEmoteEvent botVOEmoteEvent2 = new BotVOEmoteEvent();
		botVOEmoteEvent2.InternalSetEmote(BotFaceEmote.Techno);
		if (matchedPairForPage.Modifiers == null || matchedPairForPage.Modifiers.Length <= 0)
		{
			return;
		}
		lines.Add(clause.pair.prefixLine.GetVOContainer());
		emoteEvents.Add(botVOEmoteEvent);
		for (int i = 0; i < matchedPairForPage.Modifiers.Length; i++)
		{
			if (i > 0 && i == matchedPairForPage.Modifiers.Length - 1)
			{
				lines.Add(currentVOProfile.GetRandomClauseConjunctionLine().GetVOContainer());
				emoteEvents.Add(botVOEmoteEvent);
			}
			lines.Add(new VOContainer(matchedPairForPage.Modifiers[i], BotVoiceController.VOImportance.OverrideOnlySelf));
			emoteEvents.Add(botVOEmoteEvent2);
		}
		lines.Add(clause.pair.suffixLine.GetVOContainer());
		emoteEvents.Add(botVOEmoteEvent);
	}

	private WorldItemWithIconData GetWorldItemWithIconDataBasedOnCurrentPage()
	{
		TaskStatusController currentGoal = JobBoardManager.instance.EndlessModeStatusController.GetCurrentGoal();
		if (currentGoal == null)
		{
			return null;
		}
		return GetWorldItemWithIconDataForPage(currentGoal.GetCurrentPage().Data);
	}

	private WorldItemWithIconData GetWorldItemWithIconDataForPage(PageData page)
	{
		PageNameItemMatchedPair pageNameItemMatchedPairForPage = GetPageNameItemMatchedPairForPage(page);
		if (pageNameItemMatchedPairForPage != null)
		{
			return pageNameItemMatchedPairForPage.WorldItemWithIconData;
		}
		return null;
	}

	private PageNameItemMatchedPair GetPageNameItemMatchedPairForPage(PageData page)
	{
		List<PageNameItemMatchedPair> currentTaskPageItemNamePairs = JobBoardManager.instance.EndlessModeStatusController.CurrentTaskPageItemNamePairs;
		if (currentTaskPageItemNamePairs != null && currentTaskPageItemNamePairs.Count > 0)
		{
			for (int i = 0; i < currentTaskPageItemNamePairs.Count; i++)
			{
				if (currentTaskPageItemNamePairs[i].PageName == page.name)
				{
					return currentTaskPageItemNamePairs[i];
				}
			}
		}
		return null;
	}

	private void PlayEndlessTaskIntroVO(float clipBufferTime = 0f)
	{
		if (!(JobBoardManager.instance != null) || JobBoardManager.instance.EndlessModeStatusController == null || !(currentVOProfile != null))
		{
			return;
		}
		TaskStatusController currentGoal = JobBoardManager.instance.EndlessModeStatusController.GetCurrentGoal();
		List<VOContainer> introLines = new List<VOContainer>();
		List<BotVOEmoteEvent> emoteEvents = new List<BotVOEmoteEvent>();
		BotVOEmoteEvent botVOEmoteEvent = new BotVOEmoteEvent();
		botVOEmoteEvent.InternalSetEmote(BotFaceEmote.Idle);
		BotVOEmoteEvent botVOEmoteEvent2 = new BotVOEmoteEvent();
		botVOEmoteEvent2.InternalSetEmote(BotFaceEmote.Techno);
		List<float> list = new List<float>();
		for (int i = 0; i < currentGoal.Data.Pages.Count - JobBoardManager.instance.EndlessModeStatusController.Data.RequiredPages.Length; i++)
		{
			if (introLines.Count > 0)
			{
				introLines.Add(currentVOProfile.GetRandomSentenceConjunctionLine().GetVOContainer());
				emoteEvents.Add(botVOEmoteEvent);
			}
			AddIntroVOAndEmotesForPage(currentGoal.Data.Pages[i], ref introLines, ref emoteEvents);
		}
		if (introLines.Count == 0)
		{
			BotVOProfileData.VOLine randomIntro = currentVOProfile.GetRandomIntro();
			if (randomIntro != null)
			{
				VOContainer vOContainer = randomIntro.GetVOContainer();
				if (vOContainer != null)
				{
					introLines.Add(vOContainer);
					emoteEvents.Add(botVOEmoteEvent);
				}
			}
		}
		if (introLines.Count > 0)
		{
			while (list.Count < introLines.Count - 1)
			{
				list.Add(clipBufferTime);
			}
			list.Add(0.2f);
		}
		EndlessModeData.EndlessVORequestType endlessVORequestType = JobBoardManager.instance.EndlessModeStatusController.Data.VORequestType;
		if (endlessVORequestType == EndlessModeData.EndlessVORequestType.Random)
		{
			endlessVORequestType = ((UnityEngine.Random.Range(0f, 1f) < 0.5f) ? EndlessModeData.EndlessVORequestType.UpFront : EndlessModeData.EndlessVORequestType.PerPage);
		}
		switch (endlessVORequestType)
		{
		case EndlessModeData.EndlessVORequestType.UpFront:
		{
			GameEventsManager.Instance.ScriptedCauseOccurred("UseUpFrontRequests");
			int num = 0;
			int index = -1;
			for (int k = 0; k < currentGoal.Data.Pages.Count - JobBoardManager.instance.EndlessModeStatusController.Data.RequiredPages.Length; k++)
			{
				if (num > 0)
				{
					BotVOProfileData.VOLine randomSentenceConjunctionLine = currentVOProfile.GetRandomSentenceConjunctionLine();
					if (randomSentenceConjunctionLine != null)
					{
						index = introLines.Count;
						introLines.Add(randomSentenceConjunctionLine.GetVOContainer());
						emoteEvents.Add(botVOEmoteEvent);
					}
				}
				if (AddRequestVOAndEmotesForPage(currentGoal.Data.Pages[k], ref introLines, ref emoteEvents))
				{
					num++;
				}
			}
			if (num > 2)
			{
				BotVOProfileData.VOLine randomFinalSentenceConjunctionLine = currentVOProfile.GetRandomFinalSentenceConjunctionLine();
				if (randomFinalSentenceConjunctionLine != null)
				{
					VOContainer vOContainer2 = randomFinalSentenceConjunctionLine.GetVOContainer();
					if (vOContainer2 != null && vOContainer2.Clip != null)
					{
						introLines[index] = vOContainer2;
					}
				}
			}
			BotVOProfileData.VOLine randomOutro = currentVOProfile.GetRandomOutro();
			if (randomOutro != null)
			{
				introLines.Add(randomOutro.GetVOContainer());
				emoteEvents.Add(botVOEmoteEvent);
			}
			break;
		}
		case EndlessModeData.EndlessVORequestType.PerPage:
		{
			GameEventsManager.Instance.ScriptedCauseOccurred("UsePerPageRequests");
			for (int j = 0; j < currentGoal.Data.Pages.Count && !AddRequestVOAndEmotesForPage(currentGoal.Data.Pages[j], ref introLines, ref emoteEvents); j++)
			{
			}
			break;
		}
		}
		while (list.Count < introLines.Count)
		{
			list.Add(clipBufferTime);
		}
		introVOAudioSequence = voiceController.PlayAudioSequence(introLines.ToArray(), emoteEvents.ToArray(), list.ToArray());
	}

	private void OnAudioSequencePlayComplete(BotVoiceController vc, Coroutine completedSequence)
	{
		if (!(vc == voiceController) || !(worldItem != null) || !(worldItem.Data != null) || !(GameEventsManager._instanceNoCreate != null))
		{
			return;
		}
		GameEventsManager.Instance.ItemActionOccurred(worldItem.Data, "BOT_COMPLETED_AUDIO_SEQUENCE");
		if (completedSequence != null)
		{
			if (skipVOAudioSequence == completedSequence)
			{
				GameEventsManager.Instance.ScriptedCauseOccurred("SkipVOAudioSequenceCompleted");
			}
			else if (introVOAudioSequence == completedSequence)
			{
				GameEventsManager.Instance.ScriptedCauseOccurred("IntroVOAudioSequenceCompleted");
			}
			else if (successVOAudioSequence == completedSequence)
			{
				GameEventsManager.Instance.ScriptedCauseOccurred("SuccessVOAudioSequenceCompleted");
			}
		}
	}
}
