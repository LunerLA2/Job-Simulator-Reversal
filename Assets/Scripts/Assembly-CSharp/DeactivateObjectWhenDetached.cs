using UnityEngine;

public class DeactivateObjectWhenDetached : MonoBehaviour
{
	[SerializeField]
	private AttachablePoint listenForDetach;

	[SerializeField]
	private GameObject deactivateOnDetach;

	private void OnEnable()
	{
		listenForDetach.OnObjectWasDetached += Detached;
	}

	private void OnDisable()
	{
		listenForDetach.OnObjectWasDetached -= Detached;
	}

	private void Detached(AttachablePoint point, AttachableObject obj)
	{
		deactivateOnDetach.SetActive(false);
	}
}
