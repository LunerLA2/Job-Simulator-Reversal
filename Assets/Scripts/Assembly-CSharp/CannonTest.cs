using OwlchemyVR;
using UnityEngine;

public class CannonTest : MonoBehaviour
{
	private const float coolDown = 0.1f;

	public GameObject bullet;

	public Transform firePosition;

	public float cannonPower;

	private bool canFire;

	private GameObject lastFiredObj;

	private float timeUntilNextFireAble = 0.1f;

	private void Update()
	{
		PickupableItem component = GetComponent<PickupableItem>();
		if (component != null && component.IsCurrInHand && component.CurrInteractableHand.IsTrackPadTouched())
		{
			Debug.Log("Grip!");
			Fire();
		}
		if (!canFire)
		{
			timeUntilNextFireAble -= Time.deltaTime;
			if (timeUntilNextFireAble <= 0f)
			{
				canFire = true;
				timeUntilNextFireAble = 0.1f;
			}
		}
	}

	private void Fire()
	{
		if (canFire)
		{
			canFire = false;
			lastFiredObj = Object.Instantiate(bullet, firePosition.position, Quaternion.identity) as GameObject;
			lastFiredObj.GetComponent<Rigidbody>().AddForce(firePosition.up * cannonPower, ForceMode.Impulse);
		}
	}
}
