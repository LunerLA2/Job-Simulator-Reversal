using System;
using OwlchemyVR;
using UnityEngine;

public class BreadSliceController : MonoBehaviour
{
	[SerializeField]
	private ParticleCollectionZone particleCollectionZone;

	[SerializeField]
	private GameObject[] artSauced;

	[SerializeField]
	private MeshRenderer[] sauceRenderers;

	[SerializeField]
	private WorldItemData worldItemDataWhenSauced;

	[SerializeField]
	private WorldItem myWorldItem;

	private bool hasSplatAppeared;

	private void Awake()
	{
		SetSaucedArtState(false);
	}

	private void SetSaucedArtState(bool state)
	{
		for (int i = 0; i < artSauced.Length; i++)
		{
			artSauced[i].SetActive(state);
		}
	}

	private void OnEnable()
	{
		ParticleCollectionZone obj = particleCollectionZone;
		obj.OnParticleIsCollecting = (Action<ParticleCollectionZone, WorldItemData, float>)Delegate.Combine(obj.OnParticleIsCollecting, new Action<ParticleCollectionZone, WorldItemData, float>(ParticleIsCollecting));
	}

	private void OnDisable()
	{
		ParticleCollectionZone obj = particleCollectionZone;
		obj.OnParticleIsCollecting = (Action<ParticleCollectionZone, WorldItemData, float>)Delegate.Remove(obj.OnParticleIsCollecting, new Action<ParticleCollectionZone, WorldItemData, float>(ParticleIsCollecting));
	}

	public float GetAmountOfGivenFluidThatExistsAsSauce(WorldItemData fluid)
	{
		return particleCollectionZone.GetQuantityOfFluidOfTypeInContainer(fluid);
	}

	private void ParticleIsCollecting(ParticleCollectionZone zone, WorldItemData fluid, float currentAmountML)
	{
		if (!hasSplatAppeared)
		{
			if (zone.GetTotalQuantity() >= 50f)
			{
				hasSplatAppeared = true;
				SetSaucedArtState(true);
				myWorldItem.ManualSetData(worldItemDataWhenSauced);
				GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "CREATED");
			}
		}
		else
		{
			Color color = zone.CalculateCombinedFluidColor();
			for (int i = 0; i < sauceRenderers.Length; i++)
			{
				sauceRenderers[i].material.color = color;
				sauceRenderers[i].material.SetColor("_DiffColor", color);
			}
		}
	}
}
