using UnityEngine;

public class ParticleDestructor : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		if (!GetComponent<ParticleSystem>().isPlaying)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
