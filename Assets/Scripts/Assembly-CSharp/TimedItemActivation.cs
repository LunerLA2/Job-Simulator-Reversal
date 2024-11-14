using System.Collections;
using OwlchemyVR;
using UnityEngine;

public class TimedItemActivation : MonoBehaviour
{
	[SerializeField]
	private float timer;

	[SerializeField]
	private bool repeat;

	private void Awake()
	{
		StartCoroutine(ActivateItem());
	}

	private IEnumerator ActivateItem()
	{
		yield return new WaitForSeconds(timer);
		GameEventsManager.Instance.ItemActionOccurred(GetComponent<WorldItem>().Data, "ACTIVATED");
		if (repeat)
		{
			StartCoroutine(ActivateItem());
		}
	}
}
