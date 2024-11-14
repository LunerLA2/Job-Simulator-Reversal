using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class FluidColorMixingTest : MonoBehaviour
{
	[SerializeField]
	private Renderer r;

	[SerializeField]
	private WorldItemData fluid1;

	[SerializeField]
	private WorldItemData fluid2;

	[Range(0f, 1f)]
	[SerializeField]
	private float fluid1Percentage;

	private float totalParticles = 100f;

	private void Update()
	{
		Color combinedColor = GetCombinedColor(BuildCollectionList(totalParticles), totalParticles);
		r.material.color = combinedColor;
	}

	private List<CollectedParticleQuantityInfo> BuildCollectionList(float total)
	{
		List<CollectedParticleQuantityInfo> list = new List<CollectedParticleQuantityInfo>();
		list.Add(new CollectedParticleQuantityInfo(fluid1));
		list[0].SetQuantity(fluid1Percentage * total);
		list.Add(new CollectedParticleQuantityInfo(fluid2));
		list[1].SetQuantity((1f - fluid1Percentage) * total);
		return list;
	}

	private Color GetCombinedColor(List<CollectedParticleQuantityInfo> collectedParticlesList, float totalQuanity)
	{
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		for (int i = 0; i < collectedParticlesList.Count; i++)
		{
			Color overallColor = collectedParticlesList[i].FluidData.OverallColor;
			float quantity = collectedParticlesList[i].Quantity;
			float num5 = quantity / totalQuanity;
			float num6 = 1f - overallColor.r;
			float num7 = 1f - overallColor.g;
			float num8 = 1f - overallColor.b;
			float num9 = Mathf.Min(num6, Mathf.Min(num7, num8));
			if ((double)num9 == 1.0)
			{
				num6 = 0f;
				num7 = 0f;
				num8 = 0f;
			}
			else
			{
				num6 = (num6 - num9) / (1f - num9);
				num7 = (num7 - num9) / (1f - num9);
				num8 = (num8 - num9) / (1f - num9);
			}
			num += num6 * num5;
			num2 += num7 * num5;
			num3 += num8 * num5;
			num4 += num9 * num5;
		}
		return new Color((1f - num) * (1f - num4), (1f - num2) * (1f - num4), (1f - num3) * (1f - num4));
	}
}
