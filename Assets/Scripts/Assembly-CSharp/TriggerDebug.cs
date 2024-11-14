using UnityEngine;

public class TriggerDebug : MonoBehaviour
{
	private void OnTriggerEnter(Collider col)
	{
		Debug.Log(base.name + " collided with " + col.name);
	}
}
