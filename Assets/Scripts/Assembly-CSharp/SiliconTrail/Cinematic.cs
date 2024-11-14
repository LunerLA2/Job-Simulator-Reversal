using UnityEngine;

namespace SiliconTrail
{
	public class Cinematic : MonoBehaviour
	{
		[SerializeField]
		private float duration = 3f;

		[SerializeField]
		private AudioSourceHelper[] audiosToPlay;

		[SerializeField]
		private Animation[] animationsToPlay;

		public float Duration
		{
			get
			{
				return duration;
			}
		}

		private void OnEnable()
		{
			for (int i = 0; i < audiosToPlay.Length; i++)
			{
				audiosToPlay[i].Play();
			}
			for (int j = 0; j < animationsToPlay.Length; j++)
			{
				animationsToPlay[j].Play();
			}
		}

		private void OnDisable()
		{
			for (int i = 0; i < audiosToPlay.Length; i++)
			{
				audiosToPlay[i].Stop();
			}
			for (int j = 0; j < animationsToPlay.Length; j++)
			{
				animationsToPlay[j].Stop();
			}
		}
	}
}
