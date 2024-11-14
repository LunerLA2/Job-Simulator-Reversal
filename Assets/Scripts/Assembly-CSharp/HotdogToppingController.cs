using System;
using OwlchemyVR;
using UnityEngine;

public class HotdogToppingController : MonoBehaviour
{
	[SerializeField]
	private GameObject toppingsRootObject;

	[SerializeField]
	private AttachablePoint attachpointToMonitor;

	[SerializeField]
	private BasicEdibleItem edibleItem;

	[SerializeField]
	private Vector3 rootPositionWhenBitten;

	[SerializeField]
	private GameObject[] chunksToHideWhenBitten;

	[SerializeField]
	private HotdogToppingChunk[] toppingChunks;

	private BasicEdibleItem currentHotdog;

	private void Awake()
	{
		toppingsRootObject.SetActive(false);
	}

	private void OnEnable()
	{
		InitAllChunks();
		for (int i = 0; i < toppingChunks.Length; i++)
		{
			ParticleImpactZone particleImpactZone = toppingChunks[i].ParticleImpactZone;
			particleImpactZone.OnSpecificParticleAppliedUpdate = (Action<ParticleImpactZone, WorldItemData, Vector3>)Delegate.Combine(particleImpactZone.OnSpecificParticleAppliedUpdate, new Action<ParticleImpactZone, WorldItemData, Vector3>(UpdateColor));
		}
		attachpointToMonitor.OnObjectWasAttached += HotdogAttached;
		attachpointToMonitor.OnObjectWasDetached += HotdogDetached;
		BasicEdibleItem basicEdibleItem = edibleItem;
		basicEdibleItem.OnBiteTaken = (Action<EdibleItem>)Delegate.Combine(basicEdibleItem.OnBiteTaken, new Action<EdibleItem>(Bitten));
	}

	private void OnDisable()
	{
		for (int i = 0; i < toppingChunks.Length; i++)
		{
			ParticleImpactZone particleImpactZone = toppingChunks[i].ParticleImpactZone;
			particleImpactZone.OnSpecificParticleAppliedUpdate = (Action<ParticleImpactZone, WorldItemData, Vector3>)Delegate.Remove(particleImpactZone.OnSpecificParticleAppliedUpdate, new Action<ParticleImpactZone, WorldItemData, Vector3>(UpdateColor));
		}
		attachpointToMonitor.OnObjectWasAttached -= HotdogAttached;
		attachpointToMonitor.OnObjectWasDetached -= HotdogDetached;
		BasicEdibleItem basicEdibleItem = edibleItem;
		basicEdibleItem.OnBiteTaken = (Action<EdibleItem>)Delegate.Remove(basicEdibleItem.OnBiteTaken, new Action<EdibleItem>(Bitten));
	}

	private void InitAllChunks()
	{
		for (int i = 0; i < toppingChunks.Length; i++)
		{
			toppingChunks[i].Init();
		}
	}

	private void HotdogAttached(AttachablePoint point, AttachableObject obj)
	{
		currentHotdog = obj.gameObject.GetComponent<BasicEdibleItem>();
		if (currentHotdog != null)
		{
			BasicEdibleItem basicEdibleItem = currentHotdog;
			basicEdibleItem.OnBiteTaken = (Action<EdibleItem>)Delegate.Combine(basicEdibleItem.OnBiteTaken, new Action<EdibleItem>(Bitten));
		}
		UpdateToppingStateFromHotdog();
	}

	private void HotdogDetached(AttachablePoint point, AttachableObject obj)
	{
		InitAllChunks();
		if (currentHotdog != null)
		{
			BasicEdibleItem basicEdibleItem = currentHotdog;
			basicEdibleItem.OnBiteTaken = (Action<EdibleItem>)Delegate.Remove(basicEdibleItem.OnBiteTaken, new Action<EdibleItem>(Bitten));
		}
		currentHotdog = null;
		UpdateToppingStateFromHotdog();
	}

	private void Bitten(EdibleItem item)
	{
		UpdateToppingStateFromHotdog();
	}

	private void UpdateToppingStateFromHotdog()
	{
		if (currentHotdog != null)
		{
			if (currentHotdog.NumBitesTaken == 0)
			{
				SetToppingState(true, false);
			}
			else
			{
				SetToppingState(true, true);
			}
			toppingsRootObject.transform.localScale = currentHotdog.transform.localScale;
		}
		else
		{
			toppingsRootObject.transform.localScale = Vector3.one;
			SetToppingState(false, false);
		}
	}

	private void SetToppingState(bool hotdogExists, bool hotdogIsBitten)
	{
		if (!hotdogExists)
		{
			toppingsRootObject.SetActive(false);
			return;
		}
		toppingsRootObject.SetActive(true);
		toppingsRootObject.transform.localPosition = ((!hotdogIsBitten) ? Vector3.zero : rootPositionWhenBitten);
		for (int i = 0; i < chunksToHideWhenBitten.Length; i++)
		{
			chunksToHideWhenBitten[i].SetActive(!hotdogIsBitten);
		}
	}

	private void UpdateColor(ParticleImpactZone zone, WorldItemData worldItemData, Vector3 pos)
	{
		for (int i = 0; i < toppingChunks.Length; i++)
		{
			if (toppingChunks[i].ParticleImpactZone == zone)
			{
				toppingChunks[i].ApplyColor(worldItemData.OverallColor);
			}
		}
	}
}
