using OwlchemyVR;
using UnityEngine;

public class Torch : MonoBehaviour
{
	[SerializeField]
	private BlowableItem blowableItem;

	[SerializeField]
	private GameObject activateWhenBlowing;

	[SerializeField]
	private WorldItemData requiredWorldItemOnBreath;

	private bool isBurning;

	private float timeToBurnFor;

	private void Start()
	{
		activateWhenBlowing.SetActive(false);
	}

	private void Update()
	{
		if (timeToBurnFor > 0f)
		{
			timeToBurnFor -= Time.deltaTime;
			return;
		}
		timeToBurnFor = 0f;
		if (isBurning)
		{
			activateWhenBlowing.SetActive(false);
			isBurning = false;
		}
	}

	private void WasBlown(float amount)
	{
		timeToBurnFor = 1f;
		if (!isBurning)
		{
			activateWhenBlowing.SetActive(true);
			isBurning = true;
		}
	}
}
