using UnityEngine;

public class TemporaryRigidbody : MonoBehaviour
{
	private void Start()
	{
		Invoke("KillBody", 1f);
	}

	private void KillBody()
	{
		Rigidbody component = GetComponent<Rigidbody>();
		Object.Destroy(component);
	}
}
