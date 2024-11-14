using UnityEngine;

public class HangingSign : MonoBehaviour
{
	[SerializeField]
	private Rigidbody rb;

	public void Drop()
	{
		rb.isKinematic = false;
	}
}
