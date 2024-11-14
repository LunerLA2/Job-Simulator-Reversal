using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
	public float amountOfTime = 3f;

	private float timer;

	private void Update()
	{
		timer += Time.deltaTime;
		if (timer >= amountOfTime)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
