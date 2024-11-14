using UnityEngine;

public class HacksForKert : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem confetti;

	[SerializeField]
	private Transform projectorParent;

	[SerializeField]
	private MarqueeController marqueeController;

	private bool confettiOn;

	private void Start()
	{
		projectorParent.localPosition = Vector3.up * 5f;
		marqueeController.SetMessages("SRS BUSINESS INC.", "SRBSN <color=green>▲ 103</color>", "LAME <color=red>▼ 643</color>", "INIT <color=green>▲ 333</color>", "HERP <color=green>▲ 202</color>", "DERP <color=red>▼ 91</color>", "INET <color=green>▲ 144</color>");
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.C))
		{
			confettiOn = !confettiOn;
			if (!confettiOn)
			{
				confetti.Stop();
			}
			else
			{
				confetti.Play();
			}
		}
	}
}
