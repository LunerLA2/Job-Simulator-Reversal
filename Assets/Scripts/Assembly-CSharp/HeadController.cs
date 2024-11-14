using System;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class HeadController : MonoBehaviour
{
	public const float SECS_BETWEEN_BITES_WHEN_HELD_IN_MOUTH = 0.45f;

	public const float SECS_LENGTH_SINGLE_BLOW = 1f;

	public const float SECS_BLOW_COOLDOWN = 1f;

	private const int FLUID_TO_PUKE = 20;

	[SerializeField]
	private WorldItem headWorldItem;

	[SerializeField]
	private ParticleCollectionZone mouthCollectionZone;

	[SerializeField]
	private RigidbodyEnterExitTriggerEvents mouthEatZone;

	[SerializeField]
	private RigidbodyEnterExitTriggerEvents inMouthTriggerEvents;

	[SerializeField]
	private RigidbodyEnterExitTriggerEvents nearMouthTriggerEvents;

	[SerializeField]
	private PourItemOnFaceData pourItemOnFaceData;

	[SerializeField]
	private AudioSourceHelper audioSourceHelper;

	[SerializeField]
	private AudioClip glugAudioClip;

	[SerializeField]
	private AudioClip[] blowAudioClip;

	[SerializeField]
	private AudioClip[] breathInAudioClip;

	[SerializeField]
	private AudioSourceHelper pouredItemOnFaceAudioSourceHelper;

	[SerializeField]
	private AttachablePoint[] attachablePoints;

	[SerializeField]
	private ParticleSystem barfEffectSpray;

	[SerializeField]
	private ParticleSystem barfEffectChunk;

	[SerializeField]
	private MunchSFX munchDB;

	private float glugTime;

	private bool isBlowing;

	private List<BlowableItem> blowableItemsInMouth = new List<BlowableItem>();

	private int cachedLengthOfBlowableItemsInMouth;

	private List<BlowableItem> blowableItemsNearMouth = new List<BlowableItem>();

	private int cachedLengthOfBlowableItemsNearMouth;

	private bool blowDetectionIsRunning;

	private List<EdibleItem> edibleItemsInMouth = new List<EdibleItem>();

	private float nextAutomaticBiteTimer;

	private float blowTimeLeft;

	private float blowCooldownLeft;

	public Action<WorldItemData> OnWorldItemTouchedMouth;

	public Action<ParticleCollectionZone, WorldItemData> OnItemPouredInMouth;

	private Vector3 prevHeadPosition = Vector3.zero;

	private Vector3 headVelocity = Vector3.zero;

	private int currentFluidInMouth;

	private Dictionary<WorldItemData, MunchSFXData> munchSoundQuickLookup;

	private void Start()
	{
		prevHeadPosition = base.transform.position;
		audioSourceHelper.SetClip(glugAudioClip);
		audioSourceHelper.SetLooping(true);
		BuildMunchSoundQuickLookup();
	}

	private void OnEnable()
	{
		ParticleCollectionZone particleCollectionZone = mouthCollectionZone;
		particleCollectionZone.OnParticleQuantityUnitAdded = (Action<ParticleCollectionZone, WorldItemData>)Delegate.Combine(particleCollectionZone.OnParticleQuantityUnitAdded, new Action<ParticleCollectionZone, WorldItemData>(ItemPouredIntoMouth));
		ParticleCollectionZone particleCollectionZone2 = mouthCollectionZone;
		particleCollectionZone2.OnParticleIsCollecting = (Action<ParticleCollectionZone, WorldItemData, float>)Delegate.Combine(particleCollectionZone2.OnParticleIsCollecting, new Action<ParticleCollectionZone, WorldItemData, float>(ItemIsBeingPoured));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = mouthEatZone;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(RigidbodyEnteredEatTrigger));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents2 = mouthEatZone;
		rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger, new Action<Rigidbody>(RigidbodyExitedEatTrigger));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents3 = inMouthTriggerEvents;
		rigidbodyEnterExitTriggerEvents3.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents3.OnRigidbodyEnterTrigger, new Action<Rigidbody>(RigidbodyEnteredInMouthTrigger));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents4 = inMouthTriggerEvents;
		rigidbodyEnterExitTriggerEvents4.OnRigidbodyExitTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents4.OnRigidbodyExitTrigger, new Action<Rigidbody>(RigidbodyExitedInMouthTrigger));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents5 = nearMouthTriggerEvents;
		rigidbodyEnterExitTriggerEvents5.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents5.OnRigidbodyEnterTrigger, new Action<Rigidbody>(RigidbodyEnteredNearMouthTrigger));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents6 = nearMouthTriggerEvents;
		rigidbodyEnterExitTriggerEvents6.OnRigidbodyExitTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents6.OnRigidbodyExitTrigger, new Action<Rigidbody>(RigidbodyExitedNearMouthTrigger));
	}

	private void OnDisable()
	{
		ParticleCollectionZone particleCollectionZone = mouthCollectionZone;
		particleCollectionZone.OnParticleQuantityUnitAdded = (Action<ParticleCollectionZone, WorldItemData>)Delegate.Remove(particleCollectionZone.OnParticleQuantityUnitAdded, new Action<ParticleCollectionZone, WorldItemData>(ItemPouredIntoMouth));
		ParticleCollectionZone particleCollectionZone2 = mouthCollectionZone;
		particleCollectionZone2.OnParticleIsCollecting = (Action<ParticleCollectionZone, WorldItemData, float>)Delegate.Remove(particleCollectionZone2.OnParticleIsCollecting, new Action<ParticleCollectionZone, WorldItemData, float>(ItemIsBeingPoured));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = mouthEatZone;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(RigidbodyEnteredEatTrigger));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents2 = mouthEatZone;
		rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger, new Action<Rigidbody>(RigidbodyExitedEatTrigger));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents3 = inMouthTriggerEvents;
		rigidbodyEnterExitTriggerEvents3.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents3.OnRigidbodyEnterTrigger, new Action<Rigidbody>(RigidbodyEnteredInMouthTrigger));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents4 = inMouthTriggerEvents;
		rigidbodyEnterExitTriggerEvents4.OnRigidbodyExitTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents4.OnRigidbodyExitTrigger, new Action<Rigidbody>(RigidbodyExitedInMouthTrigger));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents5 = nearMouthTriggerEvents;
		rigidbodyEnterExitTriggerEvents5.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents5.OnRigidbodyEnterTrigger, new Action<Rigidbody>(RigidbodyEnteredNearMouthTrigger));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents6 = nearMouthTriggerEvents;
		rigidbodyEnterExitTriggerEvents6.OnRigidbodyExitTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents6.OnRigidbodyExitTrigger, new Action<Rigidbody>(RigidbodyExitedNearMouthTrigger));
	}

	private void Update()
	{
		headVelocity = base.transform.position - prevHeadPosition;
		if (blowDetectionIsRunning)
		{
			BlowItems();
		}
		if (glugTime > 0f)
		{
			glugTime -= Time.deltaTime;
			if (glugTime < 0f)
			{
				glugTime = 0f;
				if (audioSourceHelper.IsPlaying)
				{
					audioSourceHelper.Stop();
				}
			}
		}
		if (nextAutomaticBiteTimer > 0f)
		{
			nextAutomaticBiteTimer -= Time.deltaTime;
		}
		if (edibleItemsInMouth.Count > 0)
		{
			bool flag = false;
			if (nextAutomaticBiteTimer <= 0f)
			{
				float num = 0.45f;
				for (int i = 0; i < edibleItemsInMouth.Count; i++)
				{
					if (edibleItemsInMouth[i] != null)
					{
						if (IsEdibleItemReadyToBeEaten(edibleItemsInMouth[i]))
						{
							float num2 = edibleItemsInMouth[i].BiteTimerMultiplier * 0.45f;
							TakeBiteOfEdibleItem(edibleItemsInMouth[i]);
							flag = true;
							if (num2 > num)
							{
								num = num2;
							}
						}
					}
					else
					{
						edibleItemsInMouth.RemoveAt(i);
						i--;
					}
				}
				if (flag)
				{
					nextAutomaticBiteTimer = Mathf.Max(nextAutomaticBiteTimer + num, num);
				}
			}
		}
		prevHeadPosition = base.transform.position;
	}

	private void BuildMunchSoundQuickLookup()
	{
		munchSoundQuickLookup = new Dictionary<WorldItemData, MunchSFXData>();
		for (int i = 0; i < munchDB.MunchSfxData.Count; i++)
		{
			MunchSFXData munchSFXData = munchDB.MunchSfxData[i];
			for (int j = 0; j < munchSFXData.MunchWorldItems.Count; j++)
			{
				WorldItemData worldItemData = munchSFXData.MunchWorldItems[j];
				if (!munchSoundQuickLookup.ContainsKey(worldItemData))
				{
					munchSoundQuickLookup[worldItemData] = munchSFXData;
				}
				else
				{
					Debug.LogError("more than 1 MunchSFXData contains information about " + worldItemData.ItemName + ", not sure which one to use");
				}
			}
		}
	}

	private bool IsBlowableItemReadyToBeBlown(BlowableItem blowableItem)
	{
		if (blowableItem != null && !blowableItem.isActiveAndEnabled)
		{
			return false;
		}
		if (blowableItem.PickupableItem != null)
		{
			if (blowableItem.PickupableItem.IsCurrInHand)
			{
				return CheckItemVelocity(blowableItem.PickupableItem);
			}
			return true;
		}
		return true;
	}

	private bool IsEdibleItemReadyToBeEaten(EdibleItem edibleItem)
	{
		if (!edibleItem.isActiveAndEnabled)
		{
			return false;
		}
		if (edibleItem.PickupableItem != null)
		{
			if (edibleItem.PickupableItem.IsCurrInHand)
			{
				return CheckItemVelocity(edibleItem.PickupableItem);
			}
			return true;
		}
		return true;
	}

	private bool CheckItemVelocity(PickupableItem pickupableItem)
	{
		Vector3 vector = headVelocity - pickupableItem.CurrInteractableHand.GrabbedItemCurrVelocity;
		if (vector.z < 0.05f && Mathf.Abs(vector.x) < 0.3f && Mathf.Abs(vector.y) < 0.3f)
		{
			return true;
		}
		return false;
	}

	private void ItemIsBeingPoured(ParticleCollectionZone zone, WorldItemData itemData, float amount)
	{
		GlugSound(itemData);
	}

	private void ItemPouredIntoMouth(ParticleCollectionZone zone, WorldItemData itemData)
	{
		GlugSound(itemData);
		if (OnItemPouredInMouth != null)
		{
			OnItemPouredInMouth(zone, itemData);
		}
		List<PourItemOnFaceEffect> pourItemOnFaceEffects = pourItemOnFaceData.PourItemOnFaceEffects;
		bool flag = false;
		foreach (PourItemOnFaceEffect item in pourItemOnFaceEffects)
		{
			if (item.IsTriggeredBy(itemData))
			{
				flag = true;
				currentFluidInMouth++;
				if (currentFluidInMouth >= 20)
				{
					StartCoroutine(item.DoEffect(pouredItemOnFaceAudioSourceHelper, barfEffectChunk, barfEffectSpray));
				}
			}
		}
		if (!flag)
		{
			currentFluidInMouth = 0;
		}
	}

	private void GlugSound(WorldItemData worldItem)
	{
		audioSourceHelper.SetClip(glugAudioClip);
		audioSourceHelper.SetLooping(true);
		glugTime = 0.2f;
		if (!audioSourceHelper.IsPlaying)
		{
			audioSourceHelper.Play();
			string eventDataValue = ((!(worldItem != null)) ? "null" : worldItem.name);
			AnalyticsManager.CustomEvent("Drink Item", "Item", eventDataValue);
		}
	}

	private void PlayBlowAudio(bool isBlow)
	{
		if (isBlow)
		{
			audioSourceHelper.SetClip(blowAudioClip[UnityEngine.Random.Range(0, blowAudioClip.Length - 1)]);
		}
		else
		{
			audioSourceHelper.SetClip(breathInAudioClip[UnityEngine.Random.Range(0, breathInAudioClip.Length - 1)]);
		}
		audioSourceHelper.SetLooping(false);
		if (audioSourceHelper.IsPlaying)
		{
			audioSourceHelper.Stop();
		}
		audioSourceHelper.Play();
	}

	private void StopBlowAudio()
	{
		if (audioSourceHelper != null && audioSourceHelper.IsPlaying && (Array.IndexOf(blowAudioClip, audioSourceHelper.GetClip()) > -1 || Array.IndexOf(breathInAudioClip, audioSourceHelper.GetClip()) > -1))
		{
			audioSourceHelper.Stop();
		}
	}

	public void ForceStopAudio()
	{
		if (audioSourceHelper != null)
		{
			isBlowing = false;
			audioSourceHelper.Stop();
			EndBlowDetection();
		}
	}

	private void BlowItems()
	{
		if (!blowDetectionIsRunning)
		{
			return;
		}
		if (blowableItemsInMouth.Count < 1 && blowableItemsNearMouth.Count < 1)
		{
			EndBlowDetection();
			return;
		}
		if (blowTimeLeft > 0f)
		{
			blowTimeLeft -= Time.deltaTime;
			if (blowTimeLeft <= 0f)
			{
				blowCooldownLeft = 1f;
				if (isBlowing)
				{
					isBlowing = false;
					PlayBlowAudio(false);
				}
			}
		}
		else
		{
			blowCooldownLeft -= Time.deltaTime;
			if (blowCooldownLeft <= 0f)
			{
				blowTimeLeft = 1f;
				if (!isBlowing)
				{
					isBlowing = true;
					PlayBlowAudio(true);
				}
			}
		}
		if (isBlowing || (Application.isEditor && (Input.GetKey(KeyCode.Space) || GlobalStorage.Instance.MasterHMDAndInputController.GetIsAnyTrackpadButton())))
		{
			float deltaTime = Time.deltaTime;
			bool flag = false;
			bool flag2 = false;
			for (int i = 0; i < blowableItemsInMouth.Count; i++)
			{
				if (blowableItemsInMouth[i] == null)
				{
					flag = true;
				}
				else if (IsBlowableItemReadyToBeBlown(blowableItemsInMouth[i]))
				{
					if (blowableItemsInMouth[i] != null && blowableItemsInMouth[i].enabled)
					{
						blowableItemsInMouth[i].Blow(deltaTime, true, this);
						flag2 = true;
					}
					else
					{
						flag = true;
					}
				}
			}
			for (int j = 0; j < blowableItemsNearMouth.Count; j++)
			{
				if (blowableItemsNearMouth[j] == null)
				{
					flag = true;
				}
				else if (IsBlowableItemReadyToBeBlown(blowableItemsNearMouth[j]))
				{
					if (blowableItemsNearMouth[j] != null && blowableItemsNearMouth[j].enabled)
					{
						blowableItemsNearMouth[j].Blow(deltaTime, false, this);
						flag2 = true;
					}
					else
					{
						flag = true;
					}
				}
			}
			if (!flag2 && audioSourceHelper != null && audioSourceHelper.IsPlaying)
			{
				StopBlowAudio();
			}
			if (flag)
			{
				blowableItemsInMouth.RemoveAll((BlowableItem blowableItem) => blowableItem == null);
				blowableItemsNearMouth.RemoveAll((BlowableItem blowableItem) => blowableItem == null);
			}
			return;
		}
		bool flag3 = true;
		foreach (BlowableItem item in blowableItemsInMouth)
		{
			if (item != null && item.enabled)
			{
				flag3 = false;
				break;
			}
		}
		if (flag3)
		{
			foreach (BlowableItem item2 in blowableItemsNearMouth)
			{
				if (item2 != null && item2.enabled)
				{
					flag3 = false;
					break;
				}
			}
		}
		if (flag3 && audioSourceHelper != null && audioSourceHelper.IsPlaying)
		{
			StopBlowAudio();
		}
	}

	private void TakeBiteOfEdibleItem(EdibleItem edibleItem)
	{
		BasicEdibleItem basicEdibleItem = edibleItem as BasicEdibleItem;
		bool flag = false;
		if (basicEdibleItem != null && basicEdibleItem.IsFullyConsumed)
		{
			flag = true;
		}
		BiteResultInfo biteResultInfo = null;
		if (!flag)
		{
			biteResultInfo = edibleItem.TakeBiteAndGetResult(this);
		}
		if (biteResultInfo == null)
		{
			return;
		}
		WorldItem component = edibleItem.GetComponent<WorldItem>();
		if (component != null)
		{
			DoBiteSoundForWorldItem(component.Data);
		}
		if (biteResultInfo.ChildItemsBitten != null)
		{
			for (int i = 0; i < biteResultInfo.ChildItemsBitten.Length; i++)
			{
				if (biteResultInfo.ChildItemsBitten[i] != null)
				{
					DoBiteSoundForWorldItem(biteResultInfo.ChildItemsBitten[i].PickupableItem.InteractableItem.WorldItemData);
				}
			}
		}
		if (biteResultInfo.ChildItemsFullyConsumed != null)
		{
			for (int j = 0; j < biteResultInfo.ChildItemsFullyConsumed.Length; j++)
			{
				EdibleItemFullyConsumed(biteResultInfo.ChildItemsFullyConsumed[j]);
			}
		}
		if (biteResultInfo.WasMainItemFullyConsumed)
		{
			EdibleItemFullyConsumed(edibleItem);
		}
	}

	private void DoBiteSoundForWorldItem(WorldItemData worldItemData)
	{
		if (worldItemData != null)
		{
			audioSourceHelper.Stop();
			MunchSFXData munchSFXData = null;
			if (munchSoundQuickLookup.ContainsKey(worldItemData))
			{
				munchSFXData = munchSoundQuickLookup[worldItemData];
			}
			if (munchSFXData != null)
			{
				audioSourceHelper.SetClip(munchSFXData.MunchClipElementSequence.GetNext());
			}
			else
			{
				audioSourceHelper.SetClip(munchDB.defaultMunchClip);
			}
			audioSourceHelper.SetLooping(false);
			audioSourceHelper.Play();
		}
	}

	private void EdibleItemFullyConsumed(EdibleItem edibleItem)
	{
		WorldItemData worldItemData = edibleItem.PickupableItem.InteractableItem.WorldItemData;
		string eventDataValue = ((!(worldItemData != null)) ? "null" : worldItemData.ItemName);
		AnalyticsManager.CustomEvent("Eat Item", "Item", eventDataValue);
		GameEventsManager.Instance.ItemActionOccurred(headWorldItem.Data, "USED");
		if (edibleItemsInMouth.Contains(edibleItem))
		{
			edibleItemsInMouth.Remove(edibleItem);
		}
		if (!(worldItemData != null))
		{
			return;
		}
		List<PourItemOnFaceEffect> pourItemOnFaceEffects = pourItemOnFaceData.PourItemOnFaceEffects;
		for (int i = 0; i < pourItemOnFaceEffects.Count; i++)
		{
			if (pourItemOnFaceEffects[i].IsTriggeredBy(worldItemData))
			{
				StartCoroutine(pourItemOnFaceEffects[i].DoEffect(pouredItemOnFaceAudioSourceHelper, barfEffectChunk, barfEffectSpray));
			}
		}
		GameEventsManager.Instance.ItemAppliedToItemActionOccurred(worldItemData, headWorldItem.Data, "ADDED_TO");
	}

	private void RigidbodyEnteredEatTrigger(Rigidbody r)
	{
		EdibleItem component = r.GetComponent<EdibleItem>();
		if (!(component != null) || edibleItemsInMouth.Contains(component))
		{
			return;
		}
		edibleItemsInMouth.Add(component);
		if (nextAutomaticBiteTimer <= 0f && IsEdibleItemReadyToBeEaten(component))
		{
			TakeBiteOfEdibleItem(component);
			if (edibleItemsInMouth.Count == 1)
			{
				nextAutomaticBiteTimer = 0.45f * component.BiteTimerMultiplier;
			}
		}
	}

	private void RigidbodyExitedEatTrigger(Rigidbody r)
	{
		EdibleItem component = r.GetComponent<EdibleItem>();
		if (component != null && edibleItemsInMouth.Contains(component))
		{
			edibleItemsInMouth.Remove(component);
		}
	}

	private void RigidbodyEnteredInMouthTrigger(Rigidbody r)
	{
		WorldItem component = r.GetComponent<WorldItem>();
		if (component != null)
		{
			if (OnWorldItemTouchedMouth != null)
			{
				OnWorldItemTouchedMouth(component.Data);
			}
			BlowableItem component2 = r.GetComponent<BlowableItem>();
			if (component2 != null && !blowableItemsInMouth.Contains(component2))
			{
				blowableItemsInMouth.Add(component2);
				component2.SetInMouth(true);
				cachedLengthOfBlowableItemsInMouth++;
				StartBlowDetection();
			}
		}
	}

	private void RigidbodyExitedInMouthTrigger(Rigidbody r)
	{
		BlowableItem component = r.GetComponent<BlowableItem>();
		if (component != null && blowableItemsInMouth.Contains(component))
		{
			blowableItemsInMouth.Remove(component);
			component.SetInMouth(false);
			cachedLengthOfBlowableItemsInMouth--;
			if (cachedLengthOfBlowableItemsInMouth == 0 && cachedLengthOfBlowableItemsNearMouth == 0)
			{
				EndBlowDetection();
			}
		}
	}

	private void RigidbodyEnteredNearMouthTrigger(Rigidbody r)
	{
		BlowableItem component = r.GetComponent<BlowableItem>();
		if (component != null && !blowableItemsNearMouth.Contains(component))
		{
			blowableItemsNearMouth.Add(component);
			component.SetNearMouth(true);
			cachedLengthOfBlowableItemsNearMouth++;
			StartBlowDetection();
		}
	}

	private void RigidbodyExitedNearMouthTrigger(Rigidbody r)
	{
		BlowableItem component = r.GetComponent<BlowableItem>();
		if (component != null && blowableItemsNearMouth.Contains(component))
		{
			blowableItemsNearMouth.Remove(component);
			component.SetNearMouth(false);
			cachedLengthOfBlowableItemsNearMouth--;
			if (cachedLengthOfBlowableItemsNearMouth == 0 && cachedLengthOfBlowableItemsInMouth == 0)
			{
				EndBlowDetection();
			}
		}
	}

	private void StartBlowDetection()
	{
		if (!blowDetectionIsRunning)
		{
			blowDetectionIsRunning = true;
			isBlowing = true;
			blowTimeLeft = 0f;
			blowCooldownLeft = 0.25f;
		}
	}

	private void EndBlowDetection()
	{
		if (blowDetectionIsRunning)
		{
			blowDetectionIsRunning = false;
			isBlowing = false;
			blowTimeLeft = 0f;
			blowCooldownLeft = 0f;
		}
	}

	public void Cleanup()
	{
		for (int i = 0; i < attachablePoints.Length; i++)
		{
			AttachablePoint attachablePoint = attachablePoints[i];
			if (!(attachablePoint != null))
			{
				continue;
			}
			AttachableObject[] array = attachablePoint.AttachedObjects.ToArray();
			foreach (AttachableObject attachableObject in array)
			{
				if (attachableObject != null)
				{
					attachableObject.Detach(true, true);
					UnityEngine.Object.Destroy(attachableObject);
				}
			}
		}
	}
}
