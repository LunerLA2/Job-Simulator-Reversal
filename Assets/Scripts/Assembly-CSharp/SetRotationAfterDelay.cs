using System.Collections;
using UnityEngine;

public class SetRotationAfterDelay : MonoBehaviour
{
	[SerializeField]
	private Vector3 localEulerRot;

	[SerializeField]
	private float delay;

	private void Start()
	{
		if (delay > 0f)
		{
			StartCoroutine(WaitAndSet(delay));
		}
		else
		{
			Set();
		}
	}

	private IEnumerator WaitAndSet(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		Set();
	}

	private void Set()
	{
		base.transform.localEulerAngles = localEulerRot;
	}
}
