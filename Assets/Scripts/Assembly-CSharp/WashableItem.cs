using System;
using OwlchemyVR;
using UnityEngine;

public class WashableItem : MonoBehaviour
{
	[SerializeField]
	private WorldItem myWorldItem;

	[SerializeField]
	private WorldItemData setMyWorldItemDataWhenCleaned;

	[SerializeField]
	private ParticleImpactZone particleImpactZone;

	[SerializeField]
	private MeshRenderer[] decalRenderers;

	private float decalHighestAlphaCutoffValue = 1f;

	[SerializeField]
	private WorldItemData waterFluidData;

	[SerializeField]
	private WorldItemData soapFluidData;

	[SerializeField]
	private float secondsToWash = 2f;

	[SerializeField]
	private float requiredSecondsOfSoapingBeforeWash;

	[SerializeField]
	private ParticleSystem soapBubblesPFX;

	private float soapAmount;

	private float washedAmount;

	private bool wasCleaned;

	private float soapBubblesInitialEmissionRate;

	private float[] decalInitialAlphaCutoff;

	public Action<WashableItem> OnWasCleaned;

	public void SetHighestAlphaCutoffValue(float v)
	{
		decalHighestAlphaCutoffValue = v;
	}

	private void Awake()
	{
		if (soapBubblesPFX != null)
		{
			soapBubblesInitialEmissionRate = soapBubblesPFX.emissionRate;
			soapBubblesPFX.emissionRate = 0f;
		}
		decalInitialAlphaCutoff = new float[decalRenderers.Length];
		for (int i = 0; i < decalRenderers.Length; i++)
		{
			decalInitialAlphaCutoff[i] = decalRenderers[i].material.GetFloat("_Cutoff");
		}
	}

	private void OnEnable()
	{
		ParticleImpactZone obj = particleImpactZone;
		obj.OnSpecificParticleAppliedUpdate = (Action<ParticleImpactZone, WorldItemData, Vector3>)Delegate.Combine(obj.OnSpecificParticleAppliedUpdate, new Action<ParticleImpactZone, WorldItemData, Vector3>(SpecificFluidHitZone));
	}

	private void OnDisable()
	{
		ParticleImpactZone obj = particleImpactZone;
		obj.OnSpecificParticleAppliedUpdate = (Action<ParticleImpactZone, WorldItemData, Vector3>)Delegate.Remove(obj.OnSpecificParticleAppliedUpdate, new Action<ParticleImpactZone, WorldItemData, Vector3>(SpecificFluidHitZone));
	}

	private void SpecificFluidHitZone(ParticleImpactZone zone, WorldItemData fluidData, Vector3 pos)
	{
		if (wasCleaned)
		{
			return;
		}
		if (fluidData == soapFluidData)
		{
			soapAmount += Time.deltaTime;
			float num = 0f;
			num = ((requiredSecondsOfSoapingBeforeWash != 0f) ? Mathf.Clamp(soapAmount / requiredSecondsOfSoapingBeforeWash, 0f, 1f) : Mathf.Clamp(soapAmount / 2f, 0f, 1f));
			if (soapBubblesPFX != null)
			{
				soapBubblesPFX.emissionRate = num * soapBubblesInitialEmissionRate;
			}
		}
		if (!(fluidData == waterFluidData) || !(soapAmount >= requiredSecondsOfSoapingBeforeWash))
		{
			return;
		}
		if (soapBubblesPFX != null)
		{
			soapBubblesPFX.emissionRate = soapBubblesInitialEmissionRate;
		}
		washedAmount += Time.deltaTime;
		float num2 = Mathf.Clamp(washedAmount / Mathf.Max(0.01f, secondsToWash), 0f, 1f);
		for (int i = 0; i < decalRenderers.Length; i++)
		{
			decalRenderers[i].material.SetFloat("_Cutoff", Mathf.Lerp(decalInitialAlphaCutoff[i], decalHighestAlphaCutoffValue, num2));
		}
		GameEventsManager.Instance.ItemActionOccurredWithAmount(myWorldItem.Data, "CLEANED_PERC_CHANGE", num2);
		if (num2 >= 1f && !wasCleaned)
		{
			wasCleaned = true;
			for (int j = 0; j < decalRenderers.Length; j++)
			{
				decalRenderers[j].enabled = false;
			}
			if (OnWasCleaned != null)
			{
				OnWasCleaned(this);
			}
			GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "CLEANED");
			if (soapBubblesPFX != null)
			{
				soapBubblesPFX.emissionRate = 0f;
			}
			if (setMyWorldItemDataWhenCleaned != null)
			{
				myWorldItem.ManualSetData(setMyWorldItemDataWhenCleaned);
			}
		}
	}
}
