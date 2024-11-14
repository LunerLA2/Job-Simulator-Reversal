using UnityEngine;

public class SplatterOnSlice : AbstractSliceHandler
{
	public Object particlePrefab;

	private void Start()
	{
		if (particlePrefab == null)
		{
			Debug.LogWarning("SplatterOnSlice script needs to be connected with a particle effect prefab! Try the included 'splatter prefab' or a custom variant of your own.");
		}
	}

	public override void handleSlice(GameObject[] results)
	{
		if (particlePrefab != null)
		{
			Vector3 position = results[0].transform.position;
			Object.Instantiate(particlePrefab, position, Quaternion.identity);
		}
	}
}
