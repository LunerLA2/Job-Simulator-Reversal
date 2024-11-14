using UnityEngine;

public class TriggerButtonController : MonoBehaviour
{
	public TriggerButtonEvent OnButtonPressed;

	private void OnTriggerEnter(Collider other)
	{
		if (other.attachedRigidbody != null)
		{
			OnButtonPressed.Invoke(this);
		}
	}
}
