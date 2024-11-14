using System.Collections.Generic;
using UnityEngine;

public class TeleportPortal : MonoBehaviour
{
	[SerializeField]
	private Transform[] spawnPoints;

	[SerializeField]
	private ItemCollectionZone itemCollectionZone;

	public Transform[] SpawnPoints
	{
		get
		{
			return spawnPoints;
		}
	}

	public void TeleportItemsTo(TeleportPortal portal)
	{
		int num = 0;
		List<Transform> list = new List<Transform>();
		for (int i = 0; i < itemCollectionZone.ItemsInCollection.Count; i++)
		{
			list.Add(itemCollectionZone.ItemsInCollection[i].transform);
		}
		for (int j = 0; j < list.Count; j++)
		{
			list[j].position = portal.SpawnPoints[num].position;
			num++;
			if (num >= portal.SpawnPoints.Length)
			{
				num = 0;
			}
		}
	}

	public bool IsFacingDown()
	{
		return base.transform.position.y + 0.9f < (base.transform.position + base.transform.up).y;
	}
}
