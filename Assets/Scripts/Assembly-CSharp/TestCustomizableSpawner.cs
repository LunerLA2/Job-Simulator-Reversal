using UnityEngine;

public class TestCustomizableSpawner : MonoBehaviour
{
	[SerializeField]
	private GameObject prefab;

	[SerializeField]
	private ObjectPreset preset;

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.N))
		{
			GameObject gameObject = Object.Instantiate(prefab);
			ObjectCustomizationManager.Instance.CustomizeObject(gameObject, preset);
			gameObject.transform.position = base.transform.position;
			gameObject.transform.rotation = base.transform.rotation;
		}
	}
}
