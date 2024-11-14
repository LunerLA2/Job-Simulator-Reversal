using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class ToppedFoodItemController : MonoBehaviour
{
	[SerializeField]
	private ToppingPositionInfo[] toppingPositions;

	[SerializeField]
	private SelectedChangeOutlineController outline;

	[SerializeField]
	private bool repeatToppingsToFillAllPositions;

	private bool wasSetUp;

	public int MaximumToppings
	{
		get
		{
			return toppingPositions.Length;
		}
	}

	private void Start()
	{
		if (!wasSetUp)
		{
			SetupToppings(new List<GameObject>());
		}
	}

	public void SetupToppings(List<GameObject> toppingPrefabs)
	{
		wasSetUp = true;
		List<MeshFilter> list = new List<MeshFilter>();
		list.AddRange(outline.meshFilters);
		if (repeatToppingsToFillAllPositions)
		{
			if (toppingPrefabs.Count > 0)
			{
				int num = 0;
				for (int i = 0; i < toppingPositions.Length; i++)
				{
					if (num < toppingPrefabs.Count)
					{
						for (int j = 0; j < toppingPositions[i].SharedPositions.Length; j++)
						{
							GameObject gameObject = Object.Instantiate(toppingPrefabs[num], Vector3.zero, Quaternion.identity) as GameObject;
							gameObject.transform.SetParent(toppingPositions[i].SharedPositions[j], false);
							list.AddRange(gameObject.GetComponentsInChildren<MeshFilter>());
						}
						num++;
						if (num >= toppingPrefabs.Count)
						{
							num = 0;
						}
					}
				}
			}
		}
		else
		{
			for (int k = 0; k < toppingPrefabs.Count; k++)
			{
				if (k < toppingPositions.Length)
				{
					for (int l = 0; l < toppingPositions[k].SharedPositions.Length; l++)
					{
						GameObject gameObject2 = Object.Instantiate(toppingPrefabs[k], Vector3.zero, Quaternion.identity) as GameObject;
						gameObject2.transform.SetParent(toppingPositions[k].SharedPositions[l], false);
						list.AddRange(gameObject2.GetComponentsInChildren<MeshFilter>());
					}
				}
			}
		}
		outline.meshFilters = list.ToArray();
		outline.Build();
	}
}
