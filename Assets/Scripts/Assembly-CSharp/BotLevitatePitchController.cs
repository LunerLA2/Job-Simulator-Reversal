using UnityEngine;

public class BotLevitatePitchController : MonoBehaviour
{
	[SerializeField]
	[Range(0f, 1.5f)]
	private float pitchlow = 0.5f;

	[SerializeField]
	[Range(0f, 1.5f)]
	private float pitchhigh = 1.1f;

	[SerializeField]
	public AudioSourceHelper Source;

	private void Awake()
	{
		Source.SetPitch(Random.Range(pitchlow, pitchhigh));
	}
}
