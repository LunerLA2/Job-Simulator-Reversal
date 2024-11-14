using UnityEngine;

public class meanPos : MonoBehaviour
{
	public GameObject[] objects;

	private Vector3 posSum;

	private void Update()
	{
		posSum = Vector3.zero;
		GameObject[] array = objects;
		foreach (GameObject gameObject in array)
		{
			posSum += gameObject.transform.position;
		}
		base.transform.position = posSum / objects.Length;
	}
}
