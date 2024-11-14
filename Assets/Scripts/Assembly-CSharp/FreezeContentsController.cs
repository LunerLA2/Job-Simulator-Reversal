using System.Collections;
using UnityEngine;

public class FreezeContentsController : MonoBehaviour
{
	[SerializeField]
	private ItemCollectionZone itemCollectionZone;

	[SerializeField]
	private bool doDelayedFreezeOnAwake;

	private void Awake()
	{
		if (doDelayedFreezeOnAwake)
		{
			StartCoroutine(WaitAndFreeze());
		}
	}

	private IEnumerator WaitAndFreeze()
	{
		yield return null;
		SetFreezeState(true);
	}

	protected void SetFreezeState(bool s)
	{
		for (int i = 0; i < itemCollectionZone.ItemsInCollection.Count; i++)
		{
			if (itemCollectionZone.ItemsInCollection[i] != null)
			{
				itemCollectionZone.ItemsInCollection[i].Rigidbody.isKinematic = s;
				itemCollectionZone.ItemsInCollection[i].enabled = !s;
			}
		}
	}
}
