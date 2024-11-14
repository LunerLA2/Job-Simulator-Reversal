using UnityEngine;

public class DelayedAwake : MonoBehaviour
{
	[SerializeField]
	private Transform tr;

	[SerializeField]
	private float timeTillTurnedOn;

	private void Awake()
	{
		Invoke("LateAwake", timeTillTurnedOn);
	}

	private void LateAwake()
	{
		Debug.Log("ran");
		tr.gameObject.SetActive(true);
	}
}
