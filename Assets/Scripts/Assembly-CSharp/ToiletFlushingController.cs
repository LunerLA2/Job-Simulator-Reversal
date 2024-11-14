using UnityEngine;

public class ToiletFlushingController : MonoBehaviour
{
	private float timer;

	[SerializeField]
	private float occurrenceRate = 40f;

	[SerializeField]
	private AudioClip clip;

	private void Update()
	{
		timer += Time.deltaTime;
		if (timer > occurrenceRate)
		{
			AudioManager.Instance.Play(base.transform.position, clip, 1f, 1f);
			timer = 0f;
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.Lerp(Color.green, Color.clear, 0.6f);
		Gizmos.DrawSphere(base.transform.position, 0.4f);
	}
}
