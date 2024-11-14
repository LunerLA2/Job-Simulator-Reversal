using System.Collections;
using OwlchemyVR;
using UnityEngine;

public class MeatballController : MonoBehaviour
{
	[SerializeField]
	private WorldItem myWorldItem;

	[SerializeField]
	private WorldItemData hackTempWorldItemData;

	[SerializeField]
	private WorldItemData actualWorldItemData;

	private void Awake()
	{
		myWorldItem.ManualSetData(hackTempWorldItemData);
		StartCoroutine(HackSetWorldItemData());
	}

	private IEnumerator HackSetWorldItemData()
	{
		yield return new WaitForSeconds(0.1f);
		myWorldItem.ManualSetData(actualWorldItemData);
	}
}
