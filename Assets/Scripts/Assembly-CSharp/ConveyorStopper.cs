using UnityEngine;

public class ConveyorStopper : MonoBehaviour
{
	[SerializeField]
	private ConveyerBelt belt;

	private int itemsCurrentlyCollided;

	private void OnCollisionEnter(Collision collision)
	{
		Rigidbody component = collision.gameObject.GetComponent<Rigidbody>();
		if (component != null)
		{
			if (itemsCurrentlyCollided == 0)
			{
				belt.SetIsMoving(false);
			}
			itemsCurrentlyCollided++;
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		Rigidbody component = collision.gameObject.GetComponent<Rigidbody>();
		if (component != null)
		{
			itemsCurrentlyCollided--;
			if (itemsCurrentlyCollided == 0)
			{
				belt.SetIsMoving(true);
			}
		}
	}
}
