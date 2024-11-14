using UnityEngine;

public class HapticAudio : MonoBehaviour
{
	[SerializeField]
	private HapticTransformInfo hapticInfo;

	private void Awake()
	{
		hapticInfo.ManualAwake();
	}

	private void Update()
	{
		hapticInfo.ManualUpdate();
	}
}
